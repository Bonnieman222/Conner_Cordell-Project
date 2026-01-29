using UnityEngine;
using TMPro;

public class NightDisplay : MonoBehaviour
{
    public TextMeshProUGUI nightText;

    public void SetNight(int night)
    {
        if (nightText != null)
            nightText.text = $"Night {night}";
    }
}