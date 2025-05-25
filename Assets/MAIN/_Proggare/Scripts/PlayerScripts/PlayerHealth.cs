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

    private float healAmount = 20f;

    [Header("Health Stages")]
    [SerializeField] GameObject stage1Image;
    [SerializeField] GameObject stage2Image;
    [SerializeField] GameObject stage3Image;
    [SerializeField] GameObject stage4Image;
    [SerializeField] GameObject pulseEffect;

    [SerializeField] float invisTimer = 60f;

    [SerializeField] GameObject deathScreen;

    [Header("Audio")]
    [SerializeField] AudioClip[] takeDamageSoundClip;
    [SerializeField, Range(0, 1)] float takeDamageSoundVolume = 1f;
    [SerializeField, Range(0, 256)] int takeDamageSoundPriority = 256; // 0-256, 256 is the highest priority

    [Space]

    [SerializeField] AudioClip deathSoundClip;
    [SerializeField, Range(0, 1)] float deathSoundVolume = 1f;
    [SerializeField, Range(0, 256)] int deathSoundPriority = 256; 

    private bool isImmortal = false;

    ScreenShake screenShake;

    void Start()
    {
        screenShake = FindFirstObjectByType<ScreenShake>();

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

    //void Update()
    //{
    //    //if (Input.GetKeyDown(KeyCode.H)) Heal(healAmount);
    //    //if (Input.GetKeyDown(KeyCode.K)) ApplyTorsoDamage(damageAmount);
    //}

    public void ApplyDamage(float damageAmount)
    {
        if (isDead || isImmortal)  //Prevent damage if dead or immortal
            return;

        //ScreenShake
        screenShake.Shake(0.1f, 0.3f);

        //sound effect
        GameObject damageSound = new();
        damageSound.AddComponent<AudioSource>();
        damageSound.GetComponent<AudioSource>().playOnAwake = true;
        damageSound.GetComponent<AudioSource>().volume = takeDamageSoundVolume;
        damageSound.GetComponent<AudioSource>().priority = takeDamageSoundPriority;

        //set random clip
        int randomIndex = Random.Range(0, takeDamageSoundClip.Length);
        damageSound.GetComponent<AudioSource>().clip = takeDamageSoundClip[randomIndex];

        //instantiate the sound effect
        Instantiate(damageSound, transform.position, Quaternion.identity);

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
        GameObject deathSound = new();
        deathSound.AddComponent<AudioSource>();
        deathSound.GetComponent<AudioSource>().playOnAwake = true;
        deathSound.GetComponent<AudioSource>().clip = deathSoundClip;
        deathSound.GetComponent<AudioSource>().volume = deathSoundVolume;
        Instantiate(deathSound, transform.position, Quaternion.identity);

        isDead = true;
        deathScreen.SetActive(true);
    }

    private void CheckHealthStages()
    {
        if (isDead) return;

        if (100 <= currentHealth && currentHealth >= 81)
        {
            stage2Image.SetActive(false);
            stage3Image.SetActive(false);
            stage4Image.SetActive(false);
            pulseEffect.SetActive(false);
        }
        else if (80 <= currentHealth && currentHealth >= 61)
        {
            stage2Image.SetActive(true);

            stage3Image.SetActive(false);
            stage4Image.SetActive(false);
            pulseEffect.SetActive(false);
        }
        else if (60 <= currentHealth && currentHealth >= 41)
        {
            stage3Image.SetActive(true);

            stage4Image.SetActive(false);
            pulseEffect.SetActive(false);
        }
        else if (40 <= currentHealth && currentHealth >= 21)
        {
            stage4Image.SetActive(true);
            
            pulseEffect.SetActive(false);
        }
        else if (20 <= currentHealth && currentHealth >= 0)
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
        yield return new WaitForSeconds(invisTimer);
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
    public float GetCurrentHealth()
    {
        return currentHealth;
    }
}