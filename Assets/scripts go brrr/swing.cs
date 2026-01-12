using UnityEngine;

public class GentleRotateX : MonoBehaviour
{
    [Header("Rotation Settings")]
    public float angle = 20f;        // How far it tilts left/right
    public float speed = 2f;         // How fast it swings

    private float startX;

    void Start()
    {
        startX = transform.localEulerAngles.x;
    }

    void Update()
    {
        float rotX = startX + Mathf.Sin(Time.time * speed) * angle;
        transform.localEulerAngles = new Vector3(rotX, transform.localEulerAngles.y, transform.localEulerAngles.z);
    }
}