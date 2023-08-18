using UnityEngine;

public class CameraManager : MonoBehaviour
{
    public Vector3 defaultCameraPosition;
    public Vector3 zoomedInPosition;
    public Transform stackCenter; // This should be set to wherever the center of your stack is

    private Transform cameraTransform;
    private float panSpeed = 50.0f; // Speed at which the camera pans around the stack
    private float zoomSpeed = 2.0f; // Speed at which the camera zooms in or out
    private float cinematicSpeed = 1.0f; // Speed for cinematic focus

    private void Start()
    {
        cameraTransform = this.transform; // Reference to the camera's transform
        defaultCameraPosition = cameraTransform.position; // Optionally set this in Start if not set in the inspector
    }

    // Method to pan the camera around the focal point based on mouse movement
    public void PanCamera(Vector2 mouseMovement)
    {
        // Convert mouse movement into rotation around the focal point
        float horizontalRotation = mouseMovement.x * panSpeed * Time.deltaTime;
        float verticalRotation = -mouseMovement.y * panSpeed * Time.deltaTime;

        // Rotate the camera around the stack center
        cameraTransform.RotateAround(stackCenter.position, Vector3.up, horizontalRotation);
        cameraTransform.RotateAround(stackCenter.position, cameraTransform.right, verticalRotation);
    }

    // Method to zoom in the camera to the zoomed-in position
    public void ZoomIn()
    {
        cameraTransform.position = Vector3.Lerp(cameraTransform.position, zoomedInPosition, zoomSpeed * Time.deltaTime);
    }

    // Method to reset the camera position to its default position
    public void ZoomOut()
    {
        cameraTransform.position = Vector3.Lerp(cameraTransform.position, defaultCameraPosition, zoomSpeed * Time.deltaTime);
    }

    // Method for cinematic focus on the stack
    public void CinematicFocus()
    {
        // This is a simple approach, you might want to add more logic for smoother transitions and effects
        Vector3 cinematicPosition = defaultCameraPosition * 0.5f; // Halfway between the default and the stack, as an example
        cameraTransform.position = Vector3.Lerp(cameraTransform.position, cinematicPosition, cinematicSpeed * Time.deltaTime);
        cameraTransform.LookAt(stackCenter); // Make sure the camera is always focused on the stack center during this movement
    }
}
