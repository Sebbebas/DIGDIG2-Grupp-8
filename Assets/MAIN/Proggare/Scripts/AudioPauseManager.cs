using UnityEngine;

public class AudioPauseManager : MonoBehaviour
{
    private AudioSource audioSource;

    private bool isPaused = false;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }

    void Update()
    {
        if (Time.timeScale == 0 && audioSource.isPlaying && !isPaused)
        {
            audioSource.Pause();
            isPaused = true;
        }
        else if (Time.timeScale > 0 && isPaused)
        {
            audioSource.UnPause();
            isPaused = false;
        }
    }
}
