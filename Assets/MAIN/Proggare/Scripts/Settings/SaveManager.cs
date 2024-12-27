using UnityEngine;

public class SaveManager : MonoBehaviour
{
    const string sensitivityKey = "Sensitivity"; // Key for PlayerPrefs

    // Save the sensitivity value
    public void SaveSensitivity(int sensitivity)
    {
        PlayerPrefs.SetInt(sensitivityKey, sensitivity); // Save the value
        PlayerPrefs.Save(); // Persist the data
        Debug.Log("Sensitivity saved: " + sensitivity);
    }

    // Load the sensitivity value
    public int LoadSensitivity()
    {
        if (PlayerPrefs.HasKey(sensitivityKey))
        {
            int sensitivity = PlayerPrefs.GetInt(sensitivityKey); // Retrieve the saved value
            Debug.Log("Sensitivity loaded: " + sensitivity);
            return sensitivity;
        }
        else
        {
            Debug.Log("No saved sensitivity found, returning default value.");
            return 10; // Default value if no sensitivity is found
        }
    }
}