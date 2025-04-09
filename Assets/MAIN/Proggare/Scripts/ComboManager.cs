using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections;

//Alexander

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

    //UI Elements
    public TextMeshProUGUI killCountText;
    public TextMeshProUGUI comboMultiplierText;
    public Slider comboTimerSlider;
    public Image multikillImage;
    public Image timesXImage;
    public TextMeshProUGUI comboScoreText;

    //audio Sources
    public AudioSource countingAudioSource;
    public AudioSource scoreCompleteAudioSource;
    public AudioSource multiplierIncreaseAudioSource;

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
                comboTimer = 3f;
            }
        }
        else
        {
            killCount = 1;
        }

        lastKillTime = Time.time;

        if (isComboActive)
        {
            int newMultiplier = Mathf.Min(10, 2 + (killCount - 3) / 10);
            if (newMultiplier > comboMultiplier)
            {
                //Play multiplier increase sound
                if (multiplierIncreaseAudioSource != null)
                {
                    multiplierIncreaseAudioSource.Play();
                }
                comboMultiplier = newMultiplier;
            }

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

        //Play counting sound
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

        //Play score complete sound
        if (scoreCompleteAudioSource != null)
        {
            scoreCompleteAudioSource.Play();
        }

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