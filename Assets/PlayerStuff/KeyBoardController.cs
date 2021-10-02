using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeyBoardController : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField]
    private CharacterController characterController;
    [SerializeField]
    private float movementSpeed;
    [SerializeField]
    private float jumpForce;
    [Header("Gravity")]
    [SerializeField]
    private float gravityAcceleration;
    private Vector3 yVelocity = new Vector3();
    [SerializeField]
    private Vector3 groundCheckPos;
    [SerializeField]
    private float groundCheckRadius;
    [SerializeField]
    private LayerMask groundCheckMask;
    [SerializeField]
    private bool isGrounded = false;

    private float keyboardX;
    private float keyboardY;

    void Update()
    {
        GetKeyboardInput();
        Move();
        SetIsGrounded();
        ResetYVelocity();
        Jump();
        ApplyGravity();
        ApplyYVelocity();
    }

    private void GetKeyboardInput()
    {
        keyboardX = Input.GetAxis("Horizontal");
        keyboardY = Input.GetAxisRaw("Vertical");
    }
    private void Move()
    {
        Vector3 movementOffset = (transform.right * keyboardX + transform.forward * keyboardY) * movementSpeed * Time.deltaTime;
        characterController.Move(movementOffset);
    }
    private void ApplyGravity()
    {
        if (!isGrounded)
        {
            yVelocity += Vector3.up * gravityAcceleration * Time.deltaTime;
        }
    }
    private void ResetYVelocity()
    {
        if (yVelocity.y < 0f && isGrounded)
        {
            yVelocity = new Vector3();
        }
    }
    private void SetIsGrounded()
    {
        isGrounded = Physics.CheckSphere(transform.position + groundCheckPos, groundCheckRadius, 3);
    }

    private void Jump()
    {
        if (isGrounded && Input.GetButtonDown("Jump"))
            yVelocity = Vector3.up * jumpForce / 100f;
    }
    private void ApplyYVelocity()
    {
        characterController.Move(yVelocity);
    }


    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position + groundCheckPos, groundCheckRadius);
    }
}
