using UnityEngine;
using System.Collections;

public class CameraMoveWithBlackStart : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed = 5f;
    public float stopDistance = 0.1f;

    [Header("Fog Settings")]
    public Color fogColor = Color.gray;
    public float fogDensity = 0.02f;

    [Header("UI Settings")]
    public Canvas uiCanvas;

    [Header("Black Start Settings")]
    public float blackDuration = 5f;

    private Transform target;
    private bool movementStarted;
    private bool hasStopped;
    private Camera cam;

    private void Awake()
    {
        cam = GetComponent<Camera>();

        // 🔒 UI MUST start disabled
        if (uiCanvas != null)
            uiCanvas.gameObject.SetActive(false);

        ApplyFogSettings();
    }

    private void Start()
    {
        StartCoroutine(SceneEntryFlow());
    }

    private IEnumerator SceneEntryFlow()
    {
        movementStarted = false;
        hasStopped = false;

        // Wait for scene to fully initialize (audio loads need this)
        yield return new WaitForEndOfFrame();

        // Black screen
        cam.clearFlags = CameraClearFlags.SolidColor;
        cam.backgroundColor = Color.black;

        yield return new WaitForSeconds(blackDuration);

        target = FindCameraTarget();
        if (target == null)
            yield break;

        cam.clearFlags = CameraClearFlags.Skybox;
        cam.backgroundColor = fogColor;

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
        RenderSettings.fog = true;
        RenderSettings.fogMode = FogMode.Exponential;
        RenderSettings.fogColor = fogColor;
        RenderSettings.fogDensity = fogDensity;
    }
}