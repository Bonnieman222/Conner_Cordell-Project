using UnityEngine;

/// <summary>
/// Adds a white glow effect to the object using emission
/// and draws a red gizmo line in the Scene view for debugging.
/// </summary>
public class GlowWhite : MonoBehaviour
{
    [SerializeField] private Color glowColor = Color.white;  // Color of the glow
    [SerializeField] private float intensity = 2f;           // Brightness of the glow

    private Material material;                               // Instance material used for emission

    /// <summary>
    /// Sets up the emission glow when the object starts.
    /// </summary>
    void Start()
    {
        // Get the Renderer component
        Renderer renderer = GetComponent<Renderer>();
        if (renderer != null)
        {
            // Create a unique material instance
            material = renderer.material;

            // Enable emission
            material.EnableKeyword("_EMISSION");

            // Apply glow color and intensity
            material.SetColor("_EmissionColor", glowColor * intensity);
        }
        else
        {
            Debug.LogWarning("GlowWhite: No Renderer found on this object.");
        }
    }

    /// <summary>
    /// Draws a red gizmo line forward from the object when selected.
    /// Used as a visual debugging aid in the Scene view.
    /// </summary>
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawLine(transform.position, transform.position + transform.forward * 5f);
    }

    /// <summary>
    /// Turns off the glow when the script is disabled.
    /// </summary>
    void OnDisable()
    {
        if (material != null)
        {
            material.SetColor("_EmissionColor", Color.black);
        }
    }
}