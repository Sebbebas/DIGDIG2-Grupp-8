using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using TMPro;

public class SettingManager : MonoBehaviour
{
    [Header("InputActionMap")]
    [SerializeField] InputActionAsset inputActions;

    [Header("General settings shit")]
    [SerializeField] GameObject warningText;

    [Header("Mouse Settings")]
    [SerializeField] Slider sensitivitySlider;
    [SerializeField, Tooltip("Shows sensValue of sensitivity to player")] TextMeshProUGUI sensitivityAmount;
    [SerializeField] int sensitivityMinValue = 1;
    [SerializeField] int sensitivityMaxValue = 200;

    [Header("Camera Settings")]
    [SerializeField] Camera mainCamera;
    [SerializeField] Slider mainCamSlider;
    [SerializeField] TextMeshProUGUI FOVAmount;
    [SerializeField] int mainCamFOV = 60;
    [SerializeField] int mainFOVMinValue = 1;
    [SerializeField] int mainFOVMaxValue = 120;
    [SerializeField] Camera overlayCamera;
    [SerializeField] Slider overlayCamSlider;
    [SerializeField] TextMeshProUGUI overlayFOVAmount;
    [SerializeField] int overlayCamFOV = 60;
    [SerializeField] int overlayFOVMinValue = 1;
    [SerializeField] int overlayFOVMaxValue = 120;

    [Header("Manually Pause Game")]
    [SerializeField] GameObject pauseCanvas;
    [SerializeField] GameObject settingsCanvas;

    [Tooltip("If true game is paused")] bool gamePausedManually;
    [Tooltip("Player dead")] bool stopGame;
    bool settingsCanvasOn;
    [Tooltip("Checks if apply button has been pressed")] bool isSaved = true;
    int originalSensitivity;
    int originalMainFOV;
    int originalOverlayFOV;

    InputAction pauseAction;
    WeaponManager weaponManager;

    private void Awake()
    {
        //Load sensitivity from PlayerPrefs defaults to 100
        int savedSensitivity = PlayerPrefs.GetInt("Sensitivity", 100);
        //Set the slider value
        sensitivitySlider.value = savedSensitivity;
        //Apply to PlayerLook
        GetComponentInChildren<PlayerLook>().mouseSensitivity = savedSensitivity; 
        Debug.Log("Loaded sensitivity: " + savedSensitivity);

        //Load main FOV from PlayerPrefs defaults to 60
        int savedMainFOV = PlayerPrefs.GetInt("Main FOV", 60);
        //Set the slider value
        mainCamSlider.value = savedMainFOV;
        //Apply to PlayerLook
        mainCamFOV = savedMainFOV;
        Debug.Log("Loaded main FOV: " + savedMainFOV);

        //Load overlay FOV from PlayerPrefs defaults to 60
        int savedOverlayFOV = PlayerPrefs.GetInt("Overlay FOV", 60);
        //Set the slider value
        overlayCamSlider.value = savedOverlayFOV;
        //Apply to PlayerLook
        overlayCamFOV = savedOverlayFOV;
        Debug.Log("Loaded overlay FOV: " + savedOverlayFOV);

        weaponManager = FindFirstObjectByType<WeaponManager>();

        pauseCanvas.SetActive(false);
        settingsCanvasOn = false;
        settingsCanvas.SetActive(false);
        warningText.SetActive(false);

        #region sliderValues
        sensitivitySlider.minValue = sensitivityMinValue;
        sensitivitySlider.maxValue = sensitivityMaxValue;

        mainCamSlider.minValue = mainFOVMinValue;
        mainCamSlider.maxValue = mainFOVMaxValue;

        overlayCamSlider.minValue = overlayFOVMinValue;
        overlayCamSlider.maxValue = overlayFOVMaxValue;
        #endregion

        #region Sliders
        //Makes value of slider decide mouse sensitivity
        sensitivitySlider.onValueChanged.AddListener(sensitivityValue => OnSensitivityChange((int)sensitivityValue));
        sensitivitySlider.interactable = true;

        //Makes value of slider decide value of FOV for main camera
        mainCamSlider.onValueChanged.AddListener(mainCamFOV => OnFOVChange((int)mainCamFOV));
        
        //Makes value of slider decide how far away weapon are i HUD
        overlayCamSlider.onValueChanged.AddListener(overlayCamFOV => OnOverlayFOVChange((int)overlayCamFOV));
        #endregion
    }

    void Update()
    {
        PauseGame();

        //currentPauseState = gamePausedManually;

        while (GetComponent<PlayerHealth>().GetIsDead()) { stopGame = true; Time.timeScale = 0; return; }

        if (sensitivityAmount != null)
        {
            sensitivityAmount.text = sensitivitySlider.value.ToString();
        }

        mainCamera.fieldOfView = mainCamFOV;
        if (FOVAmount != null)
        {
            FOVAmount.text = mainCamSlider.value.ToString(); 
        }

        overlayCamera.fieldOfView = overlayCamFOV;
        if (overlayFOVAmount != null)
        {
            overlayFOVAmount.text = overlayCamSlider.value.ToString();
        }

        if (settingsCanvasOn)
        {
            pauseCanvas.SetActive(false);
            gamePausedManually = true;
        }

        if (warningText.activeInHierarchy == true)
        {
            sensitivitySlider.interactable = false;
            mainCamSlider.interactable = false;
            overlayCamSlider.interactable = false;
        }
        else 
        {
            sensitivitySlider.interactable = true;
            mainCamSlider.interactable = true;
            overlayCamSlider.interactable = true;
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

    void PauseGame()
    {
        Time.timeScale = gamePausedManually ? 0 : 1;

        Weapon currentweapon = weaponManager.GetCurrentWeapon().GetComponent<Weapon>();

        if (gamePausedManually || stopGame)
        {
            currentweapon.enabled = false;

            weaponManager.enabled = false;
        }
        else
        {
            currentweapon.enabled = true;

            weaponManager.enabled = true;
        }
    }

    void OnSensitivityChange(int sensValue)
    {
        GetComponentInChildren<PlayerLook>().mouseSensitivity = sensValue;

        if (sensitivityAmount != null)
        {
            sensitivityAmount.text = sensValue + "";
        }

        isSaved = false;
    }

    void OnFOVChange(int FOVValue)
    {
        mainCamFOV = FOVValue;

        if (FOVAmount != null)
        {
            FOVAmount.text = FOVValue + "";
        }

        isSaved = false;
    }

    void OnOverlayFOVChange(int overlayFOVValue)
    {
        overlayCamFOV = overlayFOVValue;

        if (overlayFOVAmount != null)
        {
            overlayFOVAmount.text = overlayFOVValue + "";
        }

        isSaved = false;
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
    }

    public void OnResumeClicks()
    {
        pauseCanvas.SetActive(false);
        gamePausedManually = false;
    }

    public void OnSettingsClick()
    {
        pauseCanvas.SetActive(false);
        settingsCanvas.SetActive(true);
        settingsCanvasOn = true;

        //Store original settings
        originalSensitivity = (int)sensitivitySlider.value;
        originalMainFOV = (int)mainCamSlider.value;
        originalOverlayFOV = (int)overlayCamSlider.value;
    }

    public void OnBackClick()
    {
        if (!isSaved)
        {
            warningText.SetActive(true);
        }
        else
        {
            pauseCanvas.SetActive(true);
            settingsCanvas.SetActive(false);
            settingsCanvasOn = false;
        }
    }

    //Applies changed settings and saves them
    //Used for apply button and save button in warningText
    public void OnApplyClick()
    {
        isSaved = true;
        warningText.SetActive(false);

        //If "Apply" is press new values are stored
        originalSensitivity = (int)sensitivitySlider.value;
        originalMainFOV = (int)mainCamSlider.value;
        originalOverlayFOV = (int)overlayCamSlider.value;

        //Save the sensitivity value using PlayerPrefs
        PlayerPrefs.SetInt("Sensitivity", GetComponentInChildren<PlayerLook>().mouseSensitivity);
        PlayerPrefs.Save();
        Debug.Log("Sensitivity saved: " + GetComponentInChildren<PlayerLook>().mouseSensitivity);

        //Saves the main FOV value using PlayerPrefs
        PlayerPrefs.SetInt("Main FOV", mainCamFOV);
        PlayerPrefs.Save();
        Debug.Log("Main FOV saved: " + mainCamFOV);

        //Saves the overlay FOV value using PlayerPrefs
        PlayerPrefs.SetInt("Overlay FOV", overlayCamFOV);
        PlayerPrefs.Save();
        Debug.Log("Overlay FOV saved: " + overlayCamFOV);
    }

    public void OnRevertClick()
    {
        //Reverts to original settings
        sensitivitySlider.value = originalSensitivity;
        mainCamSlider.value = originalMainFOV;
        overlayCamSlider.value = originalOverlayFOV;

        GetComponentInChildren<PlayerLook>().mouseSensitivity = originalSensitivity;
        mainCamFOV = originalMainFOV;
        overlayCamFOV = originalOverlayFOV;

        warningText.SetActive(false);
        Debug.Log("Settings reverted to original values.");
    }

    public void OnCloseClick()
    {
        //If there were any changes and the player hasnt applied them reset to original values
        if (!isSaved)
        {
            //Reset the sliders and values to their original states
            sensitivitySlider.value = originalSensitivity;
            mainCamSlider.value = originalMainFOV;
            overlayCamSlider.value = originalOverlayFOV;

            //Revert the values to their original states in the PlayerLook and camera settings
            GetComponentInChildren<PlayerLook>().mouseSensitivity = originalSensitivity;
            mainCamFOV = originalMainFOV;
            overlayCamFOV = originalOverlayFOV;

            warningText.SetActive(false);
            Debug.Log("Settings reverted to original values.");
        }

        //Close the settings menu without saving the changes
        pauseCanvas.SetActive(true);
        settingsCanvas.SetActive(false);
        settingsCanvasOn = false;

        Debug.Log("Settings menu closed without saving changes.");
    }

    /// <summary>
    /// Gets bool gamePausedManually
    /// </summary>
    public bool GetGameIsPaused()
    {
        return gamePausedManually;
    }

    public bool GetGameIsStopped()
    {
        return stopGame;
    }
}
