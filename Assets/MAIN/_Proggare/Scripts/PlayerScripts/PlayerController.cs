using UnityEngine;
using System.Collections;

//Alexander

public class PlayerController : MonoBehaviour
{
    [SerializeField] float baseSpeed = 5f;
    [SerializeField] float currentSpeed;
    [SerializeField] int ammo = 50;

    private void Start()
    {
        currentSpeed = baseSpeed;
    }

    public IEnumerator SpeedBoost(float multiplier, float duration)
    {
        currentSpeed = baseSpeed * multiplier;
        Debug.Log("Speed boosted!");
        yield return new WaitForSeconds(duration);
        currentSpeed = baseSpeed;
        Debug.Log("Speed boost ended.");
    }

    public void AddAmmo(int amount)
    {
        ammo += amount;
        Debug.Log("Ammo increased: " + amount + ", Total Ammo: " + ammo);
    }

    public float GetCurrentSpeed()
    {
        return currentSpeed;
    }
}