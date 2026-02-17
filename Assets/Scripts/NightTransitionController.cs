using UnityEngine;

public class NightTransitionController : MonoBehaviour
{
    public PauseMenuWithIntro pauseSystem;
    public NightDisplay nightDisplay;
    public FlashlightController flashlightController; // ADD THIS

    private int lastNight = -1;

    void Update()
    {
        if (NightSystem.Instance == null) return;

        int night = NightSystem.Instance.currentNight;

        if (night != lastNight)
        {
            lastNight = night;
            RunTransition(night);
        }
    }

    public void ForceTransition(int night)
    {
        lastNight = -999;
        RunTransition(night);
        lastNight = night;
    }

    private void RunTransition(int night)
    {
        if (nightDisplay != null)
            nightDisplay.SetNight(night);

        if (pauseSystem != null)
            pauseSystem.PlayIntro();

        // RESET TOOLS EACH NIGHT
        if (flashlightController != null)
            flashlightController.ResetForNewNight();
    }
}