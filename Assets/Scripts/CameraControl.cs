using UnityEngine;

public class CameraController : MonoBehaviour
{
    private Camera cam;

    [Header("References")]
    public Transform cylinderTransform;

    [Header("Camera Movement Settings")]
    public float rotationSpeed = 3f;
    public float dragSmoothing = 10f;
    public float momentumDuration = 2f;
    private float currentMomentumDuration;
    private Vector3 lastMousePosition;
    private Vector3 currentRotationVelocity;
    [Header("Keyboard Camera Movement")]
    public float keyboardMovementSpeed = 5f;
    public bool rockInteractionActive = false;  // Whether the rock is currently being interacted with.

    [Header("Camera Zoom Settings")]
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
    private AudioSource audioSource;

    void Start()
    {
        cam = GetComponent<Camera>();
        targetFieldOfView = defaultFieldOfView;
        audioSource = GetComponent<AudioSource>();
    }

    void Update()
    {
        HandleAudio();
        HandleKeyboardMovement();

        if (!rockInteractionActive)
        {
            HandleRotation();
            HandleZoom();
        }
        else
        {
            RotateRock();
            DropRock(); // New function for dropping the rock with the space key
        }
    }

    void RotateRock()
    {
        // Assuming the rock rotates around its local Y axis with the scroll wheel
        float rotationAmount = Input.mouseScrollDelta.y * rotationSpeed;
        // Rotate the rock. You need to get a reference to the rock's transform.
        // For the sake of this example, I'm rotating the cylinder.
        cylinderTransform.Rotate(0, rotationAmount, 0);
    }

    void DropRock()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            // Call a function from the rock's script to drop the rock.
            // For this example, I'm setting rockInteractionActive to false.
            SetRockInteractionActive(false);
        }
    }

    void HandleRotation()
    {
        if (Input.GetMouseButtonDown(0))
        {
            lastMousePosition = Input.mousePosition;
        }

        if (Input.GetMouseButton(0))
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

        // Apply momentum
        if (currentMomentumDuration > 0)
        {
            transform.RotateAround(cylinderTransform.position, Vector3.up, currentRotationVelocity.x * rotationSpeed * Time.deltaTime);
            transform.RotateAround(cylinderTransform.position, transform.right, -currentRotationVelocity.y * rotationSpeed * Time.deltaTime);
            currentMomentumDuration -= Time.deltaTime;
        }
    }

    void HandleZoom()
    {
        if (rockInteractionActive) return; // Avoid zooming when interacting with the rock

        targetFieldOfView -= Input.mouseScrollDelta.y * zoomSpeed;
        targetFieldOfView = Mathf.Clamp(targetFieldOfView, minFieldOfView, maxFieldOfView);

        GetComponent<Camera>().fieldOfView = Mathf.Lerp(GetComponent<Camera>().fieldOfView, targetFieldOfView, dragSmoothing * Time.deltaTime);
    }

    void HandleAudio()
    {
        zoomSoundCooldown -= Time.deltaTime;
        moveSoundCooldown -= Time.deltaTime;

        // Play zoom sound
        if (Input.mouseScrollDelta.y != 0 && zoomSoundCooldown <= 0)
        {
            AudioClip clip = Input.mouseScrollDelta.y > 0 ? zoomInSound : zoomOutSound;
            PlaySound(clip);
            zoomSoundCooldown = soundCooldown;
        }

        // Play movement sound
        if (Input.GetMouseButton(0) && moveSoundCooldown <= 0)
        {
            Vector3 deltaMousePos = Input.mousePosition - lastMousePosition;

            AudioClip clip = null;
            if (Mathf.Abs(deltaMousePos.x) > Mathf.Abs(deltaMousePos.y))
            {
                clip = deltaMousePos.x > 0 ? moveRightSound : moveLeftSound;
            }
            else
            {
                clip = deltaMousePos.y > 0 ? moveUpSound : moveDownSound;
            }

            PlaySound(clip);
            moveSoundCooldown = soundCooldown;
        }
    }

    void PlaySound(AudioClip clip)
    {
        if (clip != null && audioSource != null)
        {
            audioSource.pitch = Random.Range(pitchRange.x, pitchRange.y);
            audioSource.PlayOneShot(clip);
        }
    }

    void HandleKeyboardMovement()
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
        transform.position += movementDirection.normalized * keyboardMovementSpeed * Time.deltaTime;
    }

    public void SetRockInteractionActive(bool state)
    {
        rockInteractionActive = state;
    }
}