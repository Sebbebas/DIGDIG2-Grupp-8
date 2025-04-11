using System;
using System.Collections;
using UnityEngine;

public class MusicManager : MonoBehaviour
{
    [SerializeField] bool ZombieAgroMusicIntensity = false;
    [SerializeField] float baseVolume = 0.5f;

    //Configurable Parameters
    [SerializeField] AudioSource audioSource;

    [SerializeField] GameObject[] musicObject;

    [Space]

    [SerializeField] SongStruct[] songsStructs;

    [SerializeField] float maxZombieAgroVolume = 1f; // Maximum volume when all zombies are aggroed
    [SerializeField] float minZombieAgroVolume = 0.2f; // Minimum volume when no zombies are aggroed

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
    EnemyBehaviour enemyBehaviour;

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

            songAudioSource.Play();
        }
    }

    private void Update()
    {
        if (!ZombieAgroMusicIntensity) { return; }

        // Get the number of aggroed zombies
        int zombiesAgro = EnemyBehaviour.GetZombiesAgroCount();

        // Ensure the first sound [0] (ambient music) always plays
        AudioSource ambientAudioSource = musicObject[0].GetComponent<AudioSource>();
        ambientAudioSource.volume = songsStructs[0].volume * baseVolume;

        // Disable all other music if there are fewer than 3 aggroed zombies
        if (zombiesAgro < 3)
        {
            for (int i = 1; i < musicObject.Length; i++)
            {
                AudioSource otherAudioSource = musicObject[i].GetComponent<AudioSource>();
                otherAudioSource.volume = 0;
            }
            return;
        }

        // Adjust the second sound [1] based on the number of aggroed zombies
        AudioSource secondAudioSource = musicObject[1].GetComponent<AudioSource>();
        if (zombiesAgro >= 3 && zombiesAgro < 7)
        {
            secondAudioSource.volume = baseVolume;
        }
        else
        {
            secondAudioSource.volume = 0;
        }

        // Adjust the third sound [2] based on the number of aggroed zombies
        AudioSource thirdAudioSource = musicObject[2].GetComponent<AudioSource>();
        if (zombiesAgro >= 7)
        {
            thirdAudioSource.volume = baseVolume;
            secondAudioSource.volume = 0; // Disable the second sound when the third is active
        }
        else
        {
            thirdAudioSource.volume = 0;
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
