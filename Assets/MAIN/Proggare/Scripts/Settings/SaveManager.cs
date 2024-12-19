using UnityEngine;

public class SaveManager : MonoBehaviour
{
    public static void SaveScore(int score)
    {
        PlayerPrefs.SetInt("PlayerScore", score);
        PlayerPrefs.Save();
    }

    public static int LoadScore()
    {
        return PlayerPrefs.HasKey("PlayerScore") ? PlayerPrefs.GetInt("PlayerScore") : 0;
    }

    public static void DeleteScore()
    {
        PlayerPrefs.DeleteKey("PlayerScore");
    }
}
