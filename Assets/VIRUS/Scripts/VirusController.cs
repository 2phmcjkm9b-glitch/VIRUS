
using UnityEngine;
using UnityEngine.InputSystem;

public class VirusController : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float moveSpeed = 5f;

    [Header("Turning")]
    [SerializeField] private float rotationSpeed = 540f;

    [Header("Stopping")]
    [SerializeField] private float stopDistance = 0.2f;

    private Vector3 moveDirection = Vector3.zero;
    private Camera mainCamera;
    private bool mouseControlsMovement;

    private void Awake()
    {
        mainCamera = Camera.main;
    }

    private void Update()
    {
        ReadMouseInput();
        RotateVirus();
    }

    private void FixedUpdate()
    {
        MoveVirus();
    }

    private void ReadMouseInput()
    {
        if (Mouse.current == null)
        {
            mouseControlsMovement = false;
            moveDirection = Vector3.zero;
            return;
        }

        if (Mouse.current.leftButton.wasPressedThisFrame)
        {
            mouseControlsMovement = true;
        }

        if (!Mouse.current.leftButton.isPressed)
        {
            mouseControlsMovement = false;
            moveDirection = Vector3.zero;
            return;
        }

        if (!mouseControlsMovement)
            return;

        if (mainCamera == null)
            mainCamera = Camera.main;

        if (mainCamera == null)
            return;

        Vector2 mousePosition = Mouse.current.position.ReadValue();

        Ray ray = mainCamera.ScreenPointToRay(mousePosition);
        Plane groundPlane = new Plane(Vector3.up, Vector3.zero);

        if (!groundPlane.Raycast(ray, out float distance))
        {
            moveDirection = Vector3.zero;
            return;
        }

        Vector3 targetPosition = ray.GetPoint(distance);
        Vector3 direction = targetPosition - transform.position;

        direction.y = 0f;

        if (direction.sqrMagnitude <= stopDistance * stopDistance)
        {
            moveDirection = Vector3.zero;
            return;
        }

        moveDirection = direction.normalized;
    }

    private void MoveVirus()
    {
        if (moveDirection.sqrMagnitude == 0f)
            return;

        Vector3 movement = moveDirection * moveSpeed * Time.fixedDeltaTime;

        transform.position += movement;

        // Keep the virus on the ground plane.
        Vector3 position = transform.position;
        position.y = 0f;
        transform.position = position;
    }

    private void RotateVirus()
    {
        if (moveDirection.sqrMagnitude < 0.0001f)
            return;

        Quaternion targetRotation = Quaternion.LookRotation(
            moveDirection,
            Vector3.up
        );

        transform.rotation = Quaternion.RotateTowards(
            transform.rotation,
            targetRotation,
            rotationSpeed * Time.deltaTime
        );
    }

    public void SetMoveDirection(Vector3 direction)
    {
        mouseControlsMovement = false;

        moveDirection = new Vector3(
            direction.x,
            0f,
            direction.z
        ).normalized;
    }
}

