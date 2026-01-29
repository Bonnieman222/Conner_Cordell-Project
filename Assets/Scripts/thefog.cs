using UnityEngine;

public class ConstantFog : MonoBehaviour
{
    [Header("Fog Settings")]
    public Color fogColor = Color.gray;     // Fog color
    public float fogDensity = 0.02f;        // Fog density
    public FogMode fogMode = FogMode.Exponential; // Fog type

    void Start()
    {
        // Enable fog
        RenderSettings.fog = true;
        RenderSettings.fogMode = fogMode;
        RenderSettings.fogColor = fogColor;
        RenderSettings.fogDensity = fogDensity;
    }
}