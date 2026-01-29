using UnityEngine;

public class ResetCurrentNight : MonoBehaviour
{
    public void ResetNight()
    {
        var night = NightSystem.Instance;
        var transition = FindObjectOfType<NightTransitionController>();
        var pause = FindObjectOfType<PauseMenuWithIntro>();

        if (night == null)
        {
            Debug.LogError("NightSystem missing!");
            return;
        }

        // Reset progress but keep same night
        int target = night.currentNight;
        night.completedProcesses = 0;

        // Close pause first
        if (pause != null)
            pause.ClosePause();

        // Trigger intro canvas
        if (pause != null)
            pause.PlayIntro();

        Debug.Log("Reset night " + target + " and replay intro!");
    }
}
