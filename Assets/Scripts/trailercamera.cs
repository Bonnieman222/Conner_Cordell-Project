using UnityEngine;
using System.Collections;

public class CameraMoveWithBlackStart_Fixed : MonoBehaviour
{
    [Header("Movement Settings")]
    public float moveSpeed = 5f;
    public float stopDistance = 0.1f;

    [Header("Fog Settings")]
    public Color fogColor = Color.gray;
    [Range(0.01f, 0.2f)]
    public float fogDensity = 0.05f; // Increased slightly for visibility in builds

    [Header("UI Settings")]
    public Canvas uiCanvas;

    [Header("Black Start Settings")]
    public float blackDuration = 5f;

    private Transform target;
    private bool movementStarted = false;
    private bool hasStopped = false;
    private Camera cam;

    private void Awake()
    {
        cam = GetComponent<Camera>();

        // 🔒 UI starts disabled
        if (uiCanvas != null)
            uiCanvas.gameObject.SetActive(false);

        // Initially disable fog for black screen
        RenderSettings.fog = false;

        // Black screen first
        cam.clearFlags = CameraClearFlags.SolidColor;
        cam.backgroundColor = Color.black;
    }

    private void Start()
    {
        StartCoroutine(SceneEntryFlow());
    }

    private IEnumerator SceneEntryFlow()
    {
        movementStarted = false;
        hasStopped = false;

        // Wait a frame for scene initialization (audio, lighting, etc.)
        yield return new WaitForEndOfFrame();

        // Hold black screen for specified duration
        yield return new WaitForSeconds(blackDuration);

        // Find the target with tag "CameraTag"
        target = FindCameraTarget();
        if (target == null)
            yield break;

        // Apply fog and background
        ApplyFogSettings();

        movementStarted = true;
    }

    private void Update()
    {
        if (!movementStarted || hasStopped || target == null)
            return;

        float distance = Vector3.Distance(transform.position, target.position);

        if (distance > stopDistance)
        {
            // Move toward target smoothly
            transform.position +=
                (target.position - transform.position).normalized
                * moveSpeed * Time.deltaTime;

            transform.LookAt(target);
        }
        else
        {
            FinishMovement();
        }
    }

    private void FinishMovement()
    {
        hasStopped = true;

        if (uiCanvas != null)
            uiCanvas.gameObject.SetActive(true);
    }

    private Transform FindCameraTarget()
    {
        GameObject obj = GameObject.FindGameObjectWithTag("CameraTag");
        if (obj == null)
        {
            Debug.LogError("CameraMoveWithBlackStart: No object with tag 'CameraTag' found!");
            return null;
        }

        return obj.transform;
    }

    private void ApplyFogSettings()
    {
        // Ensure fog is enabled for both editor and build
        RenderSettings.fog = true;
        RenderSettings.fogMode = FogMode.Exponential;
        RenderSettings.fogColor = fogColor;
        RenderSettings.fogDensity = fogDensity;

        // Set camera background to solid color to make fog visible
        cam.clearFlags = CameraClearFlags.SolidColor;
        cam.backgroundColor = fogColor;

        // Optional: Log for debugging builds
        Debug.Log($"Fog Applied: Color={fogColor}, Density={fogDensity}");
    }
}
