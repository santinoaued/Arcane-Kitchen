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
    [SerializeField] float groundDistance = 0.4f;
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

        _isGrounded = Physics.Raycast(
            groundCheck.position,
            Vector3.down,
            groundDistance + 0.5f,
            groundLayer
        );

        if (_isGrounded && _velocity.y < 0)
        {
            _velocity.y = -3f;
        }

        if (Input.GetKeyDown(KeyCode.Q))
        {
            rb.AddForce(Vector3.up * actualJumpHeight * 3, ForceMode.Impulse);
            Debug.Log("Salta (?");
            Debug.Log(_isGrounded);
        }

        if (Input.GetKeyDown(KeyCode.Space) && _isGrounded)
        {
            float jumpForce = Mathf.Sqrt(jumpHeight * multiJumpHeight * -2f * Physics.gravity.y);
            rb.linearVelocity = new Vector3(rb.linearVelocity.x, jumpForce, rb.linearVelocity.z);
            Debug.Log($"Salto frenesí! Fuerza: {jumpForce}, Multiplicador: {multiJumpHeight}");
        }

        _velocity.y += gravity * Time.deltaTime;

        animationController.IsGrounded(_isGrounded);

    }

    void FixedUpdate()
    {

        _rotationAmount = _moveH * _rotationSpeed * Time.fixedDeltaTime;
        Quaternion deltaRotation = Quaternion.Euler(0, _rotationAmount, 0);
        rb.MoveRotation(rb.rotation * deltaRotation);

        Vector3 moveDirection = transform.forward * _moveV * (speed * multiSpeed);


        if (Input.GetKey(KeyCode.LeftShift))
        {
            moveDirection *= runSpeed;
        }


        if (_moveV < 0)
        {
            moveDirection *= 0.75f;
        }


        Vector3 finalVelocity = moveDirection;
        finalVelocity.y = rb.linearVelocity.y;

        rb.linearVelocity = finalVelocity;

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
}