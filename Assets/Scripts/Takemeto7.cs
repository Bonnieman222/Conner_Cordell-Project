using UnityEngine;
using UnityEngine.SceneManagement;

public class Night7CompletionLoader : MonoBehaviour
{
    [Header("Scene Settings")]
    public string sceneToLoad = "EndingScene";

    private bool triggered = false;

    private void Update()
    {
        if (triggered)
            return;

        if (NightSystem.Instance == null)
            return;

        // If the game has moved PAST night 7, that means night 7 completed
        if (NightSystem.Instance.currentNight > 7)
        {
            triggered = true;
            LoadEndingScene();
        }
    }

    private void LoadEndingScene()
    {
        Debug.Log("Night 7 completed. Loading ending scene.");
        SceneManager.LoadScene(sceneToLoad);
    }
}