using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerCrouch : MonoBehaviour
{
    [Header("Colliders")]
    [SerializeField] private Collider2D standingCollider;
    [SerializeField] private Collider2D crouchCollider;

    [Header("Movement")]
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float crouchSpeed = 2.5f;
    [SerializeField] private float speedSmooth = 10f; // smoothing factor

    [Header("Animation")]
    [SerializeField] private Animator animator;

    private Vector2 moveInput;
    private bool isCrouching;
    private bool crouchRequested;
    private float currentSpeed;

    void Update()
    {
        // Controller input: Xbox B button
        if (Gamepad.current != null)
        {
            if (Gamepad.current.buttonEast.wasPressedThisFrame)
                crouchRequested = true;
            else if (Gamepad.current.buttonEast.wasReleasedThisFrame)
                crouchRequested = false;
        }

        // Movement input
        if (Gamepad.current != null)
            moveInput = Gamepad.current.leftStick.ReadValue();
        else if (Keyboard.current != null)
            moveInput = new Vector2(
                Keyboard.current.aKey.isPressed ? -1 : Keyboard.current.dKey.isPressed ? 1 : 0,
                0
            );
        else
            moveInput = Vector2.zero;
    }

    void FixedUpdate()
    {
        // Toggle crouch
        if (crouchRequested && !isCrouching)
            StartCrouch();
        else if (!crouchRequested && isCrouching)
            StopCrouch();

        // Smoothly adjust speed
        float targetSpeed = isCrouching ? crouchSpeed : moveSpeed;
        currentSpeed = Mathf.Lerp(currentSpeed, targetSpeed, speedSmooth * Time.fixedDeltaTime);

        // Move player
        transform.position += new Vector3(moveInput.x, 0f, 0f) * currentSpeed * Time.fixedDeltaTime;
    }

    void LateUpdate()
    {
        // Animator updates
        animator.SetBool("IsCrouching", isCrouching);
        animator.SetFloat("Speed", Mathf.Abs(moveInput.x) * (currentSpeed / moveSpeed)); // scaled speed
    }

    void StartCrouch()
    {
        isCrouching = true;
        standingCollider.enabled = false;
        crouchCollider.enabled = true;
    }

    void StopCrouch()
    {
        isCrouching = false;
        standingCollider.enabled = true;
        crouchCollider.enabled = false;
    }
}
