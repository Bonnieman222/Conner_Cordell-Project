using UnityEngine;
using UnityEngine.SceneManagement;

public class NightSaveSystem : MonoBehaviour
{
    private const string NightSaveKey = "SavedNight";

    private void Awake()
    {
        // Keep this object when switching scenes
        DontDestroyOnLoad(gameObject);
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    // This replaces Start() because Start can run before NightSystem exists
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        LoadNightProgress();
    }

    /// <summary>
    /// Save the furthest night reached.
    /// </summary>
    public void SaveNightProgress(int night)
    {
        PlayerPrefs.SetInt(NightSaveKey, night);
        PlayerPrefs.Save();
        Debug.Log($"Night progress saved: Night {night}");
    }

    /// <summary>
    /// Load the saved night and apply it to NightSystem.
    /// </summary>
    private void LoadNightProgress()
    {
        if (!PlayerPrefs.HasKey(NightSaveKey))
        {
            Debug.Log("No saved night found. Starting at Night 1.");
            return;
        }

        int savedNight = PlayerPrefs.GetInt(NightSaveKey);

        if (NightSystem.Instance != null)
        {
            NightSystem.Instance.currentNight = savedNight;
            NightSystem.Instance.completedProcesses = 0;
        }

        Debug.Log($"Loaded saved night: Night {savedNight}");
    }

    /// <summary>
    /// Optional: Clear save data for testing or debug menu.
    /// </summary>
    public void ResetProgress()
    {
        PlayerPrefs.DeleteKey(NightSaveKey);
        Debug.Log("Night progress reset.");
    }
}