using UnityEngine;

public class smoothCameraFollowScript : MonoBehaviour
{
    [SerializeField] private Vector3 offset;
    [SerializeField] private float damping;

    public Transform target;

    public Vector3 vel = Vector3.zero;
    public float targetHeight;

    private void FixedUpdate()
    {
        // Only follow the target's x position, keep the camera's current y position
        Vector3 targetPosition = new Vector3(
            target.position.x + offset.x,
            transform.position.y, // Keep current y position
            transform.position.z  // Keep current z position
        );

        transform.position = Vector3.SmoothDamp(transform.position, targetPosition, ref vel, damping);
    }
}
