using UnityEngine;
using System.Collections;

public class PauseManager : MonoBehaviour
{
    [Header("Pause UI")]
    public CanvasGroup pauseCanvas;

    [Header("Other UI to Block Pause")]
    public CanvasGroup blockWhenActive;

    [Header("Settings")]
    public float freezeAfterUnpause = 1f;
    public float pauseCooldown = 30f;

    private bool isPaused = false;
    private bool isFreezeLock = false;
    private float nextAllowedPauseTime = 0f;

    private PlayerInputActions inputActions;

    private void Awake()
    {
        inputActions = new PlayerInputActions();
        inputActions.Player.Pause.performed += ctx => TogglePause();
    }

    private void OnEnable() => inputActions.Enable();
    private void OnDisable() => inputActions.Disable();

    private void Start()
    {
        SetCanvasVisible(false);
    }

    private void TogglePause()
    {
        if (Time.unscaledTime < nextAllowedPauseTime || isFreezeLock) return;

        if (isPaused) Resume();
        else Pause();
    }

    public void Pause()
    {
        isPaused = true;
        Time.timeScale = 0f;
        SetCanvasVisible(true);
    }

    public void Resume()
    {
        isPaused = false;
        SetCanvasVisible(false);
        StartCoroutine(UnfreezeAfterDelay());
        nextAllowedPauseTime = Time.unscaledTime + pauseCooldown;
    }

    private IEnumerator UnfreezeAfterDelay()
    {
        isFreezeLock = true;
        Time.timeScale = 0f;
        yield return new WaitForSecondsRealtime(freezeAfterUnpause);
        Time.timeScale = 1f;
        isFreezeLock = false;
    }

    private void SetCanvasVisible(bool visible)
    {
        if (pauseCanvas == null) return;
        pauseCanvas.alpha = visible ? 1f : 0f;
        pauseCanvas.interactable = visible;
        pauseCanvas.blocksRaycasts = visible;
    }
}