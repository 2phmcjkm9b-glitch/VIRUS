
using UnityEngine;

public class VirusPulse : MonoBehaviour
{
    [Header("Настройки пульсации")]
    [SerializeField] private float pulseSpeed = 2f;
    [SerializeField] private float pulseAmount = 0.04f;

    private Vector3 startScale;

    private void Start()
    {
        startScale = transform.localScale;
    }

    private void Update()
    {
        float pulse = 1f + Mathf.Sin(Time.time * pulseSpeed) * pulseAmount;

        transform.localScale = startScale * pulse;
    }
}
