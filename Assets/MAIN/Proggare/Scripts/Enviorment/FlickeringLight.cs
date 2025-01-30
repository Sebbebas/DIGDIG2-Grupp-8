using UnityEngine;

//Alexander

public class FlickeringLight : MonoBehaviour
{
    private Light lightSource;
    [SerializeField] float minIntensity = 0.5f;
    [SerializeField] float maxIntensity = 2f;
    [SerializeField] float flickerSpeed = 0.1f;

    public void Start()
    {
        lightSource = GetComponent<Light>();
        StartCoroutine(Flicker());
    }

    private System.Collections.IEnumerator Flicker()
    {
        while (true)
        {
            lightSource.intensity = Random.Range(minIntensity, maxIntensity);
            yield return new WaitForSeconds(flickerSpeed);
        }
    }
}
