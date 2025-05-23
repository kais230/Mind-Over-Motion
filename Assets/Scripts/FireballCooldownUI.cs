using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using TMPro; 


public class FireballCooldownUI : MonoBehaviour
{
    public Image fireballIcon;
    public TextMeshProUGUI cooldownText;

    public float cooldownDuration = 5f;

    private bool isOnCooldown = false;

    private Color originalColor;

    private void Start()
    {
        originalColor = fireballIcon.color;
        cooldownText.text = "READY!";
        //cooldownText.enabled = false;
    }

    public void TriggerCooldown()
    {
        if (!isOnCooldown)
        {
            StartCoroutine(CooldownRoutine());
        }
    }

    private IEnumerator CooldownRoutine()
    {
        isOnCooldown = true;

        // Tint the icon gray
        fireballIcon.color = Color.gray;

        // Start countdown
        float remaining = cooldownDuration;
        cooldownText.enabled = true;

        while (remaining > 0)
        {
            cooldownText.text = Mathf.CeilToInt(remaining).ToString();
            yield return new WaitForSeconds(1f);
            remaining -= 1f;
        }

        cooldownText.text = "READY!";
        // Reset icon and text
        fireballIcon.color = originalColor;
        //cooldownText.enabled = false;
        isOnCooldown = false;
    }

    public bool IsReady()
    {
        return !isOnCooldown;
    }
}
