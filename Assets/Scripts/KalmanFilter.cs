public class KalmanFilter
{
    private float A = 1; // State transition coefficient
    private float H = 1; // Measurement coefficient
    private float Q = 0.01f; // Process noise covariance
    private float R = 0.1f; // Measurement noise covariance
    private float P = 1; // Estimate error covariance
    private float X = 0; // State estimate

    public KalmanFilter(float initialPosition)
    {
        X = initialPosition;
    }

    public float Update(float measurement)
    {
        float predictedX = A * X; // Predicted location
        float predictedP = A * P * A + Q; // Predicted covariance
        float K = predictedP * H / (H * predictedP * H + R); // Kalman Gain
        X = predictedX + K * (measurement - H * predictedX);  // Update estimate
        P = (1 - K * H) * predictedP;  // Update error covariance

        return X;  
    }
}
