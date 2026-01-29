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
    public float baseFreezeDuration = 3f;

    [Header("Death Screen")]
    public GameObject deathCanvas;

    public string monsterADeathMessage =
        "Mr Fallador prefers a dark room, we aren't here to make him comfortable";

    public string monsterBDeathMessage =
        "The orphans worked in the dark, your eyes alone should be enough to keep them busy";

    int aIndex = 0;
    int bIndex = 0;
    float aTimer = 0f;
    float bTimer = 0f;

    bool aFinal = false;
    bool bFinal = false;

    float bFlashlightOffTimer = 0f;
    float resetFreezeTimer = 0f;

    FlashlightController flashlight;
    CameraMover camMover;
    NightSystem nightSystemInstance;

    bool gameFrozen = false;

    // --- New Input System ---
    private PlayerInputActions inputActions;

    private void Awake()
    {
        inputActions = new PlayerInputActions();

        // Track flashlight toggle to reset bFlashlightOffTimer
        inputActions.Player.ToggleFlashlight.performed += ctx =>
        {
            if (flashlight != null && flashlight.flashlight != null)
            {
                if (flashlight.flashlight.enabled)
                    bFlashlightOffTimer = 0f;
            }
        };
    }

    private void OnEnable() => inputActions.Enable();
    private void OnDisable() => inputActions.Disable();

    void Start()
    {
        flashlight = FindObjectOfType<FlashlightController>();
        camMover = FindObjectOfType<CameraMover>();
        nightSystemInstance = NightSystem.Instance;

        if (deathCanvas != null)
            deathCanvas.SetActive(false);

        if (monsterA != null && waypoints.Length > 0) monsterA.position = waypoints[0].position;
        if (monsterB != null && waypoints.Length > 0) monsterB.position = waypoints[0].position;
    }

    void Update()
    {
        if (waypoints.Length == 0 || gameFrozen) return;

        CheckForDeath();

        aTimer += Time.deltaTime;
        bTimer += Time.deltaTime;

        // Track flashlight off timer for monsterB
        if (flashlight == null || !flashlight.flashlight.enabled)
            bFlashlightOffTimer += Time.deltaTime;
        else
            bFlashlightOffTimer = 0f;

        if (nightSystemInstance.currentNight < 2)
        {
            bIndex = 0;
            bTimer = 0f;
            bFlashlightOffTimer = 0f;
            if (monsterB != null && waypoints.Length > 0)
                monsterB.position = waypoints[0].position;
        }

        if (resetFreezeTimer > 0f)
        {
            resetFreezeTimer -= Time.deltaTime;
            return;
        }

        HandleMonsters();
    }

    void HandleMonsters()
    {
        // Allow total inactivity
        if (Random.value < 0.30f)
            return;

        float moveTimeA = Mathf.Max(0.5f, baseMoveTimeA - (nightSystemInstance.currentNight - 1));
        float moveTimeB = Mathf.Max(0.5f, baseMoveTimeB - (nightSystemInstance.currentNight - 1));

        bool atSpot4 = IsAtCameraSpot4();
        bool flashOn = flashlight != null && flashlight.flashlight.enabled && atSpot4;

        if (flashOn && bIndex >= 1 && bIndex <= 4)
            moveTimeB /= flashlightSpeedMultiplier;

        float chanceA = Mathf.Lerp(0.9f, 0.4f, (nightSystemInstance.currentNight - 1) / 5f);

        float chanceB;
        if (nightSystemInstance.currentNight <= 4)
            chanceB = Mathf.Lerp(0.05f, 0.25f,
                Mathf.Clamp01((nightSystemInstance.currentNight - 2) / 2f));
        else
            chanceB = Mathf.Lerp(0.25f, 0.6f,
                Mathf.Clamp01((nightSystemInstance.currentNight - 4) / 3f));

        bool aCanMove =
            !aFinal &&
            aTimer >= moveTimeA &&
            Random.value <= chanceA &&
            !(bIndex >= 1 && bIndex <= 4);

        bool bCanMove =
            nightSystemInstance.currentNight >= 2 &&
            !bFinal &&
            bTimer >= moveTimeB &&
            Random.value <= chanceB &&
            !(aIndex >= 1 && aIndex <= 4);

        // Enforce ONE OR NONE moves
        if (aCanMove && bCanMove)
        {
            if (Random.value < 0.5f)
                aCanMove = false;
            else
                bCanMove = false;
        }

        if (aCanMove)
        {
            aTimer = 0f;
            MoveMonsterA();
        }
        else if (bCanMove)
        {
            bTimer = 0f;
            MoveMonsterB();
        }

        TryResetMonsterA();
        TryResetMonsterB();
    }

    void MoveMonsterA()
    {
        if (aIndex < waypoints.Length - 1)
        {
            aIndex++;
            if (monsterA != null)
                monsterA.position = waypoints[aIndex].position;
        }
    }

    void MoveMonsterB()
    {
        if (bIndex < waypoints.Length - 1)
        {
            bIndex++;
            if (monsterB != null)
                monsterB.position = waypoints[bIndex].position;
        }
    }

    void TryResetMonsterA()
    {
        if (aIndex < 2 || aFinal) return;
        if (!IsAtCameraSpot4()) return;
        if (flashlight == null || !flashlight.flashlight.enabled) return;

        aIndex = 0;
        aTimer = 0f;
        if (monsterA != null && waypoints.Length > 0)
            monsterA.position = waypoints[0].position;

        resetFreezeTimer = Mathf.Max(0.5f,
            baseFreezeDuration - (nightSystemInstance.currentNight - 1));
    }

    void TryResetMonsterB()
    {
        if (bIndex < 1 || bIndex > 3 || bFinal) return;
        if (!IsAtCameraSpot4()) return;
        if (flashlight != null && flashlight.flashlight.enabled) return;
        if (bFlashlightOffTimer < bResetDelay) return;

        bIndex = 0;
        bTimer = 0f;
        bFlashlightOffTimer = 0f;
        if (monsterB != null && waypoints.Length > 0)
            monsterB.position = waypoints[0].position;

        resetFreezeTimer = Mathf.Max(0.5f,
            baseFreezeDuration - (nightSystemInstance.currentNight - 1));
    }

    void CheckForDeath()
    {
        if (waypoints.Length <= 5) return;

        if (aIndex == 5)
            TriggerDeath(monsterADeathMessage);
        else if (bIndex == 5)
            TriggerDeath(monsterBDeathMessage);
    }

    void TriggerDeath(string msg)
    {
        gameFrozen = true;
        Time.timeScale = 0f;

        MonsterDeathText.SetDeathMessage(msg);

        if (deathCanvas != null)
            deathCanvas.SetActive(true);
    }

    bool IsAtCameraSpot4()
    {
        return camMover != null && GetCameraIndex(camMover) == 4;
    }

    int GetCameraIndex(CameraMover cm)
    {
        var field = typeof(CameraMover).GetField(
            "currentIndex",
            System.Reflection.BindingFlags.NonPublic |
            System.Reflection.BindingFlags.Instance
        );
        return (int)field.GetValue(cm);
    }
}