using UnityEngine;
using System.Collections;

public class RockStacking : MonoBehaviour
{
    public AudioSource audioSource;
    public AudioClip placeSound;
    public AudioClip unstableSound;
    public ParticleSystem dustEffect;
    public GameObject[] stones;

    private GameObject selectedStone;
    private Camera mainCamera;
    private float rotationSpeed = 50f;
    private float currentDistance;
    private bool isRotating = false;

    private void Awake()
    {
        mainCamera = Camera.main;  // Assumes you have only one camera in the scene tagged as "MainCamera"
    }

    private void Update()
    {
        StoneSelection();
        PlaceStone();

        if (selectedStone != null)
        {
            MoveStone();
            RotateStone();
        }
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

    void MoveStone()
    {
        if (!isRotating)
        {
            Vector3 mousePos = Input.mousePosition;
            mousePos.z = currentDistance;
            Vector3 worldPos = mainCamera.ScreenToWorldPoint(mousePos);
            selectedStone.transform.position = worldPos;
        }
    }

    void RotateStone()
    {
        if (Input.GetKey(KeyCode.LeftShift))
        {
            isRotating = true;
            float rotationX = Input.GetAxis("Mouse X") * rotationSpeed * Time.deltaTime;
            float rotationY = Input.GetAxis("Mouse Y") * rotationSpeed * Time.deltaTime;
            selectedStone.transform.Rotate(Vector3.up, rotationX);
            selectedStone.transform.Rotate(Vector3.right, rotationY);
        }
        else
        {
            isRotating = false;

            float horizontalRotation = Input.GetAxis("Horizontal") * rotationSpeed * Time.deltaTime;
            float verticalRotation = Input.GetAxis("Vertical") * rotationSpeed * Time.deltaTime;

            selectedStone.transform.Rotate(Vector3.up, horizontalRotation);
            selectedStone.transform.Rotate(Vector3.right, verticalRotation);

            if (Input.GetKey(KeyCode.Q))
            {
                selectedStone.transform.Rotate(Vector3.forward, rotationSpeed * Time.deltaTime);
            }
            if (Input.GetKey(KeyCode.E))
            {
                selectedStone.transform.Rotate(Vector3.forward, -rotationSpeed * Time.deltaTime);
            }

            if (Input.GetKeyDown(KeyCode.R))
            {
                selectedStone.transform.rotation = Quaternion.identity;
            }
        }
    }

    void PlaceStone()
    {
        // Place the stone on left mouse click
        if (Input.GetMouseButtonDown(0) && selectedStone != null)
        {
            if (IsStoneStable(selectedStone))
            {
                // Play sound effect for successful placement
                audioSource.PlayOneShot(placeSound);

                // Play a visual effect (e.g., a small dust cloud) at the stone's base
                dustEffect.transform.position = selectedStone.transform.position;
                dustEffect.Play();

                selectedStone = null; // Unselect the stone
            }
            else
            {
                // Indicate that the stone is unstable. This could be a shake effect, a sound, etc.
                StartCoroutine(ShakeStone(selectedStone));
                audioSource.PlayOneShot(unstableSound);
            }
        }

        bool IsStoneStable(GameObject stone)
        {
            // Check if stone's vertical velocity is close to zero
            Rigidbody stoneRb = stone.GetComponent<Rigidbody>();
            if (Mathf.Abs(stoneRb.velocity.y) > 0.1f) return false;

            // Check if stone is making contact with another stone below it
            RaycastHit hit;
            if (Physics.Raycast(stone.transform.position, Vector3.down, out hit))
            {
                if (hit.collider.CompareTag("Stone")) return true;
            }

            return false;
        }

        IEnumerator ShakeStone(GameObject stone)
        {
            Vector3 originalPosition = stone.transform.position;
            float shakeAmount = 0.5f;
            float shakeDuration = 0.5f;

            for (float elapsed = 0; elapsed < shakeDuration; elapsed += Time.deltaTime)
            {
                // Check if the stone object is still valid
                if (stone == null) yield break;

                float xOffset = Random.Range(-shakeAmount, shakeAmount);
                float zOffset = Random.Range(-shakeAmount, shakeAmount);
                stone.transform.position = new Vector3(originalPosition.x + xOffset, originalPosition.y, originalPosition.z + zOffset);
                yield return null;
            }

            // Again, check if the stone object is still valid before setting its position
            if (stone != null)
            {
                stone.transform.position = originalPosition;
            }
        }
    }

        // - Check the stability of the stack.
        // - Add sounds, effects, etc.
        // - Handle scoring.
        // - Possibly interact with other systems like leaderboards, challenges, etc.
}
