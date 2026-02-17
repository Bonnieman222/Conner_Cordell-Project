using UnityEngine;
using System.Collections;
using UnityEngine.InputSystem;

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

    private bool monster1Active = false;
    private bool monster2Active = false;

    private GameObject monster1Instance;
    private GameObject monster2Instance;

    private float nextMonster1Check = 0f;
    private float nextMonster2Check = 0f;
    private float monster2SpawnTime = 0f;

    private bool gameFrozen = false;

    private PlayerInputActions inputActions;

    private void Awake()
    {
        inputActions = new PlayerInputActions();
        inputActions.Player.FireShotgun.performed += ctx => OnFireShotgun();
    }

    private void OnEnable() => inputActions.Player.Enable();
    private void OnDisable() => inputActions.Disable();

    private void Start()
    {
        if (deathCanvas != null)
            deathCanvas.SetActive(false);

        StartCoroutine(Monster1Loop());
        StartCoroutine(Monster2Loop());
    }

    private void Update()
    {
        if (gameFrozen) return;

        if (monster2Active)
            HandleMonster2KillTimer();
    }

    private void OnFireShotgun()
    {
        if (monster1Instance != null)
        {
            Destroy(monster1Instance);
            monster1Instance = null;
            monster1Active = false;
            nextMonster1Check = Time.time + Random.Range(monster1MinRespawn, monster1MaxRespawn);
        }

        CheckMonster2Shotgun();
    }

    private void CheckMonster2Shotgun()
    {
        if (!monster2Active) return;

        if (flashlightController != null &&
            flashlightController.IsShotgunHeld())
        {
            RemoveMonster2();
            Debug.Log("Monster 2 destroyed by shotgun!");
        }
    }

    private void RemoveMonster2()
    {
        monster2Active = false;

        if (monster2Instance != null)
            Destroy(monster2Instance);

        monster2Instance = null;
        nextMonster2Check = Time.time + Random.Range(monster2MinRespawn, monster2MaxRespawn);
    }

    private void HandleMonster2KillTimer()
    {
        if (Time.time - monster2SpawnTime >= GetMonster2KillTime())
        {
            RemoveMonster2();
            TriggerMonster2Death();
        }
    }

    private float GetMonster2KillTime()
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

    private IEnumerator Monster1Loop()
    {
        while (true)
        {
            if (nightSystem.currentNight >= 2 &&
                !monster1Active &&
                Time.time >= nextMonster1Check)
            {
                yield return new WaitForSeconds(
                    Random.Range(monster1MinInitialDelay, monster1MaxInitialDelay));

                if (monsterPrefab1 != null && spawnPoint1 != null)
                {
                    monster1Active = true;
                    monster1Instance = Instantiate(
                        monsterPrefab1,
                        spawnPoint1.position,
                        spawnPoint1.rotation);
                }
            }

            yield return null;
        }
    }

    private IEnumerator Monster2Loop()
    {
        while (true)
        {
            if (nightSystem.currentNight >= 3 &&
                !monster2Active &&
                Time.time >= nextMonster2Check)
            {
                yield return new WaitForSeconds(
                    Random.Range(monster2MinInitialDelay, monster2MaxInitialDelay));

                if (monsterPrefab2 != null && spawnPoint2 != null)
                {
                    monster2Active = true;
                    monster2SpawnTime = Time.time;

                    monster2Instance = Instantiate(
                        monsterPrefab2,
                        spawnPoint2.position,
                        spawnPoint2.rotation);
                }
            }

            yield return null;
        }
    }

    private void TriggerMonster2Death()
    {
        gameFrozen = true;
        Time.timeScale = 0f;

        MonsterDeathText.SetDeathMessage(monster2DeathMessage);

        if (deathCanvas != null)
            deathCanvas.SetActive(true);
    }
}
