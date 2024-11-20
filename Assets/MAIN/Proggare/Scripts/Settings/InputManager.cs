using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;

public class InputManager : MonoBehaviour
{
    public static Rebinding inputActions;

    private void Awake()
    {
        if( inputActions == null) { inputActions = new Rebinding(); }
    }

    public static void StartRebind(string actionName, int bindingIndex, TextMeshProUGUI statusText)
    {
        
    }

    static void DoRebind(InputAction actionToRebind, int bindingIndex, TextMeshProUGUI statusText)
    {
        if (actionToRebind == null || bindingIndex < 0) return;

        statusText.text = $"Press a {actionToRebind.expectedControlType}";

        actionToRebind.Disable();

        var rebind = actionToRebind.PerformInteractiveRebinding(bindingIndex);

        rebind.OnComplete(operation =>
        {
            actionToRebind.Enable();
            operation.Dispose();
        });

        rebind.OnCancel(operation =>
        {
            actionToRebind.Disable();
            operation.Dispose();
        });

        //Starts rebinding
        rebind.Start(); 
    } 
}
