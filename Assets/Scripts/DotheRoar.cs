using UnityEngine;
using System.Collections;

public class ChargingMonster : MonoBehaviour
{
    [Header("Core References")]
    public NightSystem nightSystem;
    public CameraMover cameraMover;
    public Animator animator;
    public AudioSource roarAudio;

    [Header("Movement")]
    public Transform chargeTarget;
    public float chargeSpeed = 8f;
    public float roarDuration = 2.5f;
    public float chargeStopDistance = 0.1f;

    [Header("Spawning")]
    public float baseSpawnDelay = 90f;   // Night 5 baseline
    public float minSpawnDelay = 25f;

    [Header("Death")]
    public GameObject deathCanvas;
    public string deathMessage = "You never saw it coming.";

    [Header("Monster Control")]
    public MonoBehaviour[] monstersToDisable; // All except flashlight drainer

    bool isCharging = false;
    bool gameFrozen = false;
    bool activeInstance = false;

    void Start()
    {
        if (nightSystem.currentNight < 5)
        {
            Destroy(gameObject);
            return;
        }

        StartCoroutine(SpawnLoop());
    }

    IEnumerator SpawnLoop()
    {
        while (true)
        {
            if (!activeInstance)
            {
                float nightFactor = Mathf.Clamp01((nightSystem.currentNight - 5) / 4f);

                float spawnChance = Mathf.Lerp(0.25f, 0.75f, nightFactor);
                float delay = Mathf.Lerp(baseSpawnDelay, minSpawnDelay, nightFactor);

                yield return new WaitForSeconds(delay);

                if (Random.value <= spawnChance)
                    SpawnMonster();
            }

            yield return null;
        }
    }

    void SpawnMonster()
    {
        activeInstance = true;
        isCharging = false;

        ForceCameraSlot(2);

        transform.position = cameraMover.transform.position;
        transform.rotation = cameraMover.transform.rotation;

        StartCoroutine(RoarThenCharge());
    }

    IEnumerator RoarThenCharge()
    {
        if (animator != null)
            animator.SetTrigger("Roar");

        if (roarAudio != null)
            roarAudio.Play();

        yield return new WaitForSeconds(roarDuration);

        BeginCharge();
    }

    void BeginCharge()
    {
        isCharging = true;

        if (animator != null)
            animator.SetTrigger("Charge");

        foreach (var m in monstersToDisable)
        {
            if (m != null)
                m.enabled = false;
        }
    }

    void Update()
    {
        if (gameFrozen || !isCharging || chargeTarget == null)
            return;

        transform.position = Vector3.MoveTowards(
            transform.position,
            chargeTarget.position,
            chargeSpeed * Time.deltaTime
        );

        CheckForKill();

        if (Vector3.Distance(transform.position, chargeTarget.position) <= chargeStopDistance)
            EndCharge();
    }

    void EndCharge()
    {
        isCharging = false;
        activeInstance = false;

        foreach (var m in monstersToDisable)
        {
            if (m != null)
                m.enabled = true;
        }

        gameObject.SetActive(false);
    }

    void CheckForKill()
    {
        int camIndex = GetCameraIndex();

        if (camIndex == 1 || camIndex == 2 || camIndex == 5)
            TriggerDeath();
    }

    void TriggerDeath()
    {
        gameFrozen = true;
        Time.timeScale = 0f;

        MonsterDeathText.SetDeathMessage(deathMessage);

        if (deathCanvas != null)
            deathCanvas.SetActive(true);
    }

    void ForceCameraSlot(int slot)
    {
        typeof(CameraMover)
            .GetField("currentIndex",
                System.Reflection.BindingFlags.NonPublic |
                System.Reflection.BindingFlags.Instance)
            .SetValue(cameraMover, slot);
    }

    int GetCameraIndex()
    {
        return (int)typeof(CameraMover)
            .GetField("currentIndex",
                System.Reflection.BindingFlags.NonPublic |
                System.Reflection.BindingFlags.Instance)
            .GetValue(cameraMover);
    }
}