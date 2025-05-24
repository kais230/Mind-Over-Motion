using UnityEngine;
using UnityEngine.UI;

public class BossHealthBarUI : MonoBehaviour
{
    [Tooltip("Drag in your DragonBossController here")]
    public DragonBossController boss;

    [Tooltip("The UI Slider whose value goes from 0 → 1")]
    public Slider healthSlider;

    void Start()
    {
        // make sure slider is normalized
        healthSlider.minValue = 0f;
        healthSlider.maxValue = 1f;
    }

    void Update()
    {
        if (boss != null && healthSlider != null)
            healthSlider.value = boss.HealthPercent;
    }
}
