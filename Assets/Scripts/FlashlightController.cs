using UnityEngine;
using System.Collections;
using UnityEngine.InputSystem;

public class FlashlightController : MonoBehaviour
{
    [Header("Flashlight Settings")]
    public Light flashlight;
    public Transform cameraTransform;
    private bool isHeld = false;
    private bool isOn = false;
    private float battery = 100f;
    private Vector3 holdOffset = new Vector3(-0.3f, -0.2f, -0.2f);
    private Vector3 startPosition;
    private Quaternion startRotation;

    [Header("Shotgun Settings")]
    public GameObject shotgun;
    public AudioSource shotgunAudio;
    private bool shotgunHeld = false;
    private bool canFire = true;
    private float shotgunCooldown = 15f;
    private Vector3 shotgunHoldOffset = new Vector3(0.3f, -0.25f, 0.6f);
    private Quaternion shotgunRotationOffset = Quaternion.Euler(0f, 180f, 0f);
    private Vector3 shotgunStartPosition;
    private Quaternion shotgunStartRotation;

    [HideInInspector]
    public PlayerInputActions inputActions;

    private void Awake()
    {
        inputActions = new PlayerInputActions();

        // Flashlight
        inputActions.Player.PickUpFlashlight.performed += ctx => ToggleFlashlightPickup();
        inputActions.Player.ToggleFlashlight.performed += ctx => ToggleFlashlightLight();

        // Shotgun
        inputActions.Player.PickUpShotgun.performed += ctx => ToggleShotgunPickup();
        inputActions.Player.FireShotgun.performed += ctx => TryFireShotgun();
    }

    private void OnEnable() => inputActions.Player.Enable();
    private void OnDisable() => inputActions.Player.Disable();

    private void Start()
    {
        if (flashlight != null) flashlight.enabled = false;
        startPosition = transform.position;
        startRotation = transform.rotation;

        if (shotgun != null)
        {
            shotgunStartPosition = shotgun.transform.position;
            shotgunStartRotation = shotgun.transform.rotation;
        }
    }

    private void Update()
    {
        HandleBattery();

        if (isHeld && cameraTransform != null)
        {
            transform.position = cameraTransform.position + cameraTransform.right * holdOffset.x +
                                 cameraTransform.up * holdOffset.y + cameraTransform.forward * holdOffset.z;
            transform.rotation = Quaternion.LookRotation(cameraTransform.forward);
        }

        if (shotgunHeld && cameraTransform != null)
        {
            shotgun.transform.position = cameraTransform.position + cameraTransform.right * shotgunHoldOffset.x +
                                         cameraTransform.up * shotgunHoldOffset.y +
                                         cameraTransform.forward * shotgunHoldOffset.z;
            shotgun.transform.rotation = Quaternion.LookRotation(cameraTransform.forward) * shotgunRotationOffset;
        }
    }

    private void HandleBattery()
    {
        if (isOn && battery > 0f)
        {
            float drainRate = 0.3f + (NightSystem.Instance.currentNight - 1) * 0.1f;
            battery -= drainRate * Time.deltaTime;
            if (battery <= 0f) TurnOffFlashlight();
        }
    }

    // ------------------- FLASHLIGHT -------------------
    public void ToggleFlashlightPickup()
    {
        if (!isHeld && !shotgunHeld && IsAtCameraSlot0()) isHeld = true;
        else if (isHeld) PutDownFlashlight();
    }

    public void ToggleFlashlightLight()
    {
        if (!isHeld) return;
        isOn = !isOn;
        flashlight.enabled = isOn;
    }

    private void PutDownFlashlight()
    {
        isHeld = false;
        TurnOffFlashlight();
        transform.position = startPosition;
        transform.rotation = startRotation;
    }

    private void TurnOffFlashlight()
    {
        flashlight.enabled = false;
        isOn = false;
    }

    // ------------------- SHOTGUN -------------------
    public void ToggleShotgunPickup()
    {
        if (!shotgunHeld && !isHeld && IsAtCameraSlot0()) shotgunHeld = true;
        else if (shotgunHeld) PutDownShotgun();
    }

    private void PutDownShotgun()
    {
        shotgunHeld = false;
        if (shotgun != null)
        {
            shotgun.transform.position = shotgunStartPosition;
            shotgun.transform.rotation = shotgunStartRotation;
        }
    }

    public void TryFireShotgun()
    {
        if (!canFire || !shotgunHeld) return;
        shotgunAudio?.Play();
        canFire = false;
        StartCoroutine(ShotgunCooldown());
    }

    private IEnumerator ShotgunCooldown()
    {
        yield return new WaitForSeconds(shotgunCooldown);
        canFire = true;
    }

    // ------------------- NIGHT RESET -------------------
    public void ResetForNewNight()
    {
        PutDownFlashlight();
        shotgunHeld = false;
        canFire = true;
        if (shotgun != null)
        {
            shotgun.transform.position = shotgunStartPosition;
            shotgun.transform.rotation = shotgunStartRotation;
        }
        battery = 100f;
        TurnOffFlashlight();
        Debug.Log("Flashlight & shotgun reset for new night.");
    }

    // ------------------- PUBLIC GETTERS (legacy) -------------------
    public bool IsFlashlightHeld() => isHeld;          // legacy for old scripts
    public bool IsShotgunHeld() => shotgunHeld;        // legacy for old scripts
    public bool CanFireShotgun() => canFire;           // legacy for old scripts
    public bool IsAtCameraSlot0Public() => IsAtCameraSlot0();

    // ------------------- PRIVATE HELPERS -------------------
    private bool IsAtCameraSlot0()
    {
        CameraMover cam = FindObjectOfType<CameraMover>();
        if (cam == null) return false;

        var field = typeof(CameraMover).GetField("currentIndex",
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        return (int)field.GetValue(cam) == 0;
    }
}
