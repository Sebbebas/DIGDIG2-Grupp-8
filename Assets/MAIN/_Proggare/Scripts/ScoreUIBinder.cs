using UnityEngine;

public class ScoreUIBinder : MonoBehaviour
{
    void Start()
    {
        // Ensure the ScoreManager updates the score text when this scene is loaded
        if (ScoreManager.Instance != null)
        {
            ScoreManager.Instance.UpdateScoreUI();
        }
    }
}