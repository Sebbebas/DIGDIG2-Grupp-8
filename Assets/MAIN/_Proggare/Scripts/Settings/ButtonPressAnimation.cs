using UnityEngine;
using System.Collections;

public class ButtonPressAnimation : MonoBehaviour
{
    [SerializeField] GameObject unpressedButton, pressedButton;
    [SerializeField] float buttonPressTime = .1f;

    void Start()
    {
        pressedButton.SetActive(false);
        unpressedButton.SetActive(true);
    }

    //Add to every button for press animation
    public void ButtonSpriteChanged()
    {
        StartCoroutine(ButtonPressedAnimation());
    }

    IEnumerator ButtonPressedAnimation()
    {
        pressedButton.SetActive(true);
        unpressedButton.SetActive(false);
        yield return new WaitForSeconds(buttonPressTime);
        pressedButton.SetActive(false);
        unpressedButton.SetActive(true);
    }
}
