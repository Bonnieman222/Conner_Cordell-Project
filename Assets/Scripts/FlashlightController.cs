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

    private bool shotgunHeld = false; // Keep private
    private bool canFire = true;
    private float shotgunCooldown = 15f;

    private Vector3 shotgunHoldOffset = new Vector3(0.3f, -0.25f, 0.6f);
    private Quaternion shotgunRotationOffset = Quaternion.Euler(0f, 180f, 0f);
    private Vector3 shotgunStartPosition;
    private Quaternion shotgunStartRotation;

    private PlayerInputActions inputActions;

    private void Awake()
    {
        inputActions = new PlayerInputActions();

        // Flashlight actions
        inputActions.Player.PickUpFlashlight.performed += ctx => TryPickUpFlashlight();
        inputActions.Player.ToggleFlashlight.performed += ctx => TryToggleFlashlight();

        // Shotgun actions
        inputActions.Player.PickUpShotgun.performed += ctx => TryPickUpShotgun();
        inputActions.Player.FireShotgun.performed += ctx => TryFireShotgun();
    }

    private void OnEnable() => inputActions.Player.Enable();
    private void OnDisable() => inputActions.Player.Disable();

    private void Start()
    {
        if (flashlight != null)
            flashlight.enabled = false;

        ResetBattery();

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

        // Flashlight follows camera ONLY if held
        if (isHeld && cameraTransform != null)
        {
            transform.position = cameraTransform.position
                + cameraTransform.right * holdOffset.x
                + cameraTransform.up * holdOffset.y
                + cameraTransform.forward * holdOffset.z;

            transform.rotation = Quaternion.LookRotation(cameraTransform.forward);
        }

        // Shotgun follows camera ONLY if held
        if (shotgunHeld && cameraTransform != null)
        {
            shotgun.transform.position = cameraTransform.position
                + cameraTransform.right * shotgunHoldOffset.x
                + cameraTransform.up * shotgunHoldOffset.y
                + cameraTransform.forward * shotgunHoldOffset.z;

            shotgun.transform.rotation =
                Quaternion.LookRotation(cameraTransform.forward) * shotgunRotationOffset;
        }
    }

    private void HandleBattery()
    {
        if (isOn && battery > 0f)
        {
            float drainRate = 0.3f + (NightSystem.Instance.currentNight - 1) * 0.1f;
            battery -= drainRate * Time.deltaTime;

            if (battery <= 0f)
            {
                battery = 0f;
                TurnOffFlashlight();
            }
        }
    }

    // -------- Flashlight Methods --------
    private void TryPickUpFlashlight()
    {
        if (!isHeld && !shotgunHeld && IsAtCameraSlot0())
            isHeld = true;
        else if (isHeld && !isOn)
            PutDownFlashlight();
    }

    private void TryToggleFlashlight()
    {
        if (!isHeld) return;
        if (!isOn && battery > 0f) TurnOnFlashlight();
        else TurnOffFlashlight();
    }

    private void PutDownFlashlight()
    {
        isHeld = false;
        TurnOffFlashlight();
        transform.position = startPosition;
        transform.rotation = startRotation;
    }

    private void TurnOnFlashlight()
    {
        flashlight.enabled = true;
        isOn = true;
    }

    private void TurnOffFlashlight()
    {
        flashlight.enabled = false;
        isOn = false;
    }

    public void ResetBattery()
    {
        battery = 100f;
        TurnOffFlashlight();
    }

    // -------- Shotgun Methods --------
    private void TryPickUpShotgun()
    {
        if (!shotgunHeld && !isHeld && IsAtCameraSlot0())
            shotgunHeld = true;
        else if (shotgunHeld)
            PutDownShotgun();
    }

    private void PutDownShotgun()
    {
        shotgunHeld = false;
        shotgun.transform.position = shotgunStartPosition;
        shotgun.transform.rotation = shotgunStartRotation;
    }

    private void TryFireShotgun()
    {
        if (!canFire || !shotgunHeld) return;

        Debug.Log("SHOTGUN FIRED"); // debug message

        canFire = false;
        shotgunAudio?.Play();
        StartCoroutine(ShotgunCooldown());
    }

    private IEnumerator ShotgunCooldown()
    {
        yield return new WaitForSeconds(shotgunCooldown);
        canFire = true;
    }

    // -------- NIGHT RESET (FORCED PUT DOWN) --------
    public void ResetForNewNight()
    {
        // Flashlight reset
        isHeld = false;
        isOn = false;
        flashlight.enabled = false;
        transform.position = startPosition;
        transform.rotation = startRotation;

        // Shotgun reset
        shotgunHeld = false;
        canFire = true;
        if (shotgun != null)
        {
            shotgun.transform.position = shotgunStartPosition;
            shotgun.transform.rotation = shotgunStartRotation;
        }

        battery = 100f;

        Debug.Log("Flashlight & shotgun reset for new night.");
    }

    // ---------- PUBLIC GETTER FOR SHOTGUN ----------
    public bool IsShotgunHeld()
    {
        return shotgunHeld;
    }

    private bool IsAtCameraSlot0()
    {
        CameraMover cam = FindObjectOfType<CameraMover>();
        return cam != null && GetCameraIndex(cam) == 0;
    }

    private int GetCameraIndex(CameraMover cam)
    {
        var field = typeof(CameraMover).GetField(
            "currentIndex",
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance
        );
        return (int)field.GetValue(cam);
    }
}
