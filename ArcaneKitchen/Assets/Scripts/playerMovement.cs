using UnityEngine;
using UnityEngine.EventSystems;

public class playerMovement : MonoBehaviour
{
    [Header("Stats")]
    [SerializeField] float jumpHeight = 1.5f;
    [SerializeField] float speed = 1.5f;

    [Header("Frenesí")]
    [SerializeField] float multiJumpHeight = 2f;
    [SerializeField] float multiSpeed = 2f;

    [Header("Movimiento")]
    [SerializeField] float runSpeed = 1.5f;
    [SerializeField] float gravity = -9.81f;
    [SerializeField] float _rotationSpeed = 150f;
    [SerializeField] Transform groundCheck;
    [SerializeField] float groundDistance = 0.25f;         
    [SerializeField] LayerMask groundLayer;

    private float actualSpeed;
    private float actualJumpHeight;
    private playerSanityHealth cordura;

    Rigidbody rb;

    float _moveH, _moveV;
    float _rotationAmount;

    Vector3 _velocity;
    bool _isGrounded;

    [SerializeField] animationStateController animationController;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        
        rb.freezeRotation = true;
        rb.interpolation = RigidbodyInterpolation.Interpolate;
        rb.collisionDetectionMode = CollisionDetectionMode.Continuous;

        cordura = GetComponent<playerSanityHealth>();

        actualSpeed = speed;
        actualJumpHeight = jumpHeight;

        if (animationController == null)
        {
            Debug.LogError("El script 'animationStateController' no se encontró en este GameObject.");
        }

        FrenesiController.OnFrenesiChanged += AddStatsFrenesi;
    }

    void Update()
    {
        _moveH = Input.GetAxis("Horizontal");
        _moveV = Input.GetAxis("Vertical");

        
        _isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundLayer);

        
        if (_isGrounded && rb.linearVelocity.y < 0f)
        {
            
            Vector3 v = rb.linearVelocity;
            v.y = -2f;
            rb.linearVelocity = v;
        }

        
        if (Input.GetKeyDown(KeyCode.Q) && _isGrounded)
        {
            float jumpForce = Mathf.Sqrt(2f * Mathf.Abs(Physics.gravity.y) * jumpHeight);
            Vector3 v = rb.linearVelocity;
            v.y = jumpForce;
            rb.linearVelocity = v;
            
        }

        
        if (Input.GetKeyDown(KeyCode.Space) && _isGrounded)
        {
            float jumpForce = Mathf.Sqrt(2f * Mathf.Abs(Physics.gravity.y) * (jumpHeight * multiJumpHeight));
            Vector3 v = rb.linearVelocity;
            v.y = jumpForce;
            rb.linearVelocity = v;
            
        }

        

        animationController.IsGrounded(_isGrounded);
    }

    void FixedUpdate()
    {
        
        _rotationAmount = _moveH * _rotationSpeed * Time.fixedDeltaTime;
        Quaternion deltaRotation = Quaternion.Euler(0, _rotationAmount, 0);
        rb.MoveRotation(rb.rotation * deltaRotation);

        
        Vector3 localForward = transform.forward * _moveV * (speed * multiSpeed);

        if (Input.GetKey(KeyCode.LeftShift))
        {
            localForward *= runSpeed;
        }

        if (_moveV < 0)
        {
            localForward *= 0.75f;
        }

        
        Vector3 horizontalMove = new Vector3(localForward.x, 0f, localForward.z);
        Vector3 newPosition = rb.position + horizontalMove * Time.fixedDeltaTime;

        rb.MovePosition(newPosition);
    }

    void AddStatsFrenesi(bool frenesiActivo)
    {
        if (frenesiActivo)
        {
            multiSpeed = 2f;
            multiJumpHeight = 2f;
            Debug.Log("¡Movimiento Frenesí ACTIVADO!");
        }
        else
        {
            multiSpeed = 1f;
            multiJumpHeight = 1f;
            Debug.Log("Movimiento normal restaurado");
        }

        Debug.Log($"MultiSpeed: {multiSpeed}, MultiJump: {multiJumpHeight}");
    }

    void OnDestroy()
    {
        FrenesiController.OnFrenesiChanged -= AddStatsFrenesi;
    }

   
    void OnDrawGizmosSelected()
    {
        if (groundCheck != null)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(groundCheck.position, groundDistance);
        }
    }
}
