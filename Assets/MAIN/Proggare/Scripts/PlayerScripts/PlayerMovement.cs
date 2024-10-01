using UnityEngine;
using UnityEngine.InputSystem;

//Elian

public class PlayerMovement : MonoBehaviour
{
    [Header("InputActionMap")]
    [SerializeField] InputActionAsset inputActions;

    [Header("Movement Variables")]
    [SerializeField] float movementSpeed = 10f;
    [SerializeField, Tooltip("Makes player speed faster when moving forward")] float speedMultiplier = 1.2f;
    [SerializeField] float jumpForce = 5f;
    [SerializeField] float gravity = -98.1f;

    [SerializeField] float slowMultiplier = .5f;
    [SerializeField] float slowTime = .75f;

    [Header("Sliding")]
    [SerializeField] float slideSpeed = 15f;
    [SerializeField] float slideDuration = .5f;
    [SerializeField] float slideCooldown = 2f;
    [SerializeField, Tooltip("To change camera height when sliding")] float slideSize = .8f;
    [SerializeField, Tooltip("Multiplies with scale to get a value of 1")] float normalSize = 1.25f;

    [Header("Ground Check")]
    [SerializeField] Transform groundCheck;
    [SerializeField] float groundDistance = .4f;
    [SerializeField] LayerMask groundLayer;

    [Header("Camera")]
    [SerializeField] Transform camTransform;

    //Cached References
    InputAction moveAction;
    InputAction jumpAction;
    InputAction slideAction;
    CharacterController controller;
    Vector2 moveInput;
    Vector3 jumpVelocity;
    Vector3 slideDirection;

    //Private variables
    bool isGrounded;
    bool isSliding;
    float slideTimer;
    float slideCooldownTimer;

    void Awake()
    {
        controller = GetComponent<CharacterController>();
    }

    void Update()
    {
        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundLayer);

        if (isGrounded && jumpVelocity.y < 0)
        {
            //Reset velocity when grounded
            jumpVelocity.y = -2f; 
        }

        //Decrease the slide cooldown timer
        if (slideCooldownTimer > 0f)
        {
            slideCooldownTimer -= Time.deltaTime;
        }

        if (isSliding)
        {
            slideTimer -= Time.deltaTime;
            if (slideTimer <= 0f)
            {
                isSliding = false;
                transform.localScale *= normalSize;
            }

            //Move the player in the locked slide direction
            controller.Move(slideDirection * slideSpeed * Time.deltaTime);
        }
        else
        {
            //Calculate movement direction based on camera orientation
            Vector3 forward = camTransform.forward;
            Vector3 right = camTransform.right;

            forward.y = 0;
            right.y = 0;

            forward.Normalize();
            right.Normalize();

            //Calculate movement based on input and camera orientation
            Vector3 move = forward * moveInput.y + right * moveInput.x;

            //Apply forward speed multiplier if the player is moving forward
            float currentSpeed = movementSpeed;
            if (moveInput.y > 0)
            {
                currentSpeed *= speedMultiplier;
            }

            //Slows down movement speed when player takes damage
            if (GetComponent<PlayerHealth>().takesDamage)
            {
                currentSpeed *= slowMultiplier;
            }

            //Moves the player
            controller.Move(move * currentSpeed * Time.deltaTime);
        }

        //Apply gravity
        jumpVelocity.y += gravity * Time.deltaTime;
        controller.Move(jumpVelocity * Time.deltaTime);
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
        moveAction.canceled += OnMoveCanceled;
        jumpAction.performed += OnJump;
        slideAction.performed += OnSlide;
    }

    private void OnDisable()
    {
        //Unsubscribe from the input when the object is disabled
        moveAction.performed -= OnMove;
        moveAction.canceled -= OnMoveCanceled;
        jumpAction.performed -= OnJump;
        slideAction.performed -= OnSlide;
    }

    void OnMove(InputAction.CallbackContext context) { }

    void OnMove(InputValue value)
    {
        moveInput = value.Get<Vector2>();
    }

    void OnMoveCanceled(InputAction.CallbackContext context) { }

    void OnMoveCanceled(Input value) 
    {
        moveInput = Vector2.zero;
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
            //Lock the slide direction at the start of the slide
            Vector3 forward = camTransform.forward;
            Vector3 right = camTransform.right;

            forward.y = 0;
            right.y = 0;

            forward.Normalize();
            right.Normalize();

            //Calculate slide direction based on input
            slideDirection = forward * moveInput.y + right * moveInput.x;
            slideDirection.Normalize();

            isSliding = true;
            slideTimer = slideDuration;
            slideCooldownTimer = slideCooldown;
            transform.localScale *= slideSize;
        }
    }

    public Vector2 GetMoveInput()
    {
        return moveInput;
    }
}