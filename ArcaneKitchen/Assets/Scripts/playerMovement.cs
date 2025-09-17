using UnityEngine;
public class playerMovement : MonoBehaviour
{
    [Header("Movimiento")]
    [SerializeField] float speed = 3f;
    [SerializeField] float runSpeed = 2f;
    [SerializeField] float _rotationSpeed = 150f;

    [SerializeField] float jumpHeight = 1.5f;
    [SerializeField] float gravity = -9.81f;
    [SerializeField] Transform groundCheck;
    [SerializeField] float groundDistance = 0.4f;

    [SerializeField] LayerMask groundMask;

    Rigidbody rb;

    float _moveH, _moveV;
    Vector3 _movement;
    Vector3 _moveDirection;
    float _rotationAmount;
    Quaternion _turnOffset;

    Vector3 _velocity;
    bool _isGrounded;

    [SerializeField] animationStateController animationController;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;

        if (animationController == null)
        {
            Debug.LogError("El script 'animationStateController' no se encontró en este GameObject.");
        }
    }

    void Update()
    {

        _moveH = Input.GetAxis("Horizontal");
        _moveV = Input.GetAxisRaw("Vertical");

        Vector3 castPosition = new Vector3(
            groundCheck.position.x,
            groundCheck.position.y + 1,
            groundCheck.position.z
        );

        _isGrounded = Physics.Raycast(
            groundCheck.position,
            Vector3.down,
            groundDistance + 0.1f,
            groundMask
        );


        if (_isGrounded && _velocity.y < 0)
        {
            _velocity.y = -2f;
        }

        if (Input.GetKeyDown(KeyCode.Space) && _isGrounded)
        {
            _velocity.y = Mathf.Sqrt(jumpHeight * -1f * gravity);
        }
        _velocity.y += gravity * Time.deltaTime;

        animationController.IsGrounded(_isGrounded);
    }

    void FixedUpdate()
    {

        // Debug.Log("Horizontal " + _moveH);
        // Debug.Log("Vertical " + _moveV);

        // Rotación
        _rotationAmount = _moveH * _rotationSpeed * Time.deltaTime;
        _turnOffset = Quaternion.Euler(0, _rotationAmount, 0);
        rb.MoveRotation(rb.rotation * _turnOffset);

        Vector3 moveDirection = transform.forward * _moveV * speed;


        if (Input.GetKey(KeyCode.LeftShift))
        {
            moveDirection *= runSpeed;
        }

        if (_moveV == -1)
        {
            moveDirection *= 0.75f;
        }

        // Aplicar movimiento + gravedad
        moveDirection.y = _velocity.y;
        rb.linearVelocity = moveDirection;

    }
}