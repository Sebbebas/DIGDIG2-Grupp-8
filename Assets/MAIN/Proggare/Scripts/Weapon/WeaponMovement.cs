using UnityEngine;

//Sebbe

public class WeaponMovement : MonoBehaviour
{
    //Configurable Parameters
    [Header("Weapon Bobbing Settings")]
    [SerializeField] float bobFrequency = 5.0f;
    [SerializeField] float bobAmplitude = 0.05f;
    [SerializeField] float transitionSpeed = 5f;

    [Header("Camera Tilt Settings")]
    [SerializeField, Tooltip("Tilt angle when moving along the X-axis")] float cameraTilt = 10.0f;
    [SerializeField, Tooltip("Speed of camera tilt transition")] float tiltSpeed = 5.0f;

    [Header("Weapon Sway Settings")]
    [SerializeField, Tooltip("The amount the weapon model turns when you look around")] float swayAmount = 0.02f;
    [SerializeField, Tooltip("The smoothness of the gun model sway")] float swaySmoothness = 4.0f;
    [SerializeField, Tooltip("The maximum amout the gun model is allowed to sway")] float maxSwayAmount = 0.42f;

    [Header("Weapon Rotation Settings")]
    [SerializeField, Tooltip("Smoothness rotating towards the center of the screen")] float rotationSmoothness = 6.0f;

    //Private Variables
    private float timer;

    //Initial States\\
    private Quaternion originalRotation;
    private Vector3 originalPosition;
    private float defaultCameraTilt;
    private float defaultYPos;

    //Current Inputs\\
    private Vector2 mousePos;
    private float moveSpeed;

    //Cached References
    PlayerMovement playerMovement;
    PlayerLook playerLook;

    void Start()
    {
        //Get Cached References
        playerMovement = FindFirstObjectByType<PlayerMovement>();
        playerLook = FindFirstObjectByType<PlayerLook>();

        //Save inital states
        originalPosition = transform.localPosition;
        originalRotation = transform.localRotation;
        defaultCameraTilt = cameraTilt;
        defaultYPos = originalPosition.y;
    }

    void Update()
    {
        //Call Methods
        HandleBobbing();
        HandleSway();
        HandleLookRotation();
        HandleCameraTilt();

        //Get current inputs
        mousePos = playerLook.GetlookInput();
        moveSpeed = new Vector3(playerMovement.GetMoveInput().x, 0, playerMovement.GetMoveInput().y).magnitude;
    }

    /// <summary>
    /// Method to handle weapon bobbing while the player moves
    /// </summary>
    void HandleBobbing()
    {
        // Only apply bobbing when the player is moving
        if (moveSpeed > 0.1f)
        {
            // Increase the timer based on time and bob frequency
            timer += Time.deltaTime * bobFrequency;
            float bobOffset = Mathf.Sin(timer) * bobAmplitude;

            // New position with bobbing effect
            Vector3 newPosition = originalPosition;
            newPosition.y = defaultYPos + bobOffset;

            // Smoothly transition between current and new bobbing position
            transform.localPosition = Vector3.Lerp(transform.localPosition, newPosition, Time.deltaTime * transitionSpeed);
        }
        else
        {
            // Reset bobbing when the player is not moving
            timer = 0;
            transform.localPosition = Vector3.Lerp(transform.localPosition, originalPosition, Time.deltaTime * transitionSpeed);
        }
    }

    /// <summary>
    /// Method to handle weapon sway while looking around with the mouse
    /// </summary>
    void HandleSway()
    {
        // Calculate the target position offset based on mouse movement
        Vector3 swayOffset = new(-mousePos.x * swayAmount, -mousePos.y * swayAmount, 0);

        // Clamp the sway amount to limit extreme movement
        swayOffset.x = Mathf.Clamp(swayOffset.x, -maxSwayAmount, maxSwayAmount);
        swayOffset.y = Mathf.Clamp(swayOffset.y, -maxSwayAmount, maxSwayAmount);

        // Smoothly transition between current and new sway position
        transform.localPosition = Vector3.Lerp(transform.localPosition, originalPosition + swayOffset, Time.deltaTime * swaySmoothness);
    }

    /// <summary>
    /// Method to make the weapon rotate towards the center of the screen
    /// </summary>
    void HandleLookRotation()
    {
        // Calculate the target rotation (look towards the center of the screen)
        Quaternion targetRotation = Quaternion.Euler(mousePos.y * -maxSwayAmount, mousePos.x * maxSwayAmount, 0);

        // Smoothly rotate towards the target rotation
        transform.localRotation = Quaternion.Lerp(transform.localRotation, originalRotation * targetRotation, Time.deltaTime * rotationSmoothness);
    }

    // Method to handle camera tilt when moving along the X-axis
    void HandleCameraTilt()
    {
        return;

        if (Camera.main == null) return;

        Transform cameraTransform = Camera.main.transform;
        float moveInputX = playerMovement.GetMoveInput().x;

        // Check if the player is moving along the X-axis
        if (moveInputX != 0)
        {
            // Calculate the target tilt angle based on the direction of movement
            float targetTilt = moveInputX > 0 ? -cameraTilt : cameraTilt;

            // Smoothly transition the camera's Z rotation towards the target tilt
            Quaternion targetRotation = Quaternion.Euler(cameraTransform.localRotation.eulerAngles.x,
                                                         cameraTransform.localRotation.eulerAngles.y,
                                                         targetTilt);

            cameraTransform.localRotation = Quaternion.Lerp(cameraTransform.localRotation, targetRotation, Time.deltaTime * tiltSpeed);
        }
        else
        {
            // Smoothly return the camera tilt to the default value when not moving
            Quaternion targetRotation = Quaternion.Euler(cameraTransform.localRotation.eulerAngles.x,
                                                         cameraTransform.localRotation.eulerAngles.y,
                                                         defaultCameraTilt);

            cameraTransform.localRotation = Quaternion.Lerp(cameraTransform.localRotation, targetRotation, Time.deltaTime * tiltSpeed);
        }
    }
}
