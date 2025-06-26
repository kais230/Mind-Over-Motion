using UnityEngine;

public class HealthShop : MonoBehaviour
{
    bool isActive = false;
    [SerializeField] Vampire VampireController;
    void Start()
    {

    }
    void Update()
    {
        isActive = VampireController.VampireDefeated;
    }
    
    void OnTriggerEnter2D(Collider2D collision)
    {
        if (isActive && collision.CompareTag("Player"))
        {
            PlayerHealth playerHealth = collision.GetComponent<PlayerHealth>();
            if (playerHealth != null)
            {
                playerHealth.RestoreHealth();
                Debug.Log("Player health restored!");
            }
        }
    }
}
