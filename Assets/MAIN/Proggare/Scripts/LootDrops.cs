using UnityEngine;

//Alexander

public class PowerUp : MonoBehaviour
{
    public enum PowerUpType { SpeedBoost, HealthBoost, AmmoBoost }
    public PowerUpType powerUpType;

    [Header("General Power-Up Settings")]
    public float duration = 5f;

    [Header("Speed Boost Settings")]
    public float speedMultiplier = 1.5f;    //Temorary

    [Header("Health Boost Settings")]
    public float healthAmount = 20f;

    [Header("Ammo Pickup Settings")]
    public int ammoAmount = 10;

    private void OnTriggerEnter(Collider other)
    {
        PlayerController playerController = other.GetComponent<PlayerController>();
        PlayerHealth playerHealth = other.GetComponent<PlayerHealth>();


        if (playerController != null || playerHealth != null)
        {
            ApplyPowerUp(playerController, playerHealth);
            Destroy(gameObject); // Remove the power-up after pickup
        }
    }

    private void ApplyPowerUp(PlayerController playerController, PlayerHealth playerHealth)
    {
        switch (powerUpType)
        {
            case PowerUpType.SpeedBoost:
                if (playerController != null)
                {
                    playerController.StartCoroutine(playerController.SpeedBoost(speedMultiplier, duration));
                }
                break;

            case PowerUpType.HealthBoost:
                if (playerHealth != null)
                {
                    playerHealth.Heal(healthAmount);
                }
                break;

            case PowerUpType.AmmoBoost:
                if (playerController != null)
                {
                    playerController.AddAmmo(ammoAmount);
                }
                break;
        }
    }
}