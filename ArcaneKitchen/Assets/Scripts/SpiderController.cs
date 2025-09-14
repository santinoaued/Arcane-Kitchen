using UnityEngine;

public class SpiderPatrol : MonoBehaviour
{
    [Header("Patrulla")]
    public Transform pointA;
    public Transform pointB;
    public float speed = 2f;
    public float stoppingDistance = 0.1f;

    [Header("Rotaci�n")]
    public float rotationSpeed = 10f;
    public float rotationOffset = 180f; // Ajustar si el modelo apunta al rev�s

    private Transform currentTarget;
    private Animator animator;

    void Start()
    {
        animator = GetComponent<Animator>();

        if (pointA == null || pointB == null)
        {
            Debug.LogError("�Asign� pointA y pointB en el Inspector!");
            enabled = false;
            return;
        }

        currentTarget = pointB; // Empieza movi�ndose hacia B
    }

    void Update()
    {
        if (currentTarget == null) return;

        Vector3 dir = currentTarget.position - transform.position;
        dir.y = 0;

        if (dir.magnitude > stoppingDistance)
        {
            // Moverse hacia el target
            transform.position += dir.normalized * speed * Time.deltaTime;

            // Rotar hacia la direcci�n de movimiento con offset
            if (dir != Vector3.zero)
            {
                Quaternion targetRotation = Quaternion.LookRotation(dir) * Quaternion.Euler(0, rotationOffset, 0);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
            }

            animator.SetBool("isWalking", true);
        }
        else
        {
            animator.SetBool("isWalking", false);
            // Cambiar de punto cuando llega
            currentTarget = (currentTarget == pointA) ? pointB : pointA;
        }
    }

    private void OnDrawGizmosSelected()
    {
        if (pointA != null) Gizmos.DrawSphere(pointA.position, 0.2f);
        if (pointB != null) Gizmos.DrawSphere(pointB.position, 0.2f);
    }
}
