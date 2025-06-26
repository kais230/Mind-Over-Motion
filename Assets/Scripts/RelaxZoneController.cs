using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class RelaxZoneController : MonoBehaviour
{
    [Tooltip("The platform that will fade in")]
    public PlatformFade platform;

    [Tooltip("Your concentration provider (0–100)")]
    public ConcentrationBar concentrationBar;

    [Tooltip("Threshold under which the platform appears")]
    public float threshold = 35f;
    private bool activated = false;

    private void OnTriggerStay2D(Collider2D other)
    {
        if (activated) 
            return;

        if (!other.CompareTag("Player")) 
            return;

        float conc = concentrationBar.concentration;

        if (conc < threshold)
        {
            platform.Show();
            activated = true;
        }
    }
}
