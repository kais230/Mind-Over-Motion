using NUnit.Framework;
using UnityEngine;
using UnityEngine.UIElements;
using System.Collections;

public class PlayerController : MonoBehaviour
{
    private Rigidbody2D body;
    private Animator anim;
    [SerializeField] private float speed;
    [SerializeField] private float jumpForce;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private LayerMask wallLayer;

    [SerializeField] private LayerMask slopeLayer;
    private bool isKnockedBack = false;
    [Header("Knockback Settings")]
    [SerializeField] private float knockbackDuration = 0.3f;
    private Coroutine knockbackRoutine;

    float horizontalInput;
  

    private BoxCollider2D boxCollider;

    private void Awake()
    {
        body = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        boxCollider = GetComponent<BoxCollider2D>();
    }

    private void Update()
    {
        // Move left/right
        horizontalInput = Input.GetAxis("Horizontal");

        if(horizontalInput > 0.01f)
            transform.localScale = new Vector3(1, 1, 1); // Face right
        else if(horizontalInput < -0.01f)
            transform.localScale = new Vector3(-1, 1, 1); // Face left

        if (!isKnockedBack)
        {
            if (!IsGrounded() && OnWall())
            {
                body.linearVelocity = new Vector2(0, body.linearVelocity.y); // Cancel horizontal movement
            }
            else
            {
                body.linearVelocity = new Vector2(horizontalInput * speed, body.linearVelocity.y);
            }
            // Jumping (only when grounded)
            if (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.UpArrow))
            {
                if (IsGrounded())
                {
                    Jump();
                }      
            }
        }

             // Detect falling
            if (body.linearVelocity.y < 0 && !IsGrounded())
            {
                anim.SetBool("Fall", true);
            }
            else
            {
                anim.SetBool("Fall", false);
            }

        // Animation states
        if (IsGrounded() && !isKnockedBack)
        {
            if (horizontalInput == 0)
                anim.SetInteger("AnimState", 0); // Idle
            else
                anim.SetInteger("AnimState", 1); // Running
        }
    
    }

    private void Jump()
    {     
        body.linearVelocity = new Vector2(body.linearVelocity.x, jumpForce); // Instant jump

        anim.SetTrigger("Jump");
        anim.SetBool("Grounded", false);
    }
    
    // Detect when the player lands on the ground
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground")) 
        {
            anim.SetBool("Grounded", true);
            anim.SetBool("Fall", false);
        }
    }

    private bool IsGrounded()
    {
        RaycastHit2D raycastHit = Physics2D.BoxCast(
            boxCollider.bounds.center, 
            boxCollider.bounds.size, 
            0f, 
            Vector2.down, 
            0.1f, 
            groundLayer | slopeLayer // Include slope as ground
        );

        return raycastHit.collider != null;
    }


    private bool OnWall()
    {
        Vector2 direction = new Vector2(transform.localScale.x, 0);
        Vector2 origin = new Vector2(boxCollider.bounds.center.x, boxCollider.bounds.center.y);
        Vector2 size = new Vector2(boxCollider.bounds.size.x * 0.6f, boxCollider.bounds.size.y * 0.9f); 

        RaycastHit2D hit = Physics2D.BoxCast(origin, size, 0f, direction, 0.1f, wallLayer);

        // Only treat it as a wall if it's not a slope
        return hit.collider != null && hit.collider.gameObject.layer != LayerMask.NameToLayer("Slope");
    }


    public void Knockback(Vector2 direction, float force)
    {
        if (knockbackRoutine != null) return;
        knockbackRoutine = StartCoroutine(KnockbackRoutine(direction.normalized * force));
    }

    private IEnumerator KnockbackRoutine(Vector2 velocity)
    {
        isKnockedBack = true;
        anim.SetTrigger("Hurt");        
        body.linearVelocity = velocity;

        yield return new WaitForSeconds(knockbackDuration);

        isKnockedBack = false;
        knockbackRoutine = null;
    }
}
