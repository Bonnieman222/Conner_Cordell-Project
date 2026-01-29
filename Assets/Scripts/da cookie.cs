using UnityEngine;
using UnityEngine.InputSystem;

public class ProcessController : MonoBehaviour
{
    public CameraMover cameraMover;
    public Light processLight;

    private bool isProcessing = false;
    private bool canFinish = false;

    private float processTimer = 0f;
    private float finishWindow = 0f;

    // --- New Input System ---
    private PlayerInputActions inputActions;

    private void Awake()
    {
        inputActions = new PlayerInputActions();
        inputActions.Player.StartProcess.performed += ctx => TryStartProcess();
        inputActions.Player.FinishProcess.performed += ctx => TryFinishProcess();
    }

    private void OnEnable() => inputActions.Player.Enable();
    private void OnDisable() => inputActions.Player.Disable();

    void Start()
    {
        if (processLight != null)
            processLight.enabled = false;
    }

    void Update()
    {
        if (!isProcessing) return;

        // --- PROCESS TIMER (ALWAYS RUNS) ---
        if (!canFinish)
        {
            processTimer -= Time.deltaTime;

            if (processTimer <= 0f)
            {
                canFinish = true;
                finishWindow = 15f;
                SetLightColor(Color.green);
                Debug.Log("Process ready to finish! Look at the station and press Finish.");
            }
        }
        else
        {
            finishWindow -= Time.deltaTime;

            if (finishWindow <= 0f)
            {
                Debug.Log("Failed! You didn't finish in time. Process reset.");
                ResetProcess();
            }
        }
    }

    // --- Input System ---
    private void TryStartProcess()
    {
        if (isProcessing) return;
        if (!IsAtProcessCamera()) return;

        StartProcess();
    }

    private void TryFinishProcess()
    {
        if (!isProcessing || !canFinish) return;
        if (!IsAtProcessCamera()) return;

        FinishProcess();
    }

    // --- Process Logic ---
    private void StartProcess()
    {
        isProcessing = true;
        canFinish = false;
        processTimer = NightSystem.Instance.GetProcessTime();

        SetLightColor(Color.white);
        Debug.Log($"Started process. Cooking time: {processTimer} seconds.");
    }

    private void FinishProcess()
    {
        isProcessing = false;
        canFinish = false;

        NightSystem.Instance.ProcessCompleted();
        TurnOffLight();

        Debug.Log("Process finished successfully!");
    }

    private void ResetProcess()
    {
        isProcessing = false;
        canFinish = false;
        processTimer = 0f;
        finishWindow = 0f;

        TurnOffLight();
    }

    // --- Helpers ---
    private bool IsAtProcessCamera()
    {
        if (cameraMover == null || cameraMover.moveAndLookBits.Length == 0)
            return false;

        return Vector3.Distance(
            cameraMover.transform.position,
            cameraMover.moveAndLookBits[0].moveTarget.position
        ) < 0.05f;
    }

    private void SetLightColor(Color color)
    {
        if (processLight != null)
        {
            processLight.enabled = true;
            processLight.color = color;
        }
    }

    private void TurnOffLight()
    {
        if (processLight != null)
            processLight.enabled = false;
    }
}