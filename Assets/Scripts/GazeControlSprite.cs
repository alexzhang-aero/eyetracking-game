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


    // Start is called before the first frame update
    void Start()
    {
        try
        {
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
        // Move the ship based on moveDirection
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

            // Parse the JSON data using Newtonsoft.Json
            JObject gazeData = JObject.Parse(jsonData);

            // Directly deserialize left_pupil and right_pupil as arrays (lists in Python)
            float[] leftCoords = gazeData["left_pupil"]?.ToObject<float[]>();
            float[] rightCoords = gazeData["right_pupil"]?.ToObject<float[]>();

            if (leftCoords != null && rightCoords != null)
            {
                Vector2 leftPupil = new Vector2(leftCoords[0], leftCoords[1]);
                Vector2 rightPupil = new Vector2(rightCoords[0], rightCoords[1]);

                // Calculate the average coordinates of both pupils
                Vector2 currentGazeCoords = (leftPupil + rightPupil) / 2;

                // Calculate the difference between the current and previous gaze coordinates
                Vector2 gazeDelta = currentGazeCoords - previousGazeCoords;

                // Log the received coordinates and delta
                Debug.Log($"Left Pupil: {leftPupil}, Right Pupil: {rightPupil}");
                Debug.Log($"Current Gaze Coordinates: {currentGazeCoords}");
                Debug.Log($"Gaze Delta: {gazeDelta}");

                // Update the movement direction based on gazeDelta
                moveDirection = CalculateDirection(gazeDelta);

                // Log the direction
                Debug.Log($"Move Direction: {moveDirection}");

                // Update the previous gaze coordinates for the next frame
                previousGazeCoords = currentGazeCoords;
            }
            else
            {
                // If either pupil is null, stop moving the ship
                moveDirection = Vector2.zero;
                Debug.Log("No valid gaze data received. Stopping movement.");
            }

            // Continue reading data from the Python script
            stream.BeginRead(receivedBuffer, 0, receivedBuffer.Length, OnDataReceived, null);
        }
    }
    catch (Exception e)
    {
        Debug.LogError("Error receiving data: " + e.Message);
    }
}


    private Vector2 CalculateDirection(Vector2 gazeDelta)
    {
        // Normalize gaze coordinates to a range that maps to movement
        float normalizedX = Mathf.Clamp(gazeDelta.x / Screen.width, -1f, 1f);
        float normalizedY = Mathf.Clamp(gazeDelta.y / Screen.height, -1f, 1f);

        // Translate normalized coordinates to a direction vector
        return new Vector2(normalizedX, normalizedY);
    }

    private void OnApplicationQuit()
    {
        // Close connections when the application quits
        if (stream != null) stream.Close();
        if (client != null) client.Close();
    }
}