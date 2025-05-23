using UnityEngine;
using System.Collections;

public class Enemy : MonoBehaviour
{
    [SerializeField] private float speed = 2f;  
    [SerializeField] private float moveDistance = 3f; 
    [SerializeField] private float health = 100;
    [SerializeField] private float attackCooldown = 1.5f; 

    [SerializeField] private GameObject swordHitbox; // Reference to the sword hitbox GameObject

    BoxCollider2D boxCollider;

    private Animator anim;
    private float lastAttackTime;
    private float lastFlipTime;
    private Vector3 startPosition;
    private int direction = 1;  
    private Rigidbody2D rb;
    private bool isAttacking = false;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        boxCollider = GetComponent<BoxCollider2D>();

        startPosition = transform.position;
        lastAttackTime = -attackCooldown; 
        anim.SetBool("isMoving", true);

        swordHitbox.SetActive(false); // Disable sword hitbox initially
    }

    private void Update()
    {
        if (!isAttacking) // Stop movement while attacking
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
        anim.SetTrigger("tookDamage");
        health -= damage;
        
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
        isAttacking = true;
        anim.SetTrigger("attack");
        rb.linearVelocity = Vector2.zero; // Stop movement

        yield return new WaitForSeconds(0.4f); 

        swordHitbox.SetActive(true);

        yield return new WaitForSeconds(0.3f); 

        swordHitbox.SetActive(false); // Disable sword hitbox after swing

        lastAttackTime = Time.time; 
        isAttacking = false;
    }

    private void Die()
    {
        
        anim.SetBool("isMoving", false);
        anim.ResetTrigger("attack");
        anim.SetTrigger("death");
        
        rb.linearVelocity = Vector2.zero; // Stop movement
        boxCollider.enabled = false;
        this.enabled = false;
        
        Invoke("DisableEnemy", 3f);
    }

    private void DisableEnemy()
    {
        gameObject.SetActive(false);
    }

    private void Flip()
    {
        transform.localScale = new Vector3(-transform.localScale.x, transform.localScale.y, transform.localScale.z);
    }
}
