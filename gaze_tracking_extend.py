from gaze_tracking.gaze_tracking import GazeTracking

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
