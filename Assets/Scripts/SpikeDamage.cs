using UnityEngine;
using UnityEngine.UIElements;

public class SpikeDamage : MonoBehaviour
{
    PlayerHealth playerHealth;
    [SerializeField] private int damageAmount = 100; // Amount of damage to apply
    BoxCollider2D spikeCollider;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
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
                Debug.Log("Player hit by spike, damage applied: " + damageAmount);
            }
        }
    }
}
