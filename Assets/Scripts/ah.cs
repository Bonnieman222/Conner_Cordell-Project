using UnityEngine;

public class PrefabDropper : MonoBehaviour
{
    [Header("Drop Settings")]
    public GameObject prefab;          // What drops
    public float dropInterval = 0.2f;  // How often to drop
    public float dropDuration = 30f;   // How long it drops

    private float dropTimer = 0f;
    private float durationTimer = 0f;
    private bool isDropping = true;

    void Update()
    {
        if (!isDropping) return;

        durationTimer += Time.deltaTime;

        // Stop after 30 seconds
        if (durationTimer >= dropDuration)
        {
            isDropping = false;
            return;
        }

        dropTimer += Time.deltaTime;

        // Drop prefab on interval
        if (dropTimer >= dropInterval)
        {
            dropTimer = 0f;
            DropPrefab();
        }
    }

    void DropPrefab()
    {
        Instantiate(prefab, transform.position, Quaternion.identity);
    }
}