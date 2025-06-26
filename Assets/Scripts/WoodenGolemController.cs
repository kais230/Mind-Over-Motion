using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Rigidbody2D), typeof(Animator), typeof(Collider2D))]
public class WoodenGolemController : MonoBehaviour
{
    [Header("Patrol Points")]
    public Transform leftPoint;
    public Transform rightPoint;
    [Tooltip("Speed of patrol movement")]
    public float patrolSpeed = 2f;

    [Header("Attack (optional)")]
    public float attackRadius = 1.2f;
    public float attackCooldown = 2f;
    public int   attackDamage = 20;
    public Collider2D attackHitbox;

    private Rigidbody2D rb;
    private Animator anim;
    private bool movingRight = true;
    private float lastAttackTime = -Mathf.Infinity;
    private bool isDead = false;

    void Awake()
    {
        rb   = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();

        if (attackHitbox != null)
            attackHitbox.enabled = false;
    }

    void Start()
    {
        if (leftPoint == null || rightPoint == null)
            Debug.LogError("Left/Right patrol points not set on " + name, this);
    }

    void FixedUpdate()
    {
        if (isDead) return;
        Patrol();
    }

    private void Patrol()
    {
        // 1) Compute direction and set velocity
        float dir = movingRight ? +1f : -1f;
        rb.linearVelocity = new Vector2(dir * patrolSpeed, rb.linearVelocity.y);

        // 2) Face the movement direction
        Vector3 s = transform.localScale;
        if ((dir > 0 && s.x < 0) || (dir < 0 && s.x > 0))
        {
            s.x *= -1;
            transform.localScale = s;
        }

        // 3) Check if we've passed the patrol bounds
        float x = transform.position.x;
        if (movingRight && x >= rightPoint.position.x)
        {
            movingRight = false;
        }
        else if (!movingRight && x <= leftPoint.position.x)
        {
            movingRight = true;
        }

        // 4) (Optional) Attack if player in range—remove this block if you
        //    don't want any chasing/attacking logic.
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, attackRadius);
        foreach (var hit in hits)
        {
            if (hit.CompareTag("Player") && Time.time >= lastAttackTime + attackCooldown)
            {
                StartCoroutine(PerformAttack());
                break;
            }
        }
    }

    private IEnumerator PerformAttack()
    {
        lastAttackTime = Time.time;
        rb.linearVelocity = Vector2.zero;
        anim.SetTrigger("AttackTrigger");

        // wait until your animation event fires EnableAttackHitbox()
        yield return null;

        // animations/events handle hitbox on/off
    }

    // Call this from your AttackHitbox child's OnTriggerEnter2D
    public void DealDamage(Collider2D other)
    {
        if (other.CompareTag("Player"))
            other.GetComponent<PlayerHealth>()?.TakeDamage(attackDamage);
    }

    public void TakeDamage(int amount)
    {
        if (isDead) return;
        anim.SetTrigger("StunnedTrigger");
        // reduce HP etc.
    }

    private void Die()
    {
        isDead = true;
        anim.SetTrigger("DeathTrigger");
        rb.linearVelocity = Vector2.zero;
        GetComponent<Collider2D>().enabled = false;
    }

    // Visualize attack radius if you like
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRadius);
    }
}
