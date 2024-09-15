using System;
using System.Net.Sockets;
using System.Text;
using UnityEngine;
using Newtonsoft.Json.Linq;  // For JSON parsing

public class GazeController : MonoBehaviour
{
    private TcpClient client;
    private NetworkStream stream;
    private byte[] receivedBuffer;
    public float speed = 5.0f;  // Speed to move the ship
    private Vector2 moveDirection;
    private Vector2 previousGazeCoords = Vector2.zero;

    private KalmanFilter kalmanFilterX;
    private KalmanFilter kalmanFilterY;

    private Vector2 centerPupil = new Vector2(Screen.width / 2, Screen.height / 2);

    void Start()
    {
        try
        {
            // Initialize the Kalman filters
            kalmanFilterX = new KalmanFilter(centerPupil.x);
            kalmanFilterY = new KalmanFilter(centerPupil.y);

            // Connect to the Python server
            client = new TcpClient("127.0.0.1", 60000);
            stream = client.GetStream();
            receivedBuffer = new byte[1024];
            stream.BeginRead(receivedBuffer, 0, receivedBuffer.Length, OnDataReceived, null);
        }
        catch (Exception e)
        {
            Debug.LogError("Error connecting to server: " + e.Message);
        }
    }

    void Update()
    {
        transform.Translate(moveDirection * speed * Time.deltaTime);
    }

    private void OnDataReceived(IAsyncResult result)
    {
        try
        {
            int bytesRead = stream.EndRead(result);

            if (bytesRead > 0)
            {
                string jsonData = Encoding.UTF8.GetString(receivedBuffer, 0, bytesRead).Trim();
                Debug.Log($"Raw JSON Data: {jsonData}");
                JObject gazeData = JObject.Parse(jsonData);
                float[] centerCoord = gazeData["average_pupil"]?.ToObject<float[]>();

                if (centerCoord != null)
                {
                    Debug.Log($"Parsed Center Coordinates: {string.Join(", ", centerCoord)}");
                }
                else
                {
                    Debug.LogWarning("Center coordinates data is null.");
                }

                if (centerCoord != null && centerCoord.Length >= 2)
                {
                    Vector2 currentGazeCoords = new Vector2(centerCoord[0], centerCoord[1]);

                    // Apply Kalman filter 
                    float smoothedX = kalmanFilterX.Update(currentGazeCoords.x);
                    float smoothedY = kalmanFilterY.Update(currentGazeCoords.y);
                    Vector2 smoothedGazeCoords = new Vector2(smoothedX, smoothedY);

                    Debug.Log($"Current Gaze Coordinates: {smoothedGazeCoords}");

                    if (previousGazeCoords != Vector2.zero)
                    {
                        Vector2 direction = smoothedGazeCoords - previousGazeCoords;
                        Vector2 unitDirection = direction.normalized;
                        moveDirection = -unitDirection * 8;
                    }
                    else
                    {
                        previousGazeCoords = smoothedGazeCoords;
                    }

                    previousGazeCoords = smoothedGazeCoords;
                    Debug.Log($"Move Direction: {moveDirection}");
                }
                else
                {
                    moveDirection = Vector2.zero;
                    Debug.Log("Invalid or incomplete gaze data received.");
                }

                stream.BeginRead(receivedBuffer, 0, receivedBuffer.Length, OnDataReceived, null);
            }
        }
        catch (Exception e)
        {
            Debug.LogError("Error receiving data: " + e.Message);
        }
    }

    private void OnApplicationQuit()
    {
        // Close connections when the application quits
        if (stream != null) stream.Close();
        if (client != null) client.Close();
    }
}
