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

    void Update()
    {
        
    }

    public void TransitionFadeOut()
    {
        musicManager.MusicFadeOut();
    }
}
