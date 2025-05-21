using UnityEngine;

public class SettingsHandler : MonoBehaviour
{
    [Header("Settings Enviroment")]
    [SerializeField] GameObject generalSettings;
    [SerializeField] GameObject audioSettings;
    [SerializeField] GameObject controlSettings;

    [Header("Warning")]
    [SerializeField] GameObject noSaveWarning;
    [SerializeField] GameObject revertWarning;

    bool valueChanged; //public

    public void Apply()
    {
        valueChanged = false;
    }

    public void Revert()
    {
        if (valueChanged)
        {
            revertWarning.SetActive(true);
        }
    }

    public void Back()
    {
        if (valueChanged)
        {
            noSaveWarning.SetActive(true);
        }
        else
        {
            //disable settings Canvas
        }
    }

    public void ActiveSettings(int active)
    {
        switch (active)
        {
            case 0:
                generalSettings.SetActive(true);
                audioSettings.SetActive(false);
                controlSettings.SetActive(false);
                break;

            case 1:
                generalSettings.SetActive(false);
                audioSettings.SetActive(true);
                controlSettings.SetActive(false);
                break;

            case 2:
                generalSettings.SetActive(false);
                audioSettings.SetActive(false);
                controlSettings.SetActive(true);
                break;
        }
    }

    public void ValueChanged()
    {
        valueChanged = true;
    }
}
