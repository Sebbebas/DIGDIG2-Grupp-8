using UnityEngine;
using TMPro;
using System.Collections;
using UnityEngine.UI;

public class LootDrops : MonoBehaviour
{
    //Configurable Variables
    public enum PowerUpType { SpeedBoost, HealthBoost, AmmoBoost, Immortal, MultiplierBoost }
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
    public TextMeshProUGUI multiplierBoostText; // New UI element for multiplier boost

    [SerializeField] private float uiDisplayDuration = 3f;

    [SerializeField] private float rotationSpeed = 160f; // degrees per second
    [SerializeField] private float floatAmplitude = 0.4f; // How high it moves
    [SerializeField] private float floatSpeed = 4f;       // How fast it moves

    [Header("Audio Settings")]
    [SerializeField] private AudioClip pickupSound;

    //Private Variables
    private Vector3 startPosition;
    private Animator flashEffect;

    //Chached References
    Destroy destroy;

    private void Start()
    {
        // Locate the player object
        GameObject player = FindFirstObjectByType<PlayerHealth>().gameObject;
        if (player != null)
        {
            // Find the "GUI" child object
            Transform guiTransform = player.transform.Find("GUI");
            if (guiTransform != null)
            {
                // Find the "PickUp Flash Image" child object under "GUI"
                Transform flashImageTransform = guiTransform.Find("PickUp Flash Image");
                if (flashImageTransform != null)
                {
                    // Get the Animator component from the "PickUp Flash Image" object
                    flashEffect = flashImageTransform.GetComponent<Animator>();
                }
            }
        }

        startPosition = transform.position;

        if (healthBoostImage != null) healthBoostImage.gameObject.SetActive(false);
        if (speedBoostImage != null) speedBoostImage.gameObject.SetActive(false);
        if (ammoBoostText != null) ammoBoostText.gameObject.SetActive(false);
        if (multiplierBoostText != null) multiplierBoostText.gameObject.SetActive(false);
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

        if(flashEffect != null)
        {
            flashEffect.SetTrigger("Flash");
        }

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
            destroy.Destruct();
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
                Weapon playerWeapon = weaponManager.GetCurrentWeapon().GetComponent<Weapon>();
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

            case PowerUpType.MultiplierBoost:
                if (ComboManager.instance != null)
                {
                    ComboManager.instance.StartCoroutine(ComboManager.instance.MultiplierBoost(100, duration));
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

            case PowerUpType.MultiplierBoost:
                if (multiplierBoostText != null)
                {
                    StartCoroutine(DisplayUI(multiplierBoostText.gameObject, uiDisplayDuration));
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