using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuController : MonoBehaviour
{
    // This function will be called when the "Start Game" button is pressed
    public void StartGame()
    {
        // Assuming "GameScene" is the name of your main gameplay scene
        SceneManager.LoadScene("Main_Scene");
    }

    // This function will be called when the "Settings" button is pressed
    public void OpenSettings()
    {
        // Load settings scene, or if it's an overlay, activate a settings menu UI
        SceneManager.LoadScene("Settings");
        // Alternatively, activate/deactivate a settings panel
        // settingsPanel.SetActive(true); // Assuming you have a settings UI panel to open
    }



    public void OpenMainMenu()
    {
        
        SceneManager.LoadScene("Main Menu");
       
    }

    // This function will be called when the "Quit Game" button is pressed
    public void QuitGame()
    {
        // Log a message for debug purposes
        Debug.Log("Quit Game");

        // If we're running the game in the editor, stop play mode
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
            // Quit the application when running the build
            Application.Quit();
#endif
    }

    // This function will be called when the "Credits" button is pressed
    public void OpenCredits()
    {
        // Load the credits scene
        SceneManager.LoadScene("CreditsScene");
    }
}
