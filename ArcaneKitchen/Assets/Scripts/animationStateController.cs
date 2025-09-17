using System;
using UnityEngine;

public class animationStateController : MonoBehaviour
{
    Animator animator;

    int isWalkingHash;
    int isWalkingBackHash;
    int isRunningHash;

    // int isJumpingHash;
    int jumpTriggerHash;

    private bool _isGrounded = true;

    void Start()
    {
        animator = GetComponent<Animator>();
        animator.applyRootMotion = false;

        isWalkingHash = Animator.StringToHash("isWalking");
        isWalkingBackHash = Animator.StringToHash("isWalkingBack");
        isRunningHash = Animator.StringToHash("isRunning");

        // isJumpingHash = Animator.StringToHash("isJumping");
        jumpTriggerHash = Animator.StringToHash("jumpTrigger");


    }

    void Update()
    {

        bool isWalking = animator.GetBool(isWalkingHash);
        bool isWalkingBack = animator.GetBool(isWalkingBackHash);
        bool isRunning = animator.GetBool(isRunningHash);
        // bool isJumping = animator.GetBool(isJumpingHash);


        bool runPressed = Input.GetKey("left shift");
        bool movingFoward = Input.GetKey("w");
        bool movingBackwards = Input.GetKey("s");
        bool jumpPressed = Input.GetKeyDown(KeyCode.Space);


        // Camina para adelante
        if (!isWalking && movingFoward)
        {
            animator.SetBool(isWalkingHash, true);
        }

        if (isWalking && !movingFoward)
        {
            animator.SetBool(isWalkingHash, false);
        }

        // Camina para atrás
        if (!isWalkingBack && movingBackwards)
        {
            animator.SetBool(isWalkingBackHash, true);
        }

        if (isWalkingBack && !movingBackwards)
        {
            animator.SetBool(isWalkingBackHash, false);
        }

        // Corre
        if (!isRunning && (runPressed && movingFoward))
        {
            animator.SetBool(isRunningHash, true);
        }

        if (isRunning && (!runPressed || !movingFoward))
        {
            animator.SetBool(isRunningHash, false);
        }

        if (jumpPressed && _isGrounded)
        {
            JumpTrigger();
            Debug.Log("Entra por el salto");
        }

    }

    public void JumpTrigger()
    {
        animator.SetTrigger(jumpTriggerHash);
    }

    public void IsGrounded(bool valor)
    {
        _isGrounded = valor;

        if (_isGrounded)
        {
            // animator.SetBool(isJumpingHash, false);
        }
        else
        {

            // Debug.Log("Entra al IsGrounded cae en Else");

        }
    }
}
