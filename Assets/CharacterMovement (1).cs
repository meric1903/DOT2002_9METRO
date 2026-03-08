using UnityEngine;

using UnityEngine.InputSystem;

 

[RequireComponent(typeof(CharacterController))]

[RequireComponent(typeof(Animator))]

public class PlayerMovement : MonoBehaviour

{

    private CharacterController controller;

    private Animator animator;

    private PlayerControls playerControls; 

    private Vector2 moveInput;

    

    // Tracks whether the sprint button is being held down

    private bool isSprinting; 

 

    [Header("Movement Settings")]

    public float walkSpeed = 3.0f;

    public float runSpeed = 6.0f; 

    public float rotationSpeed = 10.0f;

    

    [Header("Animation Settings")]

    public float speedDampTime = 0.1f;

 

    private void Awake()

    {

        controller = GetComponent<CharacterController>();

        animator = GetComponent<Animator>();

        playerControls = new PlayerControls();

 

        // Movement input listeners

        playerControls.Player.Move.performed += ctx => moveInput = ctx.ReadValue<Vector2>();

        playerControls.Player.Move.canceled += ctx => moveInput = Vector2.zero;

 

        // Sprint input listeners (Button press and release)

        playerControls.Player.Sprint.performed += ctx => isSprinting = true;

        playerControls.Player.Sprint.canceled += ctx => isSprinting = false;

    }

 

    private void OnEnable()

    {

        playerControls.Player.Enable();

    }

 

    private void OnDisable()

    {

        playerControls.Player.Disable();

    }

 

    private void Update()

    {

        HandleMovement();

        HandleAnimation();

    }

 

    private void HandleMovement()

    {

        // Calculate movement strictly based on World Space (X and Z axes)

        // Ignored camera orientation entirely.

        Vector3 moveDirection = new Vector3(moveInput.x, 0f, moveInput.y).normalized;

 

        if (moveDirection.magnitude >= 0.1f)

        {

            // Rotate the character to face the global movement direction

            Quaternion targetRotation = Quaternion.LookRotation(moveDirection);

            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);

            

            // Determine physical speed based on sprint state

            float currentMoveSpeed = isSprinting ? runSpeed : walkSpeed;

            

            // Move the character globally

            controller.Move(moveDirection * currentMoveSpeed * Time.deltaTime);

        }

        

        // Simple Gravity

        if (!controller.isGrounded)

        {

            controller.Move(new Vector3(0, -9.81f, 0) * Time.deltaTime);

        }

    }

 

    private void HandleAnimation()

    {

        float inputMagnitude = moveInput.magnitude;

        float targetAnimationSpeed = 0f;

 

        if (inputMagnitude > 0.1f)

        {

            // 1.0 for Run, 0.5 for Walk

            targetAnimationSpeed = isSprinting ? 1.0f : 0.5f;

        }

 

        // Send the smoothed value to the Animator

        animator.SetFloat("Speed", targetAnimationSpeed, speedDampTime, Time.deltaTime);

    }

}

