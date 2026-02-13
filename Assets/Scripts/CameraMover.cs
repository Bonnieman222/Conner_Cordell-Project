using UnityEngine;
using UnityEngine.InputSystem;

[System.Serializable]
public class MoveAndLook
{
    public Transform moveTarget;
    public Transform lookTarget;
    public float moveSpeed = 2f;
    public float rotateSpeed = 5f;
}

public class CameraMover : MonoBehaviour
{
    [Header("Movement Settings")]
    public MoveAndLook[] moveAndLookBits;
    public float cooldownTime = 0.5f;

    [Header("Optional Audio")]
    public AudioSource moveAudio;

    [HideInInspector]
    public PlayerInputActions inputActions;

    private int currentIndex = 0;
    private bool isMoving = false;
    private float lastMoveTime = 0f;
    private bool moveAudioPlayed = false;

    private void Awake()
    {
        inputActions = new PlayerInputActions();
        inputActions.Camera.Slot1.performed += ctx => TryMoveTo(0);
        inputActions.Camera.Slot2.performed += ctx => TryMoveTo(1);
        inputActions.Camera.Slot3.performed += ctx => TryMoveTo(2);
        inputActions.Camera.Slot4.performed += ctx => TryMoveTo(3);
        inputActions.Camera.Slot5.performed += ctx => TryMoveTo(4);
    }

    private void OnEnable()
    {
        inputActions.Camera.Enable();
    }

    private void OnDisable()
    {
        inputActions.Camera.Disable();
    }

    private void Start()
    {
        if (moveAndLookBits.Length > 0 && moveAndLookBits[0].moveTarget != null && moveAndLookBits[0].lookTarget != null)
        {
            transform.position = moveAndLookBits[0].moveTarget.position;
            transform.rotation = Quaternion.LookRotation(moveAndLookBits[0].lookTarget.position - transform.position);
        }
    }

    private void Update()
    {
        PerformMoveAndLook();
    }

    public void TryMoveTo(int targetIndex)
    {
        if (isMoving) return;
        if (Time.time - lastMoveTime < cooldownTime) return;
        if (CanMove(currentIndex, targetIndex))
        {
            currentIndex = targetIndex;
            isMoving = true;
            lastMoveTime = Time.time;
            moveAudioPlayed = false;
        }
    }

    public bool CanMoveTo(int index) => CanMove(currentIndex, index);

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

        if (isMoving && !moveAudioPlayed)
        {
            moveAudio?.Play();
            moveAudioPlayed = true;
        }

        transform.position = Vector3.Lerp(transform.position, ml.moveTarget.position, Time.deltaTime * ml.moveSpeed);
        Quaternion targetRotation = Quaternion.LookRotation(ml.lookTarget.position - transform.position);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * ml.rotateSpeed);

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
