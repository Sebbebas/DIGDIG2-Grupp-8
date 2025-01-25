using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using TMPro;
using JetBrains.Annotations;
using Unity.VisualScripting;

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
    [Tooltip("Checks if apply button has been pressed")] bool isSaved = true;
    [Tooltip("Makes warning text only appear when a value is changed")] bool valueChanged;
    bool bindingChanged;

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

        weaponManager = FindFirstObjectByType<WeaponManager>();



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
        weaponCamSlider.onValueChanged.AddListener(overlayCamFOV => OnWeaponFOVChange((int)overlayCamFOV));
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
            pauseCanvas?.SetActive(false);
            gamePausedManually = true;
        }

        if(valueChanged)
        {
            isSaved = false;
            applyButton.interactable = true;
        }

        if (sensitivitySlider.value != defaultSensitivity || mainCamSlider.value != defaultMainFOV || weaponCamSlider.value != defaultWeaponFOV || valueChanged)
        {
            settingsRevertButton.interactable = true;
        }
        if (sensitivitySlider.value == defaultSensitivity && mainCamSlider.value == defaultMainFOV && weaponCamSlider.value == defaultWeaponFOV)
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

    //Don't work for some reason
    /*void OnDisable()
    {
        //Disable the action
        pauseAction.Disable();

        //Unsubscribe from the input when the object is disabled
        pauseAction.performed -= OnPause;
    }*/

    //Add into revert button
    public void ResetAllBindings()
    {
        foreach(InputActionMap map in inputActions.actionMaps)
        {
            map.RemoveAllBindingOverrides();
        }
        PlayerPrefs.DeleteKey("rebinds");
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

        valueChanged = true;
    }

    void OnFOVChange(int FOVValue)
    {
        mainCamFOV = FOVValue;

        if (FOVAmount != null)
        {
            FOVAmount.text = FOVValue + "";
        }

        valueChanged = true;
    }

    void OnWeaponFOVChange(int weaponFOVValue)
    {
        weaponCamFOV = weaponFOVValue;

        if (weaponFOVAmount != null)
        {
            weaponFOVAmount.text = weaponFOVValue + "";
        }

        valueChanged = true;
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
        if (!isSaved)
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
    public void OnResetClick(int option)
    {
        switch (option)
        {
            //Reset Sensitivity
            case 0:
                sensitivitySlider.value = originalMainFOV;
                break;

            //Reset Main FOV
            case 1:
                mainCamSlider.value = originalWeaponFOV;
                break;

            //Reset Weapon FOV
            case 2: 
                weaponCamSlider.value = originalWeaponFOV;
                break;

            //Reset All
            case 3: 
                sensitivitySlider.value = originalSensitivity;
                mainCamSlider.value = originalMainFOV;
                weaponCamSlider.value = originalWeaponFOV;
                break;

            default:
                Debug.LogWarning("Invalid reset option selected!");
                break;
        }
    }

    //Applies changed settings and saves them
    //Used for apply button and save button in notSavedWarning
    public void OnApplyClick()
    {
        isSaved = true;
        valueChanged = false;
        notSavedWarning?.SetActive(false);
        bindingChanged = false;

        //If "Apply" is press new values are stored
        originalSensitivity = (int)sensitivitySlider.value;
        originalMainFOV = (int)mainCamSlider.value;
        originalWeaponFOV = (int)weaponCamSlider.value;

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

    //Reverts all settings to default
    public void OnSettingsRevertClick()
    {
        revertWarning?.SetActive(true);
    }

    //Revert to previous settings, aka last saved settings
    public void OnRevertClick()
    {
        //Reverts to original settings
        sensitivitySlider.value = originalSensitivity;
        mainCamSlider.value = originalMainFOV;
        weaponCamSlider.value = originalWeaponFOV;

        GetComponentInChildren<PlayerLook>().mouseSensitivity = originalSensitivity;
        mainCamFOV = originalMainFOV;
        weaponCamFOV = originalWeaponFOV;

        notSavedWarning?.SetActive(false);
        Debug.Log("Settings reverted to original values.");
    }

    //Closes settings without saving 
    public void OnCloseClick()
    {
        //If there were any changes and the player hasnt applied them reset to original values
        if (!isSaved)
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

    //Cancel for Revert all settings
    public void OnCancelClick()  
    {
        revertWarning?.SetActive(false);
    }

    //Confirm button for Revert all settings
    public void OnConfirmClick() 
    {
        revertWarning?.SetActive(false);

        //Reverts to default settings
        sensitivitySlider.value = defaultSensitivity;
        mainCamSlider.value = defaultMainFOV;
        weaponCamSlider.value = defaultWeaponFOV;

        GetComponentInChildren<PlayerLook>().mouseSensitivity = defaultSensitivity;
        mainCamFOV = defaultMainFOV;
        weaponCamFOV = defaultWeaponFOV;

        valueChanged = true;
        Debug.Log("Settings reverted to default values.");
    }

    //Apply this to every RebindButton
    /// <summary>
    /// Since variables can't be used in normal scripts and samples this has to be applied to every rebind button,
    /// Makes us able to use other buttons for rebinding such as "Revert" and "Apply"
    /// </summary>
    public void BindingIsChanged()
    {
        bindingChanged = true;
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
