using System.Collections;
using UnityEngine;

public class MusicManager : MonoBehaviour
{
    //Configurable Parameters
    [SerializeField] AudioSource audioSource;

    //Private Variables


    //Cached References
    TransitionManager transitionManager;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        transitionManager = GetComponentInParent<TransitionManager>();
    }

    //MUSIC FADE OUT
    public void MusicFadeOut()
    {
        StopCoroutine(MusicFadeInRoutine());
        StartCoroutine(MusicFadeOutRoutine());
    }

    public IEnumerator MusicFadeOutRoutine()
    {
        float fadeProcent = transitionManager.GetfadeProcent();
        float fadeTime = transitionManager.GetfadeTime();
        float fadeStep = fadeProcent * audioSource.volume;
        float waitTime = fadeTime * fadeProcent;

        while (audioSource.volume > 0)
        {
            audioSource.volume -= fadeStep;
            yield return new WaitForSeconds(waitTime);

            if (audioSource.volume <= 0)
            {
                audioSource.Stop();
                audioSource.volume = 1;
            }
        }
    }

    //MUSIC FADE IN
    public void MusicFadeIn()
    {
        StopCoroutine(MusicFadeOutRoutine());
        StartCoroutine(MusicFadeInRoutine());
    }

    public IEnumerator MusicFadeInRoutine()
    {
        float fadeProcent = transitionManager.GetfadeProcent();
        float fadeTime = transitionManager.GetfadeTime();
        float fadeStep = fadeProcent;
        float waitTime = fadeTime * fadeProcent;

        audioSource.volume = 0;
        audioSource.Play();

        while (audioSource.volume < 1)
        {
            audioSource.volume += fadeStep;
            yield return new WaitForSeconds(waitTime);

            if (audioSource.volume >= 1)
            {
                audioSource.volume = 1;
            }
        }
    }

    public void SetAudioSourceVolume(int volume)
    {
        audioSource.volume = volume;
    }
}
