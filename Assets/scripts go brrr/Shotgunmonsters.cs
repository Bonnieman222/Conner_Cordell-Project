using UnityEngine;
using System.Collections;

public class ShotgunMonsters : MonoBehaviour
{
    [Header("References")]
    public NightSystem nightSystem;
    public CameraMover cameraMover;
    public FlashlightController flashlightController;

    [Header("Death Screen")]
    public GameObject deathCanvas;
    public string monster2DeathMessage =
        "You felt it watching you long before it decided to act.";

    [Header("Monster 1 Setup")]
    public Transform spawnPoint1;
    public GameObject monsterPrefab1;
    public float monster1MinInitialDelay = 30f;
    public float monster1MaxInitialDelay = 120f;
    public float monster1MinRespawn = 45f;
    public float monster1MaxRespawn = 120f;

    [Header("Monster 2 Setup")]
    public Transform spawnPoint2;
    public GameObject monsterPrefab2;
    public float monster2MinInitialDelay = 30f;
    public float monster2MaxInitialDelay = 120f;
    public float monster2MinRespawn = 45f;
    public float monster2MaxRespawn = 120f;

    [Header("Monster 2 Kill Timers")]
    public float night3_4KillTime = 60f;
    public float night5KillTime = 45f;
    public float night6KillTime = 30f;
    public float night7KillTime = 20f;

    bool monster1Active = false;
    bool monster2Active = false;

    GameObject monster1Instance;
    GameObject monster2Instance;

    float nextMonster1Check = 0f;
    float nextMonster2Check = 0f;
    float monster2SpawnTime = 0f;

    bool gameFrozen = false;

    void Start()
    {
        if (deathCanvas != null)
            deathCanvas.SetActive(false);

        StartCoroutine(Monster1Loop());
        StartCoroutine(Monster2Loop());
    }

    void Update()
    {
        if (gameFrozen) return;

        if (monster1Active)
            HandleMonster1();

        if (monster2Active)
        {
            CheckMonster2Shotgun();

            if (monster2Active)
            {
                HandleMonster2KillTimer();
                HandleMonster2CameraRemap();
            }
        }
    }

    #region Monster 1
    IEnumerator Monster1Loop()
    {
        int lastNight = nightSystem.currentNight;

        while (true)
        {
            if (nightSystem.currentNight != lastNight)
            {
                lastNight = nightSystem.currentNight;
                monster1Active = false;
            }

            if (nightSystem.currentNight >= 2 && !monster1Active && Time.time >= nextMonster1Check)
            {
                yield return new WaitForSeconds(
                    Random.Range(monster1MinInitialDelay, monster1MaxInitialDelay)
                );
                SpawnMonster1();
            }

            yield return null;
        }
    }

    void SpawnMonster1()
    {
        if (monsterPrefab1 == null || spawnPoint1 == null) return;

        monster1Active = true;
        monster1Instance = Instantiate(monsterPrefab1, spawnPoint1.position, spawnPoint1.rotation);
        nextMonster1Check = Time.time + Random.Range(monster1MinRespawn, monster1MaxRespawn);
    }

    void HandleMonster1()
    {
        if (!IsAtCameraSpot3()) return;

        if (flashlightController.flashlight != null &&
            flashlightController.flashlight.enabled)
        {
            var field = typeof(FlashlightController)
                .GetField("battery", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);

            float battery = (float)field.GetValue(flashlightController);
            battery = Mathf.Max(0f, battery - Time.deltaTime);
            field.SetValue(flashlightController, battery);
        }

        if (Input.GetKeyDown(KeyCode.N))
            RemoveMonster1();
    }

    void RemoveMonster1()
    {
        monster1Active = false;
        if (monster1Instance != null) Destroy(monster1Instance);
        monster1Instance = null;
        nextMonster1Check = Time.time + Random.Range(monster1MinRespawn, monster1MaxRespawn);
    }
    #endregion

    #region Monster 2
    IEnumerator Monster2Loop()
    {
        int lastNight = nightSystem.currentNight;

        while (true)
        {
            if (nightSystem.currentNight != lastNight)
            {
                lastNight = nightSystem.currentNight;
                monster2Active = false;
            }

            if (nightSystem.currentNight >= 3 && !monster2Active && Time.time >= nextMonster2Check)
            {
                yield return new WaitForSeconds(
                    Random.Range(monster2MinInitialDelay, monster2MaxInitialDelay)
                );
                SpawnMonster2();
            }

            yield return null;
        }
    }

    void SpawnMonster2()
    {
        if (monsterPrefab2 == null || spawnPoint2 == null) return;

        monster2Active = true;
        monster2SpawnTime = Time.time;
        monster2Instance = Instantiate(monsterPrefab2, spawnPoint2.position, spawnPoint2.rotation);
    }

    void HandleMonster2KillTimer()
    {
        if (Time.time - monster2SpawnTime >= GetMonster2KillTime())
        {
            RemoveMonster2();
            TriggerMonster2Death();
        }
    }

    float GetMonster2KillTime()
    {
        switch (nightSystem.currentNight)
        {
            case 3:
            case 4: return night3_4KillTime;
            case 5: return night5KillTime;
            case 6: return night6KillTime;
            case 7: return night7KillTime;
        }
        return 999f;
    }

    void HandleMonster2CameraRemap()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1)) cameraMover.TryMoveTo(4);
        if (Input.GetKeyDown(KeyCode.Alpha2)) cameraMover.TryMoveTo(3);
        if (Input.GetKeyDown(KeyCode.Alpha3)) cameraMover.TryMoveTo(0);
        if (Input.GetKeyDown(KeyCode.Alpha4)) cameraMover.TryMoveTo(1);
        if (Input.GetKeyDown(KeyCode.Alpha5)) cameraMover.TryMoveTo(2);
    }

    void CheckMonster2Shotgun()
    {
        if (!Input.GetKeyDown(KeyCode.N)) return;

        int camIndex = (int)typeof(CameraMover)
            .GetField("currentIndex", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
            .GetValue(cameraMover);

        if (camIndex == 0)
            RemoveMonster2();
    }

    void RemoveMonster2()
    {
        monster2Active = false;
        if (monster2Instance != null) Destroy(monster2Instance);
        monster2Instance = null;
        nextMonster2Check = Time.time + Random.Range(monster2MinRespawn, monster2MaxRespawn);
    }
    #endregion

    #region Death
    void TriggerMonster2Death()
    {
        gameFrozen = true;
        Time.timeScale = 0f;

        MonsterDeathText.SetDeathMessage(monster2DeathMessage);

        if (deathCanvas != null)
            deathCanvas.SetActive(true);
    }
    #endregion

    bool IsAtCameraSpot3()
    {
        int index = (int)typeof(CameraMover)
            .GetField("currentIndex", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
            .GetValue(cameraMover);

        return index == 3;
    }
}
