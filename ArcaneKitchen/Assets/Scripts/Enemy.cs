using UnityEditor.VersionControl;
using UnityEngine;
using UnityEngine.ProBuilder.MeshOperations;

public class Enemy : MonoBehaviour
{
    [Header("Enemy stats")]

    [SerializeField] protected string name;
    // Stats b�sicos
    [SerializeField] protected int health;
    [SerializeField] protected float speed;
    // Combate
    [SerializeField] protected float attackDamage;
    [SerializeField] protected float attackRange;
    [SerializeField] protected float attackCooldown;
    [SerializeField] protected float sightRange;
    // Patrullaje
    [SerializeField] protected Transform[] patrolPoints;
    protected int currentPatrolIndex = 0;


    protected Transform target;
    protected bool isAttacking = false;
    protected bool canAttack = true;
    protected bool inSightRange = false;
    protected bool inAttackRange = false;

    protected Animator animator;
    protected Rigidbody rb;

    protected virtual void Start()
    {
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody>();
        target = GameObject.FindGameObjectWithTag("Player").transform;
        if (target == null)
        {
            Debug.LogError("No se encontr� el objeto con la etiqueta 'Player'. Aseg�rate de que exista en la escena.");
        }

        Debug.Log(name + "se est� moviendo.");
    }

    protected virtual void ChaseTarget()
    {
        Vector3 direction = (target.position - transform.position).normalized;
        rb.MovePosition(transform.position + direction * speed * Time.deltaTime);
        Quaternion lookRotation = Quaternion.LookRotation(direction);
        transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 5f);
        animator.SetBool("isWalking", true);
    }

    protected virtual void ResetAttack()
    {
        canAttack = true;
    }

    protected virtual void AttackTarget()
    {
        if (!canAttack) return;
        canAttack = false;
        Invoke(nameof(ResetAttack), attackCooldown);

        animator.SetTrigger("Attack");

        Collider[] hitPlayers = Physics.OverlapSphere(transform.position, attackRange);
        foreach (Collider hit in hitPlayers)
        {
            if (hit.transform == target)
            {
                PlayerHealth playerHealth = hit.GetComponent<PlayerHealth>();
                if (playerHealth != null)
                {
                    playerHealth.TakeDamage(attackDamage);
                }
            }
        }

    }

    protected virtual void Patrol()
    {
        if (patrolPoints.Length == 0) return;
        Transform patrolPoint = patrolPoints[currentPatrolIndex];
        Vector3 direction = (patrolPoint.position - transform.position).normalized;

        rb.MovePosition(transform.position + direction * speed * Time.deltaTime);

        Quaternion lookRotation = Quaternion.LookRotation(direction);
        transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 5f);

        animator.SetBool("isWalking", true);

        float distanceToPoint = Vector3.Distance(transform.position, patrolPoint.position);
        if (distanceToPoint < 1f)
        {
            currentPatrolIndex = (currentPatrolIndex + 1) % patrolPoints.Length;
        }

    }

    protected virtual void Idle()
    {
        animator.SetBool("isWalking", false);
        rb.linearVelocity = Vector3.zero; // Por si hay movimiento residual
    }


    protected virtual void Update()
    {
        if (target == null) return;

        float distanceToTarget = Vector3.Distance(transform.position, target.position);
        inSightRange = distanceToTarget <= sightRange; // Rango de visi�n
        inAttackRange = distanceToTarget <= attackRange; // Rango de ataque

        if (inSightRange && !inAttackRange)
        {
            ChaseTarget();
        }
        else if (inSightRange && inAttackRange)
        {
            AttackTarget();
        }
        else
        {
            Patrol();
        }

    }


}

    
