using UnityEngine;
using TMPro;

public class StarWarsScroll : MonoBehaviour
{
    [Header("Scroll Settings")]
    public float scrollSpeed = 30f;
    public float resetPositionY = -600f;  // Where text starts
    public float endPositionY = 1200f;    // Where text disappears

    private RectTransform rectTransform;

    void Start()
    {
        rectTransform = GetComponent<RectTransform>();
        Vector2 pos = rectTransform.anchoredPosition;
        pos.y = resetPositionY;
        rectTransform.anchoredPosition = pos;
    }

    void Update()
    {
        rectTransform.anchoredPosition += Vector2.up * scrollSpeed * Time.deltaTime;

        if (rectTransform.anchoredPosition.y >= endPositionY)
        {
            // Optional: Stop scrolling when finished
            enabled = false;
        }
    }
}