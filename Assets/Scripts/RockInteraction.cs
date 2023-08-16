using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class RockInteraction : MonoBehaviour
{
    [Header("Highlighting")]
    public Shader outlineShader;
    private Shader defaultShader;
    private bool isHighlighted = false;

    [Header("Interaction")]
    private Camera mainCamera;
    private bool isHeld = false;
    private Vector3 offset;
    public float holdDistance = 2f;
    public float moveSpeed = 2f; // Movement speed towards and away from the camera
    private Rigidbody rb;

    [Header("Rotation Controls")]
    public float rotisserieRotationSpeed = 20f;
    public float popShoveItRotationSpeed = 30f;
    private bool isFrozen = false;
    private Vector3 originalPosition;

    void Start()
    {
        originalPosition = transform.position;
        defaultShader = GetComponent<Renderer>().material.shader;
        mainCamera = Camera.main;
        rb = GetComponent<Rigidbody>();
    }

    void Update()
    {
        HandleHighlighting();

        if (isHeld)
        {
            HandleHoldingLogic();
        }
    }

    void HandleHighlighting()
    {
        RaycastHit hit;
        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out hit) && hit.transform == this.transform)
        {
            if (!isHighlighted)
            {
                GetComponent<Renderer>().material.shader = outlineShader;
                isHighlighted = true;
            }

            if (Input.GetMouseButtonDown(0))
            {
                StartHolding(hit.point);
            }
        }
        else if (isHighlighted)
        {
            GetComponent<Renderer>().material.shader = defaultShader;
            isHighlighted = false;
        }
    }

    void StartHolding(Vector3 clickPosition)
    {
        Camera.main.GetComponent<CameraController>().SetRockInteractionActive(true);

        isHeld = true;
        offset = clickPosition - transform.position;
        rb.isKinematic = true;
        rb.drag = 10;  // Adding drag to dampen forces while interacting
        rb.angularDrag = 10;  // Dampen rotational forces too
    }

    void HandleHoldingLogic()
    {
        float vertical = Input.GetAxis("Vertical");
        Vector3 moveDirection = mainCamera.transform.forward;
        transform.position += moveDirection * vertical * moveSpeed * Time.deltaTime; // Adjusted this line to move the rock towards and away from the camera.

        float maxDistance = 5f;
        Vector3 displacement = transform.position - originalPosition;

        if (displacement.magnitude > maxDistance)
        {
            transform.position = originalPosition + displacement.normalized * maxDistance;
        }

        if (Input.GetMouseButtonUp(0))
        {
            Release();
            return;
        }

        Vector3 mouseScreenPosition = new Vector3(Input.mousePosition.x, Input.mousePosition.y, mainCamera.WorldToScreenPoint(transform.position).z);
        Vector3 mouseWorldPosition = mainCamera.ScreenToWorldPoint(mouseScreenPosition);
        transform.position = new Vector3(mouseWorldPosition.x - offset.x, mouseWorldPosition.y - offset.y, transform.position.z);

        if (Input.GetKeyDown(KeyCode.Space))
        {
            ToggleFreezeRotation();
        }

        if (!isFrozen)
        {
            HandleRotationControls();
        }
    }

    void ToggleFreezeRotation()
    {
        isFrozen = !isFrozen;
        if (isFrozen)
        {
            rb.constraints = RigidbodyConstraints.FreezeRotation;
        }
        else
        {
            rb.constraints = RigidbodyConstraints.None;
        }
    }

    void HandleRotationControls()
    {
        float mouseWheel = Input.GetAxis("Mouse ScrollWheel");
        transform.Rotate(Vector3.up * mouseWheel * rotisserieRotationSpeed * Time.deltaTime * 50); // Multiplied by 50 for a better sensitivity control using the mouse wheel

        float horizontal = Input.GetAxis("Horizontal");
        transform.Rotate(Vector3.forward * horizontal * popShoveItRotationSpeed * Time.deltaTime);
    }

    void Release()
    {
        Camera.main.GetComponent<CameraController>().SetRockInteractionActive(false);

        rb.velocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;

        isHeld = false;
        rb.isKinematic = false;

        rb.drag = 0;  // Reset drag
        rb.angularDrag = 0;  // Reset angular drag
    }
}
