using UnityEngine;

public class ReturnButton : MonoBehaviour
{
    public void ReturnToPreviousScene()
    {
        if (SceneTracker.Instance != null)
        {
            SceneTracker.Instance.ReturnToPreviousScene();
        }
        else
        {
            Debug.LogWarning("SceneTracker instance not found!");
        }
    }
}