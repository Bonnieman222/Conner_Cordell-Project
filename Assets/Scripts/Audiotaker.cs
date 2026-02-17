using UnityEngine;
using UnityEngine.SceneManagement;

public class ReturnToMainMenu : MonoBehaviour
{
    [Header("Main Menu Scene")]
    public string mainMenuSceneName = "MainMenu";

    /// <summary>
    /// Call this from a UI Button OnClick event
    /// </summary>
    public void GoToMainMenu()
    {
        Debug.Log("Returning to Main Menu and resetting night progress.");

        // Reset Night Save
        NightSaveSystem save = FindObjectOfType<NightSaveSystem>();
        if (save != null)
        {
            save.DeleteSave();
            // Destroy persistent save system so main menu can initialize clean
            Destroy(save.gameObject);
        }

        // Load Main Menu scene
        SceneManager.LoadScene(mainMenuSceneName);
    }
}