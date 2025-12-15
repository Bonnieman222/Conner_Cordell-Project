using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class PauseMenuWithIntro : MonoBehaviour
{
    [Header("Pause Menu")]
    public GameObject pauseCanvas;

    [Header("Night Intro")]
    public CanvasGroup introCanvas;
    public float showTime = 5f;

    private bool isPaused = false;
    private bool allowPause = true;

    // ------------------------------------------------------------
    // UPDATE
    // ------------------------------------------------------------

    void Update()
    {
        // Don't allow pause while intro is playing
        if (!allowPause) return;

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (isPaused)
                Resume();
            else
                Pause();
        }
    }

    // ------------------------------------------------------------
    // PAUSE SYSTEM
    // ------------------------------------------------------------

    public void Pause()
    {
        if (pauseCanvas == null) return;

        pauseCanvas.SetActive(true);

        Time.timeScale = 0f;

        isPaused = true;
    }

    public void Resume()
    {
        if (pauseCanvas == null) return;

        pauseCanvas.SetActive(false);

        Time.timeScale = 1f;

        isPaused = false;
    }

    public void ClosePause()
    {
        if (pauseCanvas != null)
            pauseCanvas.SetActive(false);

        Time.timeScale = 1f;

        isPaused = false;
    }

    public void DisablePause()
    {
        allowPause = false;

        if (isPaused)
            ClosePause();
    }

    public void EnablePause()
    {
        allowPause = true;
    }

    // ------------------------------------------------------------
    // NIGHT INTRO SYSTEM
    // ------------------------------------------------------------

    /// <summary>
    /// Call this to run the intro manually.
    /// Example:
    /// FindObjectOfType<PauseMenuWithIntro>().PlayIntro();
    /// </summary>
    public void PlayIntro()
    {
        StartCoroutine(ShowIntro());
    }

    private IEnumerator ShowIntro()
    {
        // Disable pause during intro
        DisablePause();

        // Pause time
        Time.timeScale = 0f;

        // Show intro
        if (introCanvas != null)
        {
            introCanvas.alpha = 1f;
            introCanvas.interactable = true;
            introCanvas.blocksRaycasts = true;
        }

        // Wait real-time
        yield return new WaitForSecondsRealtime(showTime);

        // Hide intro
        if (introCanvas != null)
        {
            introCanvas.alpha = 0f;
            introCanvas.interactable = false;
            introCanvas.blocksRaycasts = false;
        }

        // Unpause
        Time.timeScale = 1f;

        // Re-enable pause
        EnablePause();
    }
}