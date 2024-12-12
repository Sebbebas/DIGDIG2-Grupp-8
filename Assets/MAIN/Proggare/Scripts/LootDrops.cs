using UnityEngine;

//Alexander

public class PowerUp : MonoBehaviour
{
    public enum PowerUpType { SpeedBoost, HealthBoost, AmmoPickup }
    public PowerUpType powerUpType;

    [Header("General Power-Up Settings")]
    public float duration = 5f; //Duration

    [Header("Speed Boost Settings")]
    public float speedMultiplier = 1.5f;

    [Header("Health Boost Settings")]
    public float healthAmount = 25f;

    [Header("Ammo pickup Settings")]
    public int ammoAmount = 10;

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
}