using System;
using UnityEngine;

public class Projectile : MonoBehaviour
{
   [SerializeField] private float speed = 10f;
   private bool hit;
   private BoxCollider2D boxCollider;
   private Animator anim;
   private float direction;

   private float fireDamage = 20;
    private void Awake()
    {
        boxCollider = GetComponent<BoxCollider2D>();
        anim = GetComponent<Animator>();
    }

    private void Update()
    {
        if (hit)
        {
            return;
        }

        float movementSpeed = speed * Time.deltaTime * direction;
        transform.Translate(movementSpeed,0,0);
    }

    public void SetDamage(float damage)
    {
        fireDamage = 2*damage;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Ground"))
        {
            hit = true;
            boxCollider.enabled = false;
            anim.SetTrigger("explode");

            Deactivate(); 
        }
        else if (collision.CompareTag("Enemy"))
        {
            collision.gameObject.GetComponent<Enemy>().TakeDamage(fireDamage);
            hit = true;
            boxCollider.enabled = false;
            anim.SetTrigger("explode");

            Deactivate(); 
        }
        else if (collision.CompareTag("Boss"))
        {
            collision.GetComponent<DragonBossController>()?.TakeDamage((int)fireDamage);
            hit = true;
            boxCollider.enabled = false;
            anim.SetTrigger("explode");

            Deactivate(); 
        }
        else if (collision.CompareTag("Vampire"))
        {
            collision.GetComponent<Vampire>()?.TakeDamage((int)fireDamage);
            hit = true;
            boxCollider.enabled = false;
            anim.SetTrigger("explode");

            // Destroy fireball after explosion animation
            Deactivate(); // Increase delay if explosion animation is longer
        }
        else if (collision.CompareTag("StoneGolem"))
        {
            collision.GetComponent<StoneGolem>()?.TakeDamage((int)fireDamage);
            hit = true;
            boxCollider.enabled = false;
            anim.SetTrigger("explode");

            // Destroy fireball after explosion animation
            Deactivate(); // Increase delay if explosion animation is longer
        }
        else if (collision.CompareTag("CrystalGolem"))
        {
            collision.GetComponent<CrystalGolem>()?.TakeDamage((int)fireDamage);
            hit = true;
            boxCollider.enabled = false;
            anim.SetTrigger("explode");

            // Destroy fireball after explosion animation
            Deactivate(); // Increase delay if explosion animation is longer
        }
        
    }

    public void SetDirection(float _direction)
    {
        direction = _direction;
    
        gameObject.SetActive(true);
        hit = false;
        boxCollider.enabled = true;

        float localScaleX = transform.localScale.x;
        if(Mathf.Sign(localScaleX) != _direction)
        {
            localScaleX *= -1f;
        }
        transform.localScale = new Vector3(localScaleX, transform.localScale.y, transform.localScale.z);
    }

    private void Deactivate()
    {
        Debug.Log("Fireball deactivated!");
        Destroy(gameObject,0.5f); // Small delay ensures animation finishes
    }

}
