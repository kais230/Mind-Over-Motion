using UnityEngine;
using UnityEngine.UI;

public class SettingsButton : MonoBehaviour
{
    [SerializeField] private Button settingsButton;  
    [SerializeField] private GameObject settingsPanel; 

    private void Awake()
    {
        if (settingsPanel != null)
            settingsPanel.SetActive(false);
        else
            Debug.LogError("SettingsPanel not assigned!", this);
    }

    private void OnEnable()
    {
        if (settingsButton != null)
            settingsButton.onClick.AddListener(TogglePanel);
        else
            Debug.LogError("SettingsButton not assigned!", this);
    }

    private void OnDisable()
    {
        if (settingsButton != null)
            settingsButton.onClick.RemoveListener(TogglePanel);
    }

    private void TogglePanel()
    {
        if (settingsPanel == null) return;

        bool wasOpen = settingsPanel.activeSelf;
        settingsPanel.SetActive(!wasOpen);
        Debug.Log($"SettingsPanel was {wasOpen}, now {!wasOpen}");
    }
}
