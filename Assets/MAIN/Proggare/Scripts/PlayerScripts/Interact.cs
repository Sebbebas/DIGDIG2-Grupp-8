using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;
using UnityEngine.Animations;

public class Interact : MonoBehaviour
{
    [SerializeField] InputActionAsset inputActions;

    InputAction interactAction;

    [SerializeField] Animation buttonPressed;
    [SerializeField] GameObject buttonObject;

    [SerializeField, Tooltip("How far the player can reach in depth")] float interactionRange = 5f;
    [SerializeField, Tooltip("How far the player can reach in width")] float interactionAngle = 30f;
    [SerializeField, Tooltip("Amount of rays cast")] int coneResolution = 20;
    [SerializeField] float buttonPressCooldown = 5f;

    bool canInteract = true;
    bool interacting = false;

    private void Update()
    {
        if (interacting == true) 
        { 
            Debug.Log("Interacting");
        }

        while (interacting == false) { canInteract = true; }
    }

    private void OnEnable()
    {
        //Get player ActionMap
        var actionMap = inputActions.FindActionMap("Player");

        //Get actions from player ActionMap
        interactAction = actionMap.FindAction("Interact");

        //Enable the action
        interactAction.Enable();

        //Subscribe to the performed callback
        interactAction.performed += OnInteract;
    }

    private void OnDisable()
    {
        interactAction.performed -= OnInteract;
    }

    void OnInteract(InputAction.CallbackContext context) 
    {
        if (canInteract == true)
        {
            interacting = true;
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;

        for (int i = 0; i <= coneResolution; i++)
        {
            float currentAngle = Mathf.Lerp(-interactionAngle, interactionAngle, i / (float)coneResolution);

            Quaternion rot = Quaternion.Euler(0, currentAngle, 0);
            Vector3 dir = rot * transform.forward;

            Gizmos.DrawLine(transform.position, transform.position + dir * interactionRange);  
        }
    }
}
