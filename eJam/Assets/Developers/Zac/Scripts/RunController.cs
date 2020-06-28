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
    public float StartingMaxSpeed = 50.0f;
    float MaxSpeed = 50.0f;
    public float Acceleration = 25.0f;
    public float Deceleration = 25.0f;
    public Transform PushTransform; // The forward vector of this object is the direction the player will be pushed in
    public float MinPushForce = 5.0f;
    float PushForce = 5.0f;

    [Header("Jumping Values")]
    public float Gravity = 20.0f;
    public float StartJumpStrength = 20.0f;
    float currentJumpStrength = 20.0f;
    public float DoubleJumpStrength = 25.0f;
    public bool bCanDoubleJump = true;

    [Header("Kill-Z Failsafe Transform")]
    public Transform RespawnTransform;

    [Header("Animation")]
    public Animator AnimationController;

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

    IEnumerator DropEnumRef;
    bool bDropEnumRunning = false;

    Vector3 lastLookingDir;

    const float PedestrianWeight = 1.0f;
    const float PedestrianJumpWeight = 1.5f;

    //GameObject testRef;

    private void Start()
    {
        PushForce = MinPushForce;
        MaxSpeed = StartingMaxSpeed;
        currentJumpStrength = StartJumpStrength;
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
        //Destroy(testRef);
        // DropOffPedestrians(RespawnTransform.position);
    }

    void jump()
    {
        if (CurrentState == PlayerState.grounded || (bCanDoubleJump && !bDoubleJumped))
        {
            if (CurrentState != PlayerState.grounded)
            {
                bDoubleJumped = true;
                AnimationController.SetTrigger("DoubleJump");
            }
            CurrentState = PlayerState.jumping;
            if (bDoubleJumped)
            {
                currentJump = currentJumpStrength;
            }
            else
            {
                currentJump = currentJumpStrength;
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
        HandleAnimationStuff();
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

    void HandleAnimationStuff()
    {
        AnimationController.SetFloat("Speed", currentSpeed);
        AnimationController.SetBool("Grounded", CurrentState == PlayerState.grounded);
        AnimationController.SetBool("Jumping", CurrentState == PlayerState.jumping);
        AnimationController.SetBool("Falling", CurrentState == PlayerState.falling);
        AnimationController.SetBool("Holding", PedestrianRefs.Count > 0);
        if (currentSpeed > 0.1f)
        {
            AnimationController.speed = Mathf.Lerp(0.1f, 2.25f, currentSpeed / StartingMaxSpeed);
        }
        else
        {
            AnimationController.speed = 1.0f;
        }
    }

    void HandleFacingDirection()
    {
        if (baseMovementInput.magnitude > 0.2f && currentSpeed < MaxSpeed)
        {
            MyRigidBody.transform.rotation = Quaternion.LookRotation(lerpingMovementInput);
            lastLookingDir = lerpingMovementInput.normalized;
        }
        else if (lastLookingDir != Vector3.zero)
        {
            MyRigidBody.transform.rotation = Quaternion.LookRotation(lastLookingDir);
        }
    }


    void CheckForGround()
    {
        Physics.Raycast(MyCollider.transform.position + MyCollider.center, Vector3.down, out GroundHit, (MyCollider.height * 0.5f) + 0.1f);
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
        HandleFacingDirection();
        if (baseMovementInput.magnitude > 0.2f && currentSpeed < MaxSpeed)
        {
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
            if (bDropEnumRunning)
            {
                PedestrianRefs.Insert(0, pedestrianRef);
            }
            else
            {
                PedestrianQueue.Add(pedestrianRef);
            }
            if (!bGrabEnumRunning && !bDropEnumRunning)
            {
                startGrabEnum(pedestrianRef);
            }
            if (PedestrianRefs.Count > 0)
            {
                AnimationController.SetTrigger("PickupCarry");
            }
            else
            {
                AnimationController.SetTrigger("PickupNoCarry");
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
        int testFactor;
        if (PedestrianRefs.Count == 0)
        {
            testFactor = -1;
        }
        else
        {
            testFactor = 1;
        }
        Vector3 dir = (destination.position + ((destination.up * testFactor) * 0.25f)) - pedestrianRef.transform.position;
        float mag = dir.magnitude;

        float timer = 0.0f;

        while (timer < 0.5f)
        {
            dir = (destination.position + ((destination.up * testFactor) * 0.25f)) - pedestrianRef.transform.position;
            mag = dir.magnitude;
            pedestrianRef.transform.position += (dir.normalized * 20.0f) * Time.deltaTime;
            timer += 0.1f;
            yield return new WaitForSeconds(0.01f);
        }
        pedestrianRef.transform.position = (destination.position + ((destination.up * testFactor) * 0.25f));
        pedestrianRef.transform.rotation = PedestrianParent.rotation;
        PedestrianQueue.Remove(pedestrianRef);
        PedestrianRefs.Add(pedestrianRef);
        pedestrianRef.transform.parent = PedestrianParent.transform;
        MaxSpeed -= PedestrianWeight;
        if(MaxSpeed <= 0.0f)
        {
            MaxSpeed = PedestrianWeight;
        }

        currentJumpStrength -= PedestrianJumpWeight;
        if(currentJumpStrength <= 0.0f)
        {
            currentJumpStrength = PedestrianJumpWeight;
        }

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

    public bool DropOffPedestrians(GameObject destination)
    {
        //testRef = destination;
        if (!bGrabEnumRunning && !bDropEnumRunning && PedestrianRefs.Count > 0)
        {
            bDropEnumRunning = true;
            PedestrianRefs[PedestrianRefs.Count - 1].transform.parent = null;
            StartCoroutine(DropPedestrian(PedestrianRefs[PedestrianRefs.Count - 1], destination));
            return true;
        }
        return false;
    }

    void DropOffManual(GameObject destination)
    {
        PedestrianRefs[PedestrianRefs.Count - 1].transform.parent = null;

        DropEnumRef = DropPedestrian(PedestrianRefs[PedestrianRefs.Count - 1], destination);
        StartCoroutine(DropEnumRef);
    }

    public Vector3 ReturnRandomVectorWithinBounds(GameObject bounds)
    {
        var colliderScaleX = bounds.transform.localScale.x / 2;
        var colliderScaleZ = bounds.transform.localScale.z / 2;
        
        var randomX = Random.Range(bounds.transform.position.x - colliderScaleX, bounds.transform.position.x + colliderScaleX);
        var randomZ = Random.Range(bounds.transform.position.z - colliderScaleZ, bounds.transform.position.z + colliderScaleZ);
        var randomDropOffVector = new Vector3(randomX, bounds.transform.position.y, randomZ);

        return randomDropOffVector;
    }

    IEnumerator DropPedestrian(PedestrianController pedestrianRef, GameObject destination)
    {
        var randomDestination = this.transform.position;
        if (destination == null)
        {
            saveAllEarly();
        }
        else
        {
            randomDestination = ReturnRandomVectorWithinBounds(destination);
        }
        Vector3 dir = randomDestination - pedestrianRef.transform.position;
        float mag = dir.magnitude;
        //while (mag > 0.5f)
        //{
        //    dir = randomDestination - pedestrianRef.transform.position;
        //    mag = dir.magnitude;
        //    pedestrianRef.transform.position += (dir.normalized * 50.0f) * Time.deltaTime;
        //    yield return new WaitForSeconds(0.01f);
        //}
        pedestrianRef.transform.position = randomDestination;
        yield return new WaitForSeconds(0.1f);

        GameManagerScript.Instance.AddPeopleSaved(1);

        PedestrianRefs.Remove(pedestrianRef);

        MaxSpeed += PedestrianWeight;
        if(MaxSpeed > StartingMaxSpeed)
        {
            MaxSpeed = StartingMaxSpeed;
        }

        currentJumpStrength += PedestrianJumpWeight;
        if(currentJumpStrength > StartJumpStrength)
        {
            currentJumpStrength = StartJumpStrength;
        }

        if (destination == null)
        {
            saveAllEarly();
        }
        else
        {
            pedestrianRef.transform.parent = destination.transform;
            pedestrianRef.BeenDroppedOff(MyRigidBody.transform);
            if (PedestrianRefs.Count > 0)
            {
                DropOffManual(destination);
            }
            else
            {
                bDropEnumRunning = false;
                currentJumpStrength = StartJumpStrength;
                MaxSpeed = StartingMaxSpeed;

            }
        }
    }

    void saveAllEarly()
    {
        StopCoroutine(DropEnumRef);

        bDropEnumRunning = false;
        currentJumpStrength = StartJumpStrength;
        MaxSpeed = StartingMaxSpeed;

        GameManagerScript.Instance.AddPeopleSaved(PedestrianRefs.Count);

        for(int i = 0; i < PedestrianRefs.Count; i++)
        {
            PedestrianRefs[i].transform.parent = null;
            Destroy(PedestrianRefs[i].gameObject);
        }
        PedestrianRefs.Clear();
    }
}
