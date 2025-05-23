using UnityEngine;
using UnityEngine.UI; // Add this for Unity UI Button

public class SettingsButton : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public Button settingsButton; // assign in Inspector
    public GameObject settingsPanel; // assign in Inspector
    void Start()
    {
        settingsButton.onClick.AddListener(ToggleSettingsPanel);
    }
    
    private void ToggleSettingsPanel()
{
    // flip visibility
    bool isOpen = settingsPanel.activeSelf;
    settingsPanel.SetActive(!isOpen);
}
}
