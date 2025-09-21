using UnityEngine;

public class RangedEnemy : Enemy
{
    [Header("Ranged Enemy Settings")]
    [SerializeField] private GameObject projectilePrefab;
    [SerializeField] private Transform firePoint;

    // Añadido para depuración
    [SerializeField] private bool debugLogsEnabled = true;

    protected override void Start()
    {
        base.Start();

        if (rb != null)
        {
            rb.isKinematic = true;
            rb.useGravity = false;
            rb.freezeRotation = true; // Asegura que el Rigidbody no interfiera con la rotación manual
        }

        if (animator != null)
        {
            animator.SetBool("isIdle", true);
            animator.SetBool("isWalking", false);
            if (debugLogsEnabled) Debug.Log($"{gameObject.name}: Animator inicializado. isIdle = true.");
        }
        else
        {
            Debug.LogError($"{gameObject.name}: Animator no encontrado. Las animaciones no funcionarán.", this);
        }

        if (projectilePrefab == null)
        {
            Debug.LogError($"{gameObject.name}: Projectile Prefab no asignado.", this);
        }
        if (firePoint == null)
        {
            Debug.LogError($"{gameObject.name}: Fire Point no asignado. Asigna un GameObject hijo para el punto de disparo.", this);
            GameObject defaultFirePoint = new GameObject("DefaultFirePoint");
            defaultFirePoint.transform.parent = transform;
            defaultFirePoint.transform.localPosition = new Vector3(0, 1, 0.5f);
            firePoint = defaultFirePoint.transform;
            Debug.LogWarning($"{gameObject.name}: Creando Fire Point por defecto. Considere asignarlo manualmente.", this);
        }
    }

    protected override void FixedUpdate()
    {
        // 1. Reintento de encontrar el Player si no se ha asignado o se perdió
        if (target == null)
        {
            GameObject playerGameObject = GameObject.FindGameObjectWithTag("Player");
            if (playerGameObject != null)
            {
                target = playerGameObject.transform;
                if (debugLogsEnabled) Debug.Log($"{gameObject.name}: Player encontrado: {target.name}");
            }
            else
            {
                inSightRange = false;
                inAttackRange = false;
                Idle();
                if (debugLogsEnabled) Debug.Log($"{gameObject.name}: Player no encontrado. Manteniendo Idle.");
                return; // Salir de FixedUpdate si no hay target
            }
        }

        // 2. Cálculo de distancias y rangos
        float distanceToTarget = Vector3.Distance(transform.position, target.position);
        inSightRange = distanceToTarget <= sightRange;
        inAttackRange = distanceToTarget <= attackRange;

        // 3. Lógica de comportamiento
        if (inSightRange && inAttackRange)
        {
            RotateTowardsTarget();
            AttackTarget();
        }
        else
        {
            Idle();
        }

        if (debugLogsEnabled)
        {
            // Debugging de estado
            Debug.Log($"{gameObject.name}: FixedUpdate - InSightRange: {inSightRange}, InAttackRange: {inAttackRange}, CanAttack: {canAttack}");
        }
    }

    protected override void AttackTarget()
    {
        if (!canAttack)
        {
            if (debugLogsEnabled) Debug.Log($"{gameObject.name}: No puede atacar aún (cooldown).");
            return;
        }

        canAttack = false; // Bloquea futuros ataques
        Invoke(nameof(ResetAttack), attackCooldown); // Programa el reset del cooldown

        if (animator != null)
        {
            if (debugLogsEnabled) Debug.Log($"{gameObject.name}: Llamando Animator.SetTrigger(\"Attack\")");
            animator.SetTrigger("Attack");
            animator.SetBool("isIdle", false); // Asegura que no esté en idle mientras ataca
        }
        else
        {
            if (debugLogsEnabled) Debug.LogWarning($"{gameObject.name}: Animator es null al intentar Attack.");
        }

        ShootProjectile();

        if (debugLogsEnabled) Debug.Log($"{gameObject.name}: Disparando proyectil.");
    }

    protected override void ResetAttack()
    {
        base.ResetAttack(); // Llama al ResetAttack de la clase base que setea canAttack = true
        if (animator != null)
        {
            // Opcional: Asegúrate de que, tras el ataque y cooldown, el estado vuelva a Idle
            // Esto podría ser mejor gestionado por la transición del Animator 'Attack -> Idle'.
            // animator.SetBool("isIdle", true);
            // animator.ResetTrigger("Attack"); // Opcional, si quieres resetear el trigger manualmente.
        }
        if (debugLogsEnabled) Debug.Log($"{gameObject.name}: Cooldown reiniciado. CanAttack = true.");
    }

    // Asegúrate de que el método Idle también gestiona correctamente las animaciones
    protected override void Idle()
    {
        if (animator != null)
        {
            if (!animator.GetBool("isIdle")) // Solo cambia si no está ya en Idle
            {
                animator.SetBool("isIdle", true);
                animator.SetBool("isWalking", false);
                if (debugLogsEnabled) Debug.Log($"{gameObject.name}: Estableciendo Animator.SetBool(\"isIdle\", true).");
            }
        }
        rb.linearVelocity = Vector3.zero; // Asegura que el Rigidbody no se mueva
        rb.angularVelocity = Vector3.zero; // Asegura que el Rigidbody no rote
    }

    // Los demás métodos (ChaseTarget, Patrol, RotateTowardsTarget, ShootProjectile, OnDrawGizmos) se mantienen igual
    protected override void ChaseTarget() { /* NO HACER NADA */ }
    protected override void Patrol() { /* NO HACER NADA */ Idle(); }

    private void RotateTowardsTarget()
    {
        if (target != null)
        {
            Vector3 directionToTarget = (target.position - transform.position).normalized;
            directionToTarget.y = 0;
            if (directionToTarget.sqrMagnitude > 0.001f) // Usa sqrMagnitude para evitar problemas con vectores casi cero
            {
                Quaternion lookRotation = Quaternion.LookRotation(directionToTarget);
                transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * rotationSpeed);
            }
        }
    }

    private void ShootProjectile()
    {
        if (projectilePrefab != null && firePoint != null)
        {
            Instantiate(projectilePrefab, firePoint.position, firePoint.rotation);
        }
    }

    protected override void OnDrawGizmos()
    {
        base.OnDrawGizmos();
        if (firePoint != null)
        {
            Gizmos.color = Color.cyan;
            Gizmos.DrawSphere(firePoint.position, 0.1f);
            Gizmos.DrawLine(firePoint.position, firePoint.position + firePoint.forward * 2f);
        }
    }
}