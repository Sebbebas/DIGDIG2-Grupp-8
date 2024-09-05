using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    //--HOW TO USE--
    //PlayAction(int); is used to trigger a specific actionType from SceneActions[]
    //Example: SceneActions[1].actionType = 2 ("loadScene"). If you want to trigger that specific action PlayAction(1) since that is the actionType that correlates with loadScene;
    //Its a bit scuffed

    #region Variables
    //Configurable Parameters
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
            quit = 2
        }

        //Variables
        [Header("Scene")]
        [Tooltip("The type of action")] public ActionType actionType;
        [Tooltip("Load scene")] public int scene;
        [Tooltip("Seconds before action")] public float transitionTime;

        [Header("Animations")]
        [Tooltip("Animator")] public Animator animator;

        [Header("Parameters")]
        [Tooltip("Animator parameter type")] public AnimatorVariable parameterType;
        [Tooltip("Animator parameter name")] public string parameterName;
        [Tooltip("Animator parameter value")] public float parameterValue;
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
}
