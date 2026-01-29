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

        // Tell the camera to skip intro when loading this scene
        CameraMoveWithBlackStart.SetSkipIntroFlag();

        // Load the scene
        SceneManager.LoadScene(sceneName);
    }
}