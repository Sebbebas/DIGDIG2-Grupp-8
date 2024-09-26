using UnityEngine;
using TMPro;
using UnityEngine.UI;

// Alexander
public class PlayerHealth : MonoBehaviour
{
    public float maxHealth = 100f;
    public float currentHealth;
    public bool isDead = false;

    private const float HealPlayer = 20f;
    private const float DamageAmount = 20f;

    public Image stage1Image;
    public Image stage2Image;
    public Image stage3Image;
    public Image stage4Image;

    public TMP_Text healthText;

    private float speedMultiplier = 1f;
    private float slowdownPercentage = 0.05f;

    void Start()
    {
        currentHealth = maxHealth;
        HideAllImages();
        UpdateHealthText();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.H))
        {
            HealByPercentage(HealPlayer);
        }
        if (Input.GetKeyDown(KeyCode.G))
        {
            TakeDamage(DamageAmount);
        }
    }

    public void TakeDamage(float damage)
    {
        if (isDead)
            return;

        currentHealth -= damage;

        //Checks if the player is die
        if (currentHealth <= 0)
        {
            Die();
        }

        CheckHealthStages();
        UpdateHealthText();
    }

    void Die()
    {
        isDead = true;
        Debug.Log("Player has died!");
        // Destroy(gameObject);
    }

    public void Heal(float healAmount)
    {
        if (isDead)
            return;

        currentHealth += healAmount;

        //Health won't exceed maxHealth
        if (currentHealth > maxHealth)
        {
            currentHealth = maxHealth;
        }

        CheckHealthStages();
        UpdateHealthText();
    }

    public void HealByPercentage(float percentage)
    {
        float healAmount = maxHealth * (percentage / 100f);
        Heal(healAmount);
    }

    private void CheckHealthStages()
    {
        if (currentHealth <= 0f)
        {
            ShowImage(stage4Image);
            speedMultiplier = 1f - (slowdownPercentage * 10); // 100% slowdown
        }
        else if (currentHealth <= 20f)
        {
            ShowImage(stage3Image);
            speedMultiplier = 1f - (slowdownPercentage * 3); // 15% slowdown
        }
        else if (currentHealth <= 40f)
        {
            ShowImage(stage2Image);
            speedMultiplier = 1f - (slowdownPercentage * 2); // 10% slowdown
        }
        else if (currentHealth <= 60f)
        {
            ShowImage(stage1Image);
            speedMultiplier = 1f - slowdownPercentage; // 5% slowdown
        }
        else
        {
            HideAllImages();
            speedMultiplier = 1f; // No slowdown
        }
    }

    public void ApplyDamage(float DamageAmount)
    {
        TakeDamage(DamageAmount);
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
        if (stage1Image != null) stage1Image.enabled = false;
        if (stage2Image != null) stage2Image.enabled = false;
        if (stage3Image != null) stage3Image.enabled = false;
        if (stage4Image != null) stage4Image.enabled = false;
    }

    private void UpdateHealthText()
    {
        if (healthText != null)
        {
            healthText.text = $"Health: {currentHealth:F0}";
        }
    }
}