using UnityEngine;

public class LightToggle : MonoBehaviour
{
    private Light lightSource;

    private void Start()
    {
        lightSource = GetComponent<Light>();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.L))
        {
            lightSource.enabled = !lightSource.enabled;
        }
    }
}
