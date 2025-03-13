using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections;

// Alexander

public class PlayerHealth : MonoBehaviour
{
    [SerializeField] float maxHealth = 150f;
    [SerializeField] float currentHealth = 100f;
    [HideInInspector] public bool isDead = false;
    [HideInInspector] public bool takesDamage;
    [HideInInspector] public bool lowHealth;

    [Header("Testing")]
    [SerializeField] float damageAmount = 20f;
    [SerializeField] TextMeshProUGUI healthText;

    private float healAmount = 100f;

    [Header("Health Stages")]
    [SerializeField] GameObject stage1Image;
    [SerializeField] GameObject stage2Image;
    [SerializeField] GameObject stage3Image;
    [SerializeField] GameObject stage4Image;
    [SerializeField] GameObject pulseEffect;

    [SerializeField] float timer = 60f;

    [SerializeField] GameObject deathScreen;

    private bool isImmortal = false;

    void Start()
    {
        deathScreen.SetActive(false);
        stage1Image.SetActive(true);
        stage2Image.SetActive(false);
        stage3Image.SetActive(false);
        stage4Image.SetActive(false);
        pulseEffect.SetActive(false);
        currentHealth = 100f;
        UpdateHealthText();
        CheckHealthStages();
    }

    void Update()
    {
        //if (Input.GetKeyDown(KeyCode.H)) Heal(healAmount);
        //if (Input.GetKeyDown(KeyCode.K)) ApplyDamage(damageAmount);
    }

    public void ApplyDamage(float damageAmount)
    {
        if (isDead || isImmortal)  //Prevent damage if dead or immortal
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
    }

    private void CheckHealthStages()
    {
        if (isDead) return;

        if (currentHealth >= 81)
        {
            stage2Image.SetActive(false);
        }
        else if (currentHealth >= 61)
        {
            stage2Image.SetActive(true);
            stage3Image.SetActive(false);
        }
        else if (currentHealth >= 41)
        {
            stage3Image.SetActive(true);
            stage4Image.SetActive(false);
        }
        else if (currentHealth >= 21)
        {
            stage4Image.SetActive(true);
            pulseEffect.SetActive(false);
        }
        else if (currentHealth >= 0)
        {
            pulseEffect.SetActive(true);
        }

        //if (currentHealth > 60f && currentHealth <= maxHealth)
        //{
        //    lowHealth = false;
        //}
        //else if (currentHealth > 20f && currentHealth <= 60f)
        //{
        //    ShowImage(stage1Image); //Moderate health stage
        //    lowHealth = false;
        //}
        //else if (currentHealth > 0f && currentHealth <= 20f)
        //{
        //    ShowImage(stage3Image); //Low health stage
        //    lowHealth = true;
        //}
        //else if (currentHealth <= 0f)
        //{
        //    ShowImage(stage4Image);
        //    deathScreen.SetActive(true);
        //}
    }

    private void ShowImage(Image image)
    {
        if (image != null)
        {
            image.enabled = true;
        }
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
    public IEnumerator Immortality(float duration)
    {
        isImmortal = true;
        yield return new WaitForSeconds(duration);
        isImmortal = false;
    }
}