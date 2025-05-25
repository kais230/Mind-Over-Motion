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

    // set to true the moment we fire Show(), so we never do it again
    private bool activated = false;

    // We use OnTriggerStay2D so we automatically get called every physics frame
    private void OnTriggerStay2D(Collider2D other)
    {
        if (activated) 
            return;

        if (!other.CompareTag("Player")) 
            return;

        // read current concentration
        float conc = concentrationBar.concentration;

        if (conc < threshold)
        {
            platform.Show();
            activated = true;
        }
    }
}
