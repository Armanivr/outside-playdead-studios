using UnityEngine;

public class NPCFollowScript : MonoBehaviour
{
    [SerializeField] private float distanceInFront = 2f;   // NPC stays this far ahead if player is behind
    [SerializeField] private float distanceBehind = -2f;   // NPC stays this far behind if player is ahead
    [SerializeField] private float damping = 0.3f;

    public Transform player;
    private Vector3 vel = Vector3.zero;

    private void FixedUpdate()
    {
        float npcX = transform.position.x;
        float playerX = player.position.x;

        // Use Inspector values for distance
        float distance = playerX >= npcX ? distanceBehind : distanceInFront;

        Vector3 targetPosition = new Vector3(
            playerX + distance,
            transform.position.y,
            transform.position.z
        );

        transform.position = Vector3.SmoothDamp(transform.position, targetPosition, ref vel, damping);
    }
}
