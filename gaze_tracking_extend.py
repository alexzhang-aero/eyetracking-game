from gaze_tracking.gaze_tracking import GazeTracking
import cv2
from gaze_tracking.gaze_tracking.eye import Eye

class GazeTrackingExtend(GazeTracking):
    """
    This class extends the GazeTracking class by adding functionality
    to track gaze in different screen regions and logging pupil coordinates.
    """

    def __init__(self):
        super().__init__()  # Call the parent class constructor
        self.log = []  # To store logs of pupil positions
    
    def log_pupil_coordinates(self):
        """Logs the current pupil coordinates to a list"""
        if self.pupils_located:
            left_coords = self.pupil_left_coords()
            right_coords = self.pupil_right_coords()
            self.log.append({
                'left_pupil': left_coords,
                'right_pupil': right_coords
            })
            print(f"Left Pupil: {left_coords}, Right Pupil: {right_coords}")

    def is_up(self):
        """Returns true if the user is looking to the right"""
        if self.pupils_located:
            return self.vertical_ratio() <= 0.48

    def is_down(self):
        """Returns true if the user is looking to the left"""
        if self.pupils_located:
            return self.vertical_ratio() >= 0.52

    def calibrate(self, eye_frame_left, eye_frame_right):
        """Collect calibration data for both eyes."""
        if not self.calibration.is_complete():
            self.calibration.evaluate(eye_frame_left, 0)  # Calibrate left eye
            self.calibration.evaluate(eye_frame_right, 1)  # Calibrate right eye

    def is_calibrated(self):
        """Check if the calibration is complete."""
        return self.calibration.is_complete()

    def _analyze(self):
        """Detect the face and initialize Eye objects."""
        frame = cv2.cvtColor(self.frame, cv2.COLOR_BGR2GRAY)
        faces = self._face_detector(frame)

        try:
            landmarks = self._predictor(frame, faces[0])
            self.eye_left = Eye(frame, landmarks, 0, self.calibration)
            self.eye_right = Eye(frame, landmarks, 1, self.calibration)

            # Calibrate if not already calibrated
            if not self.is_calibrated():
                self.calibrate(self.eye_left.frame, self.eye_right.frame)

        except IndexError:
            self.eye_left = None
            self.eye_right = None

    def refresh(self, frame):
        """Refresh the frame and analyze it. Ensure calibration is complete."""
        self.frame = frame
        self._analyze()
        if not self.is_calibrated():
            print("Calibrating...")