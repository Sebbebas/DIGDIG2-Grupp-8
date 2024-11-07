using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using TMPro;

public class Rebinding : MonoBehaviour
{
    [SerializeField] InputActionReference inputActionReference;
    [SerializeField, Range(0, 10)] int selectedBinding;
    [SerializeField] InputBinding.DisplayStringOptions displayStringOptions;
    [SerializeField, Tooltip("DO NOT EDIT")] InputBinding inputBinding;

    [Header("UI Fields")]
    [SerializeField] TextMeshProUGUI actionText;
    [SerializeField] Button rebindButton;
    [SerializeField] TextMeshProUGUI rebindText;
    [SerializeField] Button resetButton;

    //private variables
    bool exludeMouse = true;
    int bindingIndex;
    string actionName;

    private void OnEnable()
    {
        rebindButton.onClick.AddListener(() => DoRebind());
        resetButton.onClick.AddListener(() => ResetBinding());

        if(inputActionReference != null)
        {
            GetBindingInfo();
            UpdateUI();
        }
    }

    private void OnValidate()
    {
        if (inputActionReference != null) { return; }
        GetBindingInfo();
        UpdateUI();
    }

    void GetBindingInfo()
    {
        if(inputActionReference.action != null)
        {
            actionName = inputActionReference.action.name;
        }

        if(inputActionReference.action.bindings.Count > selectedBinding)
        {
            inputBinding = inputActionReference.action.bindings[selectedBinding];
            bindingIndex = selectedBinding;
        }
    }

    void UpdateUI()
    {
        if (actionText != null)
        {
            actionText.text = actionName;
        }

        if(rebindText != null)
        {
            if(Application.isPlaying)
            {

            }
            else
            {
                rebindText.text = inputActionReference.action.GetBindingDisplayString(bindingIndex);
            }
        }
    }

    void DoRebind()
    {

    }

    void ResetBinding()
    {
        
    }
}