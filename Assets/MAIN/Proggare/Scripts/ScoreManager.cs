using UnityEngine;
using TMPro;
using System.Collections;
using UnityEngine.SceneManagement;

//Alexander

public class ScoreManager : MonoBehaviour
{
    public static ScoreManager Instance;

    [SerializeField] private int score = 0;
    public TextMeshProUGUI scoreText;
    private float targetFontSize;
    private float smoothTime = 2f;

    private void Awake()
    {
        //Ensure only one instance exists
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
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
            targetFontSize = originalFontSize * 1.2f; //Increase size
            scoreText.color = Color.yellow; //Change color to yellow

            yield return new WaitForSeconds(1f);

            targetFontSize = originalFontSize; //Reset size
            scoreText.color = Color.white; //Reset color
        }
    }

    private void Update()
    {
        if (scoreText != null)
        {
            scoreText.fontSize = Mathf.Lerp(scoreText.fontSize, targetFontSize, smoothTime * Time.deltaTime);
        }
    }

    private void UpdateScoreUI()
    {
        if (scoreText != null)
        {
            scoreText.text = "SCORE: " + score;
        }
    }
}