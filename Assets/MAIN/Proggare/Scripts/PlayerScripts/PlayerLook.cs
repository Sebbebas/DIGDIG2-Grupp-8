using UnityEngine;
using UnityEngine.InputSystem;

// Sebbe

public class PlayerLook : MonoBehaviour
{
    //Configurable Parameters
    [SerializeField] float mouseSensitivity = 5f;

    //Private Varibels
    Vector2 lookInput = Vector2.zero;  //To store the look input (mouse movement)
    float xRotation = 0f;  //Vertical rotation value for clamping

    //Cached References
    Transform playerBody;
    PlayerInputActions inputActions;

    private void Awake()
    {
        playerBody = GetComponentInParent<CharacterController>().GetComponent<Transform>();
        inputActions = new PlayerInputActions();
        Cursor.lockState = CursorLockMode.Locked;
    }
    private void Update()
    {
        //Calculate the mouse movement, apply sensitivity and Time.deltaTime
        float mouseX = lookInput.x * mouseSensitivity * Time.deltaTime;
        float mouseY = lookInput.y * mouseSensitivity * Time.deltaTime;

        //Adjust vertical rotation (Y-axis) and clamp it to prevent over-rotation
        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);

        //Apply vertical rotation to the camera (or player head)
        transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);

        //Apply horizontal rotation to the player body (left/right)
        playerBody.Rotate(Vector3.up * mouseX);
    }

    #region Input
    private void OnEnable()
    {
        //Enable the "Look" action and assign the callback functions
        inputActions.Player.Look.Enable();
        inputActions.Player.Look.performed += OnLook;
        inputActions.Player.Look.canceled += OnLookCanceled;
    }

    private void OnDisable()
    {
        //Unsubscribe from the "Look" action and disable it when not needed
        inputActions.Player.Look.performed -= OnLook;
        inputActions.Player.Look.canceled -= OnLookCanceled;
        inputActions.Player.Look.Disable();
    }

    //Callback function to handle the "Look" input action
    private void OnLook(InputAction.CallbackContext context)
    {
        //Read the mouse delta input (X and Y axis)
        lookInput = context.ReadValue<Vector2>();
    }

    //Callback function to reset the lookInput when the action is canceled (i.e., no input)
    private void OnLookCanceled(InputAction.CallbackContext context)
    {
        lookInput = Vector2.zero;
    }
    #endregion

}