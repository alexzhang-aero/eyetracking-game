using UnityEngine;
using System.Net.Sockets;
using System.Text;
using System;

public class GazeControlledSprite : MonoBehaviour
{
    private TcpListener listener;
    private TcpClient client;
    private NetworkStream stream;
    public GameObject sprite;  // Drag your sprite GameObject here

    void Start()
    {
        listener = new TcpListener(System.Net.IPAddress.Parse("127.0.0.1"), 60000);
        listener.Start();
        Debug.Log("Waiting for connection from Python...");
    }

    void Update()
    {
        if (listener.Pending())
        {
            client = listener.AcceptTcpClient();
            stream = client.GetStream();

            byte[] data = new byte[1024];
            int bytesRead = stream.Read(data, 0, data.Length);
            string jsonData = Encoding.UTF8.GetString(data, 0, bytesRead);

            ProcessGazeData(jsonData);
        }
    }

    void ProcessGazeData(string jsonData)
    {
        // Deserialize JSON data to extract horizontal and vertical ratios
        GazeData gazeData = JsonUtility.FromJson<GazeData>(jsonData);

        // Convert gaze ratios into screen space position
        Vector2 screenPos = new Vector2(gazeData.horizontal_ratio * Screen.width, (1 - gazeData.vertical_ratio) * Screen.height);

        // Move the sprite to the new position
        sprite.transform.position = Camera.main.ScreenToWorldPoint(new Vector3(screenPos.x, screenPos.y, 10));  // Z-value may depend on your setup

        Debug.Log($"Gaze Position: {screenPos.x}, {screenPos.y}");
    }

    // Ensure proper cleanup
    void OnApplicationQuit()
    {
        stream?.Close();
        client?.Close();
        listener.Stop();
    }

    [Serializable]
    public class GazeData
    {
        public float horizontal_ratio;
        public float vertical_ratio;
    }
}
