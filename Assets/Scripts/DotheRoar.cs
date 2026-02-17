using UnityEngine;
using System.Collections;
using System.Reflection;

public class ChargingMonster : MonoBehaviour
{
    [Header("Core References")]
    public NightSystem nightSystem;
    public CameraMover cameraMover;
    public Animator animator;
    public AudioSource roarAudio;

    [Header("Spawn & Target")]
    public Transform spawnPoint;     // Where the monster spawns
    public Transform chargeTarget;   // Where the monster charges to

    [Header("Movement")]
    public float chargeSpeed = 8f;
    public float roarDuration = 2.5f;
    public float killDistance = 0.4f;

    [Header("Spawning")]
    public float baseSpawnDelay = 90f;
    public float minSpawnDelay = 25f;

    [Header("Death")]
    public GameObject deathCanvas;
    public string deathMessage = "You never saw it coming.";

    [Header("Monster Control")]
    public MonoBehaviour[] monstersToDisable;

    [Header("Safe Camera Slots")]
    public int[] safeSlots = { 3, 4 };

    [Header("DEV TOOLS")]
    public bool devMode = false;
    public int devNightOverride = 5;
    public KeyCode devSpawnKey = KeyCode.F6;

    bool isCharging;
    bool activeInstance;
    bool canKill;
    bool gameFrozen;

    Vector3 lockedChargeTarget;
    FieldInfo cameraIndexField;

    void Awake()
    {
        if (animator == null)
            animator = GetComponentInChildren<Animator>();

        if (animator != null)
            animator.applyRootMotion = false;

        // Reflection for CameraMover private field
        cameraIndexField = typeof(CameraMover).GetField(
            "currentIndex",
            BindingFlags.NonPublic | BindingFlags.Instance
        );

        if (cameraIndexField == null)
            Debug.LogError("ChargingMonster: Could not find CameraMover.currentIndex");
    }

    void Start()
    {
        int night = devMode ? devNightOverride : nightSystem.currentNight;

        if (night < 5)
        {
            Destroy(gameObject);
            return;
        }

        StartCoroutine(SpawnLoop());
    }

    void Update()
    {
        if (devMode && Input.GetKeyDown(devSpawnKey))
            SpawnMonster();

        if (gameFrozen || !isCharging)
            return;

        Vector3 dir = (lockedChargeTarget - transform.position).normalized;
        transform.position += dir * chargeSpeed * Time.deltaTime;
        transform.rotation = Quaternion.LookRotation(dir);

        CheckForKill();
    }

    IEnumerator SpawnLoop()
    {
        while (true)
        {
            if (!activeInstance)
            {
                int night = devMode ? devNightOverride : nightSystem.currentNight;
                float nightFactor = Mathf.Clamp01((night - 5) / 4f);

                float spawnChance = Mathf.Lerp(0.25f, 0.75f, nightFactor);
                float delay = Mathf.Lerp(baseSpawnDelay, minSpawnDelay, nightFactor);

                yield return new WaitForSeconds(delay);

                if (Random.value <= spawnChance)
                    SpawnMonster();
            }

            yield return new WaitForSeconds(1f);
        }
    }

    void SpawnMonster()
    {
        if (activeInstance || spawnPoint == null || chargeTarget == null)
            return;

        activeInstance = true;
        isCharging = false;
        canKill = false;
        gameFrozen = false;

        // Spawn away from the camera/player
        transform.position = spawnPoint.position;
        transform.rotation = Quaternion.LookRotation(
            (chargeTarget.position - spawnPoint.position).normalized
        );

        // Play roar animation and sound
        animator?.SetTrigger("Roar");
        roarAudio?.Play();

        StartCoroutine(RoarThenCharge());
    }

    IEnumerator RoarThenCharge()
    {
        yield return new WaitForSeconds(roarDuration);
        BeginCharge();
    }

    void BeginCharge()
    {
        // Lock target so monster doesn't chase moving camera
        lockedChargeTarget = chargeTarget.position;
        isCharging = true;

        animator?.SetTrigger("Charge");

        foreach (var m in monstersToDisable)
            if (m) m.enabled = false;

        StartCoroutine(EnableKillAfterDelay(0.35f));
    }

    IEnumerator EnableKillAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        canKill = true;
    }

    void CheckForKill()
    {
        if (!canKill)
            return;

        int camSlot = GetCameraIndex();

        // If player is in a safe camera slot
        foreach (int safe in safeSlots)
        {
            if (camSlot == safe)
            {
                // Monster reached target but cannot kill → despawn
                if (Vector3.Distance(transform.position, lockedChargeTarget) <= killDistance)
                    DespawnMonster();
                return;
            }
        }

        // Kill player if reached target
        if (Vector3.Distance(transform.position, lockedChargeTarget) <= killDistance)
        {
            TriggerDeath();
            DespawnMonster(); // clean up monster after kill
        }
    }

    void TriggerDeath()
    {
        gameFrozen = true;
        Time.timeScale = 0f;

        MonsterDeathText.SetDeathMessage(deathMessage);
        if (deathCanvas) deathCanvas.SetActive(true);
    }

    void DespawnMonster()
    {
        isCharging = false;
        activeInstance = false;
        canKill = false;

        foreach (var m in monstersToDisable)
            if (m) m.enabled = true;

        // Move out of scene
        transform.position = Vector3.down * 500f;
        transform.rotation = Quaternion.identity;
    }

    int GetCameraIndex()
    {
        return cameraIndexField != null
            ? (int)cameraIndexField.GetValue(cameraMover)
            : -1;
    }
}