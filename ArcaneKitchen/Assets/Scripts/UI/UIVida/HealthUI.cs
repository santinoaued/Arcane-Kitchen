using UnityEngine;
using UnityEngine.UI;


public class HealthUI : MonoBehaviour
{
    public PlayerHealth playerHealth; // referencia al script de vida
    public Image[] hearts;            // arrastr� tus im�genes de corazones desde el Canvas

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
                hearts[i].enabled = true;   // se ve el coraz�n
            else
                hearts[i].enabled = false;  // coraz�n apagado
        }
    }
}
