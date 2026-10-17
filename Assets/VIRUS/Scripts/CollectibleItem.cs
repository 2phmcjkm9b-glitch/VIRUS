
using UnityEngine;

public class CollectibleItem : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        // Проверяем, столкнулся ли предмет с нашим вирусом.
        if (other.GetComponent<VirusController>() == null)
        {
            return;
        }

        // Находим счётчик частиц на сцене.
        ParticleCounter counter = FindAnyObjectByType<ParticleCounter>();

        if (counter != null)
        {
            // Увеличиваем счётчик на одну частицу.
            counter.CollectParticle();
        }
        else
        {
            Debug.LogWarning("ParticleCounter не найден на сцене!");
        }

        // Удаляем собранную частицу.
        Destroy(gameObject);

        Debug.Log("Частица собрана!");
    }
}
