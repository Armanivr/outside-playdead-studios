using UnityEngine;
using Outside.Input; // Only needed if PlayerControls is in this namespace

public class playerMovementScript : MonoBehaviour, PlayerControls.IPlayerActions
{
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private Animator animator;
    [SerializeField] private SpriteRenderer spriteRenderer;


    Rigidbody rb;

    float inputHorizontal;

    private PlayerControls controls;
    private Vector2 moveInput;

    private void Awake()
    {
        controls = new PlayerControls();
        controls.Player.SetCallbacks(this); // Register callbacks
    }

    private void OnEnable()
    {
        controls.Player.Enable();
    }

    private void OnDisable()
    {
        controls.Player.Disable();
    }

    public void OnMove(UnityEngine.InputSystem.InputAction.CallbackContext context)
    {
        moveInput = context.ReadValue<Vector2>();

    }

    void Update()
    {
        transform.position += new Vector3(moveInput.x, 0f, 0f) * moveSpeed * Time.deltaTime;

        animator.SetBool("isRunning", moveInput != Vector2.zero);

        if (moveInput.x > 0)
            spriteRenderer.flipX = false;
        else if (moveInput.x < 0)
            spriteRenderer.flipX = true;
    }
}
