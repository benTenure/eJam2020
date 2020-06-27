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

    [Header("Movement Values")]
    public float MaxSpeed = 50.0f;
    public float Acceleration = 25.0f;
    public float Deceleration = 25.0f;

    float currentSpeed = 0.0f;

    PlayerInputActions inputActions;
    Vector3 baseMovementInput;
    Vector3 cameraRelativeMovementInput;
    Vector3 lerpingMovementInput = Vector3.zero;
    bool bJumped;

    private void Awake()
    {
        inputActions = new PlayerInputActions();
        // Y is up in Unity, so we want the Y of the vector2 to actually be Z.
        inputActions.PlayerControls.Move.performed += ctx => baseMovementInput = new Vector3(ctx.ReadValue<Vector2>().x, 0.0f, ctx.ReadValue<Vector2>().y);
        inputActions.PlayerControls.Jump.performed += ctx => jump();
    }

    void jump()
    {
        Debug.Log("We jumped!");
    }

    // Update is called once per frame
    void Update()
    {
        UpdateCurrentSpeed();
        // Get parent of camera because camera of parent isn't rotated down but in the direction of the player.
        cameraRelativeMovementInput = MyCamera.transform.parent.TransformDirection(baseMovementInput);

        Debug.Log(lerpingMovementInput.magnitude);
        lerpingMovementInput = Vector3.Lerp(lerpingMovementInput, cameraRelativeMovementInput, Time.deltaTime) * currentSpeed;
        MyRigidBody.velocity = lerpingMovementInput;

        Debug.DrawLine(MyRigidBody.transform.position, MyRigidBody.transform.position + lerpingMovementInput, Color.black);
        Debug.DrawLine(MyRigidBody.transform.position, MyRigidBody.transform.position + cameraRelativeMovementInput, Color.blue);
    }

    void UpdateCurrentSpeed()
    {
        if(baseMovementInput.magnitude > 0.2f && currentSpeed < MaxSpeed)
        {
            currentSpeed += Acceleration * Time.deltaTime;
        }
        else if (currentSpeed > 0)
        {
            currentSpeed -= Deceleration * Time.deltaTime;
        }

        // Cuz I'm paranoid
        if(currentSpeed > MaxSpeed)
        {
            currentSpeed = MaxSpeed;
        }
        if(currentSpeed < 0)
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
}
