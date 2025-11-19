using UnityEngine;

[System.Serializable]
public class MoveAndLook
{
    public Transform moveTarget;   // Target position
    public Transform lookTarget;   // Target to look at
    public float moveSpeed = 2f;
    public float rotateSpeed = 5f;
}

public class CameraMover : MonoBehaviour
{
    [Header("Movement Settings")]
    public MoveAndLook[] moveAndLookBits;
    public float cooldownTime = 0.5f; // Time between moves

    [Header("Optional Audio")]
    public AudioSource moveAudio;

    private int currentIndex = 0;
    private bool isMoving = false;
    private float lastMoveTime = 0f;
    private bool moveAudioPlayed = false;

    private void Start()
    {
        // Start at element 0
        if (moveAndLookBits.Length > 0 && moveAndLookBits[0].moveTarget != null && moveAndLookBits[0].lookTarget != null)
        {
            transform.position = moveAndLookBits[0].moveTarget.position;
            transform.rotation = Quaternion.LookRotation(moveAndLookBits[0].lookTarget.position - transform.position);
        }
    }

    private void Update()
    {
        HandleInput();
        PerformMoveAndLook();
    }

    private void HandleInput()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1)) TryMoveTo(0);
        if (Input.GetKeyDown(KeyCode.Alpha2)) TryMoveTo(1);
        if (Input.GetKeyDown(KeyCode.Alpha3)) TryMoveTo(2);
        if (Input.GetKeyDown(KeyCode.Alpha4)) TryMoveTo(3);
        if (Input.GetKeyDown(KeyCode.Alpha5)) TryMoveTo(4);
    }

    public void TryMoveTo(int targetIndex)
    {
        // Prevent move if already moving
        if (isMoving) return;

        // Check cooldown
        if (Time.time - lastMoveTime < cooldownTime) return;

        // Check allowed transitions
        if (CanMove(currentIndex, targetIndex))
        {
            currentIndex = targetIndex;
            isMoving = true;
            lastMoveTime = Time.time;
            moveAudioPlayed = false; // Reset audio flag
        }
    }

    private bool CanMove(int from, int to)
    {
        switch (from)
        {
            case 0: return (to == 1 || to == 2 || to == 4);
            case 1: return (to == 0);
            case 2: return (to == 0 || to == 3);
            case 3: return (to == 2);
            case 4: return (to == 0);
        }
        return false;
    }

    private void PerformMoveAndLook()
    {
        MoveAndLook ml = moveAndLookBits[currentIndex];
        if (ml == null || ml.moveTarget == null || ml.lookTarget == null) return;

        // Play audio once at the start of the move
        if (isMoving && !moveAudioPlayed)
        {
            if (moveAudio != null) moveAudio.Play();
            moveAudioPlayed = true;
        }

        // Smooth movement
        transform.position = Vector3.Lerp(transform.position, ml.moveTarget.position, Time.deltaTime * ml.moveSpeed);

        // Smooth rotation
        Quaternion targetRotation = Quaternion.LookRotation(ml.lookTarget.position - transform.position);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * ml.rotateSpeed);

        // Stop moving when close enough
        float distance = Vector3.Distance(transform.position, ml.moveTarget.position);
        float angle = Quaternion.Angle(transform.rotation, targetRotation);

        if (distance < 0.05f && angle < 1f)
        {
            transform.position = ml.moveTarget.position;
            transform.rotation = targetRotation;
            isMoving = false; // Allow new moves
        }
    }
}
