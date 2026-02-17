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

    [HideInInspector]
    public PlayerInputActions inputActions;

    private void Awake()
    {
        inputActions = new PlayerInputActions();
        inputActions.Player.StartProcess.performed += ctx => TryStartProcess();
        inputActions.Player.FinishProcess.performed += ctx => TryFinishProcess();
    }

    private void OnEnable() => inputActions.Player.Enable();
    private void OnDisable() => inputActions.Player.Disable();

    private void Start()
    {
        if (processLight != null) processLight.enabled = false;
    }

    private void Update()
    {
        if (!isProcessing) return;

        if (!canFinish)
        {
            processTimer -= Time.deltaTime;
            if (processTimer <= 0f)
            {
                canFinish = true;
                finishWindow = 15f;
                SetLightColor(Color.green);
            }
        }
        else
        {
            finishWindow -= Time.deltaTime;
            if (finishWindow <= 0f)
            {
                ResetProcess();
            }
        }
    }

    public void TryStartProcess()
    {
        if (isProcessing || !IsAtProcessCamera()) return;
        StartProcess();
    }

    public void TryFinishProcess()
    {
        if (!isProcessing || !canFinish || !IsAtProcessCamera()) return;
        FinishProcess();
    }

    private void StartProcess()
    {
        isProcessing = true;
        canFinish = false;
        processTimer = NightSystem.Instance.GetProcessTime();
        SetLightColor(Color.white);
    }

    private void FinishProcess()
    {
        isProcessing = false;
        canFinish = false;
        NightSystem.Instance.ProcessCompleted();
        TurnOffLight();
    }

    private void ResetProcess()
    {
        isProcessing = false;
        canFinish = false;
        processTimer = 0f;
        finishWindow = 0f;
        TurnOffLight();
    }

    private bool IsAtProcessCamera()
    {
        if (cameraMover == null || cameraMover.moveAndLookBits.Length == 0) return false;
        return Vector3.Distance(cameraMover.transform.position,
            cameraMover.moveAndLookBits[0].moveTarget.position) < 0.05f;
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
        if (processLight != null) processLight.enabled = false;
    }

    // -------- PUBLIC GETTERS --------
    public bool IsProcessingPublic() => isProcessing;
    public bool CanFinishPublic() => canFinish;
}
