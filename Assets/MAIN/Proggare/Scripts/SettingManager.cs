using UnityEngine;
using UnityEngine.InputSystem;

public class SettingManager : MonoBehaviour
{
    [Header("InputActionMap")]
    [SerializeField] InputActionAsset inputActions;

    InputAction pauseAction;

    [Header("Manually Pause Game")]
    [SerializeField] GameObject pauseBackground;

    private bool gamePausedManually;
    private bool stopGame;

    void Update()
    {
        if (GetComponent<PlayerHealth>().GetIsDead()) { stopGame = true; Time.timeScale = 0; return; }
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
        pauseBackground.SetActive(gamePausedManually);
        Time.timeScale = gamePausedManually ? 0 : 1;
    }
    /// <summary>
    /// Gets bool gamePausedManually
    /// </summary>
    public bool GetGameIsStopped()
    {
        return stopGame;
    }
}
