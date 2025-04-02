using System.Collections;
using UnityEngine;

public class TransitionManager : MonoBehaviour
{
    //Configurable Parameters
    [Header("Music")]
    [SerializeField] bool fadeIn = true;
    [SerializeField] float fadeProcent = 0.05f;
    [SerializeField] float fadeTime = 2;

    [Header("Transition")]
    [SerializeField] GameObject transition;
    //Private Variables


    //Cached References
    SceneLoader sceneLoader;
    MusicManager musicManager;

    void Start()
    {
        if(transition != null)
        {
            transition.SetActive(true);
        }

        sceneLoader = FindFirstObjectByType<SceneLoader>();
        musicManager = FindFirstObjectByType<MusicManager>();

        if (fadeIn)
        {
            musicManager.SetAudioSourceVolume(0);
            musicManager.MusicFadeIn();
        }
    }

    public void LoadScene(int scene)
    {
        StartCoroutine(LoadSceneRoutine(scene));
    }

    IEnumerator LoadSceneRoutine(int scene)
    {
        //Play Transition
        sceneLoader.PlayAction(3);
        musicManager.MusicFadeOut();

        //WAIT
        yield return new WaitForSeconds(fadeTime);

        //LOAD
        sceneLoader.LoadSceneNumber(scene);
    }
    
    public void Quit()
    {
        StartCoroutine(QuitRoutine());
    }
    IEnumerator QuitRoutine()
    {
        //Play Transition
        sceneLoader.PlayAction(3);
        musicManager.MusicFadeOut();

        //WAIT
        yield return new WaitForSeconds(fadeTime);

        //LOAD
        sceneLoader.Quit();
    }

    public float GetfadeProcent()
    {
        return fadeProcent;
    }
    public float GetfadeTime()
    {
        return fadeTime;
    }
}
