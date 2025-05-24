using UnityEngine;

public class BreathHitbox : MonoBehaviour
{
    [Tooltip("How much damage this breath does")]
    public int damage = 30;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            other.GetComponent<PlayerHealth>()?.TakeDamage(damage);
        }
    }
}
