
using UnityEngine;
using TMPro;

public class ParticleCounter : MonoBehaviour
{
    [Header("Counter Settings")]
    [SerializeField] private TMP_Text counterText;

    private int collectedParticles = 0;

    private void Start()
    {
        UpdateCounterText();
    }

    public void CollectParticle()
    {
        collectedParticles++;
        UpdateCounterText();
    }

    private void UpdateCounterText()
    {
        if (counterText != null)
        {
            counterText.text = "Частицы: " + collectedParticles;
        }
    }
}

