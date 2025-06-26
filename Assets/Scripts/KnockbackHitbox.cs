using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Collider2D), typeof(Rigidbody2D))]
public class KnockbackHitbox : MonoBehaviour
{
    [Tooltip("Force of knockback impulse")]
    public float knockbackForce = 20f;
    [Tooltip("Damage on body contact (if any)")]
    public int contactDamage = 10;

    [Header("Delay Before Knockback")]
    [Tooltip("Seconds the player must stay in contact before knockback triggers")]
    public float contactDuration = 1f;

    private Coroutine contactTimer;

    private void OnTriggerEnter2D(Collider2D col)
    {
        if (!col.CompareTag("Player")) return;

        contactTimer = StartCoroutine(ContactCountdown(col));
    }

    private void OnTriggerExit2D(Collider2D col)
    {
        if (!col.CompareTag("Player")) return;

        if (contactTimer != null)
        {
            StopCoroutine(contactTimer);
            contactTimer = null;
        }
    }

    private IEnumerator ContactCountdown(Collider2D col)
    {
        // wait for the required duration
        yield return new WaitForSeconds(contactDuration);

        DoKnockback(col);

        contactTimer = null;
    }

    private void DoKnockback(Collider2D col)
    {
        col.GetComponent<PlayerHealth>()?.TakeDamage(contactDamage);

        Vector2 dir = (col.transform.position - transform.position).normalized;
        var pc = col.GetComponent<PlayerController>();
        if (pc != null)
        {
            pc.Knockback(dir, knockbackForce);
        }
        else
        {
            var rb = col.GetComponent<Rigidbody2D>();
            if (rb != null)
                rb.AddForce(dir * knockbackForce, ForceMode2D.Impulse);
        }
    }
}
