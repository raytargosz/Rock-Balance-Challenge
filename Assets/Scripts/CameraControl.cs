using UnityEngine;

[RequireComponent(typeof(Camera))]
[Tooltip("Handles camera controls around the rock.")]
public class CameraController : MonoBehaviour
{
    private Camera cam;
    private AudioSource audioSource;

    // References
    [Header("References")]
    [Tooltip("Transform of the cylinder or rock that the camera interacts with.")]
    public Transform cylinderTransform;

    // Camera Rotation
    [Header("Camera Rotation")]
    [Tooltip("Speed of the camera's rotation.")]
    public float rotationSpeed = 3f;
    [Tooltip("Smoothness of the rotation drag effect.")]
    public float dragSmoothing = 10f;
    [Tooltip("Duration for which the camera maintains rotation momentum.")]
    public float momentumDuration = 2f;
    private float currentMomentumDuration;
    private Vector3 lastMousePosition;
    private Vector3 currentRotationVelocity;

    // Cinematic Effects
    [Header("Cinematic Effects")]
    [Tooltip("Whether the camera is in a cinematic sequence.")]
    public bool isCinematicSequence = false;
    [Tooltip("Target the camera focuses on during the cinematic sequence.")]
    public Transform focusTarget;
    [Tooltip("Speed at which the camera looks at its focus target.")]
    public float cinematicLookAtSpeed = 1.0f;

    // Camera Movement Restrictions
    [Header("Camera Movement Restrictions")]
    [Tooltip("Maximum distance the camera can be from the cylinder's center.")]
    public float maxDistanceFromCylinder = 10f;
    [Tooltip("Minimum distance the camera can be from the cylinder's center.")]
    public float minDistanceFromCylinder = 2f;

    // Keyboard Movement
    [Header("Keyboard Movement")]
    [Tooltip("Speed of camera movement when using the keyboard.")]
    public float keyboardMovementSpeed = 5f;
    [Tooltip("Whether the camera is currently interacting with a rock.")]
    public bool rockInteractionActive = false;

    // Camera Zoom
    [Header("Camera Zoom")]
    [Tooltip("Default field of view for the camera.")]
    [Range(30, 120)] public float defaultFieldOfView = 60f;
    [Tooltip("Minimum field of view for the camera.")]
    [Range(30, 120)] public float minFieldOfView = 30f;
    [Tooltip("Maximum field of view for the camera.")]
    [Range(30, 120)] public float maxFieldOfView = 90f;
    [Tooltip("Speed at which the camera zooms in and out.")]
    public float zoomSpeed = 10f;
    private float targetFieldOfView;

    [Header("Camera Settings")]
    [Tooltip("Speed of camera rotation around the rock using Q/E keys.")]
    public float cameraRotationSpeed = 30f;  // Adjust the default value as needed

    private Transform rockTransform;  // Rock's transform

    // Sound Effects
    [Header("Sound Effects")]
    [Tooltip("Sound effect when the camera zooms in.")]
    public AudioClip zoomInSound;
    [Tooltip("Sound effect when the camera zooms out.")]
    public AudioClip zoomOutSound;
    [Tooltip("Sound effect when the camera moves up.")]
    public AudioClip moveUpSound;
    [Tooltip("Sound effect when the camera moves down.")]
    public AudioClip moveDownSound;
    [Tooltip("Sound effect when the camera moves to the left.")]
    public AudioClip moveLeftSound;
    [Tooltip("Sound effect when the camera moves to the right.")]
    public AudioClip moveRightSound;
    [Tooltip("Cooldown duration between playing sounds.")]
    public float soundCooldown = 0.5f;
    private float zoomSoundCooldown;
    private float moveSoundCooldown;
    [Tooltip("Range for randomizing the pitch of the sound effects.")]
    public Vector2 pitchRange = new Vector2(0.8f, 1.2f);

    private void Start()
    {
        rockTransform = FindObjectOfType<RockInteraction>().transform;
    }

    void Update()
    {
        HandleCameraRotation();
    }

    void HandleCameraRotation()
    {
        if (Input.GetKey(KeyCode.Q))
        {
            transform.RotateAround(rockTransform.position, Vector3.up, -cameraRotationSpeed * Time.deltaTime);
        }
        else if (Input.GetKey(KeyCode.E))
        {
            transform.RotateAround(rockTransform.position, Vector3.up, cameraRotationSpeed * Time.deltaTime);
        }
    }

    private void HandleRockPickup()
    {
        if (Input.GetMouseButtonDown(0))  // Check if the left mouse button is clicked.
        {
            Ray ray = cam.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            // Raycast and check if it hits a rock object.
            if (Physics.Raycast(ray, out hit) && hit.transform.CompareTag("Rock"))  // Assuming your rocks have the tag "Rock".
            {
                SetRockInteractionActive(true);
                cylinderTransform = hit.transform;  // Assign the hit rock to the cylinderTransform for interaction.
            }
        }
    }


    private void HandleCinematicCamera()
    {
        if (focusTarget != null)
        {
            Vector3 relativePos = focusTarget.position - transform.position;
            Quaternion rotation = Quaternion.LookRotation(relativePos);
            transform.rotation = Quaternion.Slerp(transform.rotation, rotation, cinematicLookAtSpeed * Time.deltaTime);
        }
    }

