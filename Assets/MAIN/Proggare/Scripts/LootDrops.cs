using UnityEngine;
using TMPro;
using System.Collections;
using UnityEngine.UI;

// Alexander

public class PowerUp : MonoBehaviour
{
    public enum PowerUpType { SpeedBoost, HealthBoost, AmmoBoost }
    public PowerUpType powerUpType;

    [Header("General Power-Up Settings")]
    public float duration = 5f;

    [Header("Speed Boost Settings")]
    public float speedMultiplier = 1.5f;

    [Header("Health Boost Settings")]
    public float healthAmount = 20f;

    [Header("Ammo Pickup Settings")]
    public int ammoAmount = 10;

    [Header("UI Settings")]
    public Image healthBoostImage;         
    public Image speedBoostImage;          
    public TextMeshProUGUI ammoBoostText;  

    [SerializeField] private float uiDisplayDuration = 3f;

    // Rotation speed for the power-up object
    [SerializeField] private float rotationSpeed = 160f; // degrees per second

    // Settings for up-and-down movement
    [SerializeField] private float floatAmplitude = 0.4f; // How high it moves
    [SerializeField] private float floatSpeed = 4f;       // How fast it moves

    private Vector3 startPosition; // To store the initial position

    [Header("Audio Settings")]
    [SerializeField] private AudioClip pickupSound; // The sound to play on pickup

    private void Start()
    {
        // Store the initial position of the power-up
        startPosition = transform.position;

        // Ensure UI elements are initially hidden
        if (healthBoostImage != null) healthBoostImage.gameObject.SetActive(false);
        if (speedBoostImage != null) speedBoostImage.gameObject.SetActive(false);
        if (ammoBoostText != null) ammoBoostText.gameObject.SetActive(false);
    }

    private void Update()
    {
        // Rotate the object around the Y-axis at a constant speed
        transform.Rotate(0, rotationSpeed * Time.deltaTime, 0);

        // Apply up-and-down movement using a sine wave
        float newY = startPosition.y + Mathf.Sin(Time.time * floatSpeed) * floatAmplitude;
        transform.position = new Vector3(startPosition.x, newY, startPosition.z);
    }

    private void OnTriggerEnter(Collider other)
    {
        PlayerController playerController = other.GetComponent<PlayerController>();
        PlayerHealth playerHealth = other.GetComponent<PlayerHealth>();
        WeaponManager weaponManager = FindAnyObjectByType<WeaponManager>();

        Debug.Log(weaponManager);

        if (playerController != null || playerHealth != null || weaponManager != null)
        {
            PlayPickupSound();
            ApplyPowerUp(playerController, playerHealth, weaponManager);
            ShowUIFeedback();
            Destroy(gameObject); // Remove the power-up after pickup
        }
    }

    private void PlayPickupSound()
    {
        if (pickupSound != null)
        {
            // Play the sound at the position of the power-up
            AudioSource.PlayClipAtPoint(pickupSound, transform.position);
        }
        else
        {
            Debug.LogWarning("Pickup sound not assigned!");
        }
    }

    private void ApplyPowerUp(PlayerController playerController, PlayerHealth playerHealth, WeaponManager weaponManager)
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
                Shotgun playerWeapon = weaponManager.GetCurrentWeapon().GetComponent<Shotgun>();
                if (playerWeapon != null)
                {
                    playerWeapon.AddAmmo(ammoAmount);
                }
                break;
        }
    }

    private void ShowUIFeedback()
    {
        switch (powerUpType)
        {
            case PowerUpType.SpeedBoost:
                if (speedBoostImage != null)
                {
                    StartCoroutine(DisplayUI(speedBoostImage.gameObject, uiDisplayDuration));
                }
                break;

            case PowerUpType.HealthBoost:
                if (healthBoostImage != null)
                {
                    StartCoroutine(DisplayUI(healthBoostImage.gameObject, uiDisplayDuration));
                }
                break;

            case PowerUpType.AmmoBoost:
                if (ammoBoostText != null)
                {
                    StartCoroutine(DisplayUI(ammoBoostText.gameObject, uiDisplayDuration));
                }
                break;
        }
    }

    private IEnumerator DisplayUI(GameObject uiElement, float duration)
    {
        uiElement.SetActive(true);
        yield return new WaitForSeconds(duration);
        uiElement.SetActive(false);
    }
}