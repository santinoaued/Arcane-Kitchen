using UnityEngine;

public class RangedEnemy : MonoBehaviour
{
    [Header("Player / Detección")]
    public string playerTag = "Player";
    public float shootRange = 18f;

    [Header("Disparo")]
    public GameObject projectilePrefab;
    public Transform firePoint;
    public float projectileSpeed = 10f;
    public float shootCooldown = 2.0f; // más lento por defecto

    [Header("Rotación")]
    public float rotationSpeed = 6f;

    private Transform player;
    private Animator animator;
    private float shootTimer = 0f;

    void Start()
    {
        animator = GetComponent<Animator>();
        var pgo = GameObject.FindGameObjectWithTag(playerTag);
        if (pgo != null) player = pgo.transform;
    }

    void Update()
    {
        if (player == null) return;

        // ROTAR visualmente en Y hacia el jugador (no se inclina en pitch)
        Vector3 toPlayer = player.position - transform.position;
        Vector3 toPlayerYaw = new Vector3(toPlayer.x, 0f, toPlayer.z); // solo horizontal
        if (toPlayerYaw.sqrMagnitude > 0.001f)
        {
            Quaternion targetRot = Quaternion.LookRotation(toPlayerYaw);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRot, rotationSpeed * Time.deltaTime);
        }

        // CONTROL DE DISPARO
        shootTimer -= Time.deltaTime;
        float dist = Vector3.Distance(transform.position, player.position);
        if (dist <= shootRange && shootTimer <= 0f)
        {
            if (animator != null) animator.SetTrigger("Attack");

            ShootAtPlayer();

            shootTimer = shootCooldown;
        }
    }

    void ShootAtPlayer()
    {
        if (projectilePrefab == null || firePoint == null || player == null) return;

        // Calcular dirección real tomando en cuenta la diferencia vertical (permite disparar hacia abajo)
        Vector3 direction = (player.position - firePoint.position).normalized;

        // Instanciar proyectil y darle la velocidad inicial en la dirección calculada
        GameObject proj = Instantiate(projectilePrefab, firePoint.position, Quaternion.LookRotation(direction));

        var bullet = proj.GetComponent<EnemyBullet>();
        if (bullet != null)
        {
            // Usamos el método Initialize para darle la velocidad hacia la posición del player en este instante
            bullet.Initialize(direction, projectileSpeed);
            return;
        }

        // Fallback si el prefab no tiene EnemyBullet: intentar Rigidbody
        var rb = proj.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.linearVelocity = direction * projectileSpeed;
            proj.transform.rotation = Quaternion.LookRotation(direction);
        }
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, shootRange);
        if (firePoint != null)
        {
            Gizmos.color = Color.magenta;
            Gizmos.DrawSphere(firePoint.position, 0.08f);
            Gizmos.DrawLine(transform.position, firePoint.position);
        }
    }
}
