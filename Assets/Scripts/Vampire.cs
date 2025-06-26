using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class Vampire : MonoBehaviour
{
    [SerializeField] private float speed = 2f;  
    [SerializeField] private float moveDistance = 3f; 
    [SerializeField] private float health = 500; 
    [SerializeField] private float attackCooldown = 1.5f; 
    [SerializeField] private GameObject swordHitbox; 
    private BoxCollider2D boxCollider;
    private Animator anim;
    private float lastAttackTime = -1f; 
    private float lastFlipTime;

    private Vector3 startPosition;
    private int direction = 1;  
    private Rigidbody2D rb;
    private bool isAttacking = false;
    private float lastHitTime = -Mathf.Infinity; 
    private float hitAnimDuration = 0.5f; 
    public bool VampireDefeated { get; private set; } = false; // Track if the vampire is defeated
    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        boxCollider = GetComponent<BoxCollider2D>();

        startPosition = transform.position;
        lastAttackTime = -attackCooldown;
        anim.SetBool("MoveBool", true);

        swordHitbox.SetActive(false);
        Flip();
    }

    void Update()
    {

        if (!isAttacking) 
        {
            rb.linearVelocity = new Vector2(direction * speed, rb.linearVelocity.y);
        }

        if (Mathf.Abs(transform.position.x - startPosition.x) >= moveDistance && Time.time > lastFlipTime + 0.5f)
        {
            lastFlipTime = Time.time;
            direction *= -1; 
            Flip();
        }
    }

    public void TakeDamage(float damage)
    {
        if(isAttacking) return; 

        anim.SetTrigger("StunedTrigger");

        if (Time.time >= lastHitTime + hitAnimDuration)
        {
            health -= damage;
            lastHitTime = Time.time;
        }

        if (health <= 0)
        {
            Die();
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && Time.time >= lastAttackTime + attackCooldown && !isAttacking)
        {
            StartCoroutine(PerformAttack());
        }
    }

    private IEnumerator PerformAttack()
    {
        anim.SetBool("MoveBool", false);
        isAttacking = true;
        anim.SetTrigger("ClimbTrigger"); // Trigger the attack animation, even if its named "Climb"
        rb.linearVelocity = Vector2.zero; // Stop movement during attack
        yield return new WaitForSeconds(0.5f); // Wait for the attack animation to start
        swordHitbox.SetActive(true);

        yield return new WaitForSeconds(0.4f);// Wait for the attack animation to finish

        swordHitbox.SetActive(false);
        lastAttackTime = Time.time; 
        isAttacking = false; 
        anim.SetBool("MoveBool", true);
    }

    private void Flip()
    {
        Vector3 scale = transform.localScale;
        scale.x *= -1; 
        transform.localScale = scale; 
    }
    
    private void Die()
    {
        VampireDefeated = true;
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
