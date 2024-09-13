using UnityEngine;
using UnityEngine.InputSystem;

//Elian

public class PlayerMovement : MonoBehaviour
{
    [Header("InputActionMap")]
    [SerializeField] InputActionAsset inputActions;

    [Header("Movement Variables")]
    [SerializeField] float movementSpeed = 10f;
    [SerializeField, Tooltip("makes player speed faster when moving forward")] float speedMultiplier = 1.2f;
    [SerializeField] float acceleration = 1f;
    [SerializeField] float jumpForce = 5f;
    [SerializeField] float gravity = -9.81f;
    [SerializeField] float slideSpeed = 15f;
    [SerializeField] float slideDuration = .5f;
    [SerializeField] float slideCooldown = 2f;

    [Header("Ground Check")]
    [SerializeField] Transform groundCheck;
    [SerializeField] float groundDistance = .4f;
    [SerializeField] LayerMask groundLayer;

    [Header("Camera")]
    [SerializeField] Transform camTransform;
    //[SerializeField] float normalCamHeight = 1.8f;
    //[SerializeField] float slideCamHeight = 1.2f;
    //[SerializeField, Tooltip("speed of camera transition")] float camTransistionSpeed = 5f;

    //Cached References
    InputAction moveAction;
    InputAction jumpAction;
    InputAction slideAction;
    CharacterController controller;
    Vector2 moveInput;
    Vector3 jumpVelocity;
    Vector3 slideDirection;

    //private variables
    bool isGrounded;
    bool isSliding;
    float slideTimer;
    float slideCooldownTimer;
    //float currentCamHeight;

    void Awake()
    {
        controller = GetComponent<CharacterController>();
        //currentCamHeight = normalCamHeight;
    }

    void Update()
    {
        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundLayer);

        if (isGrounded && jumpVelocity.y < 0)
        {
            jumpVelocity.y = -2f; // Reset velocity when grounded
        }

        // Decrease the slide cooldown timer
        if (slideCooldownTimer > 0f)
        {
            slideCooldownTimer -= Time.deltaTime;
        }

        // Sliding logic
        if (isSliding)
        {
            slideTimer -= Time.deltaTime;
            if (slideTimer <= 0f)
            {
                isSliding = false; // End slide when timer runs out
            }

            // Move the player in the locked slide direction
            controller.Move(slideDirection * slideSpeed * Time.deltaTime);
        }
        else
        {
            // Calculate movement direction based on camera orientation
            Vector3 forward = camTransform.forward;
            Vector3 right = camTransform.right;

            forward.y = 0;
            right.y = 0;

            forward.Normalize();
            right.Normalize();

            // Calculate movement based on input and camera orientation
            Vector3 move = forward * moveInput.y + right * moveInput.x;

            // Apply forward speed multiplier if the player is moving forward
            float currentSpeed = movementSpeed;
            if (moveInput.y > 0)
            {
                currentSpeed *= speedMultiplier;
            }

            // Moves the player
            controller.Move(move * currentSpeed * Time.deltaTime);
        }

        // Apply gravity
        jumpVelocity.y += gravity * Time.deltaTime;
        controller.Move(jumpVelocity * Time.deltaTime);

        //float targetCameraHeight = isSliding ? slideCamHeight : normalCamHeight;
        //currentCamHeight = Mathf.Lerp(currentCamHeight, targetCameraHeight, Time.deltaTime);
        //Vector3 camPos = camTransform.position;
        //camPos.y = currentCamHeight;
        //camTransform.position = camPos;
    }

    private void OnEnable()
    {
        //Get the action map and the action
        var actionMap = inputActions.FindActionMap("Player");

        moveAction = actionMap.FindAction("Move");
        jumpAction = actionMap.FindAction("Jump");
        slideAction = actionMap.FindAction("Slide");

        //Enable the action
        moveAction.Enable();
        jumpAction.Enable();
        slideAction.Enable();

        //Subscribe to the performed callback
        moveAction.performed += OnMove;
        jumpAction.performed += OnJump;
        slideAction.performed += OnSlide;
    }

    private void OnDisable()
    {
        //Unsubscribe from the input when the object is disabled
        moveAction.performed -= OnMove;
        jumpAction.performed -= OnJump;
        slideAction.performed -= OnSlide;
    }

    void OnMove(InputAction.CallbackContext context) { }

    void OnMove(InputValue value)
    {
        moveInput = value.Get<Vector2>();
    }

    void OnJump(InputAction.CallbackContext context) { }

    void OnJump(InputValue value)
    {
        if (isGrounded)
        {
            jumpVelocity.y = Mathf.Sqrt(jumpForce * -2f * gravity);
        }
    }

    void OnSlide(InputAction.CallbackContext context) { }

    void OnSlide(InputValue value)
    {
        if (isGrounded && (moveInput.y > 0 || Mathf.Abs(moveInput.x) > 0) && slideCooldownTimer <= 0f)
        {
            // Lock the slide direction at the start of the slide
            Vector3 forward = camTransform.forward;
            Vector3 right = camTransform.right;

            forward.y = 0;
            right.y = 0;

            forward.Normalize();
            right.Normalize();

            // Calculate slide direction based on input
            slideDirection = forward * moveInput.y + right * moveInput.x;
            slideDirection.Normalize();

            isSliding = true;
            slideTimer = slideDuration;
            slideCooldownTimer = slideCooldown; 
        }
    }
}