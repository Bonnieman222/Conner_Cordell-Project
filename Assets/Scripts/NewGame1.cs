using UnityEngine;
using UnityEngine.SceneManagement;

public class NewGameButton : MonoBehaviour
{
    [Header("Scene To Load")]
    public string gameplaySceneName;

    public void StartNewGame()
    {
        // Force save to Night 1
        PlayerPrefs.SetInt("SavedNight", 1);
        PlayerPrefs.Save();

        Debug.Log("New Game started — forced Night 1");

        // Force runtime state (if already loaded)
        if (NightSystem.Instance != null)
        {
            NightSystem.Instance.currentNight = 1;
            NightSystem.Instance.completedProcesses = 0;
        }

        // Load gameplay scene
        if (!string.IsNullOrEmpty(gameplaySceneName))
        {
            SceneManager.LoadScene(gameplaySceneName);
        }
        else
        {
            Debug.LogError("Gameplay scene name not set!");
        }
    }
}