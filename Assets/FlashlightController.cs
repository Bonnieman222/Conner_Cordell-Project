using UnityEngine;
using System.Collections;

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
    public GameObject shotgun;          // Assign shotgun object in Inspector
    public AudioSource shotgunSound;    // Optional: assign firing sound
    public float shotgunCooldown = 15f; // 15 seconds cooldown
    private bool shotgunHeld = false;
    private bool canFire = true;
    private Vector3 shotgunHoldOffset = new Vector3(0.2f, -0.3f, 0.5f);
    private Vector3 shotgunStartPosition;
    private Quaternion shotgunStartRotation;

    private void Start()
    {
        if (flashlight != null)
            flashlight.enabled = false;

        ResetBattery();

        // Save starting positions
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
        HandleInput();
        HandleBattery();

        // Flashlight follows camera
        if (isHeld && cameraTransform != null)
        {
            transform.position = cameraTransform.position +
                                 cameraTransform.right * holdOffset.x +
                                 cameraTransform.up * holdOffset.y +
                                 cameraTransform.forward * holdOffset.z;

            transform.rotation = Quaternion.LookRotation(cameraTransform.forward);
        }

        // Shotgun follows camera
        if (shotgunHeld && cameraTransform != null)
        {
            shotgun.transform.position = cameraTransform.position +
                                         cameraTransform.right * shotgunHoldOffset.x +
                                         cameraTransform.up * shotgunHoldOffset.y +
                                         cameraTransform.forward * shotgunHoldOffset.z;

            shotgun.transform.rotation = Quaternion.LookRotation(cameraTransform.forward);
        }
    }

    private void HandleInput()
    {
        // Flashlight pickup/drop (F)
        if (Input.GetKeyDown(KeyCode.F) && IsAtCameraSlot0())
        {
            if (!isHeld && !shotgunHeld)
            {
                PickUpFlashlight();
            }
            else if (isHeld && !isOn)
            {
                PutDownFlashlight();
            }
        }

        // Toggle flashlight (G)
        if (isHeld && Input.GetKeyDown(KeyCode.G))
        {
            if (!isOn && battery > 0f)
                TurnOn();
            else
                TurnOff();
        }

        // Shotgun pickup/drop (V)
        if (Input.GetKeyDown(KeyCode.V) && IsAtCameraSlot0())
        {
            if (!shotgunHeld && !isHeld)
            {
                PickUpShotgun();
            }
            else if (shotgunHeld)
            {
                PutDownShotgun();
            }
        }

        // Fire shotgun (B)
        if (shotgunHeld && Input.GetKeyDown(KeyCode.B))
        {
            TryFireShotgun();
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
                TurnOff();
            }
        }
    }

    #region Flashlight Methods
    private void PickUpFlashlight()
    {
        isHeld = true;
    }

    private void PutDownFlashlight()
    {
        isHeld = false;
        TurnOff();
        transform.position = startPosition;
        transform.rotation = startRotation;
    }

    private void TurnOn()
    {
        if (flashlight != null && battery > 0f)
        {
            flashlight.enabled = true;
            isOn = true;
        }
    }

    private void TurnOff()
    {
        if (flashlight != null)
        {
            flashlight.enabled = false;
            isOn = false;
        }
    }

    public void ResetBattery()
    {
        battery = 100f;
        TurnOff();
    }
    #endregion

    #region Shotgun Methods
    private void PickUpShotgun()
    {
        if (shotgun == null) return;
        shotgunHeld = true;
    }

    private void PutDownShotgun()
    {
        if (shotgun == null) return;

        shotgunHeld = false;
        shotgun.transform.position = shotgunStartPosition;
        shotgun.transform.rotation = shotgunStartRotation;
    }

    private void TryFireShotgun()
    {
        if (!canFire) return;

        FireShotgun();
        StartCoroutine(ShotgunCooldownRoutine());
    }

    private void FireShotgun()
    {
        Debug.Log("Boom! Shotgun fired.");
        if (shotgunSound != null)
            shotgunSound.Play();

        // You could also add recoil or particle effect here.
    }

    private IEnumerator ShotgunCooldownRoutine()
    {
        canFire = false;
        Debug.Log("Shotgun cooling down...");
        yield return new WaitForSeconds(shotgunCooldown);
        canFire = true;
        Debug.Log("Shotgun ready again.");
    }
    #endregion

    private bool IsAtCameraSlot0()
    {
        CameraMover camMover = FindObjectOfType<CameraMover>();
        return camMover != null && camMover.enabled && GetCameraIndex(camMover) == 0;
    }

    private int GetCameraIndex(CameraMover camMover)
    {
        var field = typeof(CameraMover).GetField("currentIndex", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        return (int)field.GetValue(camMover);
    }
}