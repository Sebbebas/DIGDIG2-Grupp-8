using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using TMPro;

public class SettingManager : MonoBehaviour
{
    [Header("InputActionMap")]
    [SerializeField] InputActionAsset inputActions;

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

    private bool gamePausedManually; //If true game is paused
    private bool stopGame;           //Player dead
    bool settingsCanvasOn;

    InputAction pauseAction;
    WeaponManager weaponManager;

    private void Awake()
    {
        weaponManager = FindFirstObjectByType<WeaponManager>();

        pauseCanvas.SetActive(false);
        settingsCanvasOn = false;
        settingsCanvas.SetActive(false);

        #region sliderValues
        sensitivitySlider.minValue = sensitivityMinValue;
        sensitivitySlider.maxValue = sensitivityMaxValue;

        mainCamSlider.minValue = mainFOVMinValue;
        mainCamSlider.maxValue = mainFOVMaxValue;

        overlayCamSlider.minValue = overlayFOVMinValue;
        overlayCamSlider.maxValue = overlayFOVMaxValue;
        #endregion

        sensitivitySlider.onValueChanged.AddListener(sensitivityValue => OnSensitivityChange((int)sensitivityValue));

        mainCamSlider.onValueChanged.AddListener(mainCamFOV => OnFOVChange((int)mainCamFOV));

        overlayCamSlider.onValueChanged.AddListener(overlayCamFOV => OnOverlayFOVChange((int)overlayCamFOV));
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
    }

    void OnFOVChange(int FOVValue)
    {
        mainCamFOV = FOVValue;

        if (FOVAmount != null)
        {
            FOVAmount.text = FOVValue + "";
        }
    }

    void OnOverlayFOVChange(int overlayFOVValue)
    {
        overlayCamFOV = overlayFOVValue;

        if (overlayFOVAmount != null)
        {
            overlayFOVAmount.text = overlayFOVValue + "";
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
    public bool GetGameIsPaused()
    {
        return gamePausedManually;
    }

    public bool GetGameIsStopped()
    {
        return stopGame;
    }
}
