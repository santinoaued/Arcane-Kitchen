using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Transform))]
public class DoorController : MonoBehaviour
{
    [Header("Movimiento")]
    [Tooltip("altura en metros que sube la puerta respecto a su posición inicial")]
    public float openHeight = 4f;
    [Tooltip("velocidad de movimiento en unidades por segundo")]
    public float speed = 3f;
    [Tooltip("si true, el collider de la puerta se desactiva mientras está abierta")]
    public bool disableColliderWhileOpen = true;

    // estados
    Vector3 closedPos;
    Vector3 openPos;
    Coroutine movingCoroutine = null;
    Collider doorCollider;

    void Awake()
    {
        closedPos = transform.position;
        openPos = closedPos + Vector3.up * openHeight;
        doorCollider = GetComponent<Collider>();
    }

    
    public void Open()
    {
        if (movingCoroutine != null) StopCoroutine(movingCoroutine);
        movingCoroutine = StartCoroutine(MoveTo(openPos));
    }

    
    public void Close()
    {
        if (movingCoroutine != null) StopCoroutine(movingCoroutine);
        movingCoroutine = StartCoroutine(MoveTo(closedPos));
    }

    IEnumerator MoveTo(Vector3 target)
    {
        
        bool targetIsOpen = target == openPos;

        
        while ((transform.position - target).sqrMagnitude > 0.0001f)
        {
            transform.position = Vector3.MoveTowards(transform.position, target, speed * Time.deltaTime);
            yield return null;
        }

        transform.position = target;
        movingCoroutine = null;

        if (disableColliderWhileOpen && doorCollider != null)
        {
            doorCollider.enabled = !targetIsOpen;
        }
    }

    
    public void SnapClosed()
    {
        if (movingCoroutine != null) StopCoroutine(movingCoroutine);
        transform.position = closedPos;
        if (doorCollider != null) doorCollider.enabled = true;
    }

    public void SnapOpen()
    {
        if (movingCoroutine != null) StopCoroutine(movingCoroutine);
        transform.position = openPos;
        if (doorCollider != null && disableColliderWhileOpen) doorCollider.enabled = false;
    }
}
