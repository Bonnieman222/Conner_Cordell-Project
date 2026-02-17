using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneTracker : MonoBehaviour
{
    public static SceneTracker Instance;

    private string previousScene = "";
    private string currentScene = "";

    void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        // Initialize current scene
        currentScene = SceneManager.GetActiveScene().name;

        // Subscribe to sceneLoaded event
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // Only update previous scene if currentScene is not empty
        if (!string.IsNullOrEmpty(currentScene) && currentScene != scene.name)
        {
            previousScene = currentScene;
        }

        currentScene = scene.name;
        // Optional debug
        Debug.Log($"Scene Loaded: {scene.name} | Previous Scene: {previousScene}");
    }

    public void ReturnToPreviousScene()
    {
        if (string.IsNullOrEmpty(previousScene))
        {
            Debug.LogWarning("No previous scene recorded!");
            return;
        }

        // Load previous scene
        SceneManager.LoadScene(previousScene);
    }

    // Optional helper to manually set previous scene
    public void SetPreviousScene(string sceneName)
    {
        previousScene = sceneName;
    }
}