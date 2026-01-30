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

    // Automatically updates previous/current scenes on load
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // Only update previous scene if this is NOT the first scene
        if (!string.IsNullOrEmpty(currentScene) && currentScene != scene.name)
        {
            previousScene = currentScene;
        }

        // Update current scene
        currentScene = scene.name;
    }

    /// <summary>
    /// Call this from a button to return to the previous scene
    /// </summary>
    public void ReturnToPreviousScene()
    {
        if (string.IsNullOrEmpty(previousScene))
        {
            Debug.LogWarning("No previous scene recorded!");
            return;
        }

        SceneManager.LoadScene(previousScene);
    }
}
