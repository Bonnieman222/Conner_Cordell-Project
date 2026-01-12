using UnityEngine;

public class GlowWhite : MonoBehaviour
{
    [SerializeField] private Color glowColor = Color.white;  // The glow color
    [SerializeField] private float intensity = 2f;           // Glow brightness

    private Material material;

    void Start()
    {
        // Get the Renderer’s material
        Renderer renderer = GetComponent<Renderer>();
        if (renderer != null)
        {
            // Create a copy of the material so it doesn’t affect others using the same one
            material = renderer.material;

            // Enable emission keyword
            material.EnableKeyword("_EMISSION");

            // Set the glow color and intensity
            material.SetColor("_EmissionColor", glowColor * intensity);
        }
        else
        {
            Debug.LogWarning("GlowWhite: No Renderer found on this object.");
        }
    }

    void OnDisable()
    {
        // Turn off glow when script is disabled
        if (material != null)
        {
            material.SetColor("_EmissionColor", Color.black);
        }
    }
}