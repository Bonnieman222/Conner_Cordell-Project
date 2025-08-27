using UnityEngine;

public class PickupAndMonsterController : MonoBehaviour
{
    [Header("References")]
    public CameraMover cameraMover;      // Reference to your CameraMover script
    public Transform holdPoint;          // Where the object will be held
    public GameObject pickupObject;      // Flashlight (must have Light)
    public GameObject monsterObject;     // Monster prefab
    public Transform monsterSpawnPoint;  // Where monster spawns

    [Header("Flashlight Settings")]
    [Range(0f, 100f)] public float startingBattery = 100f;
    [Range(0f, 100f)] public float drainRatePerSecond = 5f;
    private Light objectLight;
    private float batteryPercentage;
    private bool isHolding = false;

    private Transform originalParent;
    private Vector3 originalPosition;
    private Quaternion originalRotation;

    [Header("Monster Settings")]
    [Range(0f, 100f)] public float spawnChance = 35f;
    [Range(0f, 100f)] public float appearDuration = 30f;
    [Range(0f, 100f)] public float cycleInterval = 60f;
    [Range(0f, 100f)] public float flashlightDrainRate = 5f; // Extra drain if flashlight hits monster

    private float cycleTimer;
    private float activeTimer;
    private bool monsterActive;

    void Start()
    {
        if (pickupObject != null)
        {
            originalParent = pickupObject.transform.parent;
            originalPosition = pickupObject.transform.position;
            originalRotation = pickupObject.transform.rotation;

            objectLight = pickupObject.GetComponentInChildren<Light>();
            if (objectLight != null)
                objectLight.enabled = false;
        }

        if (monsterObject != null)
            monsterObject.SetActive(false);

        batteryPercentage = Mathf.Clamp(startingBattery, 0f, 100f);
        cycleTimer = cycleInterval;

        // 🔹 Do the first spawn roll right away
        float roll = Random.Range(0f, 100f);
        if (roll <= spawnChance)
        {
            SpawnMonster();
        }
    }

    void Update()
    {
        if (pickupObject == null || cameraMover == null) return;

        HandlePickup();
        HandleFlashlight();
        HandleMonster();
    }

    // ---------------- FLASHLIGHT / PICKUP ----------------
    private void HandlePickup()
    {
        if (Input.GetKeyDown(KeyCode.B))
        {
            if (!isHolding && IsAtOne())
                PickUp();
            else if (isHolding && IsAtOne())
                PutDown();
        }
    }

    private void HandleFlashlight()
    {
        if (isHolding && Input.GetKeyDown(KeyCode.F))
        {
            if (objectLight != null)
            {
                if (!objectLight.enabled && batteryPercentage > 0f)
                    objectLight.enabled = true;
                else
                    objectLight.enabled = false;
            }
        }

        if (isHolding && objectLight != null && objectLight.enabled)
        {
            DrainBattery(drainRatePerSecond * Time.deltaTime);
        }
    }

    private void PickUp()
    {
        pickupObject.transform.SetParent(holdPoint);
        pickupObject.transform.localPosition = Vector3.zero;
        pickupObject.transform.localRotation = Quaternion.identity;
        isHolding = true;
    }

    private void PutDown()
    {
        pickupObject.transform.SetParent(originalParent);
        pickupObject.transform.position = originalPosition;
        pickupObject.transform.rotation = originalRotation;

        if (objectLight != null)
            objectLight.enabled = false;

        isHolding = false;
    }

    private bool IsAtOne()
    {
        return cameraMover != null && GetCameraMoverIndex() == 0;
    }

    private int GetCameraMoverIndex()
    {
        var field = typeof(CameraMover).GetField("currentIndex",
                      System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        return (int)field.GetValue(cameraMover);
    }

    private void DrainBattery(float amount)
    {
        batteryPercentage -= amount;
        batteryPercentage = Mathf.Max(batteryPercentage, 0f);

        if (batteryPercentage <= 0f && objectLight != null)
            objectLight.enabled = false;
    }

    // ---------------- MONSTER ----------------
    private void HandleMonster()
    {
        if (monsterActive)
        {
            activeTimer -= Time.deltaTime;
            if (activeTimer <= 0f)
            {
                DespawnMonster();
            }
            else
            {
                if (IsFlashlightOnMonster())
                {
                    DrainBattery(flashlightDrainRate * Time.deltaTime);
                }
            }
        }
        else
        {
            cycleTimer -= Time.deltaTime;
            if (cycleTimer <= 0f)
            {
                cycleTimer = cycleInterval;
                float roll = Random.Range(0f, 100f);
                if (roll <= spawnChance)
                {
                    SpawnMonster();
                }
            }
        }
    }

    private void SpawnMonster()
    {
        if (monsterSpawnPoint != null)
            monsterObject.transform.position = monsterSpawnPoint.position;

        monsterObject.SetActive(true);
        monsterActive = true;
        activeTimer = appearDuration;
    }

    private void DespawnMonster()
    {
        monsterObject.SetActive(false);
        monsterActive = false;
    }

    private bool IsFlashlightOnMonster()
    {
        if (objectLight == null || !objectLight.enabled) return false;
        if (monsterObject == null || !monsterObject.activeInHierarchy) return false;

        // 1) Direction check (is monster inside flashlight cone?)
        Vector3 toMonster = (monsterObject.transform.position - objectLight.transform.position).normalized;
        float angle = Vector3.Angle(objectLight.transform.forward, toMonster);

        if (angle > objectLight.spotAngle / 2f) return false; // outside cone

        // 2) Raycast to confirm no walls blocking
        if (Physics.Raycast(objectLight.transform.position, toMonster, out RaycastHit hit, 50f))
        {
            if (hit.collider.gameObject == monsterObject)
                return true;
        }

        return false;
    }

    // ---------------- Public Helper ----------------
    public float GetBatteryPercentage()
    {
        return batteryPercentage;
    }
}