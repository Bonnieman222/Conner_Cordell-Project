using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using System.Collections.Generic;

public class ControllerMenuNavigationWithSliders : MonoBehaviour
{
    [Header("Menu Elements (Buttons & Sliders)")]
    public Selectable[] menuElements;

    [Header("Input")]
    public InputActionAsset inputActions;

    private InputAction moveAction;
    private InputAction submitAction;

    private int currentIndex = 0;
    private bool stickInUse;

    private List<Selectable> ActiveElements
    {
        get
        {
            List<Selectable> active = new List<Selectable>();

            foreach (var elem in menuElements)
            {
                if (elem == null)
                    continue;

                if (!elem.gameObject.activeInHierarchy)
                    continue;

                if (!elem.interactable)
                    continue;

                if (!IsElementVisible(elem))
                    continue;

                active.Add(elem);
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

        currentIndex = 0;
        HighlightElement();
    }

    void OnDisable()
    {
        moveAction.Disable();
        submitAction.Disable();
    }

    void Update()
    {
        var activeElements = ActiveElements;

        if (activeElements.Count == 0)
            return;

        currentIndex = Mathf.Clamp(currentIndex, 0, activeElements.Count - 1);

        Vector2 move = moveAction.ReadValue<Vector2>();

        // Vertical navigation
        if (!stickInUse)
        {
            if (move.y > 0.5f)
            {
                ChangeSelection(-1, activeElements);
            }
            else if (move.y < -0.5f)
            {
                ChangeSelection(1, activeElements);
            }
        }

        stickInUse = Mathf.Abs(move.y) > 0.5f;

        Selectable current = activeElements[currentIndex];

        if (current is Button button)
        {
            if (submitAction.WasPressedThisFrame())
                button.onClick.Invoke();
        }
        else if (current is Slider slider)
        {
            float step = 1f * Time.unscaledDeltaTime;
            slider.value += move.x * step;
        }
    }

    void ChangeSelection(int direction, List<Selectable> activeElements)
    {
        currentIndex += direction;

        if (currentIndex < 0)
            currentIndex = activeElements.Count - 1;
        else if (currentIndex >= activeElements.Count)
            currentIndex = 0;

        HighlightElement(activeElements);
    }

    void HighlightElement()
    {
        HighlightElement(ActiveElements);
    }

    void HighlightElement(List<Selectable> activeElements)
    {
        if (activeElements.Count == 0)
            return;

        currentIndex = Mathf.Clamp(currentIndex, 0, activeElements.Count - 1);
        activeElements[currentIndex].Select();
    }

    // ✅ Proper ScrollRect visibility check
    bool IsElementVisible(Selectable element)
    {
        ScrollRect scrollRect = element.GetComponentInParent<ScrollRect>();

        // If not inside a ScrollRect, just allow it
        if (scrollRect == null)
            return true;

        RectTransform viewport = scrollRect.viewport;
        RectTransform elementRect = element.GetComponent<RectTransform>();

        if (viewport == null || elementRect == null)
            return true;

        Bounds elementBounds = RectTransformUtility.CalculateRelativeRectTransformBounds(viewport, elementRect);

        return elementBounds.size.y > 0 &&
               elementBounds.min.y < viewport.rect.height &&
               elementBounds.max.y > 0;
    }
}