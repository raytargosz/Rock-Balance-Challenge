using UnityEngine;

public class CairnsCameraController : MonoBehaviour
{
    public Transform focusPoint;
    public float orbitSpeed = 1.0f;
    public float zoomSpeed = 1.0f;
    public float verticalShiftSpeed = 1.0f;
    public float minZoom = 5.0f;
    public float maxZoom = 15.0f;
    public float initialDistanceFromFocus = 10.0f;
    [Range(5, 85)] public float maxVerticalAngle = 80f;  // Added this to constrain vertical rotation.

    private Camera cam;

    private void Start()
    {
        cam = GetComponent<Camera>();
        SetInitialCameraPosition();
    }

    void SetInitialCameraPosition()
    {
        transform.position = focusPoint.position - transform.forward * initialDistanceFromFocus;
    }

    private void Update()
    {
        OrbitCamera();
        ZoomCamera();
        VerticalShift();
    }

    void OrbitCamera()
    {
        if (Input.GetMouseButton(0))
        {
            float horizontalInput = Input.GetAxis("Mouse X");
            float verticalInput = Input.GetAxis("Mouse Y");

            transform.RotateAround(focusPoint.position, Vector3.up, horizontalInput * orbitSpeed);

            // Calculate desired vertical rotation
            float desiredRotationX = transform.eulerAngles.x - verticalInput * orbitSpeed;

            // Clamping vertical rotation to prevent over-rotation
            if (desiredRotationX < 180f)
                desiredRotationX = Mathf.Clamp(desiredRotationX, 5f, maxVerticalAngle);
            else
                desiredRotationX = Mathf.Clamp(desiredRotationX, 360f - maxVerticalAngle, 355f);

            // Apply vertical rotation
            transform.rotation = Quaternion.Euler(desiredRotationX, transform.eulerAngles.y, transform.eulerAngles.z);
        }
    }

    void ZoomCamera()
    {
        float scrollInput = Input.mouseScrollDelta.y;
        float zoomFactor = scrollInput * zoomSpeed;

        float desiredZoom = cam.transform.localPosition.z + zoomFactor;
        desiredZoom = Mathf.Clamp(desiredZoom, -maxZoom, -minZoom);

        cam.transform.localPosition = new Vector3(0, 0, desiredZoom);
    }

    void VerticalShift()
    {
        if (Input.GetKey(KeyCode.W))
        {
            focusPoint.transform.position += Vector3.up * verticalShiftSpeed * Time.deltaTime;
        }
        else if (Input.GetKey(KeyCode.S))
        {
            focusPoint.transform.position -= Vector3.up * verticalShiftSpeed * Time.deltaTime;
        }

        transform.LookAt(focusPoint.position);
    }
}
