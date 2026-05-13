using UnityEngine;
using UnityEngine.InputSystem;

public class CameraMovement : MonoBehaviour
{
    [Header("Movement Settings")]
    public float moveSpeed = 10f;
    public Vector2 xRange = new Vector2(-20f, 20f);
    public Vector2 zRange = new Vector2(-20f, 20f);

    [Header("Vertical Height Limits")]
    public float minY = 1f;
    public float maxY = 10f;

    [Header("Rotation Settings")]
    public float mouseSensitivity = 1.5f;
    public float keyboardRotationSpeed = 90f;
    public float minPitch = -60f;
    public float maxPitch = 60f;
    public bool invertY = false;

    private Transform cam;
    private float pitch;

    private InputAction moveAction;
    private InputAction lookAction;
    private InputAction keyboardLookAction;

    private CharacterController controller;

    void Awake()
    {
        cam = Camera.main.transform;

        controller = GetComponent<CharacterController>();
        if (controller == null)
            controller = gameObject.AddComponent<CharacterController>();

        controller.minMoveDistance = 0f;

        pitch = cam.localEulerAngles.x;
        if (pitch > 180f) pitch -= 360f;

        moveAction = new InputAction("Move");
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

    void Update()
    {
        if (!Input.GetMouseButton(0))
            return;

        HandleMovement();
        HandleKeyboardRotation();
        HandleMouseRotation();
        ClampVerticalPosition();
    }

    void HandleMovement()
    {
        Vector2 input = moveAction.ReadValue<Vector2>();

        Vector3 move = new Vector3(input.x, 0, input.y);
        move = transform.TransformDirection(move) * moveSpeed * Time.deltaTime;

        controller.Move(move);

        Vector3 pos = transform.position;
        pos.x = Mathf.Clamp(pos.x, xRange.x, xRange.y);
        pos.z = Mathf.Clamp(pos.z, zRange.x, zRange.y);
        transform.position = pos;
    }

    void ClampVerticalPosition()
    {
        Vector3 pos = transform.position;
        pos.y = Mathf.Clamp(pos.y, minY, maxY);
        transform.position = pos;
    }

    void HandleKeyboardRotation()
    {
        Vector2 input = keyboardLookAction.ReadValue<Vector2>();

        float yaw = input.x;
        float pitchInput = input.y;

        if (invertY) pitchInput = -pitchInput;

        Vector3 rotation = new Vector3(pitchInput, yaw, 0f) *
                           keyboardRotationSpeed * Time.deltaTime;

        cam.Rotate(rotation, Space.Self);
    }

    void HandleMouseRotation()
    {
        Vector2 mouseDelta = lookAction.ReadValue<Vector2>() * mouseSensitivity;

        float mouseX = mouseDelta.x;
        float mouseY = mouseDelta.y;

        if (invertY) mouseY = -mouseY;

        transform.Rotate(Vector3.up * mouseX, Space.World);

        pitch -= mouseY;
        pitch = Mathf.Clamp(pitch, minPitch, maxPitch);

        Vector3 camEuler = cam.localEulerAngles;
        camEuler.x = pitch;
        cam.localEulerAngles = camEuler;
    }
}