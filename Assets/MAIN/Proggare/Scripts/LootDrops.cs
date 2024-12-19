using UnityEngine;
using System.Collections;

//Alexander

public class PowerUp : MonoBehaviour
{
    public enum PowerUpType { SpeedBoost, HealthBoost, AmmoPickup }
    public PowerUpType powerUpType;

    [Header("General Power-Up Settings")]
    public float duration = 5f; // Duration
    public float lifetime = 8f; // Time before the power-up is destroyed if not picked up

    [Header("Speed Boost Settings")]
    public float speedMultiplier = 1.5f;

    [Header("Health Boost Settings")]
    public float healthAmount = 25f;

    [Header("Ammo Pickup Settings")]
    public int ammoAmount = 10;

    private void Start()
    {
        // Start the timer to destroy the object if not picked up
        StartCoroutine(DestroyAfterLifetime());
    }

    private void OnTriggerEnter(Collider other)
    {
        PlayerController player = other.GetComponent<PlayerController>();
        if (player != null)
        {
            ApplyPowerUp(player);
            Destroy(gameObject);
        }
    }

    private void ApplyPowerUp(PlayerController player)
    {
        switch (powerUpType)
        {
            case PowerUpType.SpeedBoost:
                StartCoroutine(player.SpeedBoost(speedMultiplier, duration));
                break;

            case PowerUpType.HealthBoost:
                player.AddHealth(healthAmount);
                break;

            case PowerUpType.AmmoPickup:
                player.AddAmmo(ammoAmount);
                break;
        }
    }

    private IEnumerator DestroyAfterLifetime()
    {
        yield return new WaitForSeconds(lifetime);
        Destroy(gameObject); // Destroy the power-up after its lifetime
    }
}