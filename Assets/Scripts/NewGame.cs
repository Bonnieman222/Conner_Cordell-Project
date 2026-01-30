using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayNightOneButton : MonoBehaviour
{
    [Header("Scene To Load")]
    public string gameplaySceneName;

    [Header("Reference to Night Save System")]
    public NightSaveSystem nightSaveSystem;

    public void PlayNightOne()
    {
        if (string.IsNullOrEmpty(gameplaySceneName))
        {
            Debug.LogError("No scene name assigned!");
            return;
        }

        // Force the save to Night 1 BEFORE loading scene
        if (nightSaveSystem != null)
        {
            nightSaveSystem.SaveNightProgress(1);
            Debug.Log("Night progress overridden to Night 1.");
        }

        // Use a coroutine to ensure NightSystem is set after scene loads
        StartCoroutine(LoadSceneAndSetNight());
    }

    private System.Collections.IEnumerator LoadSceneAndSetNight()
    {
        // Load the scene asynchronously
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(gameplaySceneName);
        while (!asyncLoad.isDone)
        {
            yield return null;
        }

        // Wait one frame to ensure NightSystem exists
        yield return null;

        // Force NightSystem to Night 1 after scene load
        if (NightSystem.Instance != null)
        {
            NightSystem.Instance.currentNight = 1;
            NightSystem.Instance.completedProcesses = 0;
            Debug.Log("NightSystem set to Night 1 after scene load.");
        }
        else
        {
            Debug.LogWarning("NightSystem instance not found after scene load.");
        }
    }
}