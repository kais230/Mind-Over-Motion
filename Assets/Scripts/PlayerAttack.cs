using Microsoft.Unity.VisualStudio.Editor;
using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
    private Animator anim;
    private float cooldownTimer = Mathf.Infinity;
    [SerializeField] private float attackCooldown;
    private int attackCount = 1;

    private bool isAttacking = false;

    [SerializeField] private Transform firePoint;
    [SerializeField] private GameObject fireballPrefab;

    [SerializeField] private Collider2D attackHitbox; // Attack area collider

    [SerializeField] private ConcentrationBar concentrationBar;

    private float attackDamage = 20;
    [SerializeField] public FireballCooldownUI cooldownUI;

    private void Awake()
    {
        anim = GetComponent<Animator>();
        attackHitbox.enabled = false;  // Ensure attack hitbox starts disabled

        if (concentrationBar == null)
            concentrationBar = FindAnyObjectByType<ConcentrationBar>();
    }

    private void Update()
    {
        cooldownTimer += Time.deltaTime;
        UpdateDamage();

        if (Input.GetMouseButtonDown(0) && cooldownTimer > attackCooldown)
        {
            Attack();
            attackCount++;
            if (attackCount > 3)
                attackCount = 1;
        }
        else if (Input.GetKeyDown(KeyCode.X) && cooldownTimer > attackCooldown && cooldownUI.IsReady())
        {
            anim.SetTrigger("Attack3");
            AttackFireball();
            cooldownTimer = 0;
            cooldownUI.TriggerCooldown(); // Start UI + logic cooldown
        }
    }

    private void UpdateDamage()
    {
        attackDamage = concentrationBar.concentration;
    }

    private void AttackFireball()
    {
        GameObject fireball = Instantiate(fireballPrefab, firePoint.position, Quaternion.identity);
        
        float fireballSize = Mathf.Lerp(1f, 2.5f, concentrationBar.concentration / 100f);
        fireball.transform.localScale = new Vector3(fireballSize, fireballSize, 1f);

        fireball.GetComponent<Projectile>().SetDamage(attackDamage);
        fireball.GetComponent<Projectile>().SetDirection(Mathf.Sign(transform.localScale.x));
    }

    private void Attack()
    {
        Debug.Log("Player damage: " + attackDamage);

        isAttacking = true;
        attackHitbox.enabled = true;  // Enable attack hitbox

        if (attackCount == 1)
            anim.SetTrigger("Attack1");
        else if (attackCount == 2)
            anim.SetTrigger("Attack2");
        else if (attackCount == 3)
            anim.SetTrigger("Attack3");

        cooldownTimer = 0;

        // Disable attack hitbox after a short delay
        Invoke(nameof(DisableAttackHitbox), 0.1f);
    }

    private void DisableAttackHitbox()
    {
        attackHitbox.enabled = false;
        isAttacking = false;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Enemy") && isAttacking)
        {
            Debug.Log("Player hit an enemy!");
            collision.gameObject.GetComponent<Enemy>().TakeDamage(attackDamage);
        }
        else if (collision.CompareTag("Boss") && isAttacking)
        {
            collision.GetComponent<DragonBossController>()
             ?.TakeDamage((int)attackDamage);
        }
    }
}
