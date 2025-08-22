using UnityEngine;

public class AdvanceOnIgnore : MonoBehaviour
{
    [Header("Advance Settings")]
    public Transform startPoint;      // Slot for starting position object
    public Transform[] advancePoints; // Up to 4 forward positions
    public float timeToCheck = 5f;    // Time before advancing when ignored

    private int currentAdvanceIndex = 0;
    private float lastCheckTime;

    [Header("References")]
    public Camera playerCamera;       // Player camera
    public Light flashlight;          // Flashlight (must be a Spotlight)
    private Collider monsterCollider;

    void Start()
    {
        if (startPoint != null)
        {
            transform.position = startPoint.position;
            transform.rotation = startPoint.rotation;
        }

        monsterCollider = GetComponent<Collider>();
        lastCheckTime = Time.time;
    }

    void Update()
    {
        bool lookedAt = CameraLookingAtMonster();
        bool litByFlashlight = FlashlightHittingMonster();

        // Flashlight + collider hit → reset to start
        if (flashlight != null && flashlight.enabled && litByFlashlight)
        {
            ResetToStart();
            return;
        }

        // Looked at → freeze
        if (lookedAt)
        {
            lastCheckTime = Time.time; // stop timer
            return;
        }

        // Ignored too long → advance
        if (Time.time - lastCheckTime >= timeToCheck)
        {
            Advance();
            lastCheckTime = Time.time;
        }
    }

    private bool CameraLookingAtMonster()
    {
        if (monsterCollider == null) return false;

        Bounds bounds = monsterCollider.bounds;

        // Check multiple points on the collider
        Vector3[] checkPoints =
        {
            bounds.center,
            bounds.min,
            bounds.max,
            new Vector3(bounds.min.x, bounds.min.y, bounds.max.z),
            new Vector3(bounds.min.x, bounds.max.y, bounds.min.z),
            new Vector3(bounds.max.x, bounds.min.y, bounds.min.z),
            new Vector3(bounds.max.x, bounds.max.y, bounds.min.z),
            new Vector3(bounds.max.x, bounds.min.y, bounds.max.z),
            new Vector3(bounds.min.x, bounds.max.y, bounds.max.z),
        };

        foreach (var point in checkPoints)
        {
            Vector3 viewportPos = playerCamera.WorldToViewportPoint(point);

            // inside camera view?
            if (viewportPos.z > 0 && viewportPos.x > 0 && viewportPos.x < 1 && viewportPos.y > 0 && viewportPos.y < 1)
            {
                // check visibility with raycast
                Vector3 dir = point - playerCamera.transform.position;
                Ray ray = new Ray(playerCamera.transform.position, dir.normalized);
                RaycastHit hit;
                if (Physics.Raycast(ray, out hit, 100f))
                {
                    if (hit.collider == monsterCollider)
                        return true;
                }
            }
        }

        return false;
    }

    private bool FlashlightHittingMonster()
    {
        if (flashlight == null || flashlight.type != LightType.Spot) return false;

        Ray ray = new Ray(flashlight.transform.position, flashlight.transform.forward);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, flashlight.range))
        {
            if (hit.collider != null && hit.collider == monsterCollider)
                return true;
        }
        return false;
    }

    private void Advance()
    {
        if (currentAdvanceIndex < advancePoints.Length)
        {
            transform.position = advancePoints[currentAdvanceIndex].position;
            transform.rotation = advancePoints[currentAdvanceIndex].rotation;
            currentAdvanceIndex++;
        }
    }

    private void ResetToStart()
    {
        if (startPoint != null)
        {
            transform.position = startPoint.position;
            transform.rotation = startPoint.rotation;
        }

        currentAdvanceIndex = 0;
        lastCheckTime = Time.time;
    }
}