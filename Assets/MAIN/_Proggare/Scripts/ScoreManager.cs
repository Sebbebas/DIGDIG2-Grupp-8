using UnityEngine;
using TMPro;
using System.Collections;
using UnityEngine.SceneManagement;

public class ScoreManager : MonoBehaviour
{
    public static ScoreManager Instance;

    [SerializeField] private int score = 0;
    public TextMeshProUGUI scoreText;
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
}