import cv2
import socket
import json
from gaze_tracking.gaze_tracking import GazeTracking
from gaze_tracking_extend import GazeTrackingExtend

# Initialize GazeTracking and socket
gaze = GazeTrackingExtend()
webcam = cv2.VideoCapture(0)
sock = socket.socket(socket.AF_INET, socket.SOCK_STREAM)
sock.connect(('127.0.0.1', 60000))  # Connect to Unity on port 60000

while True:
    _, frame = webcam.read()
    gaze.refresh(frame)

    # Visual feedback (optional)
    

    new_frame = gaze.annotated_frame()
    text = ""

    if gaze.is_right():
        text = "Looking right"
    elif gaze.is_left():
        text = "Looking left"
    elif gaze.is_center():
        text = "Looking center"
    elif gaze.is_up():
        text = "Looking up"
    elif gaze.is_down():
        text = "Looking down"


    gaze.log_pupil_coordinates()

    cv2.putText(new_frame, text, (60, 60), cv2.FONT_HERSHEY_DUPLEX, 2, (255, 0, 0), 2)
    cv2.imshow("Demo", new_frame)

    # Get horizontal and vertical gaze ratios
    horizontal_ratio = gaze.horizontal_ratio() or 0.5  # Default to 0.5 if not detected
    vertical_ratio = gaze.vertical_ratio() or 0.5  # Default to 0.5 if not detected

    left_pupil = gaze.pupil_left_coords()
    if gaze.pupils_located:
        left_x = float(left_pupil[0])
        left_y = float(left_pupil[1])
    else:
        left_x = None
        left_y = None

    # Prepare data to send as JSON
    gaze_data = {
        "gaze_left_x": left_x,
        "gaze_left_y": left_y
        
    }
    json_data = json.dumps(gaze_data)

    # Send gaze data to Unity
    sock.sendall(json_data.encode('utf-8'))

    if cv2.waitKey(1) == 27:
        break

# Close resources
webcam.release()
cv2.destroyAllWindows()
sock.close()

