using UnityEngine;

public class NPCFollowScript : MonoBehaviour
{
    [Header("Follow Settings")]
    [SerializeField] private float distanceInFront = 2f;
    [SerializeField] private float distanceBehind = -2f;
    [SerializeField] private float smoothTime = 0.3f;
    [SerializeField] private float maxSpeed = 3f;

    [Header("References")]
    [SerializeField] private Transform player;
    [SerializeField] private Animator animator;

    private Vector3 velocity = Vector3.zero;
    private bool playingFootsteps = false;
    public float footstepSpeed = 0.5f;
    public float footstepThreshold = 0.05f;

    void Update()
    {
        if (player == null || animator == null) return;

        // 1️⃣ Bepaal target positie
        float distance = player.position.x > transform.position.x ? distanceBehind : distanceInFront;
        Vector3 targetPosition = new Vector3(player.position.x + distance, transform.position.y, transform.position.z);

        // 2️⃣ Smooth beweging
        transform.position = Vector3.SmoothDamp(transform.position, targetPosition, ref velocity, smoothTime);

        // 3️⃣ Bereken snelheid voor Blend Tree
        float speed = Mathf.Abs(velocity.x);
        float normalizedSpeed = Mathf.Clamp01(speed / maxSpeed);
        animator.SetFloat("Speed", normalizedSpeed);

        // 4️⃣ Flip NPC
        FlipNPC();

        // 5️⃣ Footstep geluiden
        if (speed > footstepThreshold && !playingFootsteps)
            StartFootsteps();
        else if (speed <= footstepThreshold && playingFootsteps)
            StopFootsteps();
    }

    private void FlipNPC()
    {
        if (velocity.x > 0.05f)
            transform.localScale = new Vector3(1, transform.localScale.y, transform.localScale.z);
        else if (velocity.x < -0.05f)
            transform.localScale = new Vector3(-1, transform.localScale.y, transform.localScale.z);
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
