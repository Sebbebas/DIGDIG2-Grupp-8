using UnityEngine;
using System.Collections;

//Alexander

public class PlayerController : MonoBehaviour
{
    [SerializeField] float baseSpeed = 5f;
    [SerializeField] float currentSpeed;
    [SerializeField] float health = 100f;
    [SerializeField] int ammo = 50;

    private void Start()
    {
        currentSpeed = baseSpeed;
    }

    public void AddHealth(float amount)
    {
        health += amount;
        Debug.Log("Health increased: " + amount);
    }

    public void AddAmmo(int amount)
    {
        ammo += amount;
        Debug.Log("Ammo increased: " + amount);
    }

    public IEnumerator SpeedBoost(float multiplier, float duration)
    {
        currentSpeed *= multiplier;
        Debug.Log("Speed boosted!");
        yield return new WaitForSeconds(duration);
        currentSpeed = baseSpeed;
        Debug.Log("Speed boost ended.");
    }
}