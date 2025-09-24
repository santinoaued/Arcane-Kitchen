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

        attackTimer -= Time.deltaTime; // Reduce el cooldown del ataque

        // Si el jugador est� dentro del rango de detecci�n
        if (player != null && Vector3.Distance(transform.position, player.position) <= detectionRange)
        {
            float distToPlayer = Vector3.Distance(transform.position, player.position);

            // Si el jugador est� dentro del rango de ataque
            if (distToPlayer <= attackRange)
            {
                FaceTarget(player.position); // Mira al jugador
                SetWalking(false); // Deja de caminar

                // Si el cooldown ha terminado, ataca
                if (attackTimer <= 0f)
                {
                    // ***** L�gica de Da�o AHORA *****
                    var ph = player.GetComponent<playerSanityHealth>();
                    if (ph != null)
                    {
                        ph.RecibirDanioCordura(attackDamage);
                        Debug.Log("�El enemigo ha da�ado al jugador!"); // Debug para ver si se ejecuta el da�o
                    }
                    else
                    {
                        Debug.LogWarning("No se encontr� el componente playerSanityHealth en el jugador.");
                    }

                    // Dispara la animaci�n de ataque (solo visual)
                    animator.SetTrigger("Attack");
                    attackTimer = attackCooldown; // Reinicia el cooldown
                }
                return; // Ya hemos decidido atacar o esperar al cooldown, as� que salimos del Update
            }
            else // Si el jugador est� dentro del rango de detecci�n pero fuera del de ataque, se mueve hacia �l
            {
                MoveTowards(player.position);
                return;
            }
        }

        // L�gica de patrulla (solo si no hay jugador cerca)
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
        dir.y = 0f; // Aseg�rate de que el movimiento sea solo en el plano XZ
        if (dir.sqrMagnitude < 0.0001f) // Evita divisiones por cero con vectores muy peque�os
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
        dir.y = 0f; // Solo rota en el eje Y
        if (dir.sqrMagnitude > 0.0001f) // Evita rotaciones err�ticas con vectores muy peque�os
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


    // El m�todo OnAttackHit() ha sido eliminado.
    // La l�gica de da�o ahora est� directamente en Update() cuando se cumple la condici�n de ataque.


    public void TakeDamage(int dmg)
    {
        if (isDead) return;
        currentHealth -= dmg;
        Debug.Log($"[PatrolEnemy] Da�o recibido: {dmg}. Vida actual: {currentHealth}");
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