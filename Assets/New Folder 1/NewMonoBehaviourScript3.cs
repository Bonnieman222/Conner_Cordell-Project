using UnityEngine;
using UnityEngine.SceneManagement;

public class UIButtonSceneSwitcher : MonoBehaviour
{
    [Header("Scene Settings")]
    public string sceneToLoad = "NextScene"; // Name of the scene you want to load

    // This is what the UI button will call
    public void LoadScene()
    {
        SceneManager.LoadScene(sceneToLoad);
    }
}