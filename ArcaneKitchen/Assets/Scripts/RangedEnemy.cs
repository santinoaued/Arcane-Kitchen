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
            Debug.LogError($"{gameObject.name}: Projectile Prefab no asignado. Asigna el prefab del proyectil en el Inspector.", this);
        }
        if (firePoint == null)
        {
            Debug.LogError($"{gameObject.name}: Fire Point no asignado. Asigna un GameObject hijo para el punto de disparo en el Inspector.", this);
            // Si el FirePoint no se asigna, se intentará crear uno por defecto para evitar NREs,
            // pero es mejor asignarlo manualmente en el editor.
            GameObject defaultFirePoint = new GameObject("DefaultFirePoint");
            defaultFirePoint.transform.parent = transform;
            defaultFirePoint.transform.localPosition = new Vector3(0, 1.5f, 1f); // Posición más lógica para un punto de disparo elevado
            firePoint = defaultFirePoint.transform;
            Debug.LogWarning($"{gameObject.name}: Creando Fire Point por defecto. ¡Asígnalo manualmente en el Inspector para un control preciso!", this);
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
            // Solo loguea si hay un cambio o si es un momento clave para evitar spam excesivo
            // Debug.Log($"{gameObject.name}: FixedUpdate - InSightRange: {inSightRange}, InAttackRange: {inAttackRange}, CanAttack: {canAttack}");
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
            animator.SetBool("isIdle", false); // Asegura que no esté en idle mientras ataca
            animator.SetTrigger("Attack");     // Dispara el trigger de ataque
        }
        else
        {
            if (debugLogsEnabled) Debug.LogWarning($"{gameObject.name}: Animator es null al intentar Attack.");
            // Si no hay animador, disparamos el proyectil directamente después del cooldown.
            // Esto es un fallback, la intención es usar Animation Events.
            ShootProjectile();
        }

        // IMPORTANTE: El ShootProjectile() original ahora se llama a través de un Animation Event.
        // No lo llames aquí directamente si quieres que se sincronice con la animación.
        // Si no usas Animation Events, descomenta la línea de abajo.
        // ShootProjectile();

        if (debugLogsEnabled) Debug.Log($"{gameObject.name}: Iniciando secuencia de ataque (animación).");
    }

    protected override void ResetAttack()
    {
        base.ResetAttack(); // Llama al ResetAttack de la clase base que setea canAttack = true
        if (animator != null)
        {
            // Opcional: Esto ayuda a asegurar que el Animator no se quede "atascado" en el trigger.
            // La transición de 'Attack' a 'Idle' en el Animator Controller debe manejar esto idealmente.
            animator.ResetTrigger("Attack");
            // animator.SetBool("isIdle", true); // Esto lo gestionará la transición de 'Attack' a 'Idle' con Exit Time
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
        // Asegura que el Rigidbody no se mueva o rote si es el caso (aunque es cinemático)
        if (rb != null)
        {
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
        }
    }

    // Los demás métodos (ChaseTarget, Patrol) se mantienen igual, pero los dejo comentados
    // para recordar que no hacen nada en este enemigo.
    protected override void ChaseTarget() { /* Este enemigo no persigue. */ }
    protected override void Patrol() { /* Este enemigo no patrulla. */ Idle(); }

    private void RotateTowardsTarget()
    {
        if (target != null)
        {
            // Calcula la dirección al jugador, ignorando la altura para la rotación horizontal
            Vector3 directionToTarget = (target.position - transform.position).normalized;
            directionToTarget.y = 0; // Solo rota en el eje Y

            if (directionToTarget.sqrMagnitude > 0.001f) // Evita rotar si el vector es casi cero
            {
                // Crea una rotación que mira en esa dirección
                Quaternion lookRotation = Quaternion.LookRotation(directionToTarget);
                // Interpola suavemente a la nueva rotación
                transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * rotationSpeed);
            }
        }
    }

    // NUEVA FUNCIÓN: Llamada por Animation Event para disparar el proyectil
    // Debe ser pública para que el Animation Event la pueda encontrar.
    public void TriggerProjectile()
    {
        if (debugLogsEnabled) Debug.Log($"{gameObject.name}: Animation Event 'TriggerProjectile' llamado.");
        ShootProjectile();
    }

    // Método original de disparo, ahora llamado por TriggerProjectile()
    private void ShootProjectile()
    {
        if (projectilePrefab != null && firePoint != null && target != null)
        {
            // Calcula la dirección desde el firePoint hacia la posición del target (jugador)
            // Esto hará que el proyectil se dispare hacia abajo si el enemigo está elevado.
            Vector3 directionToTarget = (target.position - firePoint.position).normalized;

            // Crea una rotación que mira en esa dirección
            Quaternion projectileRotation = Quaternion.LookRotation(directionToTarget);

            // Instancia el proyectil en el firePoint con la rotación calculada
            GameObject projectileGO = Instantiate(projectilePrefab, firePoint.position, projectileRotation);

            // Opcional: Si el proyectil tiene un componente Rigidbody y un script de movimiento,
            // podrías pasarle la dirección o aplicar una fuerza directamente.
            // Ejemplo (si tu proyectil tiene un script ProjectileMovement con un método SetDirection):
            // ProjectileMovement projScript = projectileGO.GetComponent<ProjectileMovement>();
            // if (projScript != null)
            // {
            //     projScript.SetDirection(directionToTarget);
            //     // o aplicar una fuerza si tu proyectil se mueve con Rigidbody.AddForce
            //     // projScript.ApplyInitialForce(directionToTarget * projectileSpeed);
            // }

            if (debugLogsEnabled) Debug.Log($"{gameObject.name}: Proyectil disparado desde {firePoint.position} hacia {target.position}.");
        }
        else
        {
            if (debugLogsEnabled)
            {
                string missingPart = "";
                if (projectilePrefab == null) missingPart += "Projectile Prefab ";
                if (firePoint == null) missingPart += "Fire Point ";
                if (target == null) missingPart += "Target ";
                Debug.LogWarning($"{gameObject.name}: No se pudo disparar el proyectil. Faltan componentes: {missingPart.Trim()}", this);
            }
        }
    }

    protected override void OnDrawGizmos()
    {
        base.OnDrawGizmos();
        // Dibuja el rango de visión y ataque
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, sightRange);
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);

        // Dibuja el punto de disparo y la dirección de disparo anticipada
        if (firePoint != null)
        {
            Gizmos.color = Color.cyan;
            Gizmos.DrawSphere(firePoint.position, 0.15f); // Un poco más grande para ser visible
            if (target != null)
            {
                // Dibuja una línea desde el FirePoint hacia el Target
                Gizmos.DrawLine(firePoint.position, target.position);
            }
            else
            {
                // Si no hay target, dibuja la dirección frontal del firePoint por defecto
                Gizmos.DrawLine(firePoint.position, firePoint.position + firePoint.forward * 2f);
            }
        }
    }
}