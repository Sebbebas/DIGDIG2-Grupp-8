using System;
using UnityEngine;
using UnityEngine.InputSystem;

//Elian

public class PlayerMovement : MonoBehaviour
{
    [Header("InputActionMap")]
    [SerializeField] InputActionAsset inputActions;

    [Header("Movement Variables")]
    [SerializeField] float movementSpeed = 10f;
    [SerializeField] float acceleration = 1f;
    [SerializeField] float jumpForce = 5f;
    [SerializeField] float gravity = -9.81f;
    [SerializeField] float slideSpeed = 15f;

    [Header("Ground Check")]
    [SerializeField] Transform groundCheck;
    [SerializeField] float groundDistance = .4f;
    [SerializeField] LayerMask groundLayer;

    //Cached References
    InputAction moveAction;
    InputAction jumpAction;
    CharacterController controller;
    Vector2 moveInput;
    Vector3 jumpVelocity;

    //private variables
    bool isGrounded;

    void Awake()
    {
        controller = GetComponent<CharacterController>();
    }

    void Update()
    {
        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundLayer);

        if (isGrounded && jumpVelocity.y < 0)
        {
            jumpVelocity.y = -2f; // Reset velocity when grounded
        }

        //movement logic
        Vector3 move = new Vector3(moveInput.x, 0f, moveInput.y);
        controller.Move(move * movementSpeed * Time.deltaTime);

        // Apply gravity
        jumpVelocity.y += gravity * Time.deltaTime;
        controller.Move(jumpVelocity * Time.deltaTime);
    }

    private void OnEnable()
    {
        //Get the action map and the action
        var actionMap = inputActions.FindActionMap("Player");
        if (actionMap == null)
        {
            Debug.LogError("ActionMap 'Player' not found in the InputActionAsset.");
            return;
        }
        moveAction = actionMap.FindAction("Move");
        jumpAction = actionMap.FindAction("Jump");

        if (moveAction == null)
        {
            Debug.LogError("Move action not found in the 'Player' ActionMap.");
            return;
        }

        if (jumpAction == null)
        {
            Debug.LogError("Jump action not found in the 'Player' ActionMap.");
            return;
        }

        //Enable the action
        moveAction.Enable();
        jumpAction.Enable();

        //Subscribe to the performed callback
        moveAction.performed += OnMove;
        jumpAction.performed += OnJump;
    }

    private void OnDisable()
    {
        //Unsubscribe from the input when the object is disabled
        moveAction.performed -= OnMove;
        jumpAction.performed -= OnJump;
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
}