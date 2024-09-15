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
server_socket.bind(('127.0.0.1', 60000))  # Bind the server to localhost and port 60000
server_socket.listen(1)  # Listen for 1 client connection

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
        text = ""

        if gaze.is_right():
            text = "Looking right"
        elif gaze.is_left():
            text = "Looking left"
        elif gaze.is_up():
            text = "Looking up"
        elif gaze.is_down():
            text = "Looking down"
        elif gaze.is_center():
            text = "Looking center"

        # Log the pupil coordinates
        gaze.log_pupil_coordinates()

        # Display the text on the frame
        cv2.putText(new_frame, text, (60, 60), cv2.FONT_HERSHEY_DUPLEX, 2, (255, 0, 0), 2)
        cv2.imshow("Demo", new_frame)

        # Get pupil coordinates 
        average_pupil = gaze.center_coords()

        # Convert numpy.int32 to int for JSON serialization
        #left_pupil = tuple(map(int, left_pupil)) if left_pupil else None
        #right_pupil = tuple(map(int, right_pupil)) if right_pupil else None
        # Convert numpy.int32 to int for JSON serialization

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


