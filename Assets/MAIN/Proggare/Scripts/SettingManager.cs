using UnityEngine;
using UnityEngine.InputSystem;

public class SettingManager : MonoBehaviour
{
    [Header("InputActionMap")]
    [SerializeField] InputActionAsset inputActions;

    InputAction pauseAction;

    [SerializeField] GameObject pauseBackground;

    [HideInInspector] public bool gameIsPaused;

    void Update()
    {
        if (GetComponent<PlayerHealth>().isDead) 
        { 
            pauseBackground.SetActive(false);
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

    void OnPause(InputValue value)
    {
        InvertGameIsPaused();
    }

    /// <summary>
    /// Call function when you want to pause or resume game
    /// </summary>
    public void InvertGameIsPaused()
    {
        //If false true If true false
        gameIsPaused = gameIsPaused ? false : true;

        //Set values
        pauseBackground.SetActive(gameIsPaused);
        Time.timeScale = gameIsPaused ? 0 : 1;
    }
    public bool GetIsPaused()
    {
        return gameIsPaused;
    }
}
