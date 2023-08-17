using UnityEngine;

public class CameraController : MonoBehaviour
{
    private Camera cam;
    private AudioSource audioSource;

    [Header("References")]
    public Transform cylinderTransform;

    [Header("Camera Rotation")]
    public float rotationSpeed = 3f;
    public float dragSmoothing = 10f;
    public float momentumDuration = 2f;
    private float currentMomentumDuration;
    private Vector3 lastMousePosition;
    private Vector3 currentRotationVelocity;

    [Header("Camera Movement Restrictions")]
    public float maxDistanceFromCylinder = 10f;  // Maximum distance from the cylinder's center
    public float minDistanceFromCylinder = 2f;   // Minimum distance from the cylinder's center

    [Header("Keyboard Movement")]
    public float keyboardMovementSpeed = 5f;
    public bool rockInteractionActive = false;

    [Header("Camera Zoom")]
    [Range(30, 120)] public float defaultFieldOfView = 60f;
    [Range(30, 120)] public float minFieldOfView = 30f;
    [Range(30, 120)] public float maxFieldOfView = 90f;
    public float zoomSpeed = 10f;
    private float targetFieldOfView;

    [Header("Sound Effects")]
    public AudioClip zoomInSound;
    public AudioClip zoomOutSound;
    public AudioClip moveUpSound;
    public AudioClip moveDownSound;
    public AudioClip moveLeftSound;
    public AudioClip moveRightSound;
    public float soundCooldown = 0.5f;
    private float zoomSoundCooldown;
    private float moveSoundCooldown;
    public Vector2 pitchRange = new Vector2(0.8f, 1.2f);

    private void Start()
    {
        InitializeComponents();
    }

    private void Update()
    {
        HandleAudio();
        HandleKeyboardMovement();

        if (rockInteractionActive)
        {
            HandleRockInteraction();
        }
        else
        {
            HandleRotation();
            HandleZoom();
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