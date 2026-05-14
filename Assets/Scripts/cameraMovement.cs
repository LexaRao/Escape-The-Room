using UnityEngine;
using UnityEngine.InputSystem;

public class cameraMovement : MonoBehaviour
{
<<<<<<< HEAD
=======
<<<<<<< HEAD
=======
<<<<<<< Updated upstream
    [Header("Movement Settings")]
    public float moveSpeed = 10f;
    public Vector2 xRange = new Vector2(-20f, 20f);
    public Vector2 zRange = new Vector2(-20f, 20f);

    [Header("Rotation Settings")]
    public float mouseSensitivity = 1.5f;
    public float keyboardRotationSpeed = 90f;
    public float minPitch = -60f;
    public float maxPitch = 60f;
    public bool invertY = false;

    private Transform cam;
    private float pitch;

    // Input Actions
    private InputAction moveAction;
    private InputAction lookAction;
    private InputAction keyboardLookAction;

    void Awake()
    {
        cam = Camera.main.transform;

        // Initialize pitch from current camera rotation
        pitch = cam.localEulerAngles.x;
        if (pitch > 180f) pitch -= 360f;

        // Create Input Actions
=======
>>>>>>> 418fd7fd9f1e9ac4edd46e6f334b3ad52208b29c
>>>>>>> f0a8c5930e5995cdace1c13cf5804129b2d34f49
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
<<<<<<< HEAD
=======
<<<<<<< HEAD
=======
>>>>>>> Stashed changes
>>>>>>> 418fd7fd9f1e9ac4edd46e6f334b3ad52208b29c
>>>>>>> f0a8c5930e5995cdace1c13cf5804129b2d34f49
        moveAction = new InputAction("Move", binding: "<Keyboard>/w");
        moveAction.AddCompositeBinding("2DVector")
            .With("Up", "<Keyboard>/w")
            .With("Down", "<Keyboard>/s")
            .With("Left", "<Keyboard>/a")
            .With("Right", "<Keyboard>/d");

<<<<<<< HEAD
=======
<<<<<<< HEAD
=======
<<<<<<< Updated upstream
        lookAction = new InputAction("Look", binding: "<Mouse>/delta");

=======
>>>>>>> 418fd7fd9f1e9ac4edd46e6f334b3ad52208b29c
>>>>>>> f0a8c5930e5995cdace1c13cf5804129b2d34f49
        // Mouse look
        lookAction = new InputAction("Look", binding: "<Mouse>/delta");

        // Arrow-key rotation
<<<<<<< HEAD
=======
<<<<<<< HEAD
=======
>>>>>>> Stashed changes
>>>>>>> 418fd7fd9f1e9ac4edd46e6f334b3ad52208b29c
>>>>>>> f0a8c5930e5995cdace1c13cf5804129b2d34f49
        keyboardLookAction = new InputAction("KeyboardLook");
        keyboardLookAction.AddCompositeBinding("2DVector")
            .With("Up", "<Keyboard>/upArrow")
            .With("Down", "<Keyboard>/downArrow")
            .With("Left", "<Keyboard>/leftArrow")
            .With("Right", "<Keyboard>/rightArrow");
    }

<<<<<<< HEAD
    // Enable input actions
=======
<<<<<<< HEAD
    // Enable input actions
=======
<<<<<<< Updated upstream
=======
    // Enable input actions
>>>>>>> Stashed changes
>>>>>>> 418fd7fd9f1e9ac4edd46e6f334b3ad52208b29c
>>>>>>> f0a8c5930e5995cdace1c13cf5804129b2d34f49
    void OnEnable()
    {
        moveAction.Enable();
        lookAction.Enable();
        keyboardLookAction.Enable();
    }

<<<<<<< HEAD
    // Disable input actions
=======
<<<<<<< HEAD
    // Disable input actions
=======
<<<<<<< Updated upstream
=======
    // Disable input actions
>>>>>>> Stashed changes
>>>>>>> 418fd7fd9f1e9ac4edd46e6f334b3ad52208b29c
>>>>>>> f0a8c5930e5995cdace1c13cf5804129b2d34f49
    void OnDisable()
    {
        moveAction.Disable();
        lookAction.Disable();
        keyboardLookAction.Disable();
    }

<<<<<<< HEAD
=======
<<<<<<< HEAD
=======
<<<<<<< Updated upstream
    // Update is called once per frame
    void Update()
    {
        if (!IsLeftMousePressed())
            return;

        HandleMovement();
        HandleKeyboardRotation();
        HandleMouseRotation();
    }

=======
>>>>>>> 418fd7fd9f1e9ac4edd46e6f334b3ad52208b29c
>>>>>>> f0a8c5930e5995cdace1c13cf5804129b2d34f49
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
<<<<<<< HEAD
=======
<<<<<<< HEAD
=======
>>>>>>> Stashed changes
>>>>>>> 418fd7fd9f1e9ac4edd46e6f334b3ad52208b29c
>>>>>>> f0a8c5930e5995cdace1c13cf5804129b2d34f49
    bool IsLeftMousePressed()
    {
        return Input.GetMouseButton(0);
    }

<<<<<<< HEAD
=======
<<<<<<< HEAD
=======
<<<<<<< Updated upstream
    void HandleMovement()
    {
        Vector2 input = moveAction.ReadValue<Vector2>();
        Vector3 move = new Vector3(input.x, 0, input.y) * moveSpeed * Time.deltaTime;

        Vector3 newPos = transform.position + move;

        newPos.x = Mathf.Clamp(newPos.x, xRange.x, xRange.y);
        newPos.z = Mathf.Clamp(newPos.z, zRange.x, zRange.y);

        transform.position = newPos;
    }

    void HandleKeyboardRotation()
    {
        Vector2 input = keyboardLookAction.ReadValue<Vector2>();
        float yaw = input.x;
        float pitchInput = input.y;

        if (invertY) pitchInput = -pitchInput;

        Vector3 rotation = new Vector3(pitchInput, yaw, 0f) * keyboardRotationSpeed * Time.deltaTime;
        cam.Rotate(rotation, Space.Self);
    }

    void HandleMouseRotation()
    {
        Vector2 mouseDelta = lookAction.ReadValue<Vector2>() * mouseSensitivity;

        float mouseX = mouseDelta.x;
        float mouseY = mouseDelta.y;

        if (invertY) mouseY = -mouseY;

        // Horizontal rotation on parent
        transform.Rotate(Vector3.up * mouseX, Space.World);

        // Vertical rotation on camera
        pitch -= mouseY;
        pitch = Mathf.Clamp(pitch, minPitch, maxPitch);

=======
>>>>>>> 418fd7fd9f1e9ac4edd46e6f334b3ad52208b29c
>>>>>>> f0a8c5930e5995cdace1c13cf5804129b2d34f49
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
<<<<<<< HEAD
=======
<<<<<<< HEAD
=======
>>>>>>> Stashed changes
>>>>>>> 418fd7fd9f1e9ac4edd46e6f334b3ad52208b29c
>>>>>>> f0a8c5930e5995cdace1c13cf5804129b2d34f49
        Vector3 camEuler = cam.localEulerAngles;
        camEuler.x = pitch;
        cam.localEulerAngles = camEuler;
    }
<<<<<<< HEAD
}
=======
<<<<<<< HEAD
}
=======
<<<<<<< Updated upstream
}
=======
}
>>>>>>> Stashed changes
>>>>>>> 418fd7fd9f1e9ac4edd46e6f334b3ad52208b29c
>>>>>>> f0a8c5930e5995cdace1c13cf5804129b2d34f49
