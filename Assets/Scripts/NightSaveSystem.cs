using UnityEngine;
using UnityEngine.SceneManagement;

public class NightSaveSystem : MonoBehaviour
{
    private const string NightSaveKey = "SavedNight";

    [Header("Debug / Inspector Tools")]
    [Tooltip("Check this to delete the saved night progress")]
    public bool deleteSave;

    private void Awake()
    {
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

    private void Update()
    {
        // Inspector button-style reset
        if (deleteSave)
        {
            DeleteSave();
            deleteSave = false; // reset toggle
        }
    }

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
    /// Delete saved night progress.
    /// </summary>
    public void DeleteSave()
    {
        PlayerPrefs.DeleteKey(NightSaveKey);
        PlayerPrefs.Save();
        Debug.Log("Night save deleted.");

        if (NightSystem.Instance != null)
        {
            NightSystem.Instance.currentNight = 1;
            NightSystem.Instance.completedProcesses = 0;
        }
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
}