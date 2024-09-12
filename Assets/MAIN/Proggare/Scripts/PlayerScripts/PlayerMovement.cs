using UnityEngine;
using UnityEngine.InputSystem;

//Elian

public class PlayerMovement : MonoBehaviour
{
    //variables
    [SerializeField] float movementSpeed = 10f;
    [SerializeField] float acceleration = 1f;
    [SerializeField] float jumpForce = 5f;

    [SerializeField] float slideSpeed = 15f;

    CharacterController controller;
    PlayerInputActions playerInputActions;

    Vector2 moveInput;

    void Awake()
    {
        playerInputActions = new PlayerInputActions();
        controller = GetComponent<CharacterController>();

        playerInputActions.Player.Move.performed += OnMove;
        playerInputActions.Player.Move.canceled += OnMove;
    }

    void Update()
    {
        Vector3 move = new Vector3(moveInput.x, 0f, moveInput.y);

        controller.Move(move * movementSpeed * Time.deltaTime);

    }

    private void OnEnable()
    {
        playerInputActions.Enable();
    }

    private void OnDisable()
    {
        playerInputActions.Disable();
    }

    void OnMove(InputAction.CallbackContext context)
    {
        moveInput = context.ReadValue<Vector2>();
    }

    void OnJump()
    {

    }
}
