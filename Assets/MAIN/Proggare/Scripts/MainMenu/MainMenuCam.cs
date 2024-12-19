using UnityEngine;

public class MainMenuCam : MonoBehaviour
{
    [SerializeField] Rect rotationBounds;
    [SerializeField] float smoothSpeed = 5f;

    private Quaternion targetRotation;

    private void Update()
    {
        // Get the mouse position as a normalized value (0 to 1) relative to the screen
        Vector2 mousePosition = Input.mousePosition;
        float normalizedX = mousePosition.x / Screen.width;
        float normalizedY = mousePosition.y / Screen.height;

        // Map normalized values to the rotation bounds
        float targetPitch = Mathf.Lerp(rotationBounds.yMin, rotationBounds.yMax, normalizedY); // Up-down rotation
        float targetYaw = Mathf.Lerp(rotationBounds.xMin, rotationBounds.xMax, normalizedX);   // Left-right rotation

        // Create the target rotation
        targetRotation = Quaternion.Euler(targetPitch, targetYaw, 0);

        // Smoothly rotate the camera
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * smoothSpeed);
    }

    private void OnDrawGizmosSelected()
    {
        // Visualize the bounds in the editor (not rotation-specific, but can help debug)
        Gizmos.color = Color.green;
        Gizmos.DrawWireCube(Vector3.zero, new Vector3(rotationBounds.width, rotationBounds.height, 0));
    }
}