    private void InitializeComponents()
    {
        cam = GetComponent<Camera>();
        targetFieldOfView = defaultFieldOfView;
        audioSource = GetComponent<AudioSource>();
    }

    private void HandleRockInteraction()
    {
        RotateRock();
        CheckForRockDrop();
    }

    private void RotateRock()
    {
        float rotationAmount = Input.mouseScrollDelta.y * rotationSpeed;
        cylinderTransform.Rotate(0, rotationAmount, 0);
    }

    private void CheckForRockDrop()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            SetRockInteractionActive(false);
        }
    }

    private void HandleRotation()
    {
        if (Input.GetMouseButtonDown(0))
            lastMousePosition = Input.mousePosition;

        if (Input.GetMouseButton(0))
            RotateUsingMouse();

        ApplyRotationMomentum();

        // Make sure the camera looks at the cylinder after rotating
        transform.LookAt(cylinderTransform.position);
    }

    private void RotateUsingMouse()
    {
        Vector3 deltaMousePos = Input.mousePosition - lastMousePosition;
        float deltaYaw = deltaMousePos.x * rotationSpeed * Time.deltaTime;
        float deltaPitch = -deltaMousePos.y * rotationSpeed * Time.deltaTime;

        transform.RotateAround(cylinderTransform.position, Vector3.up, deltaYaw);
        transform.RotateAround(cylinderTransform.position, transform.right, deltaPitch);

        lastMousePosition = Input.mousePosition;
        currentMomentumDuration = momentumDuration; 
        currentRotationVelocity = deltaMousePos;
    }

    private void ApplyRotationMomentum()
    {
        if (currentMomentumDuration <= 0) return;

        transform.RotateAround(cylinderTransform.position, Vector3.up, currentRotationVelocity.x * rotationSpeed * Time.deltaTime);
        transform.RotateAround(cylinderTransform.position, transform.right, -currentRotationVelocity.y * rotationSpeed * Time.deltaTime);
        currentMomentumDuration -= Time.deltaTime;
    }

    private void HandleZoom()
    {
        if (rockInteractionActive) return;

        targetFieldOfView -= Input.mouseScrollDelta.y * zoomSpeed;
        targetFieldOfView = Mathf.Clamp(targetFieldOfView, minFieldOfView, maxFieldOfView);
        cam.fieldOfView = Mathf.Lerp(cam.fieldOfView, targetFieldOfView, dragSmoothing * Time.deltaTime);
    }

    private void HandleAudio()
    {
        zoomSoundCooldown -= Time.deltaTime;
        moveSoundCooldown -= Time.deltaTime;

        if (Input.mouseScrollDelta.y != 0 && zoomSoundCooldown <= 0)
            PlayZoomSound();

        if (Input.GetMouseButton(0) && moveSoundCooldown <= 0)
            PlayMovementSound();
    }

    private void PlayZoomSound()
    {
        AudioClip clip = Input.mouseScrollDelta.y > 0 ? zoomInSound : zoomOutSound;
        PlaySound(clip);
        zoomSoundCooldown = soundCooldown;
    }

    private void PlayMovementSound()
    {
        Vector3 deltaMousePos = Input.mousePosition - lastMousePosition;
        AudioClip clip;

        if (Mathf.Abs(deltaMousePos.x) > Mathf.Abs(deltaMousePos.y))
            clip = deltaMousePos.x > 0 ? moveRightSound : moveLeftSound;
        else
            clip = deltaMousePos.y > 0 ? moveUpSound : moveDownSound;

        PlaySound(clip);
        moveSoundCooldown = soundCooldown;
    }

    private void PlaySound(AudioClip clip)
    {
        if (clip == null || audioSource == null) return;

        audioSource.pitch = Random.Range(pitchRange.x, pitchRange.y);
        audioSource.PlayOneShot(clip);
    }

    private void HandleKeyboardMovement()
    {
        Vector3 movementDirection = new Vector3();

        if (Input.GetKey(KeyCode.W))
            movementDirection += transform.forward;
        if (Input.GetKey(KeyCode.S))
            movementDirection -= transform.forward;
        if (Input.GetKey(KeyCode.A))
            movementDirection -= transform.right;
        if (Input.GetKey(KeyCode.D))
            movementDirection += transform.right;

        movementDirection.y = 0;  // Assuming you don't want the camera to move up/down with W/S.

        // Calculate the proposed new position
        Vector3 proposedPosition = transform.position + movementDirection.normalized * keyboardMovementSpeed * Time.deltaTime;

        // Check if the proposed new position is within the allowed distance range
        float proposedDistance = Vector3.Distance(proposedPosition, cylinderTransform.position);
        if (proposedDistance > maxDistanceFromCylinder || proposedDistance < minDistanceFromCylinder)
        {
            // If out of range, don't apply the proposed movement.
            // This could be enhanced to allow partial movement or a more complex response.
            return;
        }

        // If everything is okay, apply the movement
        transform.position = proposedPosition;

        // Ensure the camera looks at the cylinder after moving
        transform.LookAt(cylinderTransform.position);
    }

    public void SetRockInteractionActive(bool state)
    {
        rockInteractionActive = state;
    }
}