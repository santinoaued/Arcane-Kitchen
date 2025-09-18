using UnityEngine;

public class Enemy : MonoBehaviour
{
    [Header("Enemy Stats")]
    // Stats basicos
    [SerializeField] protected int health = 100; 
    [SerializeField] protected float speed = 3f;
    // Combate
    [SerializeField] protected float attackDamage = 10f;
    [SerializeField] protected float attackRange = 2f;
    [SerializeField] protected float attackCooldown = 1.5f;
    [SerializeField] protected float sightRange = 10f;

    [Header("Patrol Settings")]
    [SerializeField] protected Transform[] patrolPoints;
    protected int currentPatrolIndex = 0;
    [SerializeField] protected float patrolPointReachedDistance = 0.5f; 
    [SerializeField] protected float waitTimeAtPatrolPoint = 1.0f; 

    [Header("Movimiento")]
    [SerializeField] protected float rotationSpeed = 5f; 

    
    private float _currentWaitTime = 0f;
    private bool _isWaiting = false;

    // Estado del enemigo
    protected Transform target; 
    protected bool isAttacking = false;
    protected bool canAttack = true;
    protected bool inSightRange = false;
    protected bool inAttackRange = false;

    // Referencias a componentes
    protected Animator animator;
    protected Rigidbody rb;

    protected virtual void Start()
    {
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody>();

        if (rb == null)
        {
            Debug.LogError("Rigidbody no encontrado en " + gameObject.name + ". Necesario para el movimiento del enemigo.", this);
            enabled = false; 
            return;
        }
        if (animator == null)
        {
            Debug.LogWarning("Animator no encontrado en " + gameObject.name + ". Las animaciones no funcionarán.", this);
        }

        GameObject playerGameObject = GameObject.FindGameObjectWithTag("Player");

        if (playerGameObject != null)
        {
            target = playerGameObject.transform;
        }
        else
        {
            Debug.LogError("No se encontró el GameObject con la etiqueta 'Player'. El enemigo patrullará sin perseguir/atacar.", this);
         
        }

        if (animator != null)
        {
            animator.SetBool("isIdle", true);
            animator.SetBool("isWalking", false);
        }
    }

    protected virtual void ChaseTarget()
    {
        if (target == null)
        {
            Patrol(); 
            return;
        }

        Vector3 direction = (target.position - transform.position).normalized;
        
        direction.y = 0;

        if (direction.magnitude > 0.01f) 
        {
            rb.MovePosition(transform.position + direction * speed * Time.deltaTime);
        }
        else
        {
            rb.linearVelocity = Vector3.zero; // detener si no hay dirección a la que ir
        }

        if (direction != Vector3.zero)
        {
            Quaternion lookRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * rotationSpeed); // usar rotationSpeed
        }

        if (animator != null)
        {
            animator.SetBool("isWalking", true);
            animator.SetBool("isIdle", false);
        }
    }

    protected virtual void ResetAttack()
    {
        canAttack = true;
    }

    protected virtual void AttackTarget()
    {
        if (!canAttack) return; // si no puede atacar (cooldown), no hace nada

        canAttack = false;
        Invoke(nameof(ResetAttack), attackCooldown);

        if (animator != null)
        {
            animator.SetTrigger("Attack");
            animator.SetBool("isWalking", false); // detener caminar durante el ataque
            animator.SetBool("isIdle", false);    // detener idle durante el ataque
        }

        
        rb.linearVelocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;

        
        if (target != null)
        {
            Vector3 directionToTarget = (target.position - transform.position).normalized;
            directionToTarget.y = 0; // solo rotar en el eje Y
            if (directionToTarget != Vector3.zero)
            {
                Quaternion lookRotation = Quaternion.LookRotation(directionToTarget);
                transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * rotationSpeed * 2f); 
            }
        }
    }

    protected virtual void Patrol()
    {
        if (patrolPoints == null || patrolPoints.Length == 0)
        {
            Idle();
            return;
        }

        
        if (_isWaiting)
        {
            _currentWaitTime -= Time.deltaTime;
            Idle(); 
            if (_currentWaitTime <= 0)
            {
                _isWaiting = false; 
                currentPatrolIndex = (currentPatrolIndex + 1) % patrolPoints.Length; 
                
            }
            return; 
        }
        


        Transform patrolPoint = patrolPoints[currentPatrolIndex];
        Vector3 direction = (patrolPoint.position - transform.position).normalized;
        direction.y = 0; 

        float distanceToPoint = Vector3.Distance(transform.position, patrolPoint.position);

        if (distanceToPoint > patrolPointReachedDistance)
        {
            rb.MovePosition(transform.position + direction * speed * Time.deltaTime);

            if (direction != Vector3.zero) 
            {
                Quaternion lookRotation = Quaternion.LookRotation(direction);
                transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * rotationSpeed);
            }
            if (animator != null)
            {
                animator.SetBool("isWalking", true);
                animator.SetBool("isIdle", false);
            }
        }
        else 
        {
            _isWaiting = true;
            _currentWaitTime = waitTimeAtPatrolPoint;
            Idle(); 
        }
    }

    protected virtual void Idle()
    {
        if (animator != null)
        {
            animator.SetBool("isWalking", false);
            animator.SetBool("isIdle", true); 
        }
        rb.linearVelocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
    }


    protected virtual void FixedUpdate()
    {
        if (rb == null) return;

        
        if (target == null)
        {
            inSightRange = false;
            inAttackRange = false;
        }
        else
        {
            
            float distanceToTarget = Vector3.Distance(transform.position, target.position);
            inSightRange = distanceToTarget <= sightRange;
            inAttackRange = distanceToTarget <= attackRange;
        }

        
        if (target != null && inSightRange && inAttackRange)
        {
            AttackTarget();
        }
        else if (target != null && inSightRange && !inAttackRange)
        {
            ChaseTarget();
        }
        else
        {
            
            Patrol(); 
        }
    }

    protected virtual void OnDrawGizmos()
    {
        
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);

        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, sightRange);

        
        if (target != null && inSightRange)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawLine(transform.position, target.position);
        }

        
        if (patrolPoints != null && patrolPoints.Length > 0)
        {
            Gizmos.color = Color.green;
            for (int i = 0; i < patrolPoints.Length; i++)
            {
                if (patrolPoints[i] != null)
                {
                    Gizmos.DrawSphere(patrolPoints[i].position, 0.3f);
                    if (i < patrolPoints.Length - 1)
                    {
                        if (patrolPoints[i + 1] != null) Gizmos.DrawLine(patrolPoints[i].position, patrolPoints[i + 1].position);
                    }
                    else if (patrolPoints.Length > 1 && patrolPoints[0] != null) // 
                    {
                        Gizmos.DrawLine(patrolPoints[i].position, patrolPoints[0].position);
                    }
                }
            }
            
            if (patrolPoints[currentPatrolIndex] != null)
            {
                Gizmos.color = Color.magenta;
                Gizmos.DrawWireSphere(patrolPoints[currentPatrolIndex].position, 0.5f);
            }
        }
    }
}