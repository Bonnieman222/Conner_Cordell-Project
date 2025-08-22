using UnityEngine;

public class CameraMover : MonoBehaviour
{
    [System.Serializable]
    public class MoveAndLook
    {
        public Transform moveTarget; // Where the camera moves
        public Transform lookTarget; // What the camera looks at
        public float moveSpeed = 3f;
        public float rotateSpeed = 3f;
    }

    public MoveAndLook[] moveAndLookBits = new MoveAndLook[5]; // 5 slots
    private int currentIndex = 0; // Start at slot 1 (index 0)
    private bool isMoving = false;

    private float cooldownTime = 1f; // 1 second cooldown
    private float lastMoveTime = -999f;

    void Start()
    {
        // Place camera at slot 1 (index 0) right away
        if (moveAndLookBits[0] != null && moveAndLookBits[0].moveTarget != null)
        {
            transform.position = moveAndLookBits[0].moveTarget.position;

            if (moveAndLookBits[0].lookTarget != null)
            {
                transform.LookAt(moveAndLookBits[0].lookTarget);
            }
        }
    }

    void Update()
    {
        // Number keys trigger requested move
        if (Input.GetKeyDown(KeyCode.Alpha1)) TryMoveTo(0);
        if (Input.GetKeyDown(KeyCode.Alpha2)) TryMoveTo(1);
        if (Input.GetKeyDown(KeyCode.Alpha3)) TryMoveTo(2);
        if (Input.GetKeyDown(KeyCode.Alpha4)) TryMoveTo(3);
        if (Input.GetKeyDown(KeyCode.Alpha5)) TryMoveTo(4);

        if (isMoving) PerformMoveAndLook();
    }

    private void TryMoveTo(int targetIndex)
    {
        if (Time.time - lastMoveTime < cooldownTime) return; // still cooling down

        if (CanMove(currentIndex, targetIndex))
        {
            currentIndex = targetIndex;
            isMoving = true;
            lastMoveTime = Time.time;
        }
    }

    private bool CanMove(int from, int to)
    {
        // Define allowed transitions
        switch (from)
        {
            case 0: return (to == 1 || to == 2 || to == 4); // 1 → 2, 3, or 5
            case 1: return (to == 0);                       // 2 → 1
            case 2: return (to == 0 || to == 3);            // 3 → 1 or 4
            case 3: return (to == 2);                       // 4 → 3
            case 4: return (to == 0);                       // 5 → 1
        }
        return false;
    }

    private void PerformMoveAndLook()
    {
        MoveAndLook ml = moveAndLookBits[currentIndex];
        if (ml == null || ml.moveTarget == null || ml.lookTarget == null) return;

        // Smoothly move
        transform.position = Vector3.Lerp(
            transform.position,
            ml.moveTarget.position,
            Time.deltaTime * ml.moveSpeed
        );

        // Smoothly rotate
        Quaternion targetRotation = Quaternion.LookRotation(ml.lookTarget.position - transform.position);
        transform.rotation = Quaternion.Slerp(
            transform.rotation,
            targetRotation,
            Time.deltaTime * ml.rotateSpeed
        );

        // Stop when close enough
        float distance = Vector3.Distance(transform.position, ml.moveTarget.position);
        float angle = Quaternion.Angle(transform.rotation, targetRotation);

        if (distance < 0.05f && angle < 1f)
        {
            transform.position = ml.moveTarget.position;
            transform.rotation = targetRotation;
            isMoving = false;
        }
    }
}