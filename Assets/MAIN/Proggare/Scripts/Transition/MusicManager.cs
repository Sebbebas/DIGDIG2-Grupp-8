using System.Collections;
using UnityEngine;

public class MusicManager : MonoBehaviour
{
    //Configurable Parameters
    [SerializeField] AudioSource audioSource;

    //Private Variables


    //Cached References

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }

    public void MusicFadeOut()
    {
        StartCoroutine(MusicFadeOutRoutine());
    }

    public IEnumerator MusicFadeOutRoutine()
    {
        while (audioSource.volume > 0)
        {
            audioSource.volume -= 0.1f;
            yield return new WaitForSeconds(0.1f);

            if (audioSource.volume <= 0)
            {
                audioSource.Stop();
                audioSource.volume = 1;
            }
        }
    }
}
