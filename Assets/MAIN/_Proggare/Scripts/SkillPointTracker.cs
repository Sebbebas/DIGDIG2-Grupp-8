using UnityEngine;

public class SkillPointTracker : MonoBehaviour
{
    private int lowHealthKillCount = 0;
    private PlayerHealth playerHealth;

    void Start()
    {
        playerHealth = FindFirstObjectByType<PlayerHealth>();
    }

    //Call when enemy die
    public void RegisterKill()
    {
        if (playerHealth != null && playerHealth.GetCurrentHealth() <= 20f)
        {
            lowHealthKillCount++;

            Debug.Log($"Kill while low HP! Current count: {lowHealthKillCount}");

            if (lowHealthKillCount >= 3)
            {
                SkillPointManager.Instance.AddPoint();
                lowHealthKillCount = 0; //Reset
            }
        }
    }
}