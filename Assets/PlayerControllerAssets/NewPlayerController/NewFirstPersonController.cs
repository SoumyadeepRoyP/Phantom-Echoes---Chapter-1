using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController))]
public class NewFirstPersonController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] Transform cameraTransform;
    [SerializeField] Transform headBobJoint; // Renamed for clarity
    [SerializeField] UiManager uiManager;

    [Header("Movement Settings")]
    [SerializeField] float baseSpeed = 5f;
    [SerializeField] float sprintMultiplier = 1.5f;
    [SerializeField] float crouchMultiplier = 0.5f;
    [SerializeField] float jumpHeight = 2f;
    [SerializeField] float gravity = -9.81f;
    
    [Header("Look Settings")]
    [SerializeField] float mouseSensitivity = 1f;
    
    [Header("Head Bob Settings")]
    [SerializeField] float bobSpeed = 10f;
    [SerializeField] Vector2 bobAmount = new Vector2(0.1f, 0.1f);
    
    [Header("State")]
    public bool isPaused = false;

    // Private variables
    CharacterController controller;
    Vector2 moveInput;
    Vector2 lookInput;
    float verticalVelocity;
    float cameraRotationX;
    Vector3 headBobOriginalPos;
    float headBobTimer;

    // State flags
    bool isSprinting;
    bool isCrouching;
    float currentSpeed; // Current movement speed

    void Awake()
    {
        controller = GetComponent<CharacterController>();
        currentSpeed = baseSpeed;
        
        if (headBobJoint)
            headBobOriginalPos = headBobJoint.localPosition;
    }

    void OnEnable() => SetupCursor(true);
    void OnDisable() => SetupCursor(false);

    void Update()
    {
        if (isPaused) return;
        
        HandleLook();
        HandleMovement();
        HandleHeadBob();
    }

    void SetupCursor(bool gameActive)
    {
        if (gameActive)
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
        else
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
    }

    // Input handlers
    void OnMove(InputValue value) => moveInput = isPaused ? Vector2.zero : value.Get<Vector2>();
    void OnLook(InputValue value) => lookInput = isPaused ? Vector2.zero : value.Get<Vector2>();

    void OnSprint(InputValue input)
    {
        if (isCrouching) return;
        
        isSprinting = input.isPressed;
        UpdateMovementSpeed();
        uiManager.SetSprinting(isSprinting);
    }

    void OnCrouch(InputValue input)
    {
        if (isSprinting && input.isPressed) return;
        
        isCrouching = input.isPressed;
        UpdateMovementSpeed();
    }

    void OnJump(InputValue input)
    {
        if (input.isPressed && controller.isGrounded && !isCrouching)
        {
            verticalVelocity = Mathf.Sqrt(2f * Mathf.Abs(gravity) * jumpHeight);
        }
    }

    void HandleLook()
    {
        // Vertical rotation (pitch)
        cameraRotationX -= lookInput.y * mouseSensitivity;
        cameraRotationX = Mathf.Clamp(cameraRotationX, -90f, 90f);
        cameraTransform.localRotation = Quaternion.Euler(cameraRotationX, 0, 0);

        // Horizontal rotation (yaw)
        transform.Rotate(Vector3.up * lookInput.x * mouseSensitivity);
    }

    void HandleMovement()
    {
        // Calculate movement direction
        Vector3 moveDirection = (transform.forward * moveInput.y + transform.right * moveInput.x).normalized;
        Vector3 movement = moveDirection * currentSpeed;

        // Apply gravity
        verticalVelocity += gravity * Time.deltaTime;
        
        // Reset vertical velocity when grounded
        if (controller.isGrounded && verticalVelocity < 0)
            verticalVelocity = -0.5f;  // Small downward force to maintain grounding

        // Apply vertical velocity
        movement.y = verticalVelocity;

        // Move the character
        controller.Move(movement * Time.deltaTime);
    }

    void UpdateMovementSpeed()
    {
        if (isSprinting)
        {
            currentSpeed = baseSpeed * sprintMultiplier;
        }
        else if (isCrouching)
        {
            currentSpeed = baseSpeed * crouchMultiplier;
        }
        else
        {
            currentSpeed = baseSpeed;
        }
    }

    void HandleHeadBob()
    {
        if (!headBobJoint) return;
        
        bool isMoving = controller.isGrounded && moveInput.sqrMagnitude > 0.01f;
        float delta = Time.deltaTime;

        if (isMoving)
        {
            // Apply head bob motion
            headBobTimer += delta * bobSpeed;
            float wave = Mathf.Sin(headBobTimer);
            Vector3 offset = new Vector3(wave * bobAmount.x, wave * bobAmount.y, 0);
            headBobJoint.localPosition = headBobOriginalPos + offset;
        }
        else
        {
            // Smoothly return to original position
            headBobTimer = 0;
            headBobJoint.localPosition = Vector3.Lerp(
                headBobJoint.localPosition, 
                headBobOriginalPos, 
                delta * bobSpeed * 5f
            );
        }
    }
}