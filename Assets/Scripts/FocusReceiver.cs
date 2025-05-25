// using UnityEngine;
// using System.Net;
// using System.Net.Sockets;
// using System.Text;
// using System.Threading;

// public class FocusReceiver : MonoBehaviour
// {
//     UdpClient client;
//     Thread receiveThread;
//     int focusScore = 50; // Default focus score
//     public static int CurrentFocusScore => instance?.focusScore ?? 50; // Default to 50 if instance is null
//     private static FocusReceiver instance;

//     public bool isConnected = false; // Flag to check if connected to Muse headset

//     void Start()
//     {
//         instance = this;
//         client = new UdpClient(5005);
//         receiveThread = new Thread(new ThreadStart(ReceiveData));
//         receiveThread.IsBackground = true;
//         receiveThread.Start();
//     }

//      private void OnDestroy()
//     {
//         // stop the background thread
//         if (receiveThread != null && receiveThread.IsAlive)
//             receiveThread.Abort();

//         // free the UDP port
//         if (client != null)
//             client.Close();
//     }

//     void ReceiveData()
//     {
//         IPEndPoint anyIP = new IPEndPoint(IPAddress.Any, 0);
//         while (true)
//         {
//             try
//             {
//                 byte[] data = client.Receive(ref anyIP);
//                 string text = Encoding.UTF8.GetString(data);
//                 int.TryParse(text, out focusScore);

//                 Debug.Log("Focus score received: " + focusScore); // Add this line
//                 isConnected = true; 
//             }
//             catch (System.Exception e)
//             {
//                 Debug.LogError("UDP receive error: " + e.Message);
//             }
//         }
//     }

//     void OnApplicationQuit()
//     {
//         receiveThread?.Abort();
//         client?.Close();
//     }
// }

using UnityEngine;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System; // for DateTime
public class FocusReceiver : MonoBehaviour
{
    UdpClient client;
    Thread receiveThread;
    int focusScore = 50; // Default focus score
    public static int CurrentFocusScore => instance?.focusScore ?? 50;
    private static FocusReceiver instance;

    public bool isConnected = false; // Flag to check if connected to Muse headset

    // ← new flag to control the receive loop
    private volatile bool running = false;

     public float disconnectTimeout = 5f;

    private DateTime lastReceiveUtc;

    void Start()
    {
        instance = this;
        lastReceiveUtc = DateTime.UtcNow;

        client = new UdpClient(5005);

        // start loop
        running = true;
        receiveThread = new Thread(ReceiveData)
        {
            IsBackground = true
        };
        receiveThread.Start();
    }

    void ReceiveData()
    {
        IPEndPoint anyIP = new IPEndPoint(IPAddress.Any, 0);

        // ← only loop while running is true
        while (running)
        {
            try
            {
                byte[] data = client.Receive(ref anyIP);
                string text = Encoding.UTF8.GetString(data);
                int.TryParse(text, out focusScore);

                Debug.Log("Focus score received: " + focusScore);
                isConnected = true;
                lastReceiveUtc = DateTime.UtcNow;
            }
            catch (SocketException se)
            {
                // if we closed the client on purpose, just break out
                if (!running)
                    break;

                Debug.LogError("UDP socket error: " + se.Message);
                // you can optionally break or continue depending on how resilient you want it
                break;
            }
            catch (System.Exception e)
            {
                Debug.LogError("UDP receive error: " + e.Message);
                // on other exceptions, decide if you want to break or keep going
                break;
            }
        }

        // Thread will exit here cleanly
    }

     void Update()
    {
        // if we were connected but haven't seen data recently, drop the flag
        if (isConnected && (DateTime.UtcNow - lastReceiveUtc).TotalSeconds > disconnectTimeout)
        {
            Debug.LogWarning("No EEG data for " + disconnectTimeout + "s – marking disconnected");
            isConnected = false;
        }
    }

    private void ShutdownReceiver()
    {
        // signal loop to stop
        running = false;

        // closing the client will unblock client.Receive()
        client?.Close();

        // wait for the thread to finish its current iteration
        if (receiveThread != null && receiveThread.IsAlive)
            receiveThread.Join();
    }

    private void OnDestroy()
    {
        ShutdownReceiver();
    }

    private void OnApplicationQuit()
    {
        ShutdownReceiver();
    }
}

