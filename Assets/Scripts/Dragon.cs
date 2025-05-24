using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Animator), typeof(Collider2D))]
public class DragonBossController : MonoBehaviour
{
    [Header("Health")]
    public int maxHealth = 1000;
    private int currentHealth;

    [Header("Attack")]
    public float attackCooldown = 4f;       // seconds between attacks
    public int attackDamage = 30;           // how much damage the breath does
    [SerializeField] public Collider2D fireBreathHitbox;     // assign a trigger collider child

    private float lastHitTime = -Mathf.Infinity;
    public float hitAnimDuration = 1.5f;

    private Animator anim;
    private bool isDead = false;

    [Header("Knockback")]
    [Tooltip("How hard the player is pushed away (Impulse)")]
    public float knockbackForce = 10f;
    private bool isAttacking = false; // to prevent multiple hits in one breath

    public int CurrentHealth => currentHealth;
    public float HealthPercent => (float)currentHealth / maxHealth;
    public GameObject healthBarContainer;

    void Awake()
    {
        anim = GetComponent<Animator>();
        currentHealth = maxHealth;
        fireBreathHitbox.enabled = false;
    }

    void Start()
    {
        // begin looping attacks
        StartCoroutine(AttackRoutine());
    }

    private IEnumerator AttackRoutine()
    {
        // wait a bit before first attack
        yield return new WaitForSeconds(2f);

        while (!isDead)
        {
            // trigger attack
            isAttacking = true;
            anim.SetTrigger("SpecialATrigger");

            // your animation should call EnableBreath() / DisableBreath() via events
            // or we can time it here roughly:
            yield return new WaitForSeconds(1f);
            EnableBreath();
            yield return new WaitForSeconds(0.2f);
            DisableBreath();
            isAttacking = false;

            // cooldown before next
            yield return new WaitForSeconds(attackCooldown);
        }
    }

    // Called from code or via Animation Event at the right frame
    public void EnableBreath()
    {
        fireBreathHitbox.enabled = true;
    }

    public void DisableBreath()
    {
        fireBreathHitbox.enabled = false;
    }

    /// <summary>
    /// Call this from your Projectile or melee script when you hit the boss.
    /// </summary>
    public void TakeDamage(int amount)
    {
        if (isDead) return;

        if (!healthBarContainer.activeSelf)
            healthBarContainer.SetActive(true);

        currentHealth -= amount;
        if (Time.time >= lastHitTime + hitAnimDuration)
        {
            lastHitTime = Time.time;
            anim.SetTrigger("StunedTrigger");
        }

        if (currentHealth <= 0)
                Die();
    }

    private void Die()
    {
        isDead = true;
        anim.SetTrigger("DeathTrigger");
        // stop any further attacks
        StopAllCoroutines();
        // optionally disable colliders so breath / body can’t hurt the player
        fireBreathHitbox.enabled = false;
        GetComponent<Collider2D>().enabled = false;
        healthBarContainer.SetActive(false);
        Invoke("DisableEnemy", 3f);
    }
    private void DisableEnemy()
    {
        gameObject.SetActive(false);
    }

    // deal damage to player on breath hit
 
}
