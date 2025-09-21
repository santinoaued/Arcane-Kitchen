using UnityEngine;

public class EnemyBullet : MonoBehaviour
{
    [Header("Projectile Settings")]
    [SerializeField] private float speed = 20f;       // Velocidad del proyectil
    [SerializeField] private float damage = 10f;      // Daño que inflige el proyectil
    [SerializeField] private float lifetime = 5f;     // Tiempo de vida del proyectil en segundos

    private Rigidbody rb;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        if (rb == null)
        {
            Debug.LogError("EnemyBullet requiere un Rigidbody en el mismo GameObject.", this);
            enabled = false;
            return;
        }
        rb.isKinematic = false;
        rb.useGravity = false;
        // Opcional: Si la bala tiene rotación por alguna razón, puedes congelarla para un movimiento recto.
        // rb.freezeRotation = true;
    }

    void Start()
    {
        // Aplicamos la velocidad solo una vez en Start.
        rb.linearVelocity = transform.forward * speed;

        // Destruye el proyectil después de su tiempo de vida
        Destroy(gameObject, lifetime);
    }

    void OnTriggerEnter(Collider other)
    {
        HandleCollision(other.gameObject);
    }

    void OnCollisionEnter(Collision collision)
    {
        HandleCollision(collision.gameObject);
    }

    private void HandleCollision(GameObject collidedObject)
    {
        if (collidedObject.CompareTag("Player"))
        {
            Debug.Log("Proyectil golpeó al jugador! Daño: " + damage);
            // Lógica para aplicar daño al jugador
        }
        Destroy(gameObject);
    }
}