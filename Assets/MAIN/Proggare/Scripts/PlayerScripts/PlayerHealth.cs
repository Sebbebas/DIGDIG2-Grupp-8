using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections;

//Alexander

public class PlayerHealth : MonoBehaviour
{
    [SerializeField] float maxHealth = 150f;
    [SerializeField] float currentHealth = 100f; // Start with 100 health
    [HideInInspector] public bool isDead = false;
    [HideInInspector] public bool takesDamage;
    [HideInInspector] public bool lowHealth;

    [Header("Testing")]
    [SerializeField] float damageAmount = 20f;
    [SerializeField] TextMeshProUGUI healthText;

    private float healAmount = 100f;

    [Header("Health Stages")]
    [SerializeField] Image stage1Image;
    [SerializeField] Image stage2Image;
    [SerializeField] Image stage3Image;
    [SerializeField] Image stage4Image;

    [SerializeField] float timer = 60f;

    [SerializeField] GameObject deathScreen;

    void Start()
    {
        deathScreen.SetActive(false);
        currentHealth = 100f; // Set starting health to 100
        HideAllImages();
        UpdateHealthText();
        CheckHealthStages(); // Ensure the correct stage is shown at start
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.H))
        {
            Heal(healAmount);
            Debug.Log(currentHealth);
        }
        if (Input.GetKeyDown(KeyCode.K))
        {
            ApplyDamage(damageAmount);
            Debug.Log(currentHealth);
        }
    }

    public void ApplyDamage(float damageAmount)
    {
        if (isDead)
            return;

        currentHealth -= damageAmount;

        if (currentHealth < 0)
        {
            currentHealth = 0;
        }

        UpdateHealthText();
        StartCoroutine(TakeDamageTimer());

        if (currentHealth <= 0)
        {
            Die();
        }
        else
        {
            CheckHealthStages();
        }
    }

    public void Heal(float healAmount)
    {
        if (isDead)
            return;

        currentHealth += healAmount;

        if (currentHealth > maxHealth)
        {
            currentHealth = maxHealth;
        }

        UpdateHealthText();
        CheckHealthStages();
    }

    private void Die()
    {
        isDead = true;
        Debug.Log("Player has died!");
        deathScreen.SetActive(true);
        HideAllImages(); // Ensure all health stage overlays are hidden when dead.
        ShowImage(stage4Image); // Show dead stage overlay.
    }

    private void CheckHealthStages()
    {
        if (isDead) return; // Do not update health stages if the player is dead.

        HideAllImages(); // Ensure all images are hidden before displaying the relevant one.

        // Apply health stage overlays based on thresholds
        if (currentHealth > 60f && currentHealth <= maxHealth)
        {
            // No stage overlay for health > 60
            lowHealth = false;
        }
        else if (currentHealth > 20f && currentHealth <= 60f)
        {
            ShowImage(stage1Image); // Moderate health stage
            lowHealth = false;
        }
        else if (currentHealth > 0f && currentHealth <= 20f)
        {
            ShowImage(stage3Image); // Low health stage
            lowHealth = true;
        }
        else if (currentHealth <= 0f)
        {
            ShowImage(stage4Image); // Dead stage
            deathScreen.SetActive(true);
        }
    }

    private void ShowImage(Image image)
    {
        if (image != null)
        {
            image.enabled = true;
        }
    }

    private void HideAllImages()
    {
        // Reset all health stage overlays
        if (stage1Image != null) stage1Image.enabled = false;
        if (stage2Image != null) stage2Image.enabled = false;
        if (stage3Image != null) stage3Image.enabled = false;
        if (stage4Image != null) stage4Image.enabled = false;
    }

    private void UpdateHealthText()
    {
        if (healthText != null)
        {
            healthText.text = currentHealth.ToString("F0");
        }
    }

    IEnumerator TakeDamageTimer()
    {
        takesDamage = true;
        yield return new WaitForSeconds(timer);
        takesDamage = false;
    }

    public bool GetTakeDamage()
    {
        return takesDamage;
    }

    public bool GetLowHealth()
    {
        return lowHealth;
    }

    public bool GetIsDead()
    {
        return isDead;
    }
}
