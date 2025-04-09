using System;
using System.Collections;
using UnityEngine;

public class MusicManager : MonoBehaviour
{
    //Configurable Parameters
    [SerializeField] AudioSource audioSource;

    [SerializeField] GameObject[] musicObject;

    [Space]

    [SerializeField] SongStruct[] songsStructs;

    [System.Serializable]
    public struct SongStruct
    {
        public AudioClip song;
        [Range(0, 1)] public float volume;
        [Range(0, 256)]public float priority;
    }

    //Private Variables


    //Cached References
    TransitionManager transitionManager;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        transitionManager = GetComponentInParent<TransitionManager>();

        musicObject = new GameObject[songsStructs.Length];

        for (int i = 0; i < songsStructs.Length; i++)
        {
            SongStruct songStruct = songsStructs[i];
            GameObject songObject = new(songStruct.song.name);
            songObject.transform.SetParent(FindFirstObjectByType<MusicManager>().transform);

            // Add songObject to musicObject[]
            musicObject[i] = songObject;

            AudioSource songAudioSource = songObject.AddComponent<AudioSource>();
            AudioPauseManager songAudioPauseManager = songObject.AddComponent<AudioPauseManager>();
            songAudioSource.clip = songStruct.song;
            songAudioSource.playOnAwake = true;
            songAudioSource.loop = true;

            songAudioSource.Play();
        }
    }

    private void Update()
    {
        for(int i = 0; i < musicObject.Length; i++)
        {
            AudioSource songAudioSource = musicObject[i].GetComponent<AudioSource>();
            songAudioSource.volume = songsStructs[i].volume;
            songAudioSource.priority = (int)songsStructs[i].priority;
        }
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
