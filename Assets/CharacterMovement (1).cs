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

    private bool isSprinting;

    [Header("Movement Settings")]
    public float walkSpeed = 3.0f;
    public float runSpeed = 6.0f;
    public float rotationSpeed = 10.0f;

    [Header("Animation Settings")]
    public float speedDampTime = 0.1f;

    [Header("Camera")]
    public Transform cameraTransform;

    private void Awake()
    {
        controller = GetComponent<CharacterController>();
        animator = GetComponent<Animator>();
        playerControls = new PlayerControls();

        playerControls.Player.Move.performed += ctx => moveInput = ctx.ReadValue<Vector2>();
        playerControls.Player.Move.canceled += ctx => moveInput = Vector2.zero;

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
        Vector3 cameraForward = cameraTransform.forward;
        Vector3 cameraRight = cameraTransform.right;

        cameraForward.y = 0;
        cameraRight.y = 0;

        cameraForward.Normalize();
        cameraRight.Normalize();

        Vector3 moveDirection = cameraForward * moveInput.y + cameraRight * moveInput.x;

        if (moveDirection.magnitude >= 0.1f)
        {
            Quaternion targetRotation = Quaternion.LookRotation(moveDirection);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);

            float currentMoveSpeed = isSprinting ? runSpeed : walkSpeed;

            controller.Move(moveDirection.normalized * currentMoveSpeed * Time.deltaTime);
        }

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
            targetAnimationSpeed = isSprinting ? 1.0f : 0.5f;
        }

        animator.SetFloat("Speed", targetAnimationSpeed, speedDampTime, Time.deltaTime);
    }
}