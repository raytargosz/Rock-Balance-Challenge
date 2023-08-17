using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class RockInteraction : MonoBehaviour
{
    private Camera mainCamera;
    private bool isHeld = false;
    private Rigidbody rb;
    private Vector3 originalPosition;
    private float distanceToCamera;
    private Renderer rockRenderer;
    private Material originalMaterial;  // Store the original material
    public Material outlineMaterial;    // Assign this in the inspector with your outlining shader material

    [Header("Interaction Settings")]
    public float liftSpeed = 2f;
    public float moveSpeed = 2f;
    public float rotateSpeedAD = 30f;
    public float rotateSpeedMouseWheel = 20f;

    [Header("Physics Settings")]
    public float rockMass = 50f;
    public float groundedMass = 200f;
    public float groundedDrag = 10f;

    [Header("Drop Indicator Settings")]
    public GameObject dropIndicatorPrefab;
    private GameObject dropIndicatorInstance;

    private void Start()
    {
        rockRenderer = GetComponent<Renderer>();
        mainCamera = Camera.main;
        rb = GetComponent<Rigidbody>();
        originalPosition = transform.position;
        rb.mass = rockMass;

        // Instantiate drop indicator
        dropIndicatorInstance = Instantiate(dropIndicatorPrefab);
        dropIndicatorInstance.SetActive(false);

        originalMaterial = GetComponent<Renderer>().material;  // Store the original material
    }

    private void Update()
    {
        HoverOverRock();
        HandleInteraction();
        UpdateDropIndicator();
    }

    private void HoverOverRock()
    {
        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit) && hit.transform == this.transform)
        {
            // Change material to outline
            GetComponent<Renderer>().material = outlineMaterial;

            if (Input.GetKeyDown(KeyCode.Space))
            {
                if (isHeld)
                    ReleaseRock();
                else
                    StartHolding();
            }
        }
        else
        {
            // Reset to original material
            GetComponent<Renderer>().material = originalMaterial;
        }
    }

    private void HandleInteraction()
    {
        if (!isHeld) return;

        // Movement (relative to world's direction)
        float moveX = Input.GetAxis("Mouse X");
        float moveY = Input.GetAxis("Mouse Y");
        Vector3 newPos = transform.position + new Vector3(moveX, moveY, 0) * moveSpeed * Time.deltaTime;

        if (!Physics.Raycast(newPos, Vector3.down, 0.5f)) // Assuming 0.5f as half the height of the rock
            transform.position = newPos;

        // Rotation
        float horizontal = Input.GetAxis("Horizontal"); // A and D for X axis rotation
        float vertical = Input.GetAxis("Vertical"); // W and S for Z axis rotation (towards and away from the camera)
        float scroll = Input.GetAxis("Mouse ScrollWheel"); // Mouse scroll for Y axis rotation

        transform.Rotate(Vector3.up, scroll * rotateSpeedMouseWheel * Time.deltaTime);
        transform.Rotate(Vector3.right, horizontal * rotateSpeedAD * Time.deltaTime);
        transform.Rotate(Vector3.forward, vertical * rotateSpeedMouseWheel * Time.deltaTime);
    }

    private void UpdateDropIndicator()
    {
        if (!isHeld) return;

        RaycastHit hitInfo;
        if (Physics.Raycast(transform.position, -Vector3.up, out hitInfo))
        {
            dropIndicatorInstance.transform.position = hitInfo.point;
            dropIndicatorInstance.transform.LookAt(mainCamera.transform.position);
        }
    }

    private void StartHolding()
    {
        isHeld = true;
        rb.isKinematic = true;
        dropIndicatorInstance.SetActive(true);  // Assuming you want to show this as soon as the rock is held
    }

    private void ReleaseRock()
    {
        isHeld = false;
        rb.isKinematic = false;
        dropIndicatorInstance.SetActive(false);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (isHeld) return;

        // Increase mass and drag if rock is grounded to make it feel heavy
        if (collision.gameObject.CompareTag("Ground"))
        {
            rb.mass = groundedMass;
            rb.drag = groundedDrag;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Boundary"))
        {
            transform.position = originalPosition;
        }
    }
}
