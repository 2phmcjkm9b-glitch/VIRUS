using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(SpriteRenderer))]
public class VirusMovement : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float followSpeed = 12f;

    private Camera mainCamera;

    private void Awake()
    {
        mainCamera = Camera.main;
    }

    private void Update()
    {
        if (mainCamera == null)
            mainCamera = Camera.main;

        if (mainCamera == null)
            return;

        Vector2 pointerPosition;
        bool isPressed = false;

        if (Touchscreen.current != null &&
            Touchscreen.current.primaryTouch.press.isPressed)
        {
            pointerPosition = Touchscreen.current.primaryTouch.position.ReadValue();
            isPressed = true;
        }
        else if (Mouse.current != null && Mouse.current.leftButton.isPressed)
        {
            pointerPosition = Mouse.current.position.ReadValue();
            isPressed = true;
        }
        else
        {
            return;
        }

        if (!isPressed)
            return;

        float distanceToCamera = Mathf.Abs(mainCamera.transform.position.z - transform.position.z);
        Vector3 screenPosition = new Vector3(pointerPosition.x, pointerPosition.y, distanceToCamera);
        Vector3 targetPosition = mainCamera.ScreenToWorldPoint(screenPosition);
        targetPosition.z = transform.position.z;

        transform.position = Vector3.MoveTowards(
            transform.position,
            targetPosition,
            followSpeed * Time.deltaTime
        );
    }
}