using UnityEngine;

public class FlashlightController : MonoBehaviour
{
    public Light flashlight;           // Spotlight component
    public Transform cameraTransform;  // Main camera transform

    private bool isHeld = false;
    private bool isOn = false;
    private float battery = 100f;

    // Adjusted offset so flashlight is further back when held
    private Vector3 holdOffset = new Vector3(-0.3f, -0.2f, -0.2f);

    // Where flashlight started
    private Vector3 startPosition;
    private Quaternion startRotation;

    private void Start()
    {
        if (flashlight != null)
            flashlight.enabled = false; // Light starts off

        ResetBattery();

        // Save starting spot
        startPosition = transform.position;
        startRotation = transform.rotation;
    }

    private void Update()
    {
        HandleInput();
        HandleBattery();

        // If held, follow the camera
        if (isHeld && cameraTransform != null)
        {
            transform.position = cameraTransform.position +
                                 cameraTransform.right * holdOffset.x +
                                 cameraTransform.up * holdOffset.y +
                                 cameraTransform.forward * holdOffset.z;

            transform.rotation = Quaternion.LookRotation(cameraTransform.forward);
        }
    }

    private void HandleInput()
    {
        // Pickup / Drop with F (only if at camera slot 0)
        if (Input.GetKeyDown(KeyCode.F) && IsAtCameraSlot0())
        {
            if (!isHeld)
            {
                PickUp();
            }
            else if (isHeld && !isOn) // only allow putting down if flashlight is off
            {
                PutDown();
            }
        }

        // Toggle flashlight with G
        if (isHeld && Input.GetKeyDown(KeyCode.G))
        {
            if (!isOn && battery > 0f)
                TurnOn();
            else
                TurnOff();
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

    private void PickUp()
    {
        isHeld = true;
    }

    private void PutDown()
    {
        isHeld = false;
        TurnOff();

        // Return to where it started
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

    public void ResetBattery()
    {
        battery = 100f;
        TurnOff();
    }
}