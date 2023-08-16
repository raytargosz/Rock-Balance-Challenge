using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraControl : MonoBehaviour
{
    [Header("Camera Settings")]
    [Tooltip("Speed of camera rotation")]
    public float rotationSpeed = 5f;
    [Tooltip("Damping for camera rotation momentum")]
    public float rotationDamping = 0.1f;

    [Header("Reference")]
    [Tooltip("Reference to the cylinder object")]
    public Transform cylinderTransform;

    private Vector3 lastMousePosition;
    private Vector3 currentVelocity;

    private void Start()
    {
        if (cylinderTransform == null)
        {
            Debug.LogError("Cylinder transform not assigned!");
            return;
        }

        lastMousePosition = Input.mousePosition;
    }

    private void Update()
    {
        if (cylinderTransform == null)
            return;

        if (Input.GetMouseButtonDown(0))
        {
            lastMousePosition = Input.mousePosition;
        }

        if (Input.GetMouseButton(0))
        {
            Vector3 mouseDelta = Input.mousePosition - lastMousePosition;
            float rotationX = -mouseDelta.y * rotationSpeed * Time.deltaTime;
            float rotationY = mouseDelta.x * rotationSpeed * Time.deltaTime;

            Quaternion newRotation = Quaternion.Euler(rotationX, rotationY, 0f);
            transform.parent.rotation *= newRotation;

            lastMousePosition = Input.mousePosition;
        }
        else
        {
            // Apply momentum to the cylinder rotation
            cylinderTransform.Rotate(Vector3.up, currentVelocity.x * Time.deltaTime);
            cylinderTransform.Rotate(Vector3.right, currentVelocity.y * Time.deltaTime);

            // Dampen the momentum over time
            currentVelocity = Vector3.Lerp(currentVelocity, Vector3.zero, rotationDamping);
        }
    }
}