using UnityEngine;
using UnityEngine.UI;


public class PlayerHealth : MonoBehaviour
{
    public float maxHealth = 100f;
    public float currentHealth;
    public bool isDead = false;
    public bool takesDamage;
    [SerializeField, Tooltip("When counting hits this the timer is done (counts in frames)")] float endTimer = 100f;

    private const float HealPlayer = 20f;
    private const float DamageAmount = 20f;

    public Image stage1Image;
    public Image stage2Image;
    public Image stage3Image;
    public Image stage4Image;

    private float speedMultiplier = 1f;
    private float slowdownPercentage = 0.05f;

    bool timer;
    int counting = 0;

    void Start()
    {
        currentHealth = maxHealth;

        HideAllImages();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.H))
        {
            HealByPercentage(HealPlayer);
        }
        if (Input.GetKeyDown(KeyCode.J))
        {
            TakeDamage(DamageAmount);
        }

        if (timer == true)
        {
            counting++;
        }

        if (counting >= endTimer)
        {
            timer = false;
            takesDamage = false;
        }
    }

    public void TakeDamage(float damage)
    {
        if (isDead)
            return;

        currentHealth -= damage;
        takesDamage = true;
        timer = true;

        //Checks if the player has dead
        if (currentHealth <= 0)
        {
            Die();
        }

        CheckHealthStages();
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

        //Health wont go further then maxHealth
        if (currentHealth > maxHealth)
        {
            currentHealth = maxHealth;
        }

        CheckHealthStages();
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
            speedMultiplier = 1f - (slowdownPercentage * 10); //100% slowdown
        }
        if (currentHealth <= 20f)
        {
            ShowImage(stage3Image);
            speedMultiplier = 1f - (slowdownPercentage * 3); //15% slowdown
        }
        else if (currentHealth <= 40f)
        {
            ShowImage(stage2Image);
            speedMultiplier = 1f - (slowdownPercentage * 2); //10% Slowdown
        }
        else if (currentHealth <= 60f)
        {
            ShowImage(stage1Image);
            speedMultiplier = 1f - slowdownPercentage; //5% slowdown
        }
        else
        {
            HideAllImages();
            speedMultiplier = 1f; //No slowdown
        }
    }
    public void ApplyDamage(float damageAmount)
    {
        TakeDamage(damageAmount);
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

    public bool GetTakeDamage()
    {
        return takesDamage;
    }
}