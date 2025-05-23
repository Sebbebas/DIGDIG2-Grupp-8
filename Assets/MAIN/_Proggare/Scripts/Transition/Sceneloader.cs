using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    #region Variables
    //Configurable Parameters


    //ON LEVEL WAS LOADED PARAMETERS
    [SerializeField] OnLvlLoad onLvlLoad;
    private bool isFrozen = false;

    [System.Serializable]
    public struct OnLvlLoad
    {
        public bool freeze;
        public float unfreezeTime;
    }

    //WARNING TEXT
    [Header("OBS:" +
        "\r\n When using buttons with PlayAction(int), the number corresponds to the ActionType number:" +
        "\r\n- ReloadScene() will always be 0" +
        "\r\n- LoadScene() will always be 1" +
        "\r\n- Quit() will always be 2" +
        "\r\n- None() will always be 3" +
        "\r\n- Also dose not work in a build idk why")]

    //SCENE ACTIONS PARAMETERS
    [Header("Actions")]
    [SerializeField] SceneActions[] actions;

    [System.Serializable]
    public struct SceneActions
    {
        //ENUMS
        public enum AnimatorVariable
        {
            isBool = 0,
            isTrigger = 1,
            isFloat = 2,
            isInt = 3
        }
        public enum ActionType
        {
            reloadScene = 0,
            loadScene = 1,
            quit = 2,
            none = 3,
        }

        //Variables
        [Header("Scene")]
        [Tooltip("The type of action")] public ActionType actionType;
        [Tooltip("Load scene")] public int scene;
        [Tooltip("Seconds before action")] public float transitionTime;

        [Header("Animations")]
        [Tooltip("Animator")] public Animator animator;

        [Header("Animation Parameters")]
        [Tooltip("Animator parameter type")] public AnimatorVariable parameterType;
        [Tooltip("Animator parameter name")] public string parameterName;
        [Tooltip("Animator parameter value")] public float parameterValue;
    }
    #endregion

    private void Awake()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    #region OnLevelWasLoaded
    private void OnLevelWasLoaded(int level)
    {
        if (onLvlLoad.freeze) { Time.timeScale = 1.0f; }

        StartCoroutine(UnFreezeRoutine(onLvlLoad.unfreezeTime));

        isFrozen = onLvlLoad.freeze;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        int sceneIndex = scene.buildIndex;

        if (sceneIndex == 0 || sceneIndex == 2)
        {
            Cursor.lockState = CursorLockMode.Confined;
            Cursor.visible = true;
            Debug.Log("Cursor free in scene " + sceneIndex);
        }
    }

    IEnumerator UnFreezeRoutine(float waitTime)
    {
        yield return new WaitForSeconds(waitTime);
        isFrozen = false;
        Time.timeScale = 1.0f;
    }
    #endregion

    #region Base Scene Actions
    /// <summary>
    /// Loads <paramref name="sceneNumber"/> as the next scene
    /// </summary>
    public void LoadSceneNumber(int sceneNumber)
    {
        SceneManager.LoadScene(sceneNumber);
    }

    /// <summary>
    /// Loads Next Scene
    /// </summary>
    public void LoadNextScene()
    {
        LoadSceneNumber(SceneManager.GetActiveScene().buildIndex + 1);
    }

    /// <summary>
    /// Reload the current scene
    /// </summary>
    public void ReloadScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    /// <summary>
    /// Quits the application
    /// </summary>
    public void Quit()
    {
        Application.Quit();
        Debug.Log("Quit");
    }
    #endregion

    #region Action Functions
    /// <summary>
    /// <paramref name="actionNumber"/> is supposed to be the responding actionType that is meant to be played | 0 = reloadScene 1 = loadScene 2 = quit
    /// </summary>
    public void PlayAction(int actionNumber)
    {
        foreach (SceneActions animation in actions)
        {
            if ((int)animation.actionType == actionNumber)
            {
                StartCoroutine(TransitionRoutine(animation.transitionTime));
            }
        }
    }

    /// <summary>
    /// Plays out all the actions in actions[] after <paramref name="transitionTime"/>
    /// </summary>
    IEnumerator TransitionRoutine(float transitionTime)
    {
        PlayAnimations();

        yield return new WaitForSeconds(transitionTime);

        foreach (SceneActions animation in actions)
        {
            switch (animation.actionType)
            {
                //ReloadScene
                case (SceneActions.ActionType)0:
                    ReloadScene();
                    break;
                //LoadScene
                case (SceneActions.ActionType)1:
                    LoadSceneNumber(animation.scene);
                    break;
                //Quit
                case (SceneActions.ActionType)2:
                    Quit();
                    break;
                case (SceneActions.ActionType)3:
                    Debug.Log("No actionType selected");
                    break;
            }
        }
    }
    #endregion

    #region Animations
    void PlayAnimations()
    {
        foreach (SceneActions animation in actions)
        {
            if (animation.animator == null) { Debug.Log("Missing Animator"); return; }

            switch (animation.parameterType)
            {
                //is bool
                case (SceneActions.AnimatorVariable)0:
                    animation.animator.SetBool(animation.parameterName, true);
                    break;
                //is trigger
                case (SceneActions.AnimatorVariable)1:
                    animation.animator.SetTrigger(animation.parameterName);
                    break;
                //is float
                case (SceneActions.AnimatorVariable)2:
                    animation.animator.SetFloat(animation.parameterName, animation.parameterValue);
                    break;
                //is int
                case (SceneActions.AnimatorVariable)3:
                    animation.animator.SetInteger(animation.parameterName, (int)animation.parameterValue);
                    break;
            }
        }
    }
    #endregion

    public bool GetIsFrozen()
    {
        return isFrozen;
    }
}
