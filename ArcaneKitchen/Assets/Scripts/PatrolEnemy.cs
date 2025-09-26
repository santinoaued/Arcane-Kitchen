using UnityEngine;
using System.Collections;

public class PatrolEnemy : MonoBehaviour
{
    [Header("Patrol")]
    public Transform[] waypoints;
    public float moveSpeed = 2.5f;
    public float waypointTolerance = 0.2f;
    public float waitAtWaypoint = 1f;

    [Header("Detection / Attack")]
    public float detectionRange = 10f;
    public float attackRange = 2f;
    public float attackCooldown = 1.5f;
    public string playerTag = "Player";

    [Header("Damage")]
    public int attackDamage = 10;

    [Header("Health")]
    public int maxHealth = 50;

    private int currentWaypoint = 0;
    private float waitTimer = 0f;
    private float attackTimer = 0f;
    private Transform player;
    private Animator animator;
    private int currentHealth;
    private bool isDead = false;

    void Start()
    {
        animator = GetComponent<Animator>();
        GameObject p = GameObject.FindGameObjectWithTag(playerTag);
        if (p) player = p.transform;
        if (waypoints == null || waypoints.Length == 0) waypoints = new Transform[] { transform };
        currentHealth = maxHealth;
        SetWalking(false);
    }

    void Update()
    {
        if (isDead) return;

        attackTimer -= Time.deltaTime; 

        if (player != null && Vector3.Distance(transform.position, player.position) <= detectionRange)
        {
            float distToPlayer = Vector3.Distance(transform.position, player.position);

            
            if (distToPlayer <= attackRange)
            {
                FaceTarget(player.position); 
                SetWalking(false); 
                
                if (attackTimer <= 0f)
                {
                    
                    var ph = player.GetComponent<playerSanityHealth>();
                    if (ph != null)
                    {
                        ph.RecibirDanioCordura(attackDamage);
                        Debug.Log("¡El enemigo ha dañado al jugador!"); 
                    }
                    else
                    {
                        Debug.LogWarning("No se encontró el componente playerSanityHealth en el jugador.");
                    }

                    
                    animator.SetTrigger("Attack");
                    attackTimer = attackCooldown; 
                }
                return; 
            }
            else 
            {
                MoveTowards(player.position);
                return;
            }
        }

        
        if (waypoints == null || waypoints.Length == 0) return;

        Transform wp = waypoints[currentWaypoint];
        float dist = Vector3.Distance(transform.position, wp.position);
        if (dist <= waypointTolerance)
        {
            if (waitTimer <= 0f)
                waitTimer = waitAtWaypoint;
            else
            {
                waitTimer -= Time.deltaTime;
                SetWalking(false);
                if (waitTimer <= 0f)
                {
                    currentWaypoint = (currentWaypoint + 1) % waypoints.Length;
                }
            }
        }
        else
        {
            MoveTowards(wp.position);
        }
    }


    void MoveTowards(Vector3 target)
    {
        Vector3 dir = target - transform.position;
        dir.y = 0f; 
        if (dir.sqrMagnitude < 0.0001f) 
        {
            SetWalking(false);
            return;
        }

        Vector3 move = dir.normalized * moveSpeed * Time.deltaTime;
        transform.position += move;
        FaceTarget(target);
        SetWalking(true);
    }


    void FaceTarget(Vector3 target)
    {
        Vector3 dir = target - transform.position;
        dir.y = 0f; 
        if (dir.sqrMagnitude > 0.0001f) 
        {
            Quaternion targetRot = Quaternion.LookRotation(dir);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRot, 10f * Time.deltaTime);
        }
    }

    void SetWalking(bool val)
    {
        if (animator != null)
            animator.SetBool("isWalking", val);
    }




    public void TakeDamage(int dmg)
    {
        if (isDead) return;
        currentHealth -= dmg;
        Debug.Log($"[PatrolEnemy] Daño recibido: {dmg}. Vida actual: {currentHealth}");
        if (currentHealth <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        if (isDead) return;
        isDead = true;
        SetWalking(false);
        if (animator != null) animator.SetTrigger("Die");


        Collider c = GetComponent<Collider>();
        if (c != null) c.enabled = false;


        Destroy(gameObject, 3f);
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRange);
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);

        if (waypoints != null && waypoints.Length > 0)
        {
            Gizmos.color = Color.cyan;
            for (int i = 0; i < waypoints.Length; i++)
            {
                if (waypoints[i] != null)
                {
                    Gizmos.DrawSphere(waypoints[i].position, 0.12f);
                    if (i < waypoints.Length - 1)
                        Gizmos.DrawLine(waypoints[i].position, waypoints[i + 1].position);
                    else
                        Gizmos.DrawLine(waypoints[i].position, waypoints[0].position);
                }
            }
        }
    }
}