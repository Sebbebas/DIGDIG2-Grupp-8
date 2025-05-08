using UnityEngine;
using TMPro;

public class FinalScoreDisplay : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI totalScoreText;

    private void Start()
    {
        if (totalScoreText != null && ScoreManager.Instance != null)
        {
            int finalScore = ScoreManager.Instance.GetScore();
            totalScoreText.text = "TOTAL SCORE: " + finalScore;
        }
        else
        {
            Debug.LogWarning("TotalScoreText reference or ScoreManager.Instance is missing.");
        }
    }
}
