using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField] private Transform target; // Reference to the player
    [SerializeField] private float speed = 0.15f;
    private Vector3 velocity = Vector3.zero;
    [SerializeField] private float yOffset;

    private void Update()
    {
        if (target != null)
        {
            Vector3 targetPosition = new Vector3(target.position.x, target.position.y + yOffset, transform.position.z);
            transform.position = Vector3.SmoothDamp(transform.position, targetPosition, ref velocity, speed * Time.deltaTime);
        }
    }
}
