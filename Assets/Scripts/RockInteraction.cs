using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class RockInteraction : MonoBehaviour
{
    private Camera mainCamera;
    private bool isHeld = false;
    private Rigidbody rb;
    private Vector3 originalPosition;
    private float distanceToCamera;
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
        if (!isHeld) return;  // Only continue if the rock is held

        // Movement (relative to camera's forward direction without affecting the Y-axis)
        Vector3 newPosition = mainCamera.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, mainCamera.WorldToScreenPoint(transform.position).z));
        transform.position = Vector3.Lerp(transform.position, new Vector3(newPosition.x, transform.position.y, newPosition.z), moveSpeed * Time.deltaTime);

        // Rotation
        float horizontal = Input.GetAxis("Horizontal"); // A and D for X axis rotation
        float vertical = Input.GetAxis("Vertical"); // W and S for Z axis rotation (towards and away from camera)
        float scroll = Input.GetAxis("Mouse ScrollWheel"); // Mouse scroll for Y axis rotation

        transform.Rotate(Vector3.up, scroll * rotateSpeedMouseWheel * Time.deltaTime);
        transform.Rotate(Vector3.right, horizontal * rotateSpeedAD * Time.deltaTime);
        transform.position += vertical * moveSpeed * Time.deltaTime * mainCamera.transform.forward;
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
