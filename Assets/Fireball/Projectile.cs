using System;
using UnityEngine;

public class FireballProjectile : MonoBehaviour
{
   [SerializeField] private float speed = 10f;
   private bool hit;
   private BoxCollider2D boxCollider;
   private Animator anim;
   private float direction;


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

    private void OnTriggerEnter2D(Collider2D collision)
    {
        hit = true;
        boxCollider.enabled = false;
        anim.SetTrigger("explode");
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
        gameObject.SetActive(false);
    }
}
