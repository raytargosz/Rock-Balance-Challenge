using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
[Tooltip("Handles interaction and physics of a rock object.")]
public class RockInteraction : MonoBehaviour
{
    [Header("References")]
    [Tooltip("Outlining shader material for the rock.")]
    public Material outlineMaterial;
    [Tooltip("Prefab of the drop indicator.")]
    public GameObject dropIndicatorPrefab;

    [Header("Interaction Settings")]
    [Tooltip("Speed at which the rock is lifted.")]
    public float liftSpeed = 2f;
    [Tooltip("Speed at which the rock moves.")]
    public float moveSpeed = 2f;
    [Tooltip("Speed of rotation using A/D keys.")]
    public float rotateSpeedAD = 30f;
    [Tooltip("Speed of rotation using the mouse wheel.")]
    public float rotateSpeedMouseWheel = 20f;

    [Header("Physics Settings")]
    [Tooltip("Mass of the rock when it's airborne.")]
    public float rockMass = 50f;
    [Tooltip("Mass of the rock when it's on the ground.")]
    public float groundedMass = 200f;
    [Tooltip("Drag of the rock when it's on the ground.")]
    public float groundedDrag = 10f;

    private Camera mainCamera;
    private bool isHeld = false;
    private Rigidbody rb;
    private Renderer rockRenderer;
    private Material originalMaterial;
    private Vector3 originalPosition;
    private GameObject dropIndicatorInstance;

    private void Start()
    {
        InitializeComponents();
    }

    private void Update()
    {
        HoverOverRock();
        HandleInteraction();
        UpdateDropIndicator();
    }

    private void InitializeComponents()
    {
        mainCamera = Camera.main;
        rockRenderer = GetComponent<Renderer>();
        rb = GetComponent<Rigidbody>();
        originalMaterial = rockRenderer.material;
        originalPosition = transform.position;
        rb.mass = rockMass;

        dropIndicatorInstance = Instantiate(dropIndicatorPrefab);
        dropIndicatorInstance.SetActive(false);
    }

    private void HoverOverRock()
    {
        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hit) && hit.transform == transform)
        {
            rockRenderer.material = outlineMaterial;

            if (Input.GetKeyDown(KeyCode.Space))
            {
                ToggleRockState();
            }
        }
        else
        {
            rockRenderer.material = originalMaterial;
        }
    }

    private void HandleInteraction()
    {
        if (!isHeld) return;

        MoveRock();
        RotateRock();
    }

    private void UpdateDropIndicator()
    {
        if (!isHeld) return;

        if (Physics.Raycast(transform.position, -Vector3.up, out RaycastHit hitInfo))
        {
            dropIndicatorInstance.transform.position = hitInfo.point;
            dropIndicatorInstance.transform.LookAt(mainCamera.transform.position);
        }
    }

    private void ToggleRockState()
    {
        if (isHeld)
            ReleaseRock();
        else
            StartHolding();
    }

    private void MoveRock()
    {
        Vector3 moveDirection = new Vector3(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"), 0);
        Vector3 newPosition = transform.position + moveDirection * moveSpeed * Time.deltaTime;

        if (!Physics.Raycast(newPosition, Vector3.down, 0.5f))
        {
            transform.position = newPosition;
        }
    }

    private void RotateRock()
    {
        float horizontalRotation = Input.GetAxis("Horizontal");
        float verticalRotation = Input.GetAxis("Vertical");
        float scrollRotation = Input.GetAxis("Mouse ScrollWheel");

        transform.Rotate(Vector3.up, scrollRotation * rotateSpeedMouseWheel * Time.deltaTime);
        transform.Rotate(Vector3.right, horizontalRotation * rotateSpeedAD * Time.deltaTime);
        transform.Rotate(Vector3.forward, verticalRotation * rotateSpeedMouseWheel * Time.deltaTime);
    }

    private void StartHolding()
    {
        isHeld = true;
        rb.isKinematic = true;
        dropIndicatorInstance.SetActive(true);
    }

    private void ReleaseRock()
    {
        isHeld = false;
        rb.isKinematic = false;
        dropIndicatorInstance.SetActive(false);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (isHeld || !collision.gameObject.CompareTag("Ground")) return;

        rb.mass = groundedMass;
        rb.drag = groundedDrag;
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Boundary"))
        {
            transform.position = originalPosition;
        }
    }
}