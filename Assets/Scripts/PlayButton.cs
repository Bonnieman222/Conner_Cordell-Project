using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayButton : MonoBehaviour
{
    [Header("Scene To Load")]
    public string gameplaySceneName;

    public void PlayGame()
    {
        if (string.IsNullOrEmpty(gameplaySceneName))
        {
            Debug.LogError("No scene name assigned!");
            return;
        }

        Debug.Log("Loading scene: " + gameplaySceneName);
        SceneManager.LoadScene(gameplaySceneName);
    }
}