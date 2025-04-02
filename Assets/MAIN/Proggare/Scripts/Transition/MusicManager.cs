using UnityEngine;

public class MusicManager : MonoBehaviour
{
    //Configurable Parameters
    [SerializeField] AudioSource audioSource;

    //Private Variables


    //Cached References
    SceneLoader sceneLoader;

    void Start()
    {
        sceneLoader = FindFirstObjectByType<SceneLoader>();
    }

    void Update()
    {

    }

    public void MusicFadeOut()
    {
        audioSource.volume -= 0.1f;
        if (audioSource.volume <= 0)
        {
            audioSource.Stop();
            audioSource.volume = 1;
        }
    }
}
