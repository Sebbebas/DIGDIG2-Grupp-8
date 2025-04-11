using UnityEngine;

public class ButtonPressSound : MonoBehaviour
{
    [SerializeField] AudioClip buttonPressSound;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void PlaySound()
    {
        GameObject audioObject = new();
        audioObject.AddComponent<AudioSource>();
        audioObject.GetComponent<AudioSource>().clip = buttonPressSound;
        Instantiate(audioObject, FindFirstObjectByType<AudioListener>().transform);
    }
}
