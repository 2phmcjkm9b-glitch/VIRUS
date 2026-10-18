using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(SpriteRenderer))]
public class VirusMovement : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float followSpeed = 12f;

    private Camera mainCamera;
    private SpriteRenderer spriteRenderer;

    private void Awake()
    {
        mainCamera = Camera.main;
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void Update()
    {
        if (mainCamera == null)
            mainCamera = Camera.main;

        if (mainCamera == null)
            return;

        Vector2 pointerPosition;

        if (Touchscreen.current != null &&
            Touchscreen.current.primaryTouch.press.isPressed)
        {
            pointerPosition = Touchscreen.current.primaryTouch.position.ReadValue();
        }
        else if (Mouse.current != null && Mouse.current.leftButton.isPressed)
        {
            pointerPosition = Mouse.current.position.ReadValue();
        }
        else
        {
            return;
        }

        float distanceToCamera = Mathf.Abs(mainCamera.transform.position.z - transform.position.z);
        Vector3 screenPosition = new Vector3(pointerPosition.x, pointerPosition.y, distanceToCamera);
        Vector3 targetPosition = mainCamera.ScreenToWorldPoint(screenPosition);
        targetPosition.z = transform.position.z;

        Vector3 bottomLeft = mainCamera.ViewportToWorldPoint(new Vector3(0f, 0f, distanceToCamera));
        Vector3 topRight = mainCamera.ViewportToWorldPoint(new Vector3(1f, 1f, distanceToCamera));

        // Use a smaller safety margin because the sprite image has transparent padding.
        Vector3 spriteExtents = spriteRenderer.bounds.extents * 0.65f;

        targetPosition.x = Mathf.Clamp(
            targetPosition.x,
            bottomLeft.x + spriteExtents.x,
            topRight.x - spriteExtents.x
        );
        targetPosition.y = Mathf.Clamp(
            targetPosition.y,
            bottomLeft.y + spriteExtents.y,
            topRight.y - spriteExtents.y
        );

        transform.position = Vector3.MoveTowards(
            transform.position,
            targetPosition,
            followSpeed * Time.deltaTime
        );
    }
}