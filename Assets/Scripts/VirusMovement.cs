using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// Lets the virus follow the player's finger on a touchscreen,
/// or the mouse while testing in the Unity Editor.
/// Attach this component to the VirusBody GameObject.
/// </summary>
[RequireComponent(typeof(SpriteRenderer))]
public sealed class VirusMovement : MonoBehaviour
{
    [SerializeField, Min(0f)]
    private float followSpeed = 12f;

    private Camera mainCamera;
    private float objectZ;
    private bool isDragging;

    private void Awake()
    {
        mainCamera = Camera.main;
        objectZ = transform.position.z;
    }

    private void Update()
    {
        if (mainCamera == null)
        {
            mainCamera = Camera.main;
            if (mainCamera == null) return;
        }

        Vector2? pointerPosition = GetPointerPosition();

        if (WasPointerPressed())
        {
            isDragging = true;
        }

        if (WasPointerReleased())
        {
            isDragging = false;
        }

        if (isDragging && pointerPosition.HasValue)
        {
            Vector3 screenPosition = new Vector3(
                pointerPosition.Value.x,
                pointerPosition.Value.y,
                Mathf.Abs(mainCamera.transform.position.z - objectZ));

            Vector3 targetPosition = mainCamera.ScreenToWorldPoint(screenPosition);
            targetPosition.z = objectZ;

            transform.position = Vector3.Lerp(
                transform.position,
                targetPosition,
                followSpeed * Time.deltaTime);
        }
    }

    private static Vector2? GetPointerPosition()
    {
        if (Touchscreen.current != null && Touchscreen.current.primaryTouch.press.isPressed)
            return Touchscreen.current.primaryTouch.position.ReadValue();

        if (Mouse.current != null && Mouse.current.leftButton.isPressed)
            return Mouse.current.position.ReadValue();

        // Return the current pointer position while dragging.
        if (Touchscreen.current != null)
            return Touchscreen.current.primaryTouch.position.ReadValue();

        if (Mouse.current != null)
            return Mouse.current.position.ReadValue();

        return null;
    }

    private static bool WasPointerPressed()
    {
        bool touchPressed = Touchscreen.current != null &&
                            Touchscreen.current.primaryTouch.press.wasPressedThisFrame;
        bool mousePressed = Mouse.current != null &&
                            Mouse.current.leftButton.wasPressedThisFrame;

        return touchPressed || mousePressed;
    }

    private static bool WasPointerReleased()
    {
        bool touchReleased = Touchscreen.current != null &&
                             Touchscreen.current.primaryTouch.press.wasReleasedThisFrame;
        bool mouseReleased = Mouse.current != null &&
                             Mouse.current.leftButton.wasReleasedThisFrame;

        return touchReleased || mouseReleased;
    }
}
