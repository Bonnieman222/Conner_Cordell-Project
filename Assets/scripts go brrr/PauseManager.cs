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

    void Start()
    {
        SetCanvasVisible(false);
    }

    void Update()
    {
        // Block pause if other UI is showing
        if (blockWhenActive != null && blockWhenActive.alpha > 0.01f)
            return;

        // Block pause if cooldown is active
        if (Time.unscaledTime < nextAllowedPauseTime)
            return;

        // Block pause if freeze lock is happening
        if (isFreezeLock)
            return;

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (isPaused)
                Resume();
            else
                Pause();
        }
    }

    public void Pause()
    {
        if (isFreezeLock) return;

        isPaused = true;

        Time.timeScale = 0f;
        SetCanvasVisible(true);
    }

    public void Resume()
    {
        isPaused = false;

        SetCanvasVisible(false);

        // Start freeze lock
        StartCoroutine(UnfreezeAfterDelay());

        // Start cooldown
        nextAllowedPauseTime = Time.unscaledTime + pauseCooldown;
    }

    private IEnumerator UnfreezeAfterDelay()
    {
        isFreezeLock = true;

        // stay frozen
        Time.timeScale = 0f;

        yield return new WaitForSecondsRealtime(freezeAfterUnpause);

        // unfreeze
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