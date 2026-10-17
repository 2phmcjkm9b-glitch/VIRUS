
using UnityEngine;

public class VirusController : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float moveSpeed = 5f;

    private Vector3 moveDirection;

    private void Update()
    {
        MoveVirus();
    }

    private void MoveVirus()
    {
        transform.position += moveDirection * moveSpeed * Time.deltaTime;
    }

    public void SetMoveDirection(Vector3 direction)
    {
        moveDirection = direction.normalized;
    }
}
