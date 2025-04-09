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

    // UI Elements
    public TextMeshProUGUI killCountText;
    public TextMeshProUGUI comboMultiplierText;
    public Slider comboTimerSlider;
    public Image multikillImage;
    public Image timesXImage;
    public TextMeshProUGUI comboScoreText;

    private float killWindow = 3f;
    private float lastKillTime = 0f;

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
        // Ensure UI is hidden at the start
        ShowComboUI(false);
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

        // Update UI elements
        UpdateUI();
    }

    public void AddKill()
    {
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
                comboTimer = 3f; // Reset the combo timer
            }
        }
        else
        {
            killCount = 1; // Reset kill count if outside the kill window
        }

        lastKillTime = Time.time;

        if (isComboActive)
        {
            comboMultiplier = Mathf.Min(10, 2 + (killCount - 3) / 10);
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
        skillPoints++;
        comboMultiplier = Mathf.Min(10, comboMultiplier + 1);
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
        if (!isCountingDown)
        {
            StartCoroutine(CountdownComboScore());
        }
    }

    private IEnumerator CountdownComboScore()
    {
        isCountingDown = true;
        int scoreToAdd = maxComboScore;
        int increment = Mathf.Max(1, scoreToAdd / 10); // Adjust the increment as needed

        while (scoreToAdd > 0)
        {
            int addAmount = Mathf.Min(increment, scoreToAdd);
            ScoreManager.Instance.AddScore(addAmount);
            comboScore = Mathf.Max(0, comboScore - addAmount); // Decrease the combo score
            scoreToAdd -= addAmount;
            yield return new WaitForSeconds(0.1f); // Adjust the delay as needed
        }

        comboScore = 0;
        maxComboScore = 0;
        isCountingDown = false;
        ScoreManager.Instance.HighlightScore();
        ShowComboUI(false);
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
            comboScoreText.text = "Combo Score: " + comboScore.ToString();
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