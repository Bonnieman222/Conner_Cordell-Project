using UnityEngine;

public class NightSystem : MonoBehaviour
{
    public static NightSystem Instance;

    public int currentNight = 1;
    public int completedProcesses = 0;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public int GetRequiredProcesses()
    {
        switch (currentNight)
        {
            case 1: return 3;
            case 2: return 4;
            case 3: return 5;
            case 4: return 6;
            case 5: return 7;
            case 6: return 8;
            default: return 9;
        }
    }

    public float GetProcessTime()
    {
        switch (currentNight)
        {
            case 1: return 25f;
            case 2: return 30f;
            case 3: return 35f;
            case 4: return 40f;
            case 5: return 45f;
            case 6: return 50f;
            default: return 55f;
        }
    }

    public void ProcessCompleted()
    {
        completedProcesses++;

        if (completedProcesses >= GetRequiredProcesses())
        {
            currentNight++;
            completedProcesses = 0;

            // Save progress
            PlayerPrefs.SetInt("SavedNight", currentNight);
            PlayerPrefs.Save();
        }
    }
}