using UnityEngine;
using UnityEngine.EventSystems;
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

    [SerializeField] LayerMask groundLayer;

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

        if (animationController == null)
        {
            Debug.LogError("El script 'animationStateController' no se encontró en este GameObject.");
        }
    }

    void Update()
    {

        _moveH = Input.GetAxis("Horizontal");
        _moveV = Input.GetAxis("Vertical");

        _isGrounded = Physics.Raycast(
            groundCheck.position,
            Vector3.down,
            groundDistance + 0.3f,
            groundLayer
        );

        if (_isGrounded && _velocity.y < 0)
        {
            _velocity.y = -3f;
        }

        if (Input.GetKeyDown(KeyCode.Space) && _isGrounded)
        {
            float jumpVelocity = Mathf.Sqrt(jumpHeight * -2f * Physics.gravity.y);
            rb.linearVelocity = new Vector3(rb.linearVelocity.x, jumpVelocity, rb.linearVelocity.z);
        }

        _velocity.y += gravity * Time.deltaTime;

        animationController.IsGrounded(_isGrounded);
    }

    void FixedUpdate()
    {

        _rotationAmount = _moveH * _rotationSpeed * Time.fixedDeltaTime;
        Quaternion deltaRotation = Quaternion.Euler(0, _rotationAmount, 0);
        rb.MoveRotation(rb.rotation * deltaRotation);

        Vector3 moveDirection = transform.forward * _moveV * speed;


        if (Input.GetKey(KeyCode.LeftShift))
        {
            moveDirection *= runSpeed;
        }


        if (_moveV < 0)
        {
            // Debug.Log("Vertical " + _moveV);
            moveDirection *= 0.75f;
        }


        Vector3 finalVelocity = moveDirection;
        finalVelocity.y = rb.linearVelocity.y;

        rb.linearVelocity = finalVelocity;

    }
}