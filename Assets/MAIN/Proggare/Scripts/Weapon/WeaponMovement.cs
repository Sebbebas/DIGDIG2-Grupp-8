using UnityEngine;

//Sebbe

public class WeaponMovement : MonoBehaviour
{
    //Configurable Parameters
    [Header("Weapon Bobbing Settings")]
    [SerializeField] float bobFrequency = 5.0f;
    [SerializeField] float bobAmplitude = 0.05f;
    [SerializeField] float transitionSpeed = 5f;

    [Header("Weapon Sway Settings")]
    [SerializeField] float swayAmount = 0.02f;
    [SerializeField] float swaySmoothness = 4.0f;
    [SerializeField] float maxSwayAmount = 0.72f;

    //Private Variables
    private Vector3 originalPosition;
    private float timer = 0.0f;
    private float defaultYPos;

    //Cached References
    PlayerMovement playerMovement;
    PlayerLook playerLook;

    void Start()
    {
        playerMovement = FindFirstObjectByType<PlayerMovement>();
        playerLook = FindFirstObjectByType<PlayerLook>();

        originalPosition = transform.localPosition;
        defaultYPos = originalPosition.y;
    }

    void Update()
    {
        HandleBobbing();
        HandleSway();
    }

    // Method to handle weapon bobbing while the player moves
    void HandleBobbing()
    {
        // Get player movement input (ignore vertical axis)
        float speed = new Vector3(playerMovement.GetMoveInput().x, 0, playerMovement.GetMoveInput().y).magnitude;

        // Only apply bobbing when the player is moving
        if (speed > 0.1f)
        {
            // Increase the timer based on time and bob frequency
            timer += Time.deltaTime * bobFrequency;
            float bobOffset = Mathf.Sin(timer) * bobAmplitude;  // Calculate the bob offset using sine wave

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

    // Method to handle weapon sway while looking around with the mouse
    void HandleSway()
    {
        // Get mouse input
        float mouseX = playerLook.GetlookInput().x;
        float mouseY = playerLook.GetlookInput().y;

        // Calculate the target position offset based on mouse movement
        Vector3 swayOffset = new Vector3(-mouseX * swayAmount, -mouseY * swayAmount, 0);

        // Clamp the sway amount to limit extreme movement
        swayOffset.x = Mathf.Clamp(swayOffset.x, -maxSwayAmount, maxSwayAmount);
        swayOffset.y = Mathf.Clamp(swayOffset.y, -maxSwayAmount, maxSwayAmount);

        // Smoothly transition between current and new sway position
        transform.localPosition = Vector3.Lerp(transform.localPosition, originalPosition + swayOffset, Time.deltaTime * swaySmoothness);
    }
}
