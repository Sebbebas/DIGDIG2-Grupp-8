using UnityEngine;
using UnityEngine.InputSystem;

//Elian

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] CharacterController controller;

    [Header("Movement Variables")]
    [SerializeField] float speed = 6f;
    [SerializeField] float gravity = -9.81f;
    [SerializeField] float jumpHeight = 1.5f;

    [Header("Ground Check")]
    [SerializeField] Transform groundCheck;
    [SerializeField] float groundDistance = 0.4f;
    [SerializeField] LayerMask groundMask;

    Vector2 moveInput;
    bool isJumping;

    Vector3 velocity;
    bool isGrounded;

    void Update()
    {
        //Check if the player is grounded
        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);

        if (isGrounded && velocity.y < 0)
        {
            velocity.y = -2f;
        }

        // Get movement direction from the input
        Vector3 move = transform.right * moveInput.x + transform.forward * moveInput.y;

        controller.Move(move * speed * Time.deltaTime);

        if (isJumping && isGrounded)
        {
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
            isJumping = false; 
        }

        velocity.y += gravity * Time.deltaTime;

        controller.Move(velocity * Time.deltaTime);
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        moveInput = context.ReadValue<Vector2>();
    }

    public void OnJump(InputAction.CallbackContext context)
    {
        if (context.performed && isGrounded)
        {
            isJumping = true;
        }
    }
}