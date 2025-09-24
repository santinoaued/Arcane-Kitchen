using UnityEngine;

[RequireComponent(typeof(Rigidbody), typeof(Collider))]
public class EnemyBullet : MonoBehaviour
{
    [Header("Projectile Settings")]
    [SerializeField] private float speed = 12f;       // Velocidad del proyectil
    [SerializeField] private float damage = 10f;      // Daño que inflige el proyectil (se castea a int)
    [SerializeField] private float lifetime = 6f;     // Tiempo de vida del proyectil en segundos

    Rigidbody rb;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        if (rb == null)
        {
            Debug.LogError("EnemyBullet requiere un Rigidbody en el mismo GameObject.", this);
            enabled = false;
            return;
        }

        // Configuración física por defecto: no seguir la gravedad para trayecto recto
        rb.isKinematic = false;
        rb.useGravity = false;

        // Asegurar que el collider está configurado para trigger si queremos OnTriggerEnter
        var col = GetComponent<Collider>();
        if (col != null)
        {
            // opcional: preferible isTrigger = true para proyectiles que no rebotan
            // col.isTrigger = true;
        }
    }

    void Start()
    {
        // Destruye el proyectil después de su tiempo de vida
        Destroy(gameObject, lifetime);
    }

    /// <summary>
    /// Inicializa la bala con una velocidad en una dirección dada (llamar desde el spawner).
    /// </summary>
    public void Initialize(Vector3 direction, float initialSpeed)
    {
        if (rb != null)
        {
            rb.linearVelocity = direction.normalized * initialSpeed;
            // Orientar la bala hacia la dirección (opcional)
            transform.rotation = Quaternion.LookRotation(direction.normalized);
        }
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
            // Intentamos con tu sistema de cordura/vida
            var sanity = collidedObject.GetComponent<playerSanityHealth>();
            if (sanity != null)
            {
                sanity.RecibirDanioCordura((int)damage);
            }
            
        }

        // Destruir la bala al colisionar con algo (evita múltiples impactos)
        Destroy(gameObject);
    }
}
