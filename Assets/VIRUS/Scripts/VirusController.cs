
using UnityEngine;
using UnityEngine.InputSystem;

public class VirusController : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float moveSpeed = 5f;

    [Header("Stopping")]
    [SerializeField] private float stopDistance = 0.2f;

    private Vector3 moveDirection;
    private Camera mainCamera;
    private bool mouseControlsMovement;

    private void Awake()
    {
        mainCamera = Camera.main;
    }

    private void Update()
    {
        ReadMouseInput();
        MoveVirus();
    }

    private void ReadMouseInput()
    {
        if (Mouse.current == null)
        {
            mouseControlsMovement = false;
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

        // Stop before reaching the cursor to prevent jitter.
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

        transform.position += moveDirection * moveSpeed * Time.deltaTime;
    }

    public void SetMoveDirection(Vector3 direction)
    {
        mouseControlsMovement = false;
        moveDirection = new Vector3(direction.x, 0f, direction.z).normalized;
    }
}
