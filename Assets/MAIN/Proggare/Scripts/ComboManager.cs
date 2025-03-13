using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class ComboManager : MonoBehaviour
{
    public static ComboManager instance;

    public int currentCombo = 0;
    public float comboTimer = 0f;
    public float maxComboTime = 5f;

    public TextMeshProUGUI comboText;
    public Slider comboSlider; // Optional: progress bar for timer

    private void Awake()
    {
        if (instance == null) instance = this;
        else Destroy(gameObject);
    }

    private void Start()
    {
        HideComboUI();
    }

    void Update()
    {
        if (currentCombo > 0)
        {
            comboTimer -= Time.deltaTime;
            UpdateComboUI();

            if (comboTimer <= 0)
            {
                ResetCombo();
            }
        }
    }

    public void AddKill()
    {
        currentCombo++;
        comboTimer = maxComboTime;
        ShowComboUI();
    }

    public void ResetCombo()
    {
        currentCombo = 0;
        HideComboUI();
    }

    private void UpdateComboUI()
    {
        comboText.text = $"Combo: {currentCombo}";
        if (comboSlider) comboSlider.value = comboTimer / maxComboTime;
    }

    private void ShowComboUI()
    {
        comboText.gameObject.SetActive(true);
        if (comboSlider) comboSlider.gameObject.SetActive(true);
    }

    private void HideComboUI()
    {
        comboText.gameObject.SetActive(false);
        if (comboSlider) comboSlider.gameObject.SetActive(false);
    }
}