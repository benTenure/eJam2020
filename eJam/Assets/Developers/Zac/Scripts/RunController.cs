using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RunController : MonoBehaviour
{
    public enum PlayerState { grounded, jumping, falling }

    public PlayerState CurrentState = PlayerState.grounded;

    [Header("Game Object Refs")]
    public Camera MyCamera;
    public Rigidbody MyRigidBody;
    public Transform PedestrianParent;

    [Header("Movement Values")]
    public float MaxSpeed = 50.0f;
    public float Acceleration = 25.0f;
    public float Deceleration = 25.0f;
    public Transform PushTransform; // The forward vector of this object is the direction the player will be pushed in
    public float MinPushForce = 5.0f;
    float PushForce = 5.0f;

    [Header("Jumping Values")]
    public float Gravity = 20.0f;
    public float JumpStrength = 20.0f;
    public float DoubleJumpStrength = 25.0f;
    public bool bCanDoubleJump = true;

    [Header("Kill-Z Failsafe Transform")]
    public Transform RespawnTransform;

    float currentSpeed = 0.0f;
    float currentJump = 0.0f;

    PlayerInputActions inputActions;
    Vector3 baseMovementInput;
    Vector3 cameraRelativeMovementInput;
    Vector3 lerpingMovementInput = Vector3.zero;

    RaycastHit GroundHit;
    CapsuleCollider MyCollider;

    bool bDoubleJumped = false;

    public List<PedestrianController> PedestrianQueue = new List<PedestrianController>();
    public List<PedestrianController> PedestrianRefs = new List<PedestrianController>();
    IEnumerator GrabEnumRef;
    bool bGrabEnumRunning = false;

    bool bDropEnumRunning = false;

    private void Start()
    {
        PushForce = MinPushForce;
    }

    private void Awake()
    {
        inputActions = new PlayerInputActions();
        // Y is up in Unity, so we want the Y of the vector2 to actually be Z.
        inputActions.PlayerControls.Move.performed += ctx => baseMovementInput = new Vector3(ctx.ReadValue<Vector2>().x, 0.0f, ctx.ReadValue<Vector2>().y);
        inputActions.PlayerControls.Jump.performed += ctx => jump();
        inputActions.PlayerControls.Drop.performed += ctx => drop();

        MyCollider = MyRigidBody.GetComponent<CapsuleCollider>();
    }

    void drop()
    {
        DropOffPedestrians(RespawnTransform.position);
    }

    void jump()
    {
        if (CurrentState == PlayerState.grounded || (bCanDoubleJump && !bDoubleJumped))
        {
            if (CurrentState != PlayerState.grounded)
            {
                bDoubleJumped = true;
            }
            CurrentState = PlayerState.jumping;
            if (bDoubleJumped)
            {
                currentJump = DoubleJumpStrength;
            }
            else
            {
                currentJump = JumpStrength;
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        UpdateCurrentSpeed();
        // Get parent of camera because camera of parent isn't rotated down but in the direction of the player.
        // Ensure camera itself only rotates on X, but parent rotates on Y
        cameraRelativeMovementInput = MyCamera.transform.parent.TransformDirection(baseMovementInput);
        CheckForGround();

        if (CurrentState == PlayerState.grounded)
        {

        }
        else if (CurrentState == PlayerState.jumping)
        {
            HandleGravity();
        }
        else if (CurrentState == PlayerState.falling)
        {
            HandleGravity();
        }

        HandleMovement();
        Debug.DrawLine(MyRigidBody.transform.position, MyRigidBody.transform.position + lerpingMovementInput, Color.red);
        Debug.DrawLine(MyRigidBody.transform.position, MyRigidBody.transform.position + cameraRelativeMovementInput, Color.blue);
    }

    void HandleMovement()
    {
        lerpingMovementInput = Vector3.Lerp(lerpingMovementInput, cameraRelativeMovementInput, 5 * Time.deltaTime);
        Vector3 finalPushForce = Vector3.zero;
        if (PushTransform)
        {
            finalPushForce = (PushTransform.forward * PushForce);
        }
        MyRigidBody.velocity = new Vector3(lerpingMovementInput.x * currentSpeed, currentJump, lerpingMovementInput.z * currentSpeed) + finalPushForce;
        HandleFacingDirection();
    }

    void HandleFacingDirection()
    {
        if (lerpingMovementInput != Vector3.zero)
        {
            MyRigidBody.transform.rotation = Quaternion.LookRotation(lerpingMovementInput);
        }
    }

    void CheckForGround()
    {
        Physics.Raycast(MyRigidBody.transform.position, Vector3.down, out GroundHit, (MyCollider.height * 0.5f) + 0.1f);
        if (CurrentState == PlayerState.grounded)
        {
            if (!GroundHit.transform)
            {
                CurrentState = PlayerState.falling;
                currentJump = 0.0f;
            }
        }
        //else if (CurrentState == PlayerState.jumping)
        //{
        //}
        else if (CurrentState == PlayerState.falling)
        {
            if (GroundHit.transform)
            {
                CurrentState = PlayerState.grounded;
                bDoubleJumped = false;
                currentJump = 0.0f;
            }
        }
    }

    void HandleGravity()
    {
        currentJump -= Gravity * Time.deltaTime;
        if (currentJump <= 0)
        {
            CurrentState = PlayerState.falling;
        }
        if (MyRigidBody.transform.position.y < -200)
        {
            if (RespawnTransform)
            { // Just in case the player somehow falls out of the level
                MyRigidBody.transform.position = RespawnTransform.position;
            }
            else
            {
                Debug.Log("RunController.cs: Hey! You didn't set a failsafe respawn point in the scene!");
            }
        }
    }

    void UpdateCurrentSpeed()
    {
        if (baseMovementInput.magnitude > 0.2f && currentSpeed < MaxSpeed)
        {
            HandleFacingDirection();
            currentSpeed += Acceleration * Time.deltaTime;
        }
        else if (currentSpeed > 0)
        {
            currentSpeed -= Deceleration * Time.deltaTime;
        }

        // Cuz I'm paranoid
        if (currentSpeed > MaxSpeed)
        {
            currentSpeed = MaxSpeed;
        }
        if (currentSpeed < 0)
        {
            currentSpeed = 0;
        }
    }

    private void OnEnable()
    {
        inputActions.Enable();
    }

    private void OnDisable()
    {
        inputActions.Disable();
    }

    public void GrabPedestrian(PedestrianController pedestrianRef)
    {
        if (!PedestrianQueue.Contains(pedestrianRef))
        {
            PedestrianQueue.Add(pedestrianRef);
            if (!bGrabEnumRunning && !bDropEnumRunning)
            {
                startGrabEnum(pedestrianRef);
            }
        }
    }

    void startGrabEnum(PedestrianController pedestrianRef)
    {
        Transform destination = GetPedestrianBackDestination();
        GrabEnumRef = InterpPedestrian(pedestrianRef, destination);
        StartCoroutine(GrabEnumRef);
    }

    IEnumerator InterpPedestrian(PedestrianController pedestrianRef, Transform destination)
    {
        bGrabEnumRunning = true;
        Vector3 dir = (destination.position + Vector3.up) - pedestrianRef.transform.position;
        float mag = dir.magnitude;
        while (mag > 0.5f)
        {
            dir = (destination.position + Vector3.up) - pedestrianRef.transform.position;
            mag = dir.magnitude;
            pedestrianRef.transform.position += (dir.normalized * 100.0f) * Time.deltaTime;
            yield return new WaitForSeconds(0.01f);
        }
        pedestrianRef.transform.position = (destination.position + Vector3.up);
        pedestrianRef.transform.rotation = PedestrianParent.rotation;
        PedestrianQueue.Remove(pedestrianRef);
        PedestrianRefs.Add(pedestrianRef);
        pedestrianRef.transform.parent = PedestrianParent.transform;
        PushForce += 1.0f;
        if (PedestrianQueue.Count > 0)
        {
            startGrabEnum(PedestrianQueue[PedestrianQueue.Count - 1]);
        }
        else
        {
            bGrabEnumRunning = false;
        }
    }

    Transform GetPedestrianBackDestination()
    {
        Transform returnTransform;

        if (PedestrianRefs.Count == 0)
        {
            returnTransform = PedestrianParent;
        }
        else
        {
            returnTransform = PedestrianRefs[PedestrianRefs.Count - 1].transform;
        }

        return returnTransform;
    }

    public void DropOffPedestrians(Vector3 destination)
    {
        if (!bGrabEnumRunning && !bDropEnumRunning)
        {
            bDropEnumRunning = true;
            PedestrianRefs[PedestrianRefs.Count - 1].transform.parent = null;
            StartCoroutine(DropPedestrian(PedestrianRefs[PedestrianRefs.Count - 1], destination));
        }
    }

    IEnumerator DropPedestrian(PedestrianController pedestrianRef, Vector3 destination)
    {
        Vector3 dir = destination - pedestrianRef.transform.position;
        float mag = dir.magnitude;
        while (mag > 0.5f)
        {
            dir = destination - pedestrianRef.transform.position;
            mag = dir.magnitude;
            pedestrianRef.transform.position += (dir.normalized * 100.0f) * Time.deltaTime;
            yield return new WaitForSeconds(0.01f);
        }
        pedestrianRef.transform.position = destination;

        PedestrianRefs.Remove(pedestrianRef);

        PushForce -= 1.0f;
        if(PushForce < MinPushForce)
        {
            PushForce = MinPushForce;
        }

        if (PedestrianRefs.Count > 0)
        {
            DropOffPedestrians(destination);
        }
        else
        {
            bDropEnumRunning = false;
            if (PedestrianQueue.Count > 0)
            {
                startGrabEnum(PedestrianQueue[PedestrianQueue.Count - 1]);
            }
        }
    }
}
