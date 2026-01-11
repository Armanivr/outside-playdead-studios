using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerJump : MonoBehaviour
{
    [SerializeField] private Rigidbody2D rigidBody;
    [SerializeField] private Animator animator;
    [SerializeField] private float jumpForce = 6f;
    [SerializeField] private float groundCheckDistance = 0.1f;
    [SerializeField] private LayerMask groundLayer;

    private bool isGrounded;
    private bool wasGrounded;

    private bool jumpRequested;

    void Update()
    {
        // Jump input (record it, don't apply physics yet)
        if (Gamepad.current != null && Gamepad.current.buttonSouth.wasPressedThisFrame && isGrounded)
        {
            jumpRequested = true;
        }
    }

    void FixedUpdate()
    {
        // Ground check
        wasGrounded = isGrounded;
        isGrounded = Physics2D.Raycast(transform.position, Vector2.down, groundCheckDistance, groundLayer);

        // Apply jump if requested
        if (jumpRequested)
        {
            rigidBody.linearVelocity = new Vector2(rigidBody.linearVelocity.x, jumpForce);
            animator.SetTrigger("Jump");
            jumpRequested = false;
        }
    }

    void LateUpdate()
    {
        // Update animations AFTER physics
        bool isFalling = !isGrounded && rigidBody.linearVelocity.y < 0;
        animator.SetBool("IsFalling", isFalling);

        if (!wasGrounded && isGrounded)
        {
            animator.SetTrigger("Land");
        }
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawLine(transform.position, transform.position + Vector3.down * groundCheckDistance);
    }
}
