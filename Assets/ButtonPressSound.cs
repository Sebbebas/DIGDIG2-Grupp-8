using UnityEngine;

public class ButtonPressSound : MonoBehaviour
{
    //Configurable Perameters
    [SerializeField] AudioClip buttonPressSound;

    public void PlaySound()
    {
        GameObject audioObject = new();

        //Add audio source to the object
        audioObject.AddComponent<AudioSource>();
        audioObject.GetComponent<AudioSource>().clip = buttonPressSound;

        //Destroy the object after the sound has played
        audioObject.AddComponent<Destroy>();
        audioObject.GetComponent<Destroy>().SetAliveTime(buttonPressSound.length);

        //Instantiate the object at the position of the camera
        Instantiate(audioObject, FindFirstObjectByType<AudioListener>().transform);
    }
}
