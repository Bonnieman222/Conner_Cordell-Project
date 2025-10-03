using UnityEngine;

public class NightSystem : MonoBehaviour
{
    public int currentNight = 1;  // Starts at Night 1
    public int completedProcesses = 0;

    public static NightSystem Instance;  // Singleton so ProcessController can reference it

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    // How many times the process must be completed per night
    public int GetRequiredProcesses()
    {
        switch (currentNight)
        {
            case 1: return 3;
            case 2: return 4;
            case 3: return 4;
            case 4: return 5;
            case 5: return 6;
            case 6: return 7;
            default: return 7; // If beyond night 6, keep it at 7
        }
    }

    // How long the process takes depending on the night
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
            default: return 50f;
        }
    }

    // Call when a process is completed successfully
    public void ProcessCompleted()
    {
        completedProcesses++;
        Debug.Log($"Completed processes this night: {completedProcesses}/{GetRequiredProcesses()}");

        if (completedProcesses >= GetRequiredProcesses())
        {
            Debug.Log($"Night {currentNight} completed!");
            currentNight++;
            completedProcesses = 0;
        }
    }
}