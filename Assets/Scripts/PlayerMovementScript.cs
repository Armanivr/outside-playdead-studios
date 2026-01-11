using UnityEngine;
using Outside.Input;

public class playerMovementScript : MonoBehaviour, PlayerControls.IPlayerActions
{
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private Animator animator;
    [SerializeField] private SpriteRenderer spriteRenderer;

    private PlayerControls controls;
    private Vector2 moveInput;
    private bool playingFootsteps = false;
    public float footstepSpeed = 0.5f;

    [HideInInspector] public bool canMove = true;

    private void Awake()
    {
        controls = new PlayerControls();
        controls.Player.SetCallbacks(this);
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
        moveInput = canMove ? context.ReadValue<Vector2>() : Vector2.zero;
    }

    void Update()
    {
        if (!canMove)
        {
            animator.SetFloat("Speed", 0f);
            return;
        }

        // Beweging
        Vector3 movement = new Vector3(moveInput.x, 0f, 0f);
        transform.position += movement * moveSpeed * Time.deltaTime;

        // Speed check (0–1)
        float speed = Mathf.Abs(moveInput.x);
        animator.SetFloat("Speed", speed);

        if( speed > 0 && !playingFootsteps)
        {
            StartFootsteps();
        }
        else if(speed == 0)
        {
            StopFootsteps();
        }

        // Flip sprite
        if (moveInput.x > 0)
            spriteRenderer.flipX = false;
        else if (moveInput.x < 0)
            spriteRenderer.flipX = true;
    }

    void StartFootsteps()
    {
        playingFootsteps = true;
        InvokeRepeating(nameof(PlayFootstep), 0f, footstepSpeed);
    }

    void StopFootsteps()
    {
        playingFootsteps = false;
        CancelInvoke(nameof(PlayFootstep));
    }

    void PlayFootstep()
    {
        SoundEffectManager.Play("Footstep");
    }
}
