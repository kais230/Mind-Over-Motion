using UnityEngine;
using UnityEngine.UI;


// public class ConcentrationBar : MonoBehaviour
// {
//     [SerializeField] private Image barFill; // Drag the UI Image (Fill) here in Inspector
//     public float concentration = 50f; // Start at 50%
//     private float maxConcentration = 100f;
//     private float minConcentration = 0f;
//     private float changeSpeed = 30f; // Adjust speed of increase/decrease

//     private void Update()
//     {
//         // Increase with right arrow, decrease with left arrow
//         if (Input.GetKey(KeyCode.P))
//         {
//             concentration += changeSpeed * Time.deltaTime;
//         }
//         else if (Input.GetKey(KeyCode.O))
//         {
//             concentration -= changeSpeed * Time.deltaTime;
//         }

//         // Clamp value between 0 and 100
//         concentration = Mathf.Clamp(concentration, minConcentration, maxConcentration);

//         // Update UI bar fill
//         barFill.fillAmount = concentration / maxConcentration;
//     }
// }

public class ConcentrationBar : MonoBehaviour
{
    [SerializeField] private Image barFill; 

    private float maxConcentration = 100f;
    private float smoothSpeed = 5f;
    private float changeSpeed = 30f; 
    public float concentration = 0f;
    FocusReceiver FocusInstance;

    private void Start()
    {
        FocusInstance = FindFirstObjectByType<FocusReceiver>();
    }
    private void Update()
    {
        // Get real-time value from the Muse headset via FocusReceiver
        if(!FocusInstance.isConnected)
        {
            if (Input.GetKey(KeyCode.P)) // Increase with P, decrease with O, using this if headerset is not connected
            {
                concentration += changeSpeed * Time.deltaTime;
            }
            else if (Input.GetKey(KeyCode.O))
            {
                concentration -= changeSpeed * Time.deltaTime;
            }
        }
        else
        {
            float targetConcentration = Mathf.Clamp(FocusReceiver.CurrentFocusScore, 0f, 100f);
            concentration = Mathf.Lerp(concentration, targetConcentration, Time.deltaTime * smoothSpeed);
        }
        
        // Update bar fill
        barFill.fillAmount = concentration / maxConcentration;
        
    }
}