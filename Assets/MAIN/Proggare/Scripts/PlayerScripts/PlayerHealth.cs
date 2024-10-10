using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections;

// Alexander

public class PlayerHealth : MonoBehaviour
{
    [SerializeField] float maxHealth = 100f;
    [SerializeField] float currentHealth;
    [SerializeField] bool isDead = false;
    [HideInInspector] public bool takesDamage;
    [HideInInspector] public bool lowHealth;

    [Header("Testing")]
    [SerializeField] float HealPlayer = 20f;
    [SerializeField] float DamageAmount = 20f;

    [Header("Health Stages")]
    [SerializeField] Image stage1Image;
    [SerializeField] Image stage2Image;
    [SerializeField] Image stage3Image;
    [SerializeField] Image stage4Image;

    [SerializeField] float timer = 60f;

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
            Debug.Log(currentHealth);
        }
        if (Input.GetKeyDown(KeyCode.K))
        {
            TakeDamage(DamageAmount);
            Debug.Log(currentHealth);
        }
    }

    public void TakeDamage(float damage)
    {
        if (isDead)
            return;

        currentHealth -= damage;

        StartCoroutine(TakeDamageTimer());

        //Checks if the player is die
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

        //Health won't exceed maxHealth
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
        HideAllImages(); //First, hide all images before showing the correct one

        if (currentHealth <= 0f)
        {
            ShowImage(stage4Image); //Dead stage
        }
        else if (currentHealth <= 20f)
        {
            ShowImage(stage3Image); //Low health stage
            lowHealth = true;
        }
        else if (currentHealth <= 40f)
        {
            ShowImage(stage2Image); //Moderate health stage
            lowHealth = false;
        }
        else if (currentHealth <= 60f)
        {
            ShowImage(stage1Image); //Good health stage
            lowHealth = false;
        }
        else
        {
            lowHealth = false; //Healthy state, no stages visible
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
}