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

    [Header("Physics Properties")]
    public float rockMass = 50f;          // Making the rock heavier
    public float rockDrag = 4f;           // Higher drag makes it stop moving faster when released
    public float rockAngularDrag = 2f;    // Higher angular drag makes it stop rotating faster when released

    void Start()
    {
        originalPosition = transform.position;
        defaultShader = GetComponent<Renderer>().material.shader;
        mainCamera = Camera.main;
        rb = GetComponent<Rigidbody>();

        rb.mass = rockMass;                // Set the mass of the rock
        rb.drag = rockDrag;                // Set the drag of the rock
        rb.angularDrag = rockAngularDrag;  // Set the angular drag of the rock
    }

    void Update()
    {
        CheckIfGrounded();

        HandleHighlighting();

        if (isHeld)
        {
            HandleHoldingLogic();
        }
    }

    void CheckIfGrounded()
    {
        RaycastHit hit;
        if (Physics.Raycast(transform.position, Vector3.down, out hit, 1.1f))
        {
            if (!isHeld)  // If the rock is grounded and not held
            {
                rb.mass = 100;  // Increase the mass to make it harder to move
                rb.drag = 10;   // Increase drag for more resistance
            }
        }
        else
        {
            rb.mass = 1;  // Reset the mass to its original value
            rb.drag = 0;  // Reset drag
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
        float verticalInput = Input.GetAxis("Vertical");
        transform.position += mainCamera.transform.forward * verticalInput * moveSpeed * Time.deltaTime;

        Vector3 mouseScreenPosition = new Vector3(Input.mousePosition.x, Input.mousePosition.y, mainCamera.WorldToScreenPoint(transform.position).z);
        Vector3 mouseWorldPosition = mainCamera.ScreenToWorldPoint(mouseScreenPosition);
        transform.position = new Vector3(mouseWorldPosition.x - offset.x, mouseWorldPosition.y - offset.y, transform.position.z);

        if (Input.GetMouseButtonUp(0))
        {
            Release();
            return;
        }

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

        isHeld = false;
        rb.isKinematic = false;

        rb.drag = rockDrag;  // Set to default rock drag
        rb.angularDrag = 0;  // Reset angular drag
    }

}
