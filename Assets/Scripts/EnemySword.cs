using UnityEngine;

public class EnemySwordHitbox : MonoBehaviour
{
     private int swordDamage = 10;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            collision.GetComponent<PlayerHealth>().TakeDamage(swordDamage);
            Debug.Log("Player hit by sword!");
        }
    }
}
