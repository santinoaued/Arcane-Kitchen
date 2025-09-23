using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    public int maxHealth = 3;       // cantidad máxima de vida
    private int currentHealth;

    public GameObject loseCanvas;   // arrastrar el Canvas de perder desde el Inspector
    public float invulnerableTime = 1f; // tiempo de espera después de recibir daño
    private float lastDamageTime;       // guarda el momento del último daño
    void Start()
    {
        currentHealth = maxHealth;

        if (loseCanvas != null)
            loseCanvas.SetActive(false); // ocultamos el menú al inicio
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Enemigo"))
        {
            TakeDamage(1);
        }
    }

    public void TakeDamage(int amount)
    {
        currentHealth -= amount;
        Debug.Log("Vida actual: " + currentHealth);

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        Debug.Log("¡El jugador murió!");
        Time.timeScale = 0f; // pausamos el juego
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;

        if (loseCanvas != null)
            loseCanvas.SetActive(true);
    }

    public void Heal(int amount)
    {
        currentHealth += amount;
        if (currentHealth > maxHealth)
            currentHealth = maxHealth;

        Debug.Log("Curado. Vida actual: " + currentHealth);
    }

    public int GetCurrentHealth()
    {
        return currentHealth;
    }
}
