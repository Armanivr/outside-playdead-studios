using UnityEngine;
using Outside.Input; // Only needed if PlayerControls is in this namespace

public class playerMovementScript : MonoBehaviour, PlayerControls.IPlayerActions
{
    [SerializeField] private float moveSpeed = 5f;
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
        transform.position += new Vector3(moveInput.x * moveSpeed * Time.deltaTime, 0f, 0f);
    }
}
