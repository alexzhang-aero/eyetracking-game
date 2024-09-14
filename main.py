import cv2
from gaze_tracking.gaze_tracking import GazeTracking
from gaze_tracking_extend import GazeTrackingExtend


gaze = GazeTrackingExtend()  # Extended class that includes calibration logic
webcam = cv2.VideoCapture(0)

calibration_complete = False  # Flag to track if calibration is done

while True:
    _, frame = webcam.read()
    
    if not calibration_complete:
        # Perform calibration
        gaze.refresh(frame)
        
        # Check if calibration is complete
        if gaze.is_calibrated():
            print("Calibration complete!")
            calibration_complete = True
        else:
            print("Calibrating... Please look straight ahead.")
            cv2.putText(frame, "Calibrating... Please look straight ahead.", (60, 60), cv2.FONT_HERSHEY_DUPLEX, 1, (255, 0, 0), 2)
            cv2.imshow("Calibration", frame)

    else:
        # Once calibration is complete, proceed with gaze tracking
        gaze.refresh(frame)
        
        # Annotate the frame with gaze tracking results
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

        # Log pupil coordinates (if you have this method in GazeTrackingExtend)
        gaze.log_pupil_coordinates()

        # Display the text on the frame
        cv2.putText(new_frame, text, (60, 60), cv2.FONT_HERSHEY_DUPLEX, 2, (255, 0, 0), 2)
        cv2.imshow("Gaze Tracking", new_frame)

    # Exit the loop if the user presses 'Esc' (key code 27)
    if cv2.waitKey(1) == 27:
        break

# Release resources
webcam.release()
cv2.destroyAllWindows()
