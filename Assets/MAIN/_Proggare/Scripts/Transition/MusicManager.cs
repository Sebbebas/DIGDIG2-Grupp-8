using System;
using System.Collections;
using UnityEngine;

public class MusicManager : MonoBehaviour
{
    [SerializeField] bool ZombieAgroMusicIntensity = false;
    [SerializeField] float baseVolume = 0.5f;

    // Configurable Parameters
    [SerializeField] AudioSource audioSource;

    [SerializeField] GameObject[] musicObject;

    [Space]

    [SerializeField] SongStruct[] songsStructs;

    // Add serialized variables for thresholds
    [SerializeField] private int minZombieAggroThreshold = 3;
    [SerializeField] private int maxZombieAggroThreshold = 7;

    [System.Serializable]
    public struct SongStruct
    {
        public AudioClip song;
        [Range(0, 1)] public float volume;
        [Range(0, 256)] public float priority;
    }

    // Private Variables

    // Cached References
    TransitionManager transitionManager;
    EnemyBehaviour enemyBehaviour;
    private Coroutine[] fadeCoroutines;

    void Start()
    {
        enemyBehaviour = FindFirstObjectByType<EnemyBehaviour>();
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

            if (i == 0)
                songAudioSource.volume = songsStructs[0].volume * baseVolume;
            else
                songAudioSource.volume = 0f;

            songAudioSource.Play();
        }
    }

    private void Update()
    {
        if (!ZombieAgroMusicIntensity) { return; }

        EnemyScript[] allEnemies = FindObjectsByType<EnemyScript>(FindObjectsSortMode.None);

        int zombiesAgro = 0;
        foreach (var enemy in allEnemies)
        {
            if (enemy.GetAgro())
            {
                zombiesAgro++;
            }
        }

        float fadeDuration = 0.5f;

        // Ambient music always plays
        AudioSource ambientAudioSource = musicObject[0].GetComponent<AudioSource>();
        FadeToVolume(ambientAudioSource, songsStructs[0].volume * baseVolume, fadeDuration);

        if (zombiesAgro < minZombieAggroThreshold)
        {
            for (int i = 1; i < musicObject.Length; i++)
            {
                AudioSource otherAudioSource = musicObject[i].GetComponent<AudioSource>();
                FadeToVolume(otherAudioSource, 0f, fadeDuration);
            }
            return;
        }

        AudioSource secondAudioSource = musicObject[1].GetComponent<AudioSource>();
        AudioSource thirdAudioSource = musicObject[2].GetComponent<AudioSource>();

        if (zombiesAgro >= minZombieAggroThreshold && zombiesAgro < maxZombieAggroThreshold)
        {
            FadeToVolume(secondAudioSource, baseVolume, fadeDuration);
            FadeToVolume(thirdAudioSource, 0f, fadeDuration);
        }
        else if (zombiesAgro >= maxZombieAggroThreshold)
        {
            FadeToVolume(secondAudioSource, 0f, fadeDuration);
            FadeToVolume(thirdAudioSource, baseVolume, fadeDuration);
        }
        else
        {
            FadeToVolume(secondAudioSource, 0f, fadeDuration);
            FadeToVolume(thirdAudioSource, 0f, fadeDuration);
        }
    }

    // MUSIC FADE OUT
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

    // MUSIC FADE IN
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

    private void FadeToVolume(AudioSource source, float targetVolume, float duration)
    {
        if (fadeCoroutines == null)
            fadeCoroutines = new Coroutine[musicObject.Length];

        int index = Array.IndexOf(musicObject, source.gameObject);
        if (index >= 0 && fadeCoroutines[index] != null)
            StopCoroutine(fadeCoroutines[index]);

        fadeCoroutines[index] = StartCoroutine(FadeVolumeRoutine(source, targetVolume, duration, index));
    }

    private IEnumerator FadeVolumeRoutine(AudioSource source, float targetVolume, float duration, int index)
    {
        float startVolume = source.volume;
        float time = 0f;
        while (time < duration)
        {
            time += Time.deltaTime;
            source.volume = Mathf.Lerp(startVolume, targetVolume, time / duration);
            yield return null;
        }
        source.volume = targetVolume;
        fadeCoroutines[index] = null;
    }
}
