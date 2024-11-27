using UnityEngine;
using UnityEngine.InputSystem;

public class Interact : MonoBehaviour
{
    [SerializeField] InputActionAsset playerInputActions;

    [Header("Interaction area")]
    [SerializeField] float radius = 10f;
    [SerializeField] float coneAngle = 20f;
    [SerializeField, Tooltip("Amount of rays cast when drawing gizmo")] int coneResolution = 20;

    [Header("Interaction variables")]
    [SerializeField] float interactionCooldown = 1.5f;

    InputAction interactAction;

    bool canInteract;
    bool interacting;

    private void OnEnable()
    {
        var actionMap = playerInputActions.FindActionMap("Player");

        //Get actions from player ActionMap
        interactAction = actionMap.FindAction("Interact");

        //Enable the action
        interactAction.Enable();

        //Subscribe to the performed callback
        interactAction.performed += OnInteract;
    }

    void OnInteract(InputAction.CallbackContext context)
    {
        Debug.Log("interacting");
    }

    private void OnDrawGizmosSelected()
    {
        //Interact Cone
        Gizmos.color = Color.cyan;

        //Render 1 line for every "coneResolution"
        for (int i = 0; i <= coneResolution; i++)
        {
            //Angle spread between the rays
            float currentAngle = Mathf.Lerp(-coneAngle, coneAngle, i / (float)coneResolution);

            //Calculate the direction of the cone's side ray
            Quaternion rotation = Quaternion.Euler(0, currentAngle, 0);
            Vector3 direction = rotation * transform.forward;

            //Draw the ray
            Gizmos.DrawLine(transform.position, transform.position + direction * radius);
        }
    }
}
