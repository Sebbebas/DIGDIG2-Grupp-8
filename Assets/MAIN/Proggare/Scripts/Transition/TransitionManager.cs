using System.Collections;
using UnityEngine;

public class TransitionManager : MonoBehaviour
{
    //Configurable Parameters


    //Private Variables


    //Cached References
    SceneLoader sceneLoader;
    MusicManager musicManager;

    void Start()
    {
        sceneLoader = FindFirstObjectByType<SceneLoader>();
        musicManager = FindFirstObjectByType<MusicManager>();
    }

    public void TransitionFadeOut()
    {
        StartCoroutine(TransitionFadeOutRoutine());
    }

    IEnumerator TransitionFadeOutRoutine()
    {
        sceneLoader.PlayAction(3);
        musicManager.MusicFadeOut();

        yield return new WaitForSeconds(2);

        sceneLoader.LoadSceneNumber(1);
    }
}
