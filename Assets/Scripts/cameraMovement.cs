using UnityEngine;
using UnityEngine.InputSystem;

public class cameraMovement : MonoBehaviour
{
    // ============================
    // MOVEMENT SETTINGS
    // ============================

    [Header("Movement Settings")]
    public float moveSpeed = 10f;                     // Speed of WASD movement
    public Vector2 xRange = new Vector2(-20f, 20f);   // Horizontal movement limits
    public Vector2 zRange = new Vector2(-20f, 20f);   // Forward/back movement limits

    // ============================
    // VERTICAL LIMIT SETTINGS
    // ============================

    [Header("Vertical Height Limits")]
    public float minY = 1f;                           // Lowest allowed camera height (floor)
    public float maxY = 10f;                          // Highest allowed camera height (ceiling)

    // ============================
    // ROTATION SETTINGS
    // ============================

    [Header("Rotation Settings")]
    public float mouseSensitivity = 1.5f;             // Mouse look sensitivity
    public float keyboardRotationSpeed = 90f;         // Arrow-key rotation speed
    public float minPitch = -60f;                     // Minimum vertical look angle
    public float maxPitch = 60f;                      // Maximum vertical look angle
    public bool invertY = false;                      // Invert vertical look direction

    // ============================
    // INTERNAL CAMERA STATE
    // ============================

    private Transform cam;                            // Reference to the camera transform
    private float pitch;                              // Current vertical rotation value

    // ============================
    // INPUT ACTIONS (NEW INPUT SYSTEM)
    // ============================

    private InputAction moveAction;                   // WASD movement
    private InputAction lookAction;                   // Mouse look
    private InputAction keyboardLookAction;           // Arrow-key rotation

    // ============================
    // COLLISION HANDLING
    // ============================

    private CharacterController controller;           // Prevents camera from clipping through objects

    // ============================
    // INITIALIZATION
    // ============================

    void Awake()
    {
        // Cache the main camera transform
        cam = Camera.main.transform;

        // Ensure a CharacterController exists for collision
        controller = GetComponent<CharacterController>();
        if (controller == null)
            controller = gameObject.AddComponent<CharacterController>();

        controller.minMoveDistance = 0f; // Ensures smooth movement

        // Initialize pitch based on current camera rotation
        pitch = cam.localEulerAngles.x;
        if (pitch > 180f) pitch -= 360f;

        // ============================
        // CREATE INPUT ACTIONS
        // ============================

        // WASD movement
        moveAction = new InputAction("Move", binding: "<Keyboard>/w");
        moveAction.AddCompositeBinding("2DVector")
            .With("Up", "<Keyboard>/w")
            .With("Down", "<Keyboard>/s")
            .With("Left", "<Keyboard>/a")
            .With("Right", "<Keyboard>/d");

        // Mouse look
        lookAction = new InputAction("Look", binding: "<Mouse>/delta");

        // Arrow-key rotation
        keyboardLookAction = new InputAction("KeyboardLook");
        keyboardLookAction.AddCompositeBinding("2DVector")
            .With("Up", "<Keyboard>/upArrow")
            .With("Down", "<Keyboard>/downArrow")
            .With("Left", "<Keyboard>/leftArrow")
            .With("Right", "<Keyboard>/rightArrow");
    }

    // Enable input actions
    void OnEnable()
    {
        moveAction.Enable();
        lookAction.Enable();
        keyboardLookAction.Enable();
    }

    // Disable input actions
    void OnDisable()
    {
        moveAction.Disable();
        lookAction.Disable();
        keyboardLookAction.Disable();
    }

    // ============================
    // MAIN UPDATE LOOP
    // ============================

    void Update()
    {
        // Only allow movement when left mouse is held
        if (!IsLeftMousePressed())
            return;

        HandleMovement();         // WASD movement
        HandleKeyboardRotation(); // Arrow-key rotation
        HandleMouseRotation();    // Mouse look

        ClampVerticalPosition();  // Prevent camera from exceeding floor/ceiling
    }

    // Check if left mouse button is pressed
    bool IsLeftMousePressed()
    {
        return Input.GetMouseButton(0);
    }

    // ============================
    // MOVEMENT HANDLING
    // ============================

    void HandleMovement()
    {
        // Read WASD input
        Vector2 input = moveAction.ReadValue<Vector2>();

        // Convert input to world-space movement
        Vector3 move = new Vector3(input.x, 0, input.y);
        move = transform.TransformDirection(move) * moveSpeed * Time.deltaTime;

        // Move using CharacterController (prevents clipping)
        controller.Move(move);

        // Clamp X and Z movement to defined boundaries
        Vector3 pos = transform.position;
        pos.x = Mathf.Clamp(pos.x, xRange.x, xRange.y);
        pos.z = Mathf.Clamp(pos.z, zRange.x, zRange.y);
        transform.position = pos;
    }

    // ============================
    // HEIGHT LIMIT HANDLING
    // ============================

    void ClampVerticalPosition()
    {
        // Get current position
        Vector3 pos = transform.position;

        // Clamp Y between floor and ceiling
        pos.y = Mathf.Clamp(pos.y, minY,maxY - 8);

        // Apply clamped position
        transform.position = pos;
    }

    // ============================
    // KEYBOARD ROTATION (ARROWS)
    // ============================

    void HandleKeyboardRotation()
    {
        // Read arrow-key input
        Vector2 input = keyboardLookAction.ReadValue<Vector2>();
        float yaw = input.x;        // Left/right rotation
        float pitchInput = input.y; // Up/down rotation

        // Apply Y inversion if enabled
        if (invertY) pitchInput = -pitchInput;

        // Apply rotation to camera
        Vector3 rotation = new Vector3(pitchInput, yaw, 0f) *
                           keyboardRotationSpeed * Time.deltaTime;

        cam.Rotate(rotation, Space.Self);
    }

    // ============================
    // MOUSE LOOK ROTATION
    // ============================

    void HandleMouseRotation()
    {
        // Read mouse delta
        Vector2 mouseDelta = lookAction.ReadValue<Vector2>() * mouseSensitivity;

        float mouseX = mouseDelta.x; // Horizontal movement
        float mouseY = mouseDelta.y; // Vertical movement

        // Apply Y inversion if enabled
        if (invertY) mouseY = -mouseY;

        // Rotate parent object horizontally
        transform.Rotate(Vector3.up * mouseX, Space.World);

        // Adjust pitch (vertical rotation)
        pitch -= mouseY;
        pitch = Mathf.Clamp(pitch, minPitch, maxPitch);

        // Apply pitch to camera
        Vector3 camEuler = cam.localEulerAngles;
        camEuler.x = pitch;
        cam.localEulerAngles = camEuler;
    }
}