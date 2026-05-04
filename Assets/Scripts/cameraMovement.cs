using UnityEngine;
using UnityEngine.InputSystem;

public class cameraMovement : MonoBehaviour
{
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
        moveAction = new InputAction("Move", binding: "<Keyboard>/w");
        moveAction.AddCompositeBinding("2DVector")
            .With("Up", "<Keyboard>/w")
            .With("Down", "<Keyboard>/s")
            .With("Left", "<Keyboard>/a")
            .With("Right", "<Keyboard>/d");

        lookAction = new InputAction("Look", binding: "<Mouse>/delta");

        keyboardLookAction = new InputAction("KeyboardLook");
        keyboardLookAction.AddCompositeBinding("2DVector")
            .With("Up", "<Keyboard>/upArrow")
            .With("Down", "<Keyboard>/downArrow")
            .With("Left", "<Keyboard>/leftArrow")
            .With("Right", "<Keyboard>/rightArrow");
    }

    void OnEnable()
    {
        moveAction.Enable();
        lookAction.Enable();
        keyboardLookAction.Enable();
    }

    void OnDisable()
    {
        moveAction.Disable();
        lookAction.Disable();
        keyboardLookAction.Disable();
    }

    // Update is called once per frame
    void Update()
    {
        if (!IsLeftMousePressed())
            return;

        HandleMovement();
        HandleKeyboardRotation();
        HandleMouseRotation();
    }

    bool IsLeftMousePressed()
    {
        return Input.GetMouseButton(0);
    }

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

        Vector3 camEuler = cam.localEulerAngles;
        camEuler.x = pitch;
        cam.localEulerAngles = camEuler;
    }
}
