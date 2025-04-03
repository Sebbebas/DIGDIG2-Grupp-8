using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using TMPro;

public class SettingsHandler : MonoBehaviour
{
    [Header("Settings Enviroment")]
    [SerializeField] Button applyButton;
    [SerializeField] Button revertButton;
    [SerializeField] Button backButton;

    [Header("Warning")]
    [SerializeField] GameObject noSaveWarning;

    bool valueChanged; //public
}
