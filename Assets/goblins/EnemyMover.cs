using UnityEngine;

public class EnemyMoverInstant : MonoBehaviour
{
    [Header("Waypoints")]
    public Transform[] waypoints;

    [Header("Monster References")]
    public Transform monsterA;
    public Transform monsterB;

    [Header("Base Move Times")]
    public float baseMoveTimeA = 6f;
    public float baseMoveTimeB = 8f;

    [Header("Monster B Flashlight Speed")]
    public float flashlightSpeedMultiplier = 2f;

    [Header("Monster B Reset")]
    public float bResetDelay = 10f;

    [Header("Post-Reset Freeze")]
    public float baseFreezeDuration = 3f; // Freeze after reset on Night 1

    // Internal state
    private int aIndex = 0;
    private int bIndex = 0;
    private float aTimer = 0f;
    private float bTimer = 0f;
    private bool aFinal = false;
    private bool bFinal = false;
    private float bFlashlightOffTimer = 0f;
    private float resetFreezeTimer = 0f;

    // Systems
    private FlashlightController flashlight;
    private CameraMover camMover;
    private NightSystem nightSystemInstance;

    void Start()
    {
        flashlight = FindObjectOfType<FlashlightController>();
        camMover = FindObjectOfType<CameraMover>();
        nightSystemInstance = NightSystem.Instance;

        if (monsterA != null) monsterA.position = waypoints[0].position;
        if (monsterB != null) monsterB.position = waypoints[0].position;
    }

    void Update()
    {
        if (waypoints.Length == 0) return;

        // Update timers
        aTimer += Time.deltaTime;
        bTimer += Time.deltaTime;

        // Flashlight off timer for Monster B reset
        if (flashlight == null || !flashlight.flashlight.enabled) bFlashlightOffTimer += Time.deltaTime;
        else bFlashlightOffTimer = 0f;

        // Reduce freeze timer
        if (resetFreezeTimer > 0f)
        {
            resetFreezeTimer -= Time.deltaTime;
            return; // Skip movement while freeze active
        }

        // Handle monsters
        HandleMonsters();
    }

    void HandleMonsters()
    {
        // Determine movement time for each monster
        float moveTimeA = Mathf.Max(0.5f, baseMoveTimeA - (nightSystemInstance.currentNight - 1));
        float moveTimeB = Mathf.Max(0.5f, baseMoveTimeB - (nightSystemInstance.currentNight - 1));

        bool flashOn = flashlight != null && flashlight.flashlight.enabled;
        if (flashOn && bIndex >= 1 && bIndex <= 4) moveTimeB /= flashlightSpeedMultiplier;

        // Rarity scaling per night
        float chanceA = Mathf.Lerp(0.9f, 0.4f, (nightSystemInstance.currentNight - 1) / 5f);
        float chanceB = Mathf.Lerp(0.1f, 0.6f, (nightSystemInstance.currentNight - 1) / 5f);

        // Mutual blocking: a monster cannot leave 0 if the other is at 1–4
        bool aBlocked = bIndex >= 1 && bIndex <= 4;
        bool bBlocked = aIndex >= 1 && aIndex <= 4;

        // Randomized first mover at element 0, weighted by night
        if (aIndex == 0 && bIndex == 0)
        {
            float aPriority = Mathf.Lerp(0.9f, 0.4f, (nightSystemInstance.currentNight - 1) / 5f); // A favoured early, B late
            if (Random.value <= aPriority)
            {
                TryMoveA(moveTimeA, chanceA, aBlocked);
                TryMoveB(moveTimeB, chanceB, bBlocked);
            }
            else
            {
                TryMoveB(moveTimeB, chanceB, bBlocked);
                TryMoveA(moveTimeA, chanceA, aBlocked);
            }
        }
        else
        {
            // Otherwise check movement independently with mutual blocking
            TryMoveA(moveTimeA, chanceA, aBlocked);
            TryMoveB(moveTimeB, chanceB, bBlocked);
        }

        // Check resets
        TryResetMonsterA();
        TryResetMonsterB();
    }

    void TryMoveA(float moveTimeA, float chanceA, bool blocked)
    {
        if (!aFinal && !blocked && aTimer >= moveTimeA && Random.value <= chanceA)
        {
            aTimer = 0f;
            MoveMonsterA();
        }
    }

    void TryMoveB(float moveTimeB, float chanceB, bool blocked)
    {
        if (!bFinal && !blocked && bTimer >= moveTimeB && Random.value <= chanceB)
        {
            bTimer = 0f;
            MoveMonsterB();
        }
    }

    void MoveMonsterA()
    {
        if (aIndex >= waypoints.Length - 1) { aFinal = true; return; }
        aIndex++;
        if (monsterA != null) monsterA.position = waypoints[aIndex].position;
    }

    void MoveMonsterB()
    {
        if (bIndex >= waypoints.Length - 1) { bFinal = true; return; }
        bIndex++;
        if (monsterB != null) monsterB.position = waypoints[bIndex].position;
    }

    void TryResetMonsterA()
    {
        if (aIndex < 2 || aFinal) return;
        if (flashlight == null || !flashlight.flashlight.enabled) return;
        if (camMover == null || GetCameraIndex(camMover) != 4) return;

        aIndex = 0;
        if (monsterA != null) monsterA.position = waypoints[0].position;
        aTimer = 0f;

        // Set post-reset freeze
        resetFreezeTimer = Mathf.Max(0.5f, baseFreezeDuration - (nightSystemInstance.currentNight - 1));
    }

    void TryResetMonsterB()
    {
        if (bIndex < 1 || bIndex > 3 || bFinal) return;
        if (flashlight != null && flashlight.flashlight.enabled) return;
        if (bFlashlightOffTimer < bResetDelay) return;
        if (camMover == null || GetCameraIndex(camMover) != 4) return;

        bIndex = 0;
        if (monsterB != null) monsterB.position = waypoints[0].position;
        bTimer = 0f;
        bFlashlightOffTimer = 0f;

        // Set post-reset freeze
        resetFreezeTimer = Mathf.Max(0.5f, baseFreezeDuration - (nightSystemInstance.currentNight - 1));
    }

    int GetCameraIndex(CameraMover cm)
    {
        var field = typeof(CameraMover).GetField("currentIndex", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        return (int)field.GetValue(cm);
    }
}