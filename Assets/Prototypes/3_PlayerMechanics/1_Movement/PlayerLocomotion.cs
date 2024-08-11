using System;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController))]
public class PlayerLocomotion : MonoBehaviour
{
    public InputActionAsset inputActions;
    private Vector2 currentMovementInput;

    private bool isRunning;

    [SerializeField]
    [Tooltip("Normal Walking Speed")]
    private float speed = 2.0f; 

    [SerializeField]
    [Tooltip("Multiplier for running speed")]
    private float runMultiplier = 2.0f;

    [SerializeField]
    [Tooltip("Customize jump force")]
    private float jumpForce = 10f;

    [SerializeField]
    [Tooltip("The ammount of force to apply to Gravity")]
    private float gravity = -9.81f;

    [SerializeField]
    [Tooltip("The current Vertical Velocity")]
    private float verticalVelocity;

    [SerializeField]
    [Tooltip("Speed at which the character turns to face movement direction")]
    private float rotationSpeed = 10f;

    private CharacterController characterController;
    private Transform cameraTransform;

    private void Awake()
    {
        //Components
        characterController = GetComponent<CharacterController>();
        if (characterController == null)
        {
            Debug.LogError($"Character Controller Component is missing from {this.name}");
            return;
        }

        cameraTransform = Camera.main.transform;

        //ActionMap
        InputActionMap movementActionMap = inputActions.FindActionMap("Movement");
        movementActionMap.Enable();

        //Movement
        InputAction moveAction = movementActionMap.FindAction("Move");
        moveAction.performed += ctx => currentMovementInput = ctx.ReadValue<Vector2>();
        moveAction.canceled += ctx => currentMovementInput = Vector2.zero;

        //Run
        InputAction runAction = movementActionMap.FindAction("Run");
        runAction.performed += ctx => isRunning = true;
        runAction.canceled += ctx => isRunning = false;

        //Jump
        InputAction jumpAction = movementActionMap.FindAction("Jump");
        jumpAction.performed += _ => Jump();

    }

    private void FixedUpdate()
    {
        ApplyGravity();
        Move();
    }

    private void ApplyGravity()
    {
        if (characterController.isGrounded && verticalVelocity < 0)
            verticalVelocity = 0f;

        //Apply gravity continuously unless grounded
        verticalVelocity += gravity * Time.deltaTime;
        characterController.Move(new Vector3(0, verticalVelocity, 0) * Time.deltaTime);
    }

    private void Move()
    {
        //Convert input vector into world space relative to the camera's orientation
        Vector3 forward = cameraTransform.forward;
        Vector3 right = cameraTransform.right;
        forward.y = 0; //ensure movement is strictly horizontal
        right.y = 0;
        forward.Normalize();
        right.Normalize();

        Vector3 moveDirection = forward * currentMovementInput.y + right * currentMovementInput.x;

        float moveSpeed = speed * (isRunning ? runMultiplier : 1.0f);

        Vector3 movement = moveDirection * moveSpeed * Time.deltaTime;
        movement.y = verticalVelocity * Time.deltaTime;
        characterController.Move(movement);

        //Move the character with CharacterController
        characterController.Move(moveDirection * moveSpeed * Time.deltaTime + Vector3.down * .01f); //Added gravity effect

        //Face the character in the direction of movement
        if (moveDirection != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(moveDirection);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
        }

    }

    private void Jump()
    {
        if (characterController.isGrounded)
            verticalVelocity = jumpForce;
    }

    private void OnDisable()
    {
        inputActions.Disable();
    }
}
