using UnityEngine;

public class RockStacking : MonoBehaviour
{
    public GameObject[] stones; // Array of available stone prefabs
    private GameObject selectedStone;

    private float rotationSpeed = 50f;

    private void Update()
    {
        StoneSelection();
        PlaceStone();
        RotateStone();
    }

    void StoneSelection()
    {
        // Use mouse wheel to cycle through available stones
        int index = Mathf.Clamp((int)Input.mouseScrollDelta.y, 0, stones.Length - 1);

        if (selectedStone != null)
        {
            Destroy(selectedStone);
        }

        selectedStone = Instantiate(stones[index], new Vector3(0, 5, 0), Quaternion.identity);
    }

    void RotateStone()
    {
        if (selectedStone == null)
            return;

        // Rotate the stone using W/A/S/D and Q/E
        float horizontalRotation = Input.GetAxis("Horizontal") * rotationSpeed * Time.deltaTime;
        float verticalRotation = Input.GetAxis("Vertical") * rotationSpeed * Time.deltaTime;

        selectedStone.transform.Rotate(Vector3.up, horizontalRotation);
        selectedStone.transform.Rotate(Vector3.right, verticalRotation);

        // Tilt the stone with Q and E
        if (Input.GetKey(KeyCode.Q))
        {
            selectedStone.transform.Rotate(Vector3.forward, rotationSpeed * Time.deltaTime);
        }
        if (Input.GetKey(KeyCode.E))
        {
            selectedStone.transform.Rotate(Vector3.forward, -rotationSpeed * Time.deltaTime);
        }

        // Reset stone's orientation with R
        if (Input.GetKeyDown(KeyCode.R))
        {
            selectedStone.transform.rotation = Quaternion.identity;
        }
    }

    void PlaceStone()
    {
        // Place the stone on left mouse click
        if (Input.GetMouseButtonDown(0) && selectedStone != null)
        {
            // Here, you might want to do a physics check to make sure the stone is stable.
            // If it's unstable, you might shake it, play a sound effect, etc.

            selectedStone = null; // Unselect the stone
        }

        // Cancel stone placement on right mouse click
        if (Input.GetMouseButtonDown(1))
        {
            Destroy(selectedStone);
            selectedStone = null;
        }
    }

    // This is a very simple version. You'd also want functions to:
    // - Check the stability of the stack.
    // - Add sounds, effects, etc.
    // - Handle scoring.
    // - Possibly interact with other systems like leaderboards, challenges, etc.
}
