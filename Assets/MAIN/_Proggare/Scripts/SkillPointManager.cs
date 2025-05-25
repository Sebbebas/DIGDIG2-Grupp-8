using UnityEngine;
using TMPro;
using System.Collections;
using System.Collections.Generic;

public class SkillPointManager : MonoBehaviour
{
    public static SkillPointManager Instance;

    [SerializeField] private GameObject popupPrefab;
    [SerializeField] private Transform popupParent;
    [SerializeField] private float fadeDuration = 2f;

    private int skillPoints = 0;

    private Dictionary<string, List<string>> messageBank = new();

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        InitializeMessages();
    }

    private void InitializeMessages()
    {
        messageBank["LowHealthKill"] = new List<string> {
            "No Rest for the Wicked",
            "Mercy",
            "Low HP"
        };

        //More options if needed
    }

    public void AddPoint()
    {
        skillPoints++;
        Debug.Log($"Skill Point Earned! Total: {skillPoints}");
        ShowPopup("LowHealthKill");
    }

    public void ShowPopup(string category)
    {
        if (!messageBank.ContainsKey(category)) return;

        string randomMessage = messageBank[category][Random.Range(0, messageBank[category].Count)];
        StartCoroutine(SpawnPopup(randomMessage));
    }

    private IEnumerator SpawnPopup(string message)
    {
        GameObject popup = Instantiate(popupPrefab, popupParent);
        TextMeshProUGUI text = popup.GetComponentInChildren<TextMeshProUGUI>();

        text.text = message;
        Color originalColor = text.color;

        float elapsed = 0f;
        while (elapsed < fadeDuration)
        {
            float alpha = Mathf.Lerp(1f, 0f, elapsed / fadeDuration);
            text.color = new Color(originalColor.r, originalColor.g, originalColor.b, alpha);
            popup.transform.position += Vector3.up * Time.deltaTime * 30f;
            elapsed += Time.deltaTime;
            yield return null;
        }

        Destroy(popup);
    }
}