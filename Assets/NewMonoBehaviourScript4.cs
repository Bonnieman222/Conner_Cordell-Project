using UnityEngine;
using TMPro;

public class TMPFontFromTTF : MonoBehaviour
{
    [Header("References")]
    public TMP_Text tmpText;        // TMP component to apply font to
    public Font ttfFont;            // Drag your TTF/OTF font here in inspector

    void Start()
    {
        if (tmpText == null)
            tmpText = GetComponent<TMP_Text>();

        if (ttfFont != null)
        {
            ApplyTTFToTMP(ttfFont);
        }
        else
        {
            Debug.LogWarning("No TTF font assigned!");
        }
    }

    void ApplyTTFToTMP(Font font)
    {
        // Create TMP Font Asset from TTF
        TMP_FontAsset tmpFont = TMP_FontAsset.CreateFontAsset(font);
        if (tmpFont != null)
        {
            tmpText.font = tmpFont;
            Debug.Log("TMP font updated from TTF successfully!");
        }
        else
        {
            Debug.LogError("Failed to create TMP Font Asset.");
        }
    }
}