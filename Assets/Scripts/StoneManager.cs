using UnityEngine;

public class StoneManager : MonoBehaviour
{
    public bool IsStonePickedUp { get; private set; }

    public GameObject currentStone; // Reference to the stone this script is attached to
    public bool isPickedUp = false; // Boolean to check if stone is picked up

    private Rigidbody stoneRigidbody; // Reference to stone's Rigidbody

    private void Start()
    {
        // Get the Rigidbody component from the current stone
        stoneRigidbody = currentStone.GetComponent<Rigidbody>();
    }

    // Method to pick up the stone
    public void PickUpStone()
    {
        if (currentStone == null)
        {
            Debug.LogError("No stone referenced.");
            return;
        }

        isPickedUp = true;

        // Disable physics interactions while stone is picked up
        stoneRigidbody.isKinematic = true;

        // Position the stone in front of the camera
        // You may need to adjust this positioning based on your specific game setup
        Vector3 stonePickupPosition = Camera.main.transform.position + Camera.main.transform.forward * 2.0f;
        currentStone.transform.position = stonePickupPosition;

        // Optional: You can add logic here to snap the stone to a grid or prevent it from intersecting other objects
    }

    public void MoveStone(Vector2 movement)
    {
        // Convert 2D movement to 3D space
        Vector3 targetMovement = new Vector3(movement.x, movement.y, 0);

        // Apply the movement to the stone's position
        currentStone.transform.position += targetMovement;
    }

    public void RotateStone(Vector3 axis, float rotationSpeed)
    {
        // Calculate rotation
        Quaternion rotation = Quaternion.AngleAxis(rotationSpeed, axis);

        // Apply the rotation to the stone
        currentStone.transform.rotation *= rotation;
    }

    // Method to drop the stone
    public void DropStone()
    {
        isPickedUp = false;

        // Enable physics interactions when stone is dropped
        stoneRigidbody.isKinematic = false;
    }

    // Method to rotate the stone
    public void RotateStone(char axis, float direction)
    {
        float rotationSpeed = 50.0f; // Adjust this value as needed for faster/slower rotation

        Vector3 rotationAxis = Vector3.zero;

        // Determine the rotation axis based on the input
        switch (axis)
        {
            case 'X':
                rotationAxis = Vector3.right;
                break;
            case 'Y':
                rotationAxis = Vector3.up;
                break;
            case 'Z':
                rotationAxis = Vector3.forward;
                break;
            default:
                Debug.LogError("Invalid rotation axis provided.");
                return;
        }

        // Rotate the stone
        currentStone.transform.Rotate(rotationAxis, direction * rotationSpeed * Time.deltaTime);
    }

    // Method for free rotation of the stone based on mouse movement
    public void FreeRotateStone(Vector2 mouseMovement)
    {
        float sensitivity = 0.5f; // Adjust this value for higher/lower sensitivity

        // Convert mouse movement into rotation around X and Y axes
        float mouseX = mouseMovement.x * sensitivity * Time.deltaTime;
        float mouseY = -mouseMovement.y * sensitivity * Time.deltaTime;

        currentStone.transform.Rotate(Vector3.up, mouseX, Space.World);
        currentStone.transform.Rotate(Vector3.right, mouseY, Space.World);
    }
}
