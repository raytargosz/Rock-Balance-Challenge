using UnityEngine;

public class InputManager : MonoBehaviour
{
    public StoneManager stoneManager; // Reference to StoneManager script
    public CameraManager cameraManager; // Reference to CameraManager script

    private Vector2 mouseMovement; // Store the mouse movement

    private void Update()
    {
        CheckInputs();
    }

    public void CheckInputs()
    {
        // Check for mouse movement
        mouseMovement = new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"));

        // If right mouse button is pressed and held, either pan the camera or control the stone
        if (Input.GetMouseButton(1))
        {
            if (stoneManager.IsStonePickedUp) // Assuming IsStonePickedUp is a public property in StoneManager
            {
                // Control stone placement with mouse movement
                stoneManager.MoveStone(mouseMovement); // Assuming MoveStone is a method in StoneManager to handle stone movement
            }
            else
            {
                cameraManager.PanCamera(mouseMovement);
            }
        }

        // Right click to pick up or drop the stone
        if (Input.GetMouseButtonDown(1))
        {
            if (stoneManager.IsStonePickedUp)
            {
                stoneManager.DropStone();
            }
            else
            {
                stoneManager.PickUpStone();
            }
        }

        // Check for stone rotation inputs
        if (stoneManager.IsStonePickedUp)
        {
            if (Input.GetKey(KeyCode.D))
                stoneManager.RotateStone(Vector3.up, 1f); // Rotate right
            else if (Input.GetKey(KeyCode.A))
                stoneManager.RotateStone(Vector3.up, -1); // Rotate left
            else if (Input.GetKey(KeyCode.W))
                stoneManager.RotateStone(Vector3.right, 1); // Tilt up
            else if (Input.GetKey(KeyCode.S))
                stoneManager.RotateStone(Vector3.right, -1); // Tilt down
            else if (Input.GetKey(KeyCode.Q))
                stoneManager.RotateStone(Vector3.forward, 1); // Tilt in one direction
            else if (Input.GetKey(KeyCode.E))
                stoneManager.RotateStone(Vector3.forward, -1); // Tilt in opposite direction
        }

        // Middle mouse button for free rotation
        if (Input.GetMouseButton(2) && stoneManager.IsStonePickedUp)
        {
            stoneManager.FreeRotateStone(mouseMovement);
        }
    }
}
