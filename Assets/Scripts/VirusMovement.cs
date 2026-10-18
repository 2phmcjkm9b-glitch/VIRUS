using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(SpriteRenderer))]
public class VirusMovement : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float followSpeed = 12f;

    private Camera mainCamera;
    private bool isFollowing;

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

        Vector3 pointerPosition = Vector3.zero;

        if (Touchscreen.current != null &&
            Touchscreen.current.primaryTouch.press.isPressed)
        {
            pointerPosition = Touchscreen.current.primaryTouch.position.ReadValue();
            isFollowing = true;
        }
        else if (Mouse.current != null &&
                 Mouse.current.leftButton.isPressed)
        {
            pointerPosition = Mouse.current.position.ReadValue();
            isFollowing = true;
        }
        else
        {
            isFollowing = false;
        }

        if (!isFollowing)
            return;

        Vector3 targetPosition = mainCamera.ScreenToWorldPoint(
            new Vector3(pointerPosition.x, pointerPosition.y,
                Mathf.Abs(mainCamera.transform.position.z - transform.position.z))
        );

        targetPosition.z = transform.position.z;

        transform.position = Vector3.MoveTowards(
            transform.position,
            targetPosition,
            followSpeed * Time.deltaTime
        );
    }
}
