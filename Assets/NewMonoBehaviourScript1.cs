using UnityEngine;

public class PickupController : MonoBehaviour
{
    public CameraMover cameraMover; // Reference to your CameraMover script
    public Transform holdPoint;     // Where the object will be held (child of camera for example)
    public GameObject pickupObject; // The object to pick up (must have a Light attached)

    private Transform originalParent;
    private Vector3 originalPosition;
    private Quaternion originalRotation;
    private Light objectLight;
    private bool isHolding = false;

    void Start()
    {
        if (pickupObject != null)
        {
            // Save starting place
            originalParent = pickupObject.transform.parent;
            originalPosition = pickupObject.transform.position;
            originalRotation = pickupObject.transform.rotation;

            objectLight = pickupObject.GetComponentInChildren<Light>();
            if (objectLight != null)
                objectLight.enabled = false; // Start with light off
        }
    }

    void Update()
    {
        if (pickupObject == null || cameraMover == null) return;

        // Press B to pick up or put down
        if (Input.GetKeyDown(KeyCode.B))
        {
            if (!isHolding && IsAtOne())
            {
                PickUp();
            }
            else if (isHolding && IsAtOne())
            {
                PutDown();
            }
        }

        // Press F to toggle light if holding
        if (isHolding && Input.GetKeyDown(KeyCode.F))
        {
            if (objectLight != null)
                objectLight.enabled = !objectLight.enabled;
        }
    }

    private bool IsAtOne()
    {
        return cameraMover != null && GetCameraMoverIndex() == 0;
    }

    private int GetCameraMoverIndex()
    {
        // Access the private currentIndex with reflection since it’s private in CameraMover
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
}