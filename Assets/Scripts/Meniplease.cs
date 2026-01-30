using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using System.Collections.Generic;

public class ControllerMenuNavigation : MonoBehaviour
{
    [Header("Menu Buttons (Top to Bottom)")]
    public Button[] buttons;

    [Header("Input")]
    public InputActionAsset inputActions;

    private InputAction moveAction;
    private InputAction submitAction;

    private int currentIndex = 0;
    private bool stickInUse;

    // Only consider buttons that are active in hierarchy
    private List<Button> ActiveButtons
    {
        get
        {
            List<Button> active = new List<Button>();
            foreach (var btn in buttons)
            {
                if (btn.gameObject.activeInHierarchy && btn.interactable)
                    active.Add(btn);
            }
            return active;
        }
    }

    void OnEnable()
    {
        var uiMap = inputActions.FindActionMap("UI");

        moveAction = uiMap.FindAction("Navigate");
        submitAction = uiMap.FindAction("Submit");

        moveAction.Enable();
        submitAction.Enable();

        // Make sure the first active button is highlighted
        currentIndex = 0;
        HighlightButton();
    }

    void OnDisable()
    {
        moveAction.Disable();
        submitAction.Disable();
    }

    void Update()
    {
        var move = moveAction.ReadValue<Vector2>();
        var activeButtons = ActiveButtons;

        if (activeButtons.Count == 0) return; // Nothing to select

        if (!stickInUse)
        {
            if (move.y > 0.5f)
            {
                ChangeSelection(-1, activeButtons);
            }
            else if (move.y < -0.5f)
            {
                ChangeSelection(1, activeButtons);
            }
        }

        stickInUse = Mathf.Abs(move.y) > 0.5f;

        if (submitAction.WasPressedThisFrame())
        {
            activeButtons[currentIndex].onClick.Invoke();
        }
    }

    void ChangeSelection(int direction, List<Button> activeButtons)
    {
        currentIndex += direction;

        if (currentIndex < 0)
            currentIndex = activeButtons.Count - 1;
        else if (currentIndex >= activeButtons.Count)
            currentIndex = 0;

        HighlightButton(activeButtons);
    }

    void HighlightButton()
    {
        HighlightButton(ActiveButtons);
    }

    void HighlightButton(List<Button> activeButtons)
    {
        if (activeButtons.Count == 0) return;
        currentIndex = Mathf.Clamp(currentIndex, 0, activeButtons.Count - 1);
        activeButtons[currentIndex].Select();
    }
}