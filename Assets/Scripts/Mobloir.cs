using UnityEngine;
using UnityEngine.UI;

public class ActiveInputUIController : MonoBehaviour
{
    [Header("References")]
    public CameraMover cameraMover;
    public FlashlightController flashlightController;
    public ProcessController processController;

    [Header("UI Buttons")]
    public Button moveSlot1Button;
    public Button moveSlot2Button;
    public Button moveSlot3Button;
    public Button moveSlot4Button;
    public Button moveSlot5Button;

    public Button pickUpFlashlightButton;
    public Button toggleFlashlightButton;

    public Button pickUpShotgunButton;
    public Button fireShotgunButton;

    public Button startProcessButton;
    public Button finishProcessButton;

    public Button pauseButton;

    [Header("Pause Menu Reference")]
    public GameObject pauseCanvas;

#if UNITY_ANDROID || UNITY_IOS
    private bool isMobile = true;
#else
    private bool isMobile = false;
#endif

    private void Awake()
    {
        // Show canvas only on mobile
        if (pauseCanvas != null)
            pauseCanvas.SetActive(isMobile);

        if (!isMobile)
        {
            HideAllButtons();
            return;
        }

        SetupButtonListeners();
        UpdateButtonVisibility();
    }

    // -----------------------
    // Setup Event Listeners
    // -----------------------
    private void SetupButtonListeners()
    {
        // Camera movement buttons
        moveSlot1Button.onClick.AddListener(() => TryMoveToSlot(0));
        moveSlot2Button.onClick.AddListener(() => TryMoveToSlot(1));
        moveSlot3Button.onClick.AddListener(() => TryMoveToSlot(2));
        moveSlot4Button.onClick.AddListener(() => TryMoveToSlot(3));
        moveSlot5Button.onClick.AddListener(() => TryMoveToSlot(4));

        // Flashlight buttons
        pickUpFlashlightButton.onClick.AddListener(TryPickUpFlashlight);
        toggleFlashlightButton.onClick.AddListener(TryToggleFlashlight);

        // Shotgun buttons
        pickUpShotgunButton.onClick.AddListener(TryPickUpShotgun);
        fireShotgunButton.onClick.AddListener(TryFireShotgun);

        // Process buttons
        startProcessButton.onClick.AddListener(TryStartProcess);
        finishProcessButton.onClick.AddListener(TryFinishProcess);

        // Pause button
        pauseButton.onClick.AddListener(TogglePause);
    }

    // -----------------------
    // Button Methods
    // -----------------------
    private void TryMoveToSlot(int slotIndex)
    {
        if (cameraMover == null) return;
        if (!cameraMover.CanMoveTo(slotIndex)) return;

        cameraMover.TryMoveTo(slotIndex);
        UpdateButtonVisibility();
    }

    private void TryPickUpFlashlight()
    {
        if (flashlightController == null) return;
        if (!flashlightController.IsAtCameraSlot0Public()) return;

        flashlightController.ToggleFlashlightPickup();
        UpdateButtonVisibility();
    }

    private void TryToggleFlashlight()
    {
        if (flashlightController == null) return;
        if (!flashlightController.IsFlashlightHeld()) return;

        flashlightController.ToggleFlashlightLight();
    }

    private void TryPickUpShotgun()
    {
        if (flashlightController == null) return;
        if (!flashlightController.IsAtCameraSlot0Public()) return;

        flashlightController.ToggleShotgunPickup();
        UpdateButtonVisibility();
    }

    private void TryFireShotgun()
    {
        if (flashlightController == null) return;
        if (!flashlightController.IsShotgunHeld() || !flashlightController.CanFireShotgun()) return;

        flashlightController.TryFireShotgun();
        UpdateButtonVisibility();
    }

    private void TryStartProcess()
    {
        if (processController == null) return;
        if (processController.IsProcessingPublic()) return;

        processController.TryStartProcess();
        UpdateButtonVisibility();
    }

    private void TryFinishProcess()
    {
        if (processController == null) return;
        if (!processController.CanFinishPublic()) return;

        processController.TryFinishProcess();
        UpdateButtonVisibility();
    }

    private void TogglePause()
    {
        if (pauseCanvas == null) return;

        bool isActive = pauseCanvas.activeSelf;
        pauseCanvas.SetActive(!isActive);
        Time.timeScale = isActive ? 1f : 0f;
    }

    // -----------------------
    // Update Button Visibility
    // -----------------------
    public void UpdateButtonVisibility()
    {
        if (!isMobile) return;

        if (cameraMover != null)
        {
            moveSlot1Button.gameObject.SetActive(cameraMover.CanMoveTo(0));
            moveSlot2Button.gameObject.SetActive(cameraMover.CanMoveTo(1));
            moveSlot3Button.gameObject.SetActive(cameraMover.CanMoveTo(2));
            moveSlot4Button.gameObject.SetActive(cameraMover.CanMoveTo(3));
            moveSlot5Button.gameObject.SetActive(cameraMover.CanMoveTo(4));
        }

        if (flashlightController != null)
        {
            pickUpFlashlightButton.gameObject.SetActive(!flashlightController.IsFlashlightHeld() && flashlightController.IsAtCameraSlot0Public());
            toggleFlashlightButton.gameObject.SetActive(flashlightController.IsFlashlightHeld());

            pickUpShotgunButton.gameObject.SetActive(!flashlightController.IsShotgunHeld() && flashlightController.IsAtCameraSlot0Public());
            fireShotgunButton.gameObject.SetActive(flashlightController.IsShotgunHeld() && flashlightController.CanFireShotgun());
        }

        if (processController != null)
        {
            startProcessButton.gameObject.SetActive(!processController.IsProcessingPublic());
            finishProcessButton.gameObject.SetActive(processController.CanFinishPublic());
        }
    }

    // -----------------------
    // Utility
    // -----------------------
    private void HideAllButtons()
    {
        moveSlot1Button.gameObject.SetActive(false);
        moveSlot2Button.gameObject.SetActive(false);
        moveSlot3Button.gameObject.SetActive(false);
        moveSlot4Button.gameObject.SetActive(false);
        moveSlot5Button.gameObject.SetActive(false);

        pickUpFlashlightButton.gameObject.SetActive(false);
        toggleFlashlightButton.gameObject.SetActive(false);

        pickUpShotgunButton.gameObject.SetActive(false);
        fireShotgunButton.gameObject.SetActive(false);

        startProcessButton.gameObject.SetActive(false);
        finishProcessButton.gameObject.SetActive(false);

        pauseButton.gameObject.SetActive(false);
    }
}
