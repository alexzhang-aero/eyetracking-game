import cv2
import socket
import json
import numpy as np
from gaze_tracking.gaze_tracking import GazeTracking
from gaze_tracking_extend import GazeTrackingExtend

# Initialize GazeTracking and socket
gaze = GazeTrackingExtend()
webcam = cv2.VideoCapture(0)

# Create a server socket that listens for a connection from Unity
server_socket = socket.socket(socket.AF_INET, socket.SOCK_STREAM)
server_socket.bind(('127.0.0.1', 60000))  
server_socket.listen(1)  

print("Waiting for a connection from Unity...")

# Accept the connection from Unity
client_socket, addr = server_socket.accept()
print(f"Connection established with {addr}")

try:
    while True:
        # Capture frame from the webcam
        _, frame = webcam.read()
        gaze.refresh(frame)

        # Get the annotated frame for display
        new_frame = gaze.annotated_frame()

        # Display the text on the frame
        cv2.imshow("Demo", cv2.flip(new_frame, 1))

        # Get pupil coordinates 
        average_pupil = gaze.center_coords()

        # not very memory efficient: can use improvements
        average_pupil = list(map(int, average_pupil)) if average_pupil else None

        # Prepare gaze data dictionary
        gaze_data = {
            "average_pupil": average_pupil
        }

        # Convert gaze data to JSON string
        json_data = json.dumps(gaze_data)

        # Send gaze data to Unity
        try:
            client_socket.sendall((json_data + "\n").encode('utf-8'))
        except Exception as e:
            print(f"Error sending data to Unity: {e}")

        # Esc to exit
        if cv2.waitKey(1) == 27:
            break

except Exception as e:
    print(f"An error occurred: {e}")

finally:
    webcam.release()
    cv2.destroyAllWindows()
    client_socket.close()  
    server_socket.close()  
    print("Sockets and webcam resources released")


