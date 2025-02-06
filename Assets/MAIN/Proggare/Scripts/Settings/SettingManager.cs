using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using TMPro;

public class SettingManager : MonoBehaviour
{
    [Header("InputActionMap")]
    [SerializeField] InputActionAsset inputActions;

    [Header("General settings shit")]
    [SerializeField] GameObject notSavedWarning;
    [SerializeField] GameObject revertWarning;
    [SerializeField] Button settingsRevertButton;
    [SerializeField] Button backButton;
    [SerializeField] Button applyButton; 

    [Header("Mouse Settings")]
    [SerializeField] Slider sensitivitySlider;
    [SerializeField, Tooltip("Shows sensValue of sensitivity to player")] TextMeshProUGUI sensitivityAmount;
    [SerializeField] int sensitivityMinValue = 1;
    [SerializeField] int sensitivityMaxValue = 200;

    [Header("Camera Settings")]
    [SerializeField] Camera mainCamera;
    [SerializeField] Slider mainCamSlider;
    [SerializeField] TextMeshProUGUI FOVAmount;
    [SerializeField] int mainCamFOV = 90;
    [SerializeField] int mainFOVMinValue = 1;
    [SerializeField] int mainFOVMaxValue = 120;
    [SerializeField] Camera weaponCamera;
    [SerializeField] Slider weaponCamSlider;
    [SerializeField] TextMeshProUGUI weaponFOVAmount;
    [SerializeField] int weaponCamFOV = 60;
    [SerializeField] int weaponFOVMinValue = 1;
    [SerializeField] int weaponFOVMaxValue = 120;

    [Header("Manually Pause Game")]
    [SerializeField] GameObject mainMenu;
    [SerializeField] GameObject pauseCanvas;
    [SerializeField] GameObject settingsCanvas;

    [Tooltip("If true game is paused")] bool gamePausedManually;
    [Tooltip("Player dead")] bool stopGame;
    bool settingsCanvasOn;
    [Tooltip("Makes warning text only appear when a value is changed")] bool valueChanged; 

    int originalSensitivity;
    int originalMainFOV;
    int originalWeaponFOV;

    int defaultSensitivity = 100; 
    int defaultMainFOV = 90;
    int defaultWeaponFOV = 60;

    InputAction pauseAction;
    WeaponManager weaponManager;

    private void Awake()
    {
        #region Save
        //Load sensitivity from PlayerPrefs defaults to 100
        int savedSensitivity = PlayerPrefs.GetInt("Sensitivity", defaultSensitivity);
        //Set the slider value
        sensitivitySlider.value = savedSensitivity;
        //Apply to PlayerLook
        GetComponentInChildren<PlayerLook>().mouseSensitivity = savedSensitivity;
        //Debug.Log("Loaded sensitivity: " + savedSensitivity);

        //Load main FOV from PlayerPrefs defaults to 60
        int savedMainFOV = PlayerPrefs.GetInt("Main FOV", defaultMainFOV);
        //Set the slider value
        mainCamSlider.value = savedMainFOV;
        //Apply to PlayerLook
        mainCamFOV = savedMainFOV;
        //Debug.Log("Loaded main FOV: " + savedMainFOV);

        //Load weapon FOV from PlayerPrefs defaults to 60
        int savedWeaponFOV = PlayerPrefs.GetInt("Weapon FOV", defaultWeaponFOV);
        //Set the slider value
        weaponCamSlider.value = savedWeaponFOV;
        //Apply to PlayerLook
        weaponCamFOV = savedWeaponFOV;
        //Debug.Log("Loaded weapon FOV: " + savedWeaponFOV);
        #endregion

        weaponManager = FindFirstObjectByType<WeaponManager>();

        //Loads new binding
        var rebinds = PlayerPrefs.GetString("rebinds");
        if (!string.IsNullOrEmpty(rebinds))
            inputActions.LoadBindingOverridesFromJson(rebinds);

        pauseCanvas.SetActive(false);
        settingsCanvasOn = false;
        settingsCanvas?.SetActive(false);
        notSavedWarning?.SetActive(false);
        revertWarning?.SetActive(false);

        if(mainMenu != null) { mainMenu.SetActive(false); } else { return; }

        #region sliderValues
        sensitivitySlider.minValue = sensitivityMinValue;
        sensitivitySlider.maxValue = sensitivityMaxValue;

        mainCamSlider.minValue = mainFOVMinValue;
        mainCamSlider.maxValue = mainFOVMaxValue;

        weaponCamSlider.minValue = weaponFOVMinValue;
        weaponCamSlider.maxValue = weaponFOVMaxValue;
        #endregion

        #region Sliders
        //Makes value of slider decide mouse sensitivity
        sensitivitySlider.onValueChanged.AddListener(sensitivityValue => OnSensitivityChange((int)sensitivityValue));

        //Makes value of slider decide value of FOV for main camera
        mainCamSlider.onValueChanged.AddListener(mainCamFOV => OnFOVChange((int)mainCamFOV));
        
        //Makes value of slider decide how far away weapon are i HUD
        weaponCamSlider.onValueChanged.AddListener(weaponCamFOV => OnWeaponFOVChange((int)weaponCamFOV)); 
        #endregion

        applyButton.interactable = false;
        settingsRevertButton.interactable = false;
    }

