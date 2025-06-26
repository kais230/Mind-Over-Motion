using UnityEngine;
using UnityEngine.UIElements;

public class SpikeDamage : MonoBehaviour
{
    PlayerHealth playerHealth;
    [SerializeField] private int damageAmount = 100; 
    BoxCollider2D spikeCollider;
   
    void Start()
    {
        spikeCollider = GetComponent<BoxCollider2D>();
    }

    void Update()
    {

    }
    
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerHealth = other.GetComponent<PlayerHealth>();
            if (playerHealth != null)
            {
                playerHealth.TakeDamage(damageAmount);
            }
        }
    }
}
