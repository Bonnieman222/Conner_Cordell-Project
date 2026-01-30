using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneSwitchButton : MonoBehaviour
{
    public string sceneName;

    public void SwitchScene()
    {
        if (string.IsNullOrEmpty(sceneName))
        {
            Debug.LogError("Scene name is empty!");
            return;
        }

        // Ensure time is running normally when entering the new scene
        Time.timeScale = 1f;

        // Load the scene fresh so its intro plays normally
        SceneManager.LoadScene(sceneName, LoadSceneMode.Single);
    }
}