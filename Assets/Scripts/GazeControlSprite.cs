using System;
using System.Net.Sockets;
using System.Text;
using UnityEngine;
using System.Collections; 
using Newtonsoft.Json.Linq;  // For JSON parsing

public class GazeController : MonoBehaviour
{
    private TcpClient client;
    private NetworkStream stream;
    private byte[] receivedBuffer;
    public float speed = 5.0f;  // Speed to move the ship
    private Vector2 moveDirection;
    private Vector2 previousGazeCoords = Vector2.zero;

    // Example center coordinate for comparison (could be a fixed point or calculated)
    private Vector2 centerPupil = new Vector2(Screen.width / 2, Screen.height / 2); 

    // Start is called before the first frame update
     void Start()
    {
        StartCoroutine(ConnectToServerAfterDelay(10f));  // Adjust the delay as needed
    }

    IEnumerator ConnectToServerAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);

        try
        {
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

                // Log raw JSON data for debugging
                Debug.Log($"Raw JSON Data: {jsonData}");

                // Parse the JSON data using Newtonsoft.Json
                JObject gazeData = JObject.Parse(jsonData);

                // Directly deserialize as arrays (lists in Python)
                float[] centerCoord = gazeData["average_pupil"]?.ToObject<float[]>();

                // Log the parsed array and its length
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

                    // Log the received coordinates
                    Debug.Log($"Current Gaze Coordinates: {currentGazeCoords}");

                    if (previousGazeCoords != Vector2.zero)
                    {
                        // Calculate the direction vector from previous to current gaze coordinates
                        Vector2 direction = currentGazeCoords - previousGazeCoords;

                        // Calculate the unit direction vector
                        Vector2 unitDirection = direction.normalized;

                        // Log the direction and unit vector
                        Debug.Log($"Direction: {direction}");
                        Debug.Log($"Unit Direction: {unitDirection}");

                        // Update the movement direction based on unitDirection
                        moveDirection = unitDirection;
                    }
                    else
                    {
                        // Initialize previousGazeCoords if it's the first valid data
                        previousGazeCoords = currentGazeCoords;
                        Debug.Log("Initialized previous gaze coordinates.");
                    }

                    // Update the previous gaze coordinates for the next frame
                    previousGazeCoords = currentGazeCoords;

                    // Log the movement direction
                    Debug.Log($"Move Direction: {moveDirection}");
                }
                else
                {
                    // If the data is invalid or incomplete, stop moving the ship
                    moveDirection = Vector2.zero;
                    Debug.Log("Invalid or incomplete gaze data received. Stopping movement.");
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

    private void OnApplicationQuit()
    {
        // Close connections when the application quits
        if (stream != null) stream.Close();
        if (client != null) client.Close();
    }
}
