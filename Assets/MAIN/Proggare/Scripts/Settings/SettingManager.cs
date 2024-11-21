using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using TMPro;

public class SettingManager : MonoBehaviour
{
    [Header("InputActionMap")]
    [SerializeField] InputActionAsset inputActions;

    InputAction pauseAction;

    [Header("Mouse Settings")]
    [SerializeField] Slider sensitivitySlider;
    [SerializeField, Tooltip("Shows value of sensitivity to player")] TextMeshProUGUI sensitivityText;
    [SerializeField] int sensitivityMinValue = 1;
    [SerializeField] int sensitivityMaxValue = 200;
    [SerializeField] GameObject crossair;

    [Header("Manually Pause Game")]
    [SerializeField] GameObject pauseCanvas;
    [SerializeField] GameObject settingsCanvas;

    private bool gamePausedManually; //Pause menu
    private bool stopGame;           //Player dead
    //private bool currentPauseState;  //Pause state
    bool settingsCanvasOn;

    private void Awake()
    {
        //currentPauseState = false; 
        //gamePausedManually = false;

        pauseCanvas.SetActive(false);
        settingsCanvasOn = false;
        settingsCanvas.SetActive(false);

        sensitivitySlider.minValue = sensitivityMinValue;
        sensitivitySlider.maxValue = sensitivityMaxValue;
        //sensitivitySlider.value = GetComponent<PlayerLook>().mouseSensitivity;

        sensitivitySlider.onValueChanged.AddListener(value => OnSensitivityChange((int)value));
    }

    void Update()
    {
        //currentPauseState = gamePausedManually;

        while (GetComponent<PlayerHealth>().GetIsDead()) { stopGame = true; Time.timeScale = 0; return; }

        //if (gamePausedManually == false) { settingsCanvas.SetActive(false); }

        if (gamePausedManually) { crossair.SetActive(false); }

        if (sensitivityText != null)
        {
            sensitivityText.text = sensitivitySlider.value.ToString();
        }

        if (settingsCanvasOn)
        {
            pauseCanvas.SetActive(false);
            gamePausedManually = true;
        }
    }

    void OnEnable()
    {
        //Get player ActionMap
        var actionMap = inputActions.FindActionMap("Player");

        //Get actions from player ActionMap
        pauseAction = actionMap.FindAction("Pause");

        //Enable the action
        pauseAction.Enable();

        //Subscribe to the performed callback
        pauseAction.performed += OnPause;
    }

    //Don't work for some reason
    /*void OnDisable()
    {
        //Disable the action
        pauseAction.Disable();

        //Unsubscribe from the input when the object is disabled
        pauseAction.performed -= OnPause;
    }*/

    void OnSensitivityChange(int value)
    {
        GetComponent<PlayerLook>().mouseSensitivity = value;

        if (sensitivityText != null)
        {
            sensitivityText.text = "Sensitivity: " + value + "%";
        }
    }

    void OnPause(InputAction.CallbackContext context) { }

    /// <summary>
    /// Invert the current pause state
    /// </summary>
    void OnPause(InputValue value)
    {
        if (!settingsCanvasOn) { ManualPause(); }

        else 
        {
            //Makes esc function as a back button when in settings
            OnBackClick();
            Debug.Log("fuck u");
        }
    }
    /// <summary>
    /// Call function when you want to pause or resume game
    /// </summary>
    public void ManualPause()
    {
        if (stopGame) { return; }

        //If false true || If true false
        gamePausedManually = gamePausedManually ? false : true;

        //Set values acording to current state
        pauseCanvas.SetActive(gamePausedManually);
        Time.timeScale = gamePausedManually ? 0 : 1;
    }

    public void OnSettingsClick()
    {
        pauseCanvas.SetActive(false);
        settingsCanvas.SetActive(true);
        settingsCanvasOn = true;
    }

    public void OnBackClick()
    {
        pauseCanvas.SetActive(true);
        settingsCanvas.SetActive(false);
        settingsCanvasOn = false;
    }
    /// <summary>
    /// Gets bool gamePausedManually
    /// </summary>
    public bool GetGameIsStopped()
    {
        return gamePausedManually;
    }
}
