using UnityEngine;
using UnityEngine.ProBuilder;

public class RangedEnemy : MonoBehaviour
{
    [Header("Player / Detecci�n")]
    public string playerTag = "Player";
    public float shootRange = 12f;

    [Header("Disparo")]
    public GameObject projectilePrefab; 
    public Transform firePoint;        
    public float projectileSpeed = 12f;
    public float shootCooldown = 1.2f;

    [Header("Rotaci�n")]
    public float rotationSpeed = 8f; 

    private Transform player;
    private Animator animator;
    private float shootTimer = 0f;

    void Start()
    {
        animator = GetComponent<Animator>();
        GameObject pgo = GameObject.FindGameObjectWithTag(playerTag);
        if (pgo != null) player = pgo.transform;

        
    }

    void Update()
    {
        if (player == null) return;

        
        Vector3 toPlayer = player.position - transform.position;
        toPlayer.y = 0f;
        if (toPlayer.sqrMagnitude > 0.001f)
        {
            Quaternion targetRot = Quaternion.LookRotation(toPlayer);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRot, rotationSpeed * Time.deltaTime);
        }

        
        shootTimer -= Time.deltaTime;
        float dist = Vector3.Distance(transform.position, player.position);
        if (dist <= shootRange && shootTimer <= 0f)
        {
            
            if (animator != null) animator.SetTrigger("Attack");

            
            Shoot();

            shootTimer = shootCooldown;
        }
    }

    void Shoot()
    {
        if (projectilePrefab == null || firePoint == null) return;

        GameObject proj = Instantiate(projectilePrefab, firePoint.position, firePoint.rotation);
               
        var rb = proj.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.linearVelocity = firePoint.forward * projectileSpeed;
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
