using UnityEngine;

public class PickupController : MonoBehaviour
{
    [Header("References")]
    public CameraMover cameraMover; // Reference to your CameraMover script
    public Transform holdPoint;     // Where the object will be held (child of camera for example)
    public GameObject pickupObject; // The object to pick up (must have a Light attached)

    [Header("Flashlight Settings")]
    [Range(0f, 100f)]
    public float startingBattery = 100f;    // Initial battery percentage
    public float drainRatePerSecond = 5f;   // Percentage drained per second when light is on

    private Transform originalParent;
    private Vector3 originalPosition;
    private Quaternion originalRotation;
    private Light objectLight;
    private bool isHolding = false;
    private float batteryPercentage;

    void Start()
    {
        if (pickupObject != null)
        {
            originalParent = pickupObject.transform.parent;
            originalPosition = pickupObject.transform.position;
            originalRotation = pickupObject.transform.rotation;

            objectLight = pickupObject.GetComponentInChildren<Light>();
            if (objectLight != null)
                objectLight.enabled = false; // Start with light off
        }

        batteryPercentage = Mathf.Clamp(startingBattery, 0f, 100f);
    }

    void Update()
    {
        if (pickupObject == null || cameraMover == null) return;

        // Pick up / put down
        if (Input.GetKeyDown(KeyCode.B))
        {
            if (!isHolding && IsAtOne())
                PickUp();
            else if (isHolding && IsAtOne())
                PutDown();
        }

        // Toggle flashlight
        if (isHolding && Input.GetKeyDown(KeyCode.F))
        {
            if (objectLight != null)
            {
                // Can only turn on if battery > 0
                if (!objectLight.enabled && batteryPercentage > 0f)
                    objectLight.enabled = true;
                else
                    objectLight.enabled = false;
            }
        }

        // Drain battery if flashlight is on
        if (isHolding && objectLight != null && objectLight.enabled)
        {
            batteryPercentage -= drainRatePerSecond * Time.deltaTime;
            batteryPercentage = Mathf.Max(batteryPercentage, 0f);

            // Turn off if battery hits 0
            if (batteryPercentage <= 0f)
                objectLight.enabled = false;
        }
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
            objectLight.enabled = false; // Turn off when dropped

        isHolding = false;
    }

    // Optional: get battery percentage for UI
    public float GetBatteryPercentage()
    {
        return batteryPercentage;
    }
}