using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using TMPro;
using JetBrains.Annotations;

public class SettingManager : MonoBehaviour
{
    [Header("InputActionMap")]
    [SerializeField] InputActionAsset inputActions;

    InputAction pauseAction;

    [Header("Mouse Settings")]
    [SerializeField] Slider sensitivitySlider;
    [SerializeField] TextMeshProUGUI sensitivityText;
    [SerializeField] GameObject crossair;

    [Header("Manually Pause Game")]
    [SerializeField] GameObject pauseCanvas;
    [SerializeField] GameObject settingsCanvas;

    private bool gamePausedManually; //Pause menu
    private bool stopGame;           //Player Dead
    private bool currentPauseState;  //Pause state

    int mouseSensitivityInt;

    private void Awake()
    {
        currentPauseState = false; 

        pauseCanvas.SetActive(false);
        settingsCanvas.SetActive(false);

        sensitivitySlider.onValueChanged.AddListener(delegate { Debug.Log(sensitivitySlider.value); });
        //sensitivitySlider.value = Mathf.RoundToInt(mouseSensitivityInt);
        //GetComponent<PlayerLook>().mouseSensitivity = mouseSensitivityInt;
    }

    void Update()
    {
        if (GetComponent<PlayerHealth>().GetIsDead()) { stopGame = true; Time.timeScale = 0; return; }

        if (gamePausedManually == false) { settingsCanvas.SetActive(false); }

        if(currentPauseState) { crossair.SetActive(false); }

        if (stopGame)
        {
            currentPauseState = stopGame;
        }
        else
        {
            currentPauseState = gamePausedManually;
        }

        if (sensitivityText != null)
        {
            sensitivityText.text = sensitivitySlider.value.ToString();
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

    void OnDisable()
    {
        //Disable the action
        pauseAction.Disable();

        //Unsubscribe from the input when the object is disabled
        pauseAction.performed -= OnPause;
    }

    void OnPause(InputAction.CallbackContext context) { }

    /// <summary>
    /// Invert the current pause state
    /// </summary>
    void OnPause(InputValue value)
    {
        ManualPause();
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

    public void OnResumeClick()
    {
        pauseCanvas.SetActive(false);
    }

    public void OnSettingsClick()
    {
        pauseCanvas.SetActive(false);
        settingsCanvas.SetActive(true);
    }

    public void OnBackClick()
    {
        pauseCanvas.SetActive(true);
        settingsCanvas.SetActive(false);
    }
    /// <summary>
    /// Gets bool gamePausedManually
    /// </summary>
    public bool GetGameIsStopped()
    {
        return currentPauseState;
    }
}
