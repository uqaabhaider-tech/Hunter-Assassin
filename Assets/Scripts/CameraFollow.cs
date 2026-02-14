using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform target;       // Drag your Assassin character here
    public Vector3 offset = new Vector3(0, 50, -8); // Position relative to player
    public float smoothSpeed = 0.125f; // Higher number = faster follow

    void LateUpdate()
    {
        if (target == null) return;

        // The position we want the camera to be at
        Vector3 desiredPosition = target.position + offset;

        // Smoothly interpolate between current position and desired position
        // Vector3.Lerp is the magic for "smoothness"
        Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed);

        transform.position = smoothedPosition;

        // Optional: Ensure the camera is always looking at the player
        // transform.LookAt(target); 
    }
}