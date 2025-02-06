using UnityEngine;
using TMPro;

//Alexander

public class ScoreManager : MonoBehaviour
{
    public static ScoreManager Instance;

    [SerializeField] int score = 0;
    public TextMeshProUGUI scoreText;

    private void Awake()
    {
        //Ensure only one instance exists
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        UpdateScoreUI();
    }

    public void AddScore(int points)
    {
        score += points;
        UpdateScoreUI();
    }

    private void UpdateScoreUI()
    {
        if (scoreText != null)
        {
            scoreText.text = "SCORE: " + score;
        }
    }
}
