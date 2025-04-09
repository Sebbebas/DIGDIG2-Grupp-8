using UnityEngine;
using UnityEngine.InputSystem;

// Sebbe

public class PlayerLook : MonoBehaviour
{
    //Configurable Parameters
    [SerializeField] Texture2D cursorTexture;
    [SerializeField] Vector2 cursorPos;
    public int mouseSensitivity = 5;

    //Private Varibels
    Vector2 lookInput = Vector2.zero;
    float xRotation = 0f;

    //Cached References
    Transform playerBody;
    InputActions inputActions;

    private void Awake()
    {
        playerBody = GetComponentInParent<CharacterController>().GetComponent<Transform>();
        inputActions = new InputActions();
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.SetCursor(cursorTexture, cursorPos, CursorMode.Auto);
    }

    private void Update()
    {
        Cursor.SetCursor(cursorTexture, cursorPos, CursorMode.Auto);

        if (GetComponentInParent<SettingManager>().GetGameIsPaused() || GetComponentInParent<PlayerHealth>().isDead)
        {
            Cursor.lockState = CursorLockMode.Confined;
        }
        else if (!GetComponentInParent<SettingManager>().GetGameIsPaused() || !GetComponentInParent<PlayerHealth>().isDead)
        {
            Cursor.lockState = CursorLockMode.Locked;
        }

        //Calculate the mouse movement, apply sensitivity and Time.deltaTime
        float mouseX = lookInput.x * mouseSensitivity * .1f * Time.deltaTime;
        float mouseY = lookInput.y * mouseSensitivity * .1f * Time.deltaTime;

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

    public Vector2 GetlookInput()
    {
        return lookInput;
    }
}
