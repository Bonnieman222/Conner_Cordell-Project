using UnityEngine;
using UnityEngine.SceneManagement;
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

    // Static flag set by the SceneSwitchButton to skip intro
    private static bool skipIntro = false;

    private void Awake()
    {
        cam = GetComponent<Camera>();

        // Always apply fog settings
        RenderSettings.fog = true;
        RenderSettings.fogMode = FogMode.Exponential;
        RenderSettings.fogColor = fogColor;
        RenderSettings.fogDensity = fogDensity;
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void Start()
    {
        if (!skipIntro)
        {
            // Normal intro
            StartCoroutine(InitializeAfterSceneLoad());
        }
        else
        {
            // Skip intro immediately
            SkipToEnd();
        }

        skipIntro = false; // Reset flag for future scene loads
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (skipIntro)
        {
            SkipToEnd();
            skipIntro = false;
        }
    }

    public static void SetSkipIntroFlag()
    {
        skipIntro = true;
    }

    private IEnumerator InitializeAfterSceneLoad()
    {
        movementStarted = false;
        hasStopped = false;

        // Hide UI
        if (uiCanvas != null)
            uiCanvas.gameObject.SetActive(false);

        // Black screen
        cam.clearFlags = CameraClearFlags.SolidColor;
        cam.backgroundColor = Color.black;

        yield return new WaitForSeconds(blackDuration);

        // Find target
        target = GameObject.FindGameObjectWithTag("CameraTag")?.transform;
        if (target == null)
        {
            Debug.LogError("CameraMoveWithBlackStart: No object with tag 'CameraTag' found!");
            yield break;
        }

        cam.backgroundColor = fogColor;
        movementStarted = true;
    }

    private void Update()
    {
        if (!movementStarted || hasStopped || target == null)
            return;

        float distance = Vector3.Distance(transform.position, target.position);

        if (distance > stopDistance)
        {
            Vector3 direction = (target.position - transform.position).normalized;
            transform.position += direction * moveSpeed * Time.deltaTime;
            transform.LookAt(target);
        }
        else
        {
            hasStopped = true;

            // Show UI
            if (uiCanvas != null)
                uiCanvas.gameObject.SetActive(true);
        }
    }

    private void SkipToEnd()
    {
        // Find target
        target = GameObject.FindGameObjectWithTag("CameraTag")?.transform;
        if (target == null)
        {
            Debug.LogError("CameraMoveWithBlackStart: No object with tag 'CameraTag' found!");
            return;
        }

        // Optional: end position object
        GameObject endPosObj = GameObject.Find("CameraEndPosition");
        Vector3 endPos = (endPosObj != null) ? endPosObj.transform.position : target.position;

        // Snap camera
        transform.position = endPos;
        transform.LookAt(target);

        // Show UI immediately
        if (uiCanvas != null)
            uiCanvas.gameObject.SetActive(true);

        // Mark as finished
        movementStarted = true;
        hasStopped = true;
        cam.backgroundColor = fogColor;
    }
}