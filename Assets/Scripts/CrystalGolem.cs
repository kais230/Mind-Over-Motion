using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class CrystalGolem : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    [SerializeField] private float speed = 2f;  // Speed of the vampire
    [SerializeField] private float moveDistance = 5f; // Distance the vampire moves before flipping direction
    [SerializeField] private float health = 300; // Health of the vampire
    [SerializeField] private float attackCooldown = 1.5f; // Cooldown time between attacks
    [SerializeField] private GameObject swordHitbox; // Reference to the sword hitbox GameObject
    private BoxCollider2D boxCollider;
    private Animator anim;
    private float lastAttackTime = -1f; // Initialize to allow immediate first attack
    private float lastFlipTime;

    private Vector3 startPosition;
    private int direction = 1;  // Direction the vampire is facing (1 for right, -1 for left)
    private Rigidbody2D rb;
    private bool isAttacking = false;
    private float lastHitTime = -Mathf.Infinity; // To track the last time the vampire was hit
    private float hitAnimDuration = 0.5f; // Duration of the hit animation
    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        boxCollider = GetComponent<BoxCollider2D>();

        startPosition = transform.position;
        lastAttackTime = -attackCooldown; // Initialize last attack time to allow immediate first attack
        anim.SetBool("MoveBool", true);

        swordHitbox.SetActive(false); // Disable sword hitbox initially
        Flip();
    }


    // Update is called once per frame
    void Update()
    {

        if (!isAttacking) // Stop movement while attacking
        {
            rb.linearVelocity = new Vector2(direction * speed, rb.linearVelocity.y);
        }

        if (Mathf.Abs(transform.position.x - startPosition.x) >= moveDistance && Time.time > lastFlipTime + 0.5f)
        {
            lastFlipTime = Time.time;
            direction *= -1; // Flip direction
            Flip();
        }
    }

    public void TakeDamage(float damage)
    {
        if(isAttacking) return; // Prevent taking damage while attacking

        anim.SetTrigger("StunedTrigger");
        rb.linearVelocity = Vector2.zero;

        if (Time.time >= lastHitTime + hitAnimDuration)
        {
            health -= damage;
            lastHitTime = Time.time; // Update last hit time}
        }

        if (health <= 0)
        {
            Die();
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        //anim.SetTrigger("AttackTrigger");
        if (collision.CompareTag("Player") && Time.time >= lastAttackTime + attackCooldown && !isAttacking)
        {
            StartCoroutine(PerformAttack());
        }
    }

    private IEnumerator PerformAttack()
    {
        anim.SetBool("MoveBool", false);
        isAttacking = true;
        anim.SetTrigger("AttackTrigger"); // Trigger the attack animation
        rb.linearVelocity = Vector2.zero; // Stop movement during attack
        yield return new WaitForSeconds(0.5f); // Wait for the attack animation to start
        swordHitbox.SetActive(true); // Enable the sword hitbox

        yield return new WaitForSeconds(0.4f);// Wait for the attack animation to finish

        swordHitbox.SetActive(false); // Disable the sword hitbox
        lastAttackTime = Time.time; // Update last attack time
        isAttacking = false; // Reset attacking state
        anim.SetBool("MoveBool", true); // Resume movement after attack
    }

    private void Flip()
    {
        Vector3 scale = transform.localScale;
        scale.x *= -1; // Flip the x scale
        transform.localScale = scale; // Apply the flipped scale
    }

    // private void Die()
    // {
    //     // Handle death logic here, e.g., play death animation, disable the vampire, etc.
    //     anim.SetTrigger("DeathTrigger");
    //     rb.linearVelocity = Vector2.zero; // Stop movement
    //     //gameObject.SetActive(false); // Disable the vampire GameObject
    // }
    
    private void Die()
    {
        
        anim.SetBool("MoveBool", false);
        anim.SetTrigger("DeathTrigger");
        
        rb.linearVelocity = Vector2.zero; // Stop movement
        boxCollider.enabled = false;
        this.enabled = false;
        
        Invoke("DisableEnemy", 3f);
    }

    private void DisableEnemy()
    {
        gameObject.SetActive(false);
    }
    

}
