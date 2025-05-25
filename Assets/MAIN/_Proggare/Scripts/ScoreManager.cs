using UnityEngine;
using TMPro;
using System.Collections;
using UnityEngine.SceneManagement;
using System;

public enum StatType
{
    ScoreText,
    Time,

    EnemiesKilled,
    TotalEnemies,
    
    DamageTaken,
    DamageDealt,
    
    KillCombos,
    HighestCombo,
    LongestKillCombo,
    LongestKillComboTime,

    Accuracy,
    ShotsFired,
    ShotsHit,
}

public class ScoreManager : MonoBehaviour
{
    public static ScoreManager Instance;

    [Header("Stats")]
    [SerializeField] int score = 0;
    [SerializeField] int time = 0;

    [SerializeField] int enemiesKilled;
    [SerializeField] int totalEnemies;

    [SerializeField] int damageTaken = 0;
    [SerializeField] int damageDealt = 0;

    [SerializeField] int killCombos = 0;
    [SerializeField] int HighestCombo = 0;
    [SerializeField] int longestKillCombo = 0;
    [SerializeField] int longestKillComboTime = 0;

    [SerializeField] int accuracy = 0;
    [SerializeField] int shotsFired = 0;
    [SerializeField] int shotsHit = 0;

    [Header("UI")]
    public TextMeshProUGUI scoreText;

    //Private Variables
    private float targetFontSize;
    private float smoothTime = 2f;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(this.gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void Start()
    {
        UpdateScoreUI();
        if (scoreText != null)
            targetFontSize = scoreText.fontSize;
    }

    public void AddScore(int points)
    {
        score += points;
        UpdateScoreUI();
    }

    public void HighlightScore()
    {
        StartCoroutine(HighlightScoreRoutine());
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        TextMeshProUGUI newScoreText = GameObject.FindWithTag("ScoreText")?.GetComponent<TextMeshProUGUI>();
        if (newScoreText != null)
        {
            scoreText = newScoreText;
            UpdateScoreUI();
            targetFontSize = scoreText.fontSize;
        }
    }

    private static void DisplayStats()
    {
        int score = Instance.GetStatValue(StatType.ScoreText);
        int enemiesKilled = Instance.GetStatValue(StatType.EnemiesKilled);
        int totalEnemies = Instance.GetStatValue(StatType.TotalEnemies);
        int accuracy = Instance.GetStatValue(StatType.Accuracy);
        int totalShotsFired = Instance.GetStatValue(StatType.ShotsFired);
        int shotsHit = Instance.GetStatValue(StatType.ShotsHit);

        foreach (StatType stat in Enum.GetValues(typeof(StatType)))
        {
            string statName = stat.ToString();
            GameObject obj = GameObject.Find(statName);

            if (obj == null)
            {
                Debug.LogWarning($"No GameObject found with the name '{statName}'. Skipping.");
                continue;
            }

            TextMeshProUGUI statText = obj.GetComponent<TextMeshProUGUI>();
            if (statText == null)
            {
                Debug.LogWarning($"GameObject '{statName}' found but missing TextMeshProUGUI component.");
                continue;
            }

            // Custom display logic
            switch (stat)
            {
                case StatType.ScoreText:
                    statText.text = $"SCORE: {score}";
                    break;
                case StatType.EnemiesKilled:
                    statText.text = $"ENEMIES: {enemiesKilled} / {totalEnemies}";
                    break;

                case StatType.TotalEnemies:
                    // Do nothing — it's shown as part of EnemiesKilled above
                    obj.SetActive(false); // Optionally hide it
                    break;

                case StatType.Accuracy:
                    statText.text = $"ACCURACY: {accuracy}%";
                    break;

                case StatType.ShotsFired:
                    statText.text = $"SHOTS HIT: {shotsHit} / {totalShotsFired}";
                    break;
                default:
                    statText.text = $"{statName.ToUpper()}: {Instance.GetStatValue(stat)}";
                    break;
            }
        }
    }
    private IEnumerator HighlightScoreRoutine()
    {
        if (scoreText != null)
        {
            float originalFontSize = scoreText.fontSize;
            targetFontSize = originalFontSize * 1.2f;
            scoreText.color = Color.yellow;

            yield return new WaitForSeconds(1f);

            targetFontSize = originalFontSize;
            scoreText.color = Color.white;
        }
    }

    private void Update()
    {
        if (scoreText != null)
        {
            scoreText.fontSize = Mathf.Lerp(scoreText.fontSize, targetFontSize, smoothTime * Time.deltaTime);
        }
    }

    public void UpdateScoreUI()
    {
        if (scoreText != null)
        {
            scoreText.text = "SCORE: " + score;
        }
    }

    public int GetScore()
    {
        return score;
    }

    public void ResetScore()
    {
        score = 0;
        UpdateScoreUI();
    }

    //button to reset and restart the game
    public void RestartGame(string sceneName)
    {
        ResetScore();
        SceneManager.LoadScene(sceneName);
    }
    public void SetStat(StatType stat, int value)
    {
        switch (stat)
        {
            case StatType.ScoreText:
                score = value;
                UpdateScoreUI();
                break;
            case StatType.Time:
                time = value;
                break;
            case StatType.EnemiesKilled:
                enemiesKilled = value;
                break;
            case StatType.TotalEnemies:
                totalEnemies = value;
                break;
            case StatType.DamageTaken:
                damageTaken = value;
                break;
            case StatType.DamageDealt:
                damageDealt = value;
                break;
            case StatType.KillCombos:
                killCombos = value;
                break;
            case StatType.HighestCombo:
                HighestCombo = value;
                break;
            case StatType.LongestKillCombo:
                longestKillCombo = value;
                break;
            case StatType.LongestKillComboTime:
                longestKillComboTime = value;
                break;
            case StatType.Accuracy:
                accuracy = value;
                break;
            case StatType.ShotsFired:
                shotsFired = value;
                break;
            case StatType.ShotsHit:
                shotsHit = value;
                break;
            default:
                Debug.LogWarning("Unknown stat type: " + stat);
                break;
        }
    }

    private int GetStatValue(StatType stat)
    {
        return stat switch
        {
            StatType.ScoreText => score,
            StatType.Time => time,
            StatType.EnemiesKilled => enemiesKilled,
            StatType.TotalEnemies => totalEnemies,
            StatType.DamageTaken => damageTaken,
            StatType.DamageDealt => damageDealt,
            StatType.KillCombos => killCombos,
            StatType.HighestCombo => HighestCombo,
            StatType.LongestKillCombo => longestKillCombo,
            StatType.LongestKillComboTime => longestKillComboTime,
            StatType.Accuracy => accuracy,
            StatType.ShotsFired => shotsFired,
            StatType.ShotsHit => shotsHit,
            _ => 0
        };
    }
}