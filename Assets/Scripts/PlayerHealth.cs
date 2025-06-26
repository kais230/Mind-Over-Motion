using UnityEngine;
using UnityEngine.UI;

public class PlayerHealth : MonoBehaviour
{
    [SerializeField] private int maxHealth = 100;
    private int currentHealth;

    [SerializeField] private Slider healthBar; // Reference to the UI health bar
    bool playerDead = false;

    [SerializeField] private GameObject restartButton;

    private void Start()
    {
        currentHealth = maxHealth;
        healthBar.maxValue = maxHealth;
        healthBar.value = currentHealth;
        if (restartButton != null)
            restartButton.SetActive(false); // Hide at start
    }

    public void TakeDamage(int damage)
    {
        if (playerDead == true){
            return;
        }

        currentHealth -= damage;
        healthBar.value = currentHealth; // Update health bar
        gameObject.GetComponent<Animator>().SetTrigger("Hurt"); // Play hurt animation

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    public void RestoreHealth()
    {
        if (playerDead == true){
            return;
        }

        currentHealth = 100;   
        healthBar.value = currentHealth; // Update health bar
    }

    private void Die()
    {
        if (!playerDead)
        {
            Debug.Log("Player died!");
            GetComponent<Animator>().SetTrigger("Death");

            GetComponent<PlayerController>().enabled = false;      // Disable movement
            GetComponent<PlayerAttack>().enabled = false;          // Disable attacking

            playerDead = true;

            if (restartButton != null)
                restartButton.SetActive(true);
        }
    }
}
