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

    // Internal public fields
    public int aIndex = 0;
    public int bIndex = 0;
    public float aTimer = 0f;
    public float bTimer = 0f;
    public bool aFinal = false;
    public bool bFinal = false;
    public float bFlashlightOffTimer = 0f;
    public float resetFreezeTimer = 0f;

    private FlashlightController flashlight;
    private CameraMover camMover;
    private NightSystem nightSystemInstance;

    private bool gameFrozen = false;

    void Start()
    {
        flashlight = FindObjectOfType<FlashlightController>();
        camMover = FindObjectOfType<CameraMover>();
        nightSystemInstance = NightSystem.Instance;

        if (deathCanvas != null)
            deathCanvas.SetActive(false);

        if (monsterA != null) monsterA.position = waypoints[0].position;
        if (monsterB != null) monsterB.position = waypoints[0].position;
    }

    void Update()
    {
        if (waypoints.Length == 0) return;

        CheckForDeath();

        if (gameFrozen) return;

        aTimer += Time.deltaTime;
        bTimer += Time.deltaTime;

        if (flashlight == null || !flashlight.flashlight.enabled)
            bFlashlightOffTimer += Time.deltaTime;
        else
            bFlashlightOffTimer = 0f;

        if (resetFreezeTimer > 0f)
        {
            resetFreezeTimer -= Time.deltaTime;
            return;
        }

        HandleMonsters();
    }

    void CheckForDeath()
    {
        if (waypoints.Length <= 5) return;

        if (aIndex == 5)
        {
            TriggerDeath(monsterADeathMessage);
        }
        else if (bIndex == 5)
        {
            TriggerDeath(monsterBDeathMessage);
        }
    }

    void TriggerDeath(string msg)
    {
        gameFrozen = true;
        Time.timeScale = 0f;

        MonsterDeathText.SetDeathMessage(msg);

        if (deathCanvas != null)
        {
            deathCanvas.SetActive(true);

            // Force the text to update immediately
            MonsterDeathText textComp = deathCanvas.GetComponentInChildren<MonsterDeathText>();
            if (textComp != null)
            {
                if (textComp.tmpText != null)
                    textComp.tmpText.text = msg;
                if (textComp.uiText != null)
                    textComp.uiText.text = msg;
            }
        }
    }

    void HandleMonsters()
    {
        float moveTimeA = Mathf.Max(0.5f, baseMoveTimeA - (nightSystemInstance.currentNight - 1));
        float moveTimeB = Mathf.Max(0.5f, baseMoveTimeB - (nightSystemInstance.currentNight - 1));

        bool flashOn = flashlight != null && flashlight.flashlight.enabled;
        if (flashOn && bIndex >= 1 && bIndex <= 4)
            moveTimeB /= flashlightSpeedMultiplier;

        float chanceA = Mathf.Lerp(0.9f, 0.4f, (nightSystemInstance.currentNight - 1) / 5f);
        float chanceB = Mathf.Lerp(0.1f, 0.6f, (nightSystemInstance.currentNight - 1) / 5f);

        bool aBlocked = bIndex >= 1 && bIndex <= 4;
        bool bBlocked = aIndex >= 1 && aIndex <= 4;

        if (aIndex == 0 && bIndex == 0)
        {
            float aPriority = Mathf.Lerp(0.9f, 0.4f, (nightSystemInstance.currentNight - 1) / 5f);
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
            TryMoveA(moveTimeA, chanceA, aBlocked);
            TryMoveB(moveTimeB, chanceB, bBlocked);
        }

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
        if (flashlight == null || !flashlight.flashlight.enabled) return;
        if (camMover == null || GetCameraIndex(camMover) != 4) return;

        aIndex = 0;
        if (monsterA != null) monsterA.position = waypoints[0].position;
        aTimer = 0f;

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

        resetFreezeTimer = Mathf.Max(0.5f, baseFreezeDuration - (nightSystemInstance.currentNight - 1));
    }

    int GetCameraIndex(CameraMover cm)
    {
        var field = typeof(CameraMover).GetField(
            "currentIndex",
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance
        );
        return (int)field.GetValue(cm);
    }
}