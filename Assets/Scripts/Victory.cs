using UnityEngine;

public class Victory : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public GameObject victoryPanel;
    Animator chestAnimator;
    private bool isOpened = false;

    public void ShowVictoryScreen()
    {
        victoryPanel.SetActive(true);
        //Time.timeScale = 0f; // Optional: pause the game
    }
    void Start()
    {
        chestAnimator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {

            if (isOpened == false)
            {
                chestAnimator.SetTrigger("Open chest");
                isOpened = true;
            }
            Invoke("ShowVictoryScreen", 1f); // Delay to allow chest animation to play
            Debug.Log("Player has reached the victory point!");
        }
    }
}
