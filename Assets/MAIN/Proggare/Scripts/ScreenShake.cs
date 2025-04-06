using UnityEngine;

public class ScreenShake : MonoBehaviour
{
    [SerializeField] private float dampingSpeed = 1.0f;
    private Vector3 originalPosition;
    private float shakeDuration = 0f;
    private float shakeIntensity = 0.7f;

    void Awake()
    {
        originalPosition = transform.localPosition;
    }

    void Update()
    {
        if (shakeDuration > 0)
        {
            Vector3 randomPoint = originalPosition + Random.insideUnitSphere * shakeIntensity;
            transform.localPosition = new Vector3(randomPoint.x, randomPoint.y, originalPosition.z);
            shakeDuration -= Time.unscaledDeltaTime * dampingSpeed;
        }
        else
        {
            shakeDuration = 0f;
            transform.localPosition = originalPosition;
        }
    }

    public void Shake(float duration, float intensity)
    {
        shakeDuration = duration;
        shakeIntensity = intensity;
    }
}
