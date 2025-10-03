using UnityEngine;

public class ProcessController : MonoBehaviour
{
    public CameraMover cameraMover;   // Drag in the CameraMover reference
    public Light processLight;        // Drag a Light object here in Inspector

    private bool isProcessing = false;
    private float processTimer = 0f;
    private bool canFinish = false;
    private float finishWindow = 0f; // 15-second grace period

    void Start()
    {
        // Make sure light starts off
        if (processLight != null)
            processLight.enabled = false;
    }

    void Update()
    {
        // Only allow process if at slot 1 (index 0)
        if (cameraMover != null &&
            cameraMover.moveAndLookBits[0] != null &&
            cameraMover.transform.position == cameraMover.moveAndLookBits[0].moveTarget.position)
        {
            if (!isProcessing && Input.GetKeyDown(KeyCode.C))
            {
                StartProcess();
            }

            if (isProcessing)
            {
                if (!canFinish)
                {
                    // Countdown until process is ready
                    processTimer -= Time.deltaTime;
                    if (processTimer <= 0f)
                    {
                        canFinish = true;
                        finishWindow = 15f; // 15 seconds to press V
                        SetLightColor(Color.green);
                        Debug.Log("Process ready to finish! Press V within 15 seconds!");
                    }
                }
                else
                {
                    // Countdown finish window
                    finishWindow -= Time.deltaTime;

                    if (finishWindow <= 0f)
                    {
                        Debug.Log("Failed! You didn't press V in time. Process reset.");
                        ResetProcess();
                    }
                    else if (Input.GetKeyDown(KeyCode.V))
                    {
                        FinishProcess();
                    }
                }
            }
        }
    }

    private void StartProcess()
    {
        if (isProcessing) return;

        isProcessing = true;
        canFinish = false;
        processTimer = NightSystem.Instance.GetProcessTime();
        SetLightColor(Color.white); // Light ON (white) during processing
        Debug.Log($"Started process. Must wait {processTimer} seconds before finishing.");
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
        TurnOffLight();
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