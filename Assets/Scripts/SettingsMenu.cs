// using UnityEngine;
// using UnityEngine.UI;
// using TMPro;
// using System.Diagnostics;
// using System.Collections;

// public class SettingsMenu : MonoBehaviour
// {
//     [Header("UI References")]
//     public GameObject settingsPanel;
//     public TextMeshProUGUI statusText;
//     public Button connectButton;

//     [Header("Python Launcher")]
//     [Tooltip("Usually 'py' on Windows or 'python' on Mac/Linux")]
//     public string pythonLauncher = "py";

//     [Header("Muse-Connect Command")]
//     [Tooltip("Everything after the launcher. e.g. \"-3.11 -m muselsl stream\"")]
//     [TextArea(1,2)]
//     public string connectArgs = "-3.11 -m muselsl stream";
//     [Tooltip("Folder where the above command should run")]
//     public string connectWorkingDir = 
//         @"C:\Users\kais2\Mind Over Motion"; 

//     [Header("Concentration-Streamer Command")]
//     [Tooltip("Everything after the launcher. e.g. \"-3.11 focus_streamer.py\"")]
//     [TextArea(1,2)]
//     public string receiveArgs = "-3.11 focus_streamer.py";
//     [Tooltip("Folder where focus_streamer.py lives")]
//     public string receiveWorkingDir = 
//         @"C:\Users\kais2\Mind Over Motion\Python_Concentration_Level";

//     private Process connectProcess;
//     private Process receiveProcess;
//     private bool isRunning = false;

//     void Start()
//     {
//         settingsPanel.SetActive(false);
//         connectButton.onClick.AddListener(OnConnectClicked);
//     }

//     void OnConnectClicked()
//     {
//         if (!isRunning)
//             StartCoroutine(RunConnectionSequence());
//     }

//     IEnumerator RunConnectionSequence()
//     {
//         isRunning = true;
//         statusText.text = "CONNECTING...";
//         statusText.color = Color.yellow;
//         connectButton.interactable = false;

//         // 1) Launch the "muse-connect" command
//         var psi1 = new ProcessStartInfo(pythonLauncher, connectArgs)
//         {
//             UseShellExecute = false,
//             RedirectStandardOutput = true,
//             CreateNoWindow = true,
//             WorkingDirectory = connectWorkingDir
//         };
//         connectProcess = Process.Start(psi1);
//         UnityEngine.Debug.Log($"Started connect: {pythonLauncher} {connectArgs}");

//         // give it a moment to finish pairing
//         yield return new WaitForSeconds(15f);

//         // 2) Launch the "focus_streamer" script
//         var psi2 = new ProcessStartInfo(pythonLauncher, receiveArgs)
//         {
//             UseShellExecute = false,
//             RedirectStandardOutput = true,
//             CreateNoWindow = true,
//             WorkingDirectory = receiveWorkingDir
//         };
//         receiveProcess = Process.Start(psi2);
//         UnityEngine.Debug.Log($"Started streamer: {pythonLauncher} {receiveArgs}");

//         statusText.text = "CONNECTED";
//         statusText.color = Color.green;
//     }

//     void OnDestroy()
//     {
//         if (connectProcess != null && !connectProcess.HasExited)
//             connectProcess.Kill();
//         if (receiveProcess != null && !receiveProcess.HasExited)
//             receiveProcess.Kill();
//     }
// }


// using UnityEngine;
// using UnityEngine.UI;
// using TMPro;
// using System.Diagnostics;
// using System.Collections;

// public class SettingsMenu : MonoBehaviour
// {
//     [Header("UI References")]
//     public GameObject settingsPanel;
//     public TextMeshProUGUI statusText;
//     public Button connectButton;

//     [Header("Connection Watcher")]
//     [Tooltip("Your FocusReceiver script that sets isConnected = true on first data")]
//     public FocusReceiver focusReceiver;

//     [Header("Python Launcher")]
//     [Tooltip("Usually 'py' on Windows or 'python' on Mac/Linux")]
//     public string pythonLauncher = "py";

//     [Header("Muse-Connect Command")]
//     [TextArea(1,2)]
//     public string connectArgs = "-3.11 -m muselsl stream";
//     public string connectWorkingDir = @"C:\Users\kais2\Mind Over Motion";

//     [Header("Concentration-Streamer Command")]
//     [TextArea(1,2)]
//     public string receiveArgs = "-3.11 focus_streamer.py";
//     public string receiveWorkingDir = @"C:\Users\kais2\Mind Over Motion\Python_Concentration_Level";

//     private Process connectProcess;
//     private Process receiveProcess;
//     private bool isRunning = false;

//     void Start()
//     {
//         // hide panel at start
//         settingsPanel.SetActive(false);

//         // initialize status
//         SetStatus("NOT CONNECTED", Color.red);

//         // wire up our connect button
//         connectButton.onClick.AddListener(OnConnectClicked);

//         // auto-find the focus receiver if not set
//         if (focusReceiver == null)
//             focusReceiver = FindFirstObjectByType<FocusReceiver>();
//     }

//     void Update()
//     {
//         // if we’re marked “connected” but the streamer process died, go back to NOT CONNECTED
//         if (isRunning && receiveProcess != null && receiveProcess.HasExited)
//         {
//             //Debug.LogWarning("Streamer process exited—marking disconnected");
//             SetStatus("NOT CONNECTED", Color.red);
//             connectButton.interactable = true;
//             isRunning = false;
//         }
//     }

//     void OnConnectClicked()
//     {
//         if (isRunning) return;

//         // kill any stray Python scripts + free port
//         KillProcesses();

//         // small safety delay to ensure OS has torn down the socket
//         StartCoroutine(DelayedStart(0.2f));
//     }

//     IEnumerator DelayedStart(float delay)
//     {
//         yield return new WaitForSeconds(delay);
//         focusReceiver.isConnected = false;
//         StartCoroutine(RunConnectionSequence());
//     }

//     IEnumerator RunConnectionSequence()
//     {
//         isRunning = true;
//         connectButton.interactable = false;
//         SetStatus("CONNECTING…", Color.yellow);

//         // 1) Launch the “muse‐connect” command
//         var psi1 = new ProcessStartInfo(pythonLauncher, connectArgs) {
//             UseShellExecute    = false,
//             RedirectStandardOutput = true,
//             CreateNoWindow     = true,
//             WorkingDirectory   = connectWorkingDir
//         };
//         connectProcess = Process.Start(psi1);

//         // Blind wait for pairing
//         const float pairingWait = 8f;
//         yield return new WaitForSeconds(pairingWait);

//         // 2) Launch the “focus_streamer” script
//         var psi2 = new ProcessStartInfo(pythonLauncher, receiveArgs) {
//             UseShellExecute       = false,
//             RedirectStandardOutput  = true,
//             CreateNoWindow        = true,
//             WorkingDirectory      = receiveWorkingDir
//         };
//         receiveProcess = Process.Start(psi2);

//         // 3) Now wait up to dataTimeout for real data
//         float timer   = 0f;
//         const float dataTimeout = 12f;
//         while (timer < dataTimeout && (focusReceiver == null || !focusReceiver.isConnected))
//         {
//             SetStatus($"CONNECTING… ({Mathf.Ceil(dataTimeout - timer)}s)", Color.yellow);
//             yield return new WaitForSeconds(1f);
//             timer += 1f;
//         }

//         // 4) Success or failure?
//         if (focusReceiver != null && focusReceiver.isConnected)
//         {
//             SetStatus("CONNECTED", Color.green);
//         }
//         else
//         {
//             // timed out → clean up so port 5005 is freed
//             //Debug.LogWarning("[Settings] Connection attempt timed out.");
//             KillProcesses();
//             SetStatus("NOT CONNECTED", Color.red);
//         }

//         // in either case, allow retry
//         connectButton.interactable = true;
//         isRunning = false;
//     }

//     void OnDestroy()
//     {
//         KillProcesses();
//     }

//     private void KillProcesses()
//     {
//         if (connectProcess != null && !connectProcess.HasExited)
//             connectProcess.Kill();
//         if (receiveProcess != null && !receiveProcess.HasExited)
//             receiveProcess.Kill();
//     }

//     private void SetStatus(string text, Color col)
//     {
//         statusText.text = text;
//         statusText.color = col;
//     }
// }

using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Diagnostics;
using System.Collections;

public class SettingsMenu : MonoBehaviour
{
    enum ConnectionState { Disconnected, Connecting, Connected }
    public FocusReceiver focusReceiver;
    [Header("UI References")]
    public GameObject settingsPanel;
    public TextMeshProUGUI statusText;
    public Button connectButton;
    public Button  settingsButton;

    [Header("Python Launcher")]
    public string pythonLauncher = "py";

    [Header("Muse Connect")]
    [TextArea(1,2)] public string connectArgs = "-3.11 -m muselsl stream";
    public string connectWorkingDir = @"C:\Users\kais2\Mind Over Motion";

    [Header("Streamer")]
    [TextArea(1,2)] public string receiveArgs = "-3.11 focus_streamer.py";
    public string receiveWorkingDir = @"C:\Users\kais2\Mind Over Motion\Python_Concentration_Level";

    private Process connectProcess;
    private Process receiveProcess;

    private ConnectionState state = ConnectionState.Disconnected;

    const float pairingWait = 8f;
    const float dataTimeout = 12f;  // total = 20s

    void Start()
    {
        if (focusReceiver == null)
            focusReceiver = FindFirstObjectByType<FocusReceiver>();

        TransitionToDisconnected();
        connectButton.onClick.AddListener(OnConnectClicked);
    }

    

    void Update()
    {
        // If we ever drop out while supposedly Connected, go back to Disconnected
        if (state == ConnectionState.Connected)
        {
            bool streamDead = receiveProcess != null && receiveProcess.HasExited;
            bool lostData   = focusReceiver != null && !focusReceiver.isConnected;
            if (streamDead || lostData)
                TransitionToDisconnected();
        }
    }

    void OnConnectClicked()
    {
        if (state != ConnectionState.Disconnected)
            return;

        StartCoroutine(ConnectionRoutine());
    }

    IEnumerator ConnectionRoutine()
    {
        TransitionToConnecting();

        // ensure no old processes holding the port
        KillProcesses();
        if (focusReceiver != null)
            focusReceiver.isConnected = false;

        // 1) Launch pairing script
        connectProcess = StartPython(connectArgs, connectWorkingDir);

        // wait for pairing
        yield return new WaitForSeconds(pairingWait);

        // 2) Launch streamer
        receiveProcess = StartPython(receiveArgs, receiveWorkingDir);

        // 3) Wait for real data up to dataTimeout
        float timer = 0f;
        while (timer < dataTimeout && (focusReceiver == null || !focusReceiver.isConnected))
        {
            float left = dataTimeout - timer;
            statusText.text = $"CONNECTING… ({left:0}s)";
            yield return new WaitForSeconds(1f);
            timer += 1f;
        }

        // 4) Decide success or fail
        if (focusReceiver != null && focusReceiver.isConnected)
            TransitionToConnected();
        else
        {
            TransitionToDisconnected();
            KillProcesses(); // free port immediately
        }
    }

    void TransitionToDisconnected()
    {
        state = ConnectionState.Disconnected;
        statusText.text = "NOT CONNECTED";
        statusText.color = Color.red;
        connectButton.interactable = true;
    }

    void TransitionToConnecting()
    {
        state = ConnectionState.Connecting;
        statusText.text = "CONNECTING…";
        statusText.color = Color.yellow;
        connectButton.interactable = false;
    }

    void TransitionToConnected()
    {
        state = ConnectionState.Connected;
        statusText.text = "CONNECTED";
        statusText.color = Color.green;
        connectButton.interactable = true; // still allow re-connect/drop-reconnect
    }

    Process StartPython(string args, string workingDir)
    {
        var psi = new ProcessStartInfo(pythonLauncher, args)
        {
            UseShellExecute      = false,
            RedirectStandardOutput = true,
            CreateNoWindow       = true,
            WorkingDirectory     = workingDir
        };
        return Process.Start(psi);
    }

    void KillProcesses()
    {
        if (connectProcess != null && !connectProcess.HasExited)
            connectProcess.Kill();
        if (receiveProcess != null && !receiveProcess.HasExited)
            receiveProcess.Kill();
    }

    void OnDestroy()
    {
        KillProcesses();
    }
}
