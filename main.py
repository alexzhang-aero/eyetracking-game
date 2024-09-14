import cv2
from gaze_tracking.gaze_tracking import GazeTracking
from gaze_tracking_extend import GazeTrackingExtend

gaze = GazeTrackingExtend()
webcam = cv2.VideoCapture(0)

while True:
    _, frame = webcam.read()
    gaze.refresh(frame)

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

    if cv2.waitKey(1) == 27:
        break