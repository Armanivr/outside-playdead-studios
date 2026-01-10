using UnityEngine;

public class NPCFollowScript : MonoBehaviour
{
    [Header("Follow Settings")]
    [SerializeField] private float distanceInFront = 2f;  // NPC stays ahead if player is behind
    [SerializeField] private float distanceBehind = -2f;  // NPC stays behind if player is ahead
    [SerializeField] private float smoothTime = 0.3f;     // Smoothing for movement
    [SerializeField] private float maxSpeed = 3f;         // Max NPC speed for animation normalization

    [Header("References")]
    [SerializeField] private Transform player;
    [SerializeField] private Animator animator;

    private Vector3 velocity = Vector3.zero;

    void Update()
    {
        if (player == null || animator == null) return;

        // 1️⃣ Calculate target position
        float distance = player.position.x > transform.position.x
            ? distanceBehind
            : distanceInFront;

        Vector3 targetPosition = new Vector3(
            player.position.x + distance,
            transform.position.y,
            transform.position.z
        );

        // 2️⃣ Smooth movement
        transform.position = Vector3.SmoothDamp(
            transform.position,
            targetPosition,
            ref velocity,
            smoothTime
        );

        // 3️⃣ Calculate speed for Blend Tree
        float speed = Mathf.Abs(velocity.x);
        float normalizedSpeed = Mathf.Clamp01(speed / maxSpeed);
        animator.SetFloat("Speed", normalizedSpeed);

        // 4️⃣ Flip NPC based on direction
        FlipNPC();
    }

    private void FlipNPC()
    {
        if (velocity.x > 0.05f)
        {
            transform.localScale = new Vector3(1, transform.localScale.y, transform.localScale.z);
        }
        else if (velocity.x < -0.05f)
        {
            transform.localScale = new Vector3(-1, transform.localScale.y, transform.localScale.z);
        }
    }
}
