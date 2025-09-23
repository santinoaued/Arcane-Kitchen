using UnityEngine;
using UnityEngine.UI;


public class HealthUI : MonoBehaviour
{
    public PlayerHealth playerHealth; // referencia al script de vida
    public Image[] hearts;            // arrastrá tus imágenes de corazones desde el Canvas

    void Update()
    {
        UpdateHearts();
    }

    void UpdateHearts()
    {
        int health = playerHealth.GetCurrentHealth();

        // Recorremos todos los corazones
        for (int i = 0; i < hearts.Length; i++)
        {
            if (i < health)
                hearts[i].enabled = true;   // se ve el corazón
            else
                hearts[i].enabled = false;  // corazón apagado
        }
    }
}
