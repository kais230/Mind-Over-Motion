using UnityEngine;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

public class FocusReceiver : MonoBehaviour
{
    UdpClient client;
    Thread receiveThread;
    int focusScore = 50; // Default focus score
    public static int CurrentFocusScore => instance?.focusScore ?? 50; // Default to 50 if instance is null
    private static FocusReceiver instance;

    public bool isConnected = false; // Flag to check if connected to Muse headset

    void Start()
    {
        instance = this;
        client = new UdpClient(5005);
        receiveThread = new Thread(new ThreadStart(ReceiveData));
        receiveThread.IsBackground = true;
        receiveThread.Start();
    }

     private void OnDestroy()
    {
        // stop the background thread
        if (receiveThread != null && receiveThread.IsAlive)
            receiveThread.Abort();

        // free the UDP port
        if (client != null)
            client.Close();
    }

    void ReceiveData()
    {
        IPEndPoint anyIP = new IPEndPoint(IPAddress.Any, 0);
        while (true)
        {
            try
            {
                byte[] data = client.Receive(ref anyIP);
                string text = Encoding.UTF8.GetString(data);
                int.TryParse(text, out focusScore);

                Debug.Log("Focus score received: " + focusScore); // Add this line
                isConnected = true; 
            }
            catch (System.Exception e)
            {
                Debug.LogError("UDP receive error: " + e.Message);
            }
        }
    }

    void OnApplicationQuit()
    {
        receiveThread?.Abort();
        client?.Close();
    }
}
