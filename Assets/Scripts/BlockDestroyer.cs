using UnityEngine;

public class BlockDestroyer : MonoBehaviour
{


    [SerializeField] private ConcentrationBar concentrationBar;
    [SerializeField] private GameObject destroyEffect;

    private void Awake()
    {
        if (concentrationBar == null)
            concentrationBar = FindAnyObjectByType<ConcentrationBar>();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player") && concentrationBar.concentration >= 80)
        {
            if (destroyEffect != null)
            {
                Instantiate(destroyEffect, transform.position, Quaternion.identity);
            }

            Destroy(gameObject);
        }
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player") && concentrationBar.concentration >= 80)
        {
            if (destroyEffect != null)
            {
                Instantiate(destroyEffect, transform.position, Quaternion.identity);
            }
            
            Destroy(gameObject);
        }
    }
}