    void Update()
    {
        PauseGame();

        while (GetComponent<PlayerHealth>().GetIsDead()) { stopGame = true; Time.timeScale = 0; return; }

        sensitivityAmount.text = sensitivitySlider?.value.ToString();

        mainCamera.fieldOfView = mainCamFOV;
        FOVAmount.text = mainCamSlider?.value.ToString();

        weaponCamera.fieldOfView = weaponCamFOV;
        weaponFOVAmount.text = weaponCamSlider?.value.ToString();

        if (settingsCanvasOn)
        {
            pauseCanvas.SetActive(false);
            gamePausedManually = true;
        }

        if (sensitivitySlider.value != defaultSensitivity || mainCamSlider.value != defaultMainFOV || weaponCamSlider.value != defaultWeaponFOV || valueChanged)
        {
            settingsRevertButton.interactable = true;
        }
        else
        {
            settingsRevertButton.interactable = false;
        }
 
        //Sends out warning that settings is not saved, also make sliders non interactable while warning is true
        if (notSavedWarning.activeInHierarchy || revertWarning.activeInHierarchy)
        {
            sensitivitySlider.interactable = false;
            mainCamSlider.interactable = false;
            weaponCamSlider.interactable = false;

            backButton.interactable = false;
            settingsRevertButton.interactable = false;
        }
        else 
        {
            sensitivitySlider.interactable = true;
            mainCamSlider.interactable = true;
            weaponCamSlider.interactable = true;

            backButton.interactable = true;
        }

        if (!valueChanged || notSavedWarning.activeInHierarchy || revertWarning.activeInHierarchy)
        {
            applyButton.interactable = false;
        }
        else
        {
            applyButton.interactable = true;
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
    }

    void OnFOVChange(int FOVValue)
    {
        mainCamFOV = FOVValue;

        if (FOVAmount != null)
        {
            FOVAmount.text = FOVValue + "";
        }
    }

    void OnWeaponFOVChange(int weaponFOVValue)
    {
        weaponCamFOV = weaponFOVValue;

        if (weaponFOVAmount != null)
        {
            weaponFOVAmount.text = weaponFOVValue + "";
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
        pauseCanvas?.SetActive(gamePausedManually);
    }

    //bool ValueIsChanged()
    //{
    //    return valueChanged;
    //}

    #region UI Buttons
    //Unpauses game
    public void OnResumeClicks()
    {
        pauseCanvas?.SetActive(false);
        gamePausedManually = false;
    }

    //Opens settings
    public void OnSettingsClick()
    {
        pauseCanvas?.SetActive(false);
        settingsCanvas?.SetActive(true);
        settingsCanvasOn = true;

        //Store original settings
        originalSensitivity = (int)sensitivitySlider.value;
        originalMainFOV = (int)mainCamSlider.value;
        originalWeaponFOV = (int)weaponCamSlider.value;
    }

    //Close settings menu, gets warning if not saved
    public void OnBackClick()
    {
        if (valueChanged)
        {
            notSavedWarning?.SetActive(true);
        }
        else
        {
            pauseCanvas?.SetActive(true);
            //mainMenu?.SetActive(true);
            settingsCanvas?.SetActive(false);
            settingsCanvasOn = false;
        }
    }

    //Reset button for sliders
    public void OnResetClick(string option)
    {
        switch (option)
        {
            //Reset Sensitivity
            case "Sensitivity":
                if(sensitivitySlider.value != defaultSensitivity)
                {
                    sensitivitySlider.value = defaultSensitivity;
                    valueChanged = true;
                }
                else { return; }
                break;

            //Reset Main FOV
            case "MainFOV":
                if(mainCamSlider.value != defaultMainFOV)
                {
                    mainCamSlider.value = defaultMainFOV;
                    valueChanged = true;
                }
                else { return; }
                break;

            //Reset Weapon FOV
            case "WeaponFOV": 
                if(weaponCamSlider.value != defaultWeaponFOV)
                {
                    weaponCamSlider.value = defaultWeaponFOV;
                    valueChanged = true;
                }
                else { return; }
                break;

            //Reset All
            case "Reset All": 
                sensitivitySlider.value = defaultSensitivity;
                mainCamSlider.value = defaultMainFOV;
                weaponCamSlider.value = defaultWeaponFOV;
                valueChanged = true;
                break;

            default:
                Debug.LogWarning("Invalid reset option selected");
                break;
        }
    }

    //Applies changed settings and saves them
    //Used for apply button and save button in notSavedWarning
    public void OnApplyClick()
    {
        valueChanged = false;
        notSavedWarning?.SetActive(false);

        //If "Apply" is press new values are stored
        originalSensitivity = (int)sensitivitySlider.value;
        originalMainFOV = (int)mainCamSlider.value;
        originalWeaponFOV = (int)weaponCamSlider.value;

        //Saves bindings
        var rebinds = inputActions.SaveBindingOverridesAsJson();
        PlayerPrefs.SetString("rebinds", rebinds);

        //Save the sensitivity value using PlayerPrefs
        PlayerPrefs.SetInt("Sensitivity", GetComponentInChildren<PlayerLook>().mouseSensitivity);
        PlayerPrefs.Save();
        Debug.Log("Sensitivity saved: " + GetComponentInChildren<PlayerLook>().mouseSensitivity);

        //Saves the main FOV value using PlayerPrefs
        PlayerPrefs.SetInt("Main FOV", mainCamFOV);
        PlayerPrefs.Save();
        Debug.Log("Main FOV saved: " + mainCamFOV);

        //Saves the weapon FOV value using PlayerPrefs
        PlayerPrefs.SetInt("Weapon FOV", weaponCamFOV);
        PlayerPrefs.Save();
        Debug.Log("Weapon FOV saved: " + weaponCamFOV);
    }

    public void OnRevertClick(int option)
    {
        switch (option)
        {
            //Reverts to original settings and closing window
            case 0:
                sensitivitySlider.value = originalSensitivity;
                mainCamSlider.value = originalMainFOV;
                weaponCamSlider.value = originalWeaponFOV;

                GetComponentInChildren<PlayerLook>().mouseSensitivity = originalSensitivity;
                mainCamFOV = originalMainFOV;
                weaponCamFOV = originalWeaponFOV;

                notSavedWarning.SetActive(false);
                pauseCanvas?.SetActive(true);
                settingsCanvas?.SetActive(false);
                settingsCanvasOn = false;
                valueChanged = false;
                Debug.Log("Settings reverted to original values.");
                break;

            //Resets to original settings without closing window
            case 1:
                sensitivitySlider.value = originalSensitivity;
                mainCamSlider.value = originalMainFOV;
                weaponCamSlider.value = originalWeaponFOV;

                GetComponentInChildren<PlayerLook>().mouseSensitivity = originalSensitivity;
                mainCamFOV = originalMainFOV;
                weaponCamFOV = originalWeaponFOV;

                //Resets all bindings
                foreach (InputActionMap map in inputActions.actionMaps)
                {
                    map.RemoveAllBindingOverrides();
                }
                PlayerPrefs.DeleteKey("rebinds");

                valueChanged = false;
                notSavedWarning.SetActive(false);
                break;

            //Reverts to default settings
            case 2:
                revertWarning.SetActive(true);
                break;

            //Cancel warning before reverting to default settings
            case 3:
                revertWarning.SetActive(false);
                break;

            //Confirms warning before reverting to default settings
            case 4:
                revertWarning?.SetActive(false);

                sensitivitySlider.value = defaultSensitivity;
                mainCamSlider.value = defaultMainFOV;
                weaponCamSlider.value = defaultWeaponFOV;

                GetComponentInChildren<PlayerLook>().mouseSensitivity = defaultSensitivity;
                mainCamFOV = defaultMainFOV;
                weaponCamFOV = defaultWeaponFOV;

                foreach (InputActionMap map in inputActions.actionMaps)
                {
                    map.RemoveAllBindingOverrides();
                }
                PlayerPrefs.DeleteKey("rebinds");

                valueChanged = false;
                Debug.Log("Settings reverted to default values.");
                break;
        }
    }

    //Closes settings without saving 
    public void OnCloseClick()
    {
        //If there were any changes and the player hasnt applied them reset to original values
        if (valueChanged)
        {
            //Reset the sliders and values to their original states
            sensitivitySlider.value = originalSensitivity;
            mainCamSlider.value = originalMainFOV;
            weaponCamSlider.value = originalWeaponFOV;

            //Revert the values to their original states in the PlayerLook and camera settings
            GetComponentInChildren<PlayerLook>().mouseSensitivity = originalSensitivity;
            mainCamFOV = originalMainFOV;
            weaponCamFOV = originalWeaponFOV;

            notSavedWarning.SetActive(false);
            Debug.Log("Settings reverted to original values.");
        }

        //Close the settings menu without saving the changes
        pauseCanvas?.SetActive(true);
        settingsCanvas?.SetActive(false);
        settingsCanvasOn = false;

        Debug.Log("Settings menu closed without saving changes.");
    }

    //Apply this to every RebindButton or Sliders
    /// <summary>
    /// Since variables can't be used in normal scripts and samples this has to be applied to every rebind button,
    /// Makes us able to use other buttons for rebinding such as "Revert" and "Apply"
    /// </summary>
    public void BindingIsChanged()
    {
        valueChanged = true;
    }
    #endregion

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
