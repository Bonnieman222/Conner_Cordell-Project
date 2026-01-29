using UnityEngine;
using UnityEngine.SceneManagement;

public class ReloadSceneAndLoadSave : MonoBehaviour
{
    public void ReloadScene()
    {
        // Reload the current active scene
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    private void Start()
    {
        // After the scene loads, restore last save
        int savedNight = PlayerPrefs.HasKey("SavedNight") ? PlayerPrefs.GetInt("SavedNight") : 1;

        if (NightSystem.Instance != null)
        {
            NightSystem.Instance.currentNight = savedNight;
            NightSystem.Instance.completedProcesses = 0;
        }

        // Trigger the intro canvas if PauseMenuWithIntro exists
        var pause = FindObjectOfType<PauseMenuWithIntro>();
        if (pause != null)
        {
            pause.PlayIntro();
        }
    }
}