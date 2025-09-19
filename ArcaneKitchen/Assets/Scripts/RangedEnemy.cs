using UnityEngine;

public class RangedEnemy : Enemy
{
    [Header("Ranged Settings")]
    [SerializeField] private Transform firePoint; // desde dónde dispara
    [SerializeField] private GameObject projectilePrefab; // asignar en inspector
    [SerializeField] private float projectileSpeed = 10f; // configurable
    [SerializeField] private bool alwaysFacePlayer = true; // girar hacia el player aunque esté quieto

    protected override void FixedUpdate()
    {
        if (rb == null || target == null) return;

        float distanceToTarget = Vector3.Distance(transform.position, target.position);
        inSightRange = distanceToTarget <= sightRange;
        inAttackRange = distanceToTarget <= attackRange;

        if (alwaysFacePlayer && target != null)
        {
            Vector3 directionToTarget = (target.position - transform.position).normalized;
            directionToTarget.y = 0;
            if (directionToTarget != Vector3.zero)
            {
                Quaternion lookRotation = Quaternion.LookRotation(directionToTarget);
                transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * rotationSpeed);
            }
        }

        if (inSightRange && inAttackRange)
        {
            AttackTarget();
        }
        else
        {
            Idle(); // se queda quieto
        }
    }

    protected override void AttackTarget()
    {
        if (!canAttack) return;

        canAttack = false;
        Invoke(nameof(ResetAttack), attackCooldown);

        if (animator != null)
        {
            animator.SetTrigger("Attack");
            animator.SetBool("isWalking", false);
            animator.SetBool("isIdle", false);
        }

        ShootProjectile(); // método que implementaremos después
    }

    /// <summary>
    /// Método para disparar un proyectil. Por ahora vacío, se implementará después.
    /// </summary>
    protected virtual void ShootProjectile()
    {
        // Aquí iría la lógica de instanciar el proyectil y asignarle dirección
        // Ejemplo futuro:
        // GameObject proj = Instantiate(projectilePrefab, firePoint.position, firePoint.rotation);
        // proj.GetComponent<Rigidbody>().velocity = firePoint.forward * projectileSpeed;
    }

    protected override void ChaseTarget() { /* no se mueve */ }
    protected override void Patrol() { /* no patrulla */ }

    protected override void OnDrawGizmos()
    {
        base.OnDrawGizmos();

        // Dibujar el punto de disparo
        if (firePoint != null)
        {
            Gizmos.color = Color.cyan;
            Gizmos.DrawSphere(firePoint.position, 0.2f);
        }
    }
}
