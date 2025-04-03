using UnityEngine;
using TMPro;
using System.Collections;
using UnityEngine.UI;

// Alexander

public class LootDrops : MonoBehaviour
{
    public enum PowerUpType { SpeedBoost, HealthBoost, AmmoBoost, Immortal }
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

    [SerializeField] private float rotationSpeed = 160f; // degrees per second
    [SerializeField] private float floatAmplitude = 0.4f; // How high it moves
    [SerializeField] private float floatSpeed = 4f;       // How fast it moves

    private Vector3 startPosition;

    [Header("Audio Settings")]
    [SerializeField] private AudioClip pickupSound;

    Destroy destroy;

    private void Start()
    {
        startPosition = transform.position;

        if (healthBoostImage != null) healthBoostImage.gameObject.SetActive(false);
        if (speedBoostImage != null) speedBoostImage.gameObject.SetActive(false);
        if (ammoBoostText != null) ammoBoostText.gameObject.SetActive(false);
    }

    private void Update()
    {
        transform.Rotate(0, rotationSpeed * Time.deltaTime, 0);

        float newY = startPosition.y + Mathf.Sin(Time.time * floatSpeed) * floatAmplitude;
        transform.position = new Vector3(startPosition.x, newY, startPosition.z);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        PlayerController playerController = other.GetComponent<PlayerController>();
        PlayerHealth playerHealth = other.GetComponent<PlayerHealth>();
        WeaponManager weaponManager = FindAnyObjectByType<WeaponManager>();

        if (playerHealth == null) return;

        PlayPickupSound();
        ApplyPowerUp(playerController, playerHealth, weaponManager);
        ShowUIFeedback();

        destroy = GetComponent<Destroy>();
        if (destroy != null)
        {
            destroy.enabled = true;
            destroy.Destuct();
        }

    }

    private void PlayPickupSound()
    {
        if (pickupSound != null)
        {
            AudioSource.PlayClipAtPoint(pickupSound, transform.position);
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
                if (playerWeapon != null && playerWeapon.totalAmmo < playerWeapon.maxAmmo)
                {
                    playerWeapon.AddAmmo(ammoAmount);
                }
                break;

            case PowerUpType.Immortal:
                if (playerHealth != null)
                {
                    playerHealth.StartCoroutine(playerHealth.Immortality(duration));
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