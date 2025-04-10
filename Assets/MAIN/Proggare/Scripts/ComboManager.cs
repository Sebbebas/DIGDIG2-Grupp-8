using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections;

public class ComboManager : MonoBehaviour
{
    public static ComboManager instance;

    public int killCount = 0;
    public int comboMultiplier = 1;
    public float comboTimer = 0f;
    public bool isComboActive = false;
    public int skillPoints = 0;
    private int comboScore = 0;
    private int maxComboScore = 0;
    private bool isCountingDown = false;
    private int previousMultiplier = 1;

    // UI Elements
    [Header("UI Elements")]
    [SerializeField] TextMeshProUGUI killCountText;
    [SerializeField] TextMeshProUGUI comboMultiplierText;
    [SerializeField] TextMeshProUGUI comboScoreText;
    [SerializeField] Slider comboTimerSlider;
    [SerializeField] Image multikillImage;
    [SerializeField] Image timesXImage;
    [SerializeField] Image multiplierSplash;

    // Audio Sources
    [Header("Audio Sources")]
    public AudioSource countingAudioSource;
    public AudioSource scoreCompleteAudioSource;
    public AudioSource multiplierIncreaseAudioSource;

    private float killWindow = 3f;
    private float lastKillTime = 0f;
    private int multiplierKillCount = 0;
    private float multiplierKillTimer = 0f;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        ShowComboUI(false);
        if (multiplierSplash != null)
        {
            multiplierSplash.gameObject.SetActive(false); // Ensure the splash is hidden initially
        }
    }

    private void Update()
    {
        if (isComboActive)
        {
            comboTimer -= Time.deltaTime;
            if (comboTimer <= 0f)
            {
                EndCombo();
            }
        }

        // Update the multiplier kill timer
        if (multiplierKillTimer > 0)
        {
            multiplierKillTimer -= Time.deltaTime;
        }

        UpdateUI();
    }

    public void AddKill()
    {
        if (isCountingDown)
        {
            // Prevent new kills from affecting the combo while counting down
            return;
        }

        killCount++;
        float timeSinceLastKill = Time.time - lastKillTime;

        if (timeSinceLastKill <= killWindow)
        {
            if (killCount >= 3 && !isComboActive)
            {
                StartCombo();
            }
            else if (isComboActive)
            {
                comboTimer = 3f;
            }
        }
        else
        {
            killCount = 1;
        }

        lastKillTime = Time.time;

        // Check for multiplier increase
        multiplierKillCount++;
        if (multiplierKillCount == 3)
        {
            multiplierKillCount = 0;
            multiplierKillTimer = killWindow;
            IncreaseMultiplier();
        }
        else if (multiplierKillTimer <= 0)
        {
            multiplierKillCount = 1;
            multiplierKillTimer = killWindow;
        }

        if (isComboActive)
        {
            int scoreToAdd = 10 * comboMultiplier;
            comboScore += scoreToAdd;
            maxComboScore = Mathf.Max(maxComboScore, comboScore);
        }
        else
        {
            ScoreManager.Instance.AddScore(10);
        }
    }

    public void AddSkillPoint()
    {
        if (isCountingDown)
        {
            // Prevent adding skill points while counting down
            return;
        }

        skillPoints++;
        IncreaseMultiplier();
    }

    private void IncreaseMultiplier()
    {
        int newMultiplier = Mathf.Min(10, comboMultiplier + 1);
        if (newMultiplier > comboMultiplier)
        {
            // Play multiplier increase sound
            if (multiplierIncreaseAudioSource != null)
            {
                multiplierIncreaseAudioSource.Play();
            }
            comboMultiplier = newMultiplier;

            if (isComboActive)
            {
                StartCoroutine(ShowMultiplierSplash());
            }
        }
    }

    private void StartCombo()
    {
        isComboActive = true;
        comboTimer = 3f;
        comboTimerSlider.maxValue = 3f;
        comboTimerSlider.value = comboTimer;
        ShowComboUI(true);
    }

    private void EndCombo()
    {
        isComboActive = false;
        killCount = 0;
        comboMultiplier = 1;
        previousMultiplier = 1;
        if (!isCountingDown)
        {
            StartCoroutine(CountdownComboScore());
        }
    }

    private IEnumerator CountdownComboScore()
    {
        isCountingDown = true;
        int scoreToAdd = maxComboScore;
        int increment = Mathf.Max(1, scoreToAdd / 25);
        float delay = 0.1f;

        // Play counting sound
        if (countingAudioSource != null)
        {
            countingAudioSource.Play();
        }

        while (scoreToAdd > 0)
        {
            int addAmount = Mathf.Min(increment, scoreToAdd);
            ScoreManager.Instance.AddScore(addAmount);
            comboScore = Mathf.Max(0, comboScore - addAmount);
            scoreToAdd -= addAmount;

            yield return new WaitForSeconds(delay);
        }

        comboScore = 0;
        maxComboScore = 0;
        isCountingDown = false;

        // Play score complete sound
        if (scoreCompleteAudioSource != null)
        {
            scoreCompleteAudioSource.Play();
        }

        ScoreManager.Instance.HighlightScore();

        // Start the flickering effect after counting is done
        StartCoroutine(FlickerComboUI());
    }

    private IEnumerator FlickerComboUI()
    {
        float flickerDuration = 0.5f; // Total duration for the flickering effect
        float flickerSpeed = 0.05f; // Time interval for each flicker
        float timer = 0f;

        while (timer < flickerDuration)
        {
            SetComboUIAlpha(0f, true); // Include comboScoreText in the flicker
            yield return new WaitForSeconds(flickerSpeed);
            SetComboUIAlpha(1f, true); // Include comboScoreText in the flicker
            yield return new WaitForSeconds(flickerSpeed);
            timer += flickerSpeed * 2;
        }

        ShowComboUI(false);
    }

    private IEnumerator ShowMultiplierSplash()
    {
        if (multiplierSplash != null)
        {
            multiplierSplash.gameObject.SetActive(true);
            Vector3 originalScale = multiplierSplash.transform.localScale;
            float duration = 0.5f; // Duration for the pulsate effect
            float elapsed = 0f;

            // Pulsate effect
            while (elapsed < duration)
            {
                multiplierSplash.transform.localScale = Vector3.Lerp(originalScale, originalScale * 1.2f, Mathf.PingPong(elapsed / duration, 1f));
                elapsed += Time.deltaTime;
                yield return null;
            }

            multiplierSplash.gameObject.SetActive(false);
            multiplierSplash.transform.localScale = originalScale;
        }
    }

    private void SetComboUIAlpha(float alpha, bool includeComboScore)
    {
        if (killCountText != null)
        {
            Color textColor = killCountText.color;
            textColor.a = alpha;
            killCountText.color = textColor;
        }

        if (comboMultiplierText != null)
        {
            Color textColor = comboMultiplierText.color;
            textColor.a = alpha;
            comboMultiplierText.color = textColor;
        }

        if (comboTimerSlider != null)
        {
            Color sliderColor = comboTimerSlider.image.color;
            sliderColor.a = alpha;
            comboTimerSlider.image.color = sliderColor;
        }

        if (multikillImage != null)
        {
            Color imageColor = multikillImage.color;
            imageColor.a = alpha;
            multikillImage.color = imageColor;
        }

        if (timesXImage != null)
        {
            Color imageColor = timesXImage.color;
            imageColor.a = alpha;
            timesXImage.color = imageColor;
        }

        if (includeComboScore && comboScoreText != null)
        {
            Color textColor = comboScoreText.color;
            textColor.a = alpha;
            comboScoreText.color = textColor;
        }
    }

    public IEnumerator MultiplierBoost(int multiplier, float duration)
    {
        previousMultiplier = comboMultiplier;
        comboMultiplier = multiplier;
        StartCoroutine(AnimateMultiplierText(duration));
        yield return new WaitForSeconds(duration);
        comboMultiplier = previousMultiplier;
        comboMultiplierText.transform.localScale = Vector3.one;
    }

    private IEnumerator AnimateMultiplierText(float duration)
    {
        float elapsed = 0f;
        float scaleFactor = 1.5f;
        Vector3 originalScale = comboMultiplierText.transform.localScale;

        while (elapsed < duration)
        {
            float scale = Mathf.Lerp(1f, scaleFactor, Mathf.PingPong(elapsed * 5, 1f));
            comboMultiplierText.transform.localScale = originalScale * scale;
            elapsed += Time.deltaTime;
            yield return null;
        }

        comboMultiplierText.transform.localScale = originalScale;
    }

    private void UpdateUI()
    {
        if (killCountText != null)
        {
            killCountText.text = "Kill Count: " + killCount.ToString();
        }

        if (comboMultiplierText != null)
        {
            comboMultiplierText.text = "" + comboMultiplier.ToString();
        }

        if (comboTimerSlider != null)
        {
            comboTimerSlider.value = comboTimer;
        }

        if (comboScoreText != null)
        {
            comboScoreText.text = comboScore.ToString();
        }
    }

    private void ShowComboUI(bool show)
    {
        if (killCountText != null)
        {
            killCountText.gameObject.SetActive(show);
        }

        if (comboMultiplierText != null)
        {
            comboMultiplierText.gameObject.SetActive(show);
        }

        if (comboTimerSlider != null)
        {
            comboTimerSlider.gameObject.SetActive(show);
        }

        if (multikillImage != null)
        {
            multikillImage.gameObject.SetActive(show);
        }

        if (timesXImage != null)
        {
            timesXImage.gameObject.SetActive(show);
        }

        if (comboScoreText != null)
        {
            comboScoreText.gameObject.SetActive(show);
        }
    }
}