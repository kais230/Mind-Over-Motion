using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Diagnostics;
using System.Collections;

public class SettingsMenu : MonoBehaviour
{
    [Header("UI References")]
    public GameObject settingsPanel;
    public TextMeshProUGUI statusText;
    public Button connectButton;

    [Header("Python Launcher")]
    [Tooltip("Usually 'py' on Windows or 'python' on Mac/Linux")]
    public string pythonLauncher = "py";

    [Header("Muse-Connect Command")]
    [Tooltip("Everything after the launcher. e.g. \"-3.11 -m muselsl stream\"")]
    [TextArea(1,2)]
    public string connectArgs = "-3.11 -m muselsl stream";
    [Tooltip("Folder where the above command should run")]
    public string connectWorkingDir = 
        @"C:\Users\kais2\Mind Over Motion"; 

    [Header("Concentration-Streamer Command")]
    [Tooltip("Everything after the launcher. e.g. \"-3.11 focus_streamer.py\"")]
    [TextArea(1,2)]
    public string receiveArgs = "-3.11 focus_streamer.py";
    [Tooltip("Folder where focus_streamer.py lives")]
    public string receiveWorkingDir = 
        @"C:\Users\kais2\Mind Over Motion\Python_Concentration_Level";

    private Process connectProcess;
    private Process receiveProcess;
    private bool isRunning = false;

    void Start()
    {
        settingsPanel.SetActive(false);
        connectButton.onClick.AddListener(OnConnectClicked);
    }

    void OnConnectClicked()
    {
        if (!isRunning)
            StartCoroutine(RunConnectionSequence());
    }

    IEnumerator RunConnectionSequence()
    {
        isRunning = true;
        statusText.text = "CONNECTING...";
        connectButton.interactable = false;

        // 1) Launch the "muse-connect" command
        var psi1 = new ProcessStartInfo(pythonLauncher, connectArgs)
        {
            UseShellExecute = false,
            RedirectStandardOutput = true,
            CreateNoWindow = true,
            WorkingDirectory = connectWorkingDir
        };
        connectProcess = Process.Start(psi1);
        UnityEngine.Debug.Log($"Started connect: {pythonLauncher} {connectArgs}");

        // give it a moment to finish pairing
        yield return new WaitForSeconds(15f);

        // 2) Launch the "focus_streamer" script
        var psi2 = new ProcessStartInfo(pythonLauncher, receiveArgs)
        {
            UseShellExecute = false,
            RedirectStandardOutput = true,
            CreateNoWindow = true,
            WorkingDirectory = receiveWorkingDir
        };
        receiveProcess = Process.Start(psi2);
        UnityEngine.Debug.Log($"Started streamer: {pythonLauncher} {receiveArgs}");

        statusText.text = "CONNECTED";
    }

    void OnDestroy()
    {
        if (connectProcess != null && !connectProcess.HasExited)
            connectProcess.Kill();
        if (receiveProcess != null && !receiveProcess.HasExited)
            receiveProcess.Kill();
    }
}
