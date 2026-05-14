/*
 * Purpose: Handles camera movement. Needs lots of commenting
 */

using UnityEngine;
using UnityEngine.InputSystem;
using Cursor = UnityEngine.Cursor;

public class MainCamera : MonoBehaviour
{
    [Header("Placement of Camera")]
    [SerializeField] private Transform camSpot;

    [Header("Input Actions References")]
    [SerializeField] private InputActionReference clickAndDrag;
    [SerializeField] private InputActionReference doubleClick;
    [SerializeField] private InputActionReference mouseDelta; // Tracks change in mouse coordinates
    [SerializeField] private InputActionReference mousePos;

    // Input actions
    private InputAction clickAndDragIA;
    private InputAction doubleClickIA;
    private InputAction mouseDeltaIA;
    private InputAction mousePosIA;

    [Header("Look Sensitivity & Momentum")]
    [SerializeField] private float sensitivity = 1000f;
    [SerializeField] private float momentumDropoff = 0.1f; // The factor by which momentum decreases
    [SerializeField] private float momentumThreshold = 1.0f; // The limit value for momentum to be set to 0

    [Header("Camera Node Script")]
    [SerializeField] private CameraNode camNode;

    // Rotational coordinates
    private float yaw; // Horizontal movement
    private float pitch; // Vertical movement

    // Momentum for lingering camera movement
    private Vector2 momentum;

    // Variables for raycasting
    private Camera mainCam;
    private Collider currentNodeCollider;

    // Enumerator to help prevent overlap between left-click responses
    private enum CameraState
    {
        Idle,
        Dragging,
        Moving,
        PuzzleMoving,
        ViewingPuzzle
    }

    // Enumerator "helper" variable
    private CameraState state = CameraState.Idle;

    private Vector3 startMovePos; // just gonna be camSpot
    private Quaternion startMoveRot; // Gonna be the rotation of the cammera

    private Vector3 endMovePos;
    private Quaternion endMoveRot;

    // Progress trackers
    private float elapsedTime = 0f;
    private float progress = 0f;

    // Camera node handling variables
    private CameraNode targetNode;
    private CameraNode.CamConnections foundConnection = default;
    bool isConnected = false;

    // Puzzle object handling variables
    private PuzzleObj currentPuzzle;
    [SerializeField] private GameObject puzzleBackButton;
    private bool returningFromPuzzle = false;

    // Stores all puzzle object scripts in a scene
    private PuzzleObj[] allPuzzles;

    private Vector3 puzzleStartPos;
    private Quaternion puzzleStartRot;
    private Vector3 puzzleEndPos;
    private Quaternion puzzleEndRot;

    private void Awake()
    {
        // Assign input action references to input action equivalents for easier access
        clickAndDragIA = clickAndDrag.action;
        doubleClickIA = doubleClick.action;
        mouseDeltaIA = mouseDelta.action;
        mousePosIA = mousePos.action;

        // Initialize main camera for later raycast use
        mainCam = Camera.main;
    }

    private void OnEnable()
    {
        // Enable input actions
        clickAndDrag.action.Enable();
        doubleClick.action.Enable();
        mouseDelta.action.Enable();
        mousePos.action.Enable();

        // Subscribe double click action to related function
        doubleClickIA.performed += DoubleClicked;
    }

    private void Start()
    {
        // Set the value for move duration.
        foundConnection.moveDuration = 1.0f;
        foundConnection.rotationDelay = 0.2f;


        // If a starting location was not assigned
        if (camSpot == null)
        {
            Debug.LogError("Camera spot is not assigned.");
            return;
        }

        if (isConnected && foundConnection.moveDuration <= 0f)
        {
            Debug.LogError("Invalid Move Duration value. Must be greater than 0.");
        }

        // Get the collider for the camera node that is initially visited and disable it to prevent raycast interference
        currentNodeCollider = camNode.GetComponent<Collider>();
        if (currentNodeCollider != null)
        {
            currentNodeCollider.enabled = false;
        }

        // Find and store all PuzzleObj scripts
        allPuzzles = FindObjectsByType<PuzzleObj>(FindObjectsSortMode.None);

        // Make sure the back button isn't showing and define any puzzles available from the starting node as accessible
        puzzleBackButton.SetActive(false);
        SetNodePuzzleAccess(camNode);

        // Set initial camera rotation and momentum
        yaw = camSpot.rotation.eulerAngles.y;
        pitch = camSpot.rotation.eulerAngles.x;
        momentum = Vector2.zero;
        state = CameraState.Idle;
    }

    void Update()
    {
        if (state == CameraState.Moving)
        {
            elapsedTime += Time.deltaTime;

            float t = Mathf.Clamp01(elapsedTime / foundConnection.moveDuration);
            float smoothT = Mathf.SmoothStep(0f, 1f, t);

            mainCam.transform.position = Vector3.Lerp(startMovePos, endMovePos, smoothT);
            if (elapsedTime >= foundConnection.rotationDelay)
            {
                float rotT = Mathf.Clamp01((elapsedTime - foundConnection.rotationDelay) / (foundConnection.moveDuration - foundConnection.rotationDelay));
                float smoothRotT = Mathf.SmoothStep(0f, 1f, rotT);
                mainCam.transform.rotation = Quaternion.Slerp(startMoveRot, endMoveRot, smoothRotT);
            }

            progress = elapsedTime / foundConnection.moveDuration;
            if (progress >= 1f)
            {
                mainCam.transform.position = endMovePos;
                mainCam.transform.rotation = endMoveRot;

                momentum = Vector2.zero;

                yaw = endMoveRot.eulerAngles.y;
                pitch = endMoveRot.eulerAngles.x;

                camNode = targetNode;

                camSpot = camNode.transform;

                SetNodePuzzleAccess(camNode);

                elapsedTime = 0;

                Cursor.visible = true;

                foundConnection = default;
                isConnected = false;

                state = CameraState.Idle;
            }
            return;
        }
        if (state == CameraState.PuzzleMoving)
        {
            elapsedTime += Time.deltaTime;

            float t = Mathf.Clamp01(elapsedTime / currentPuzzle.moveDuration);
            float smoothT = Mathf.SmoothStep(0f, 1f, t);

            if (currentPuzzle.viewMode == PuzzleViewMode.BringPuzzleToCamera)
            {
                currentPuzzle.transform.position = Vector3.Lerp(puzzleStartPos, puzzleEndPos, smoothT);
                currentPuzzle.transform.rotation = Quaternion.Slerp(puzzleStartRot, puzzleEndRot, smoothT);
            }
            else
            {
                mainCam.transform.position = Vector3.Lerp(startMovePos, endMovePos, smoothT);
                mainCam.transform.rotation = Quaternion.Slerp(startMoveRot, endMoveRot, smoothT);
            }

            if (t >= 1f)
            {
                elapsedTime = 0f;

                if (!returningFromPuzzle)
                {
                    currentPuzzle.ActivatePuzzle();
                    puzzleBackButton.SetActive(true);
                    state = CameraState.ViewingPuzzle;
                }
                else
                {
                    returningFromPuzzle = false;
                    currentPuzzle = null;

                    ResetCameraInputState();
                    SetNodePuzzleAccess(camNode);
                    state = CameraState.Idle;
                }
            }

            return;
        }
        if (state == CameraState.ViewingPuzzle)
        {
            puzzleBackButton.SetActive(true);
            return;
        }

        HandleInput();

        // Clamp the pitch and yaw so that it can't exceed the limits
        pitch = Mathf.Clamp(pitch, camNode.pitchLimits.x, camNode.pitchLimits.y); // Typically (-90 to 90 degrees)
        if (camNode.hasYawLimits) // If the camera has a limited horizontal range, clamp
        {
            yaw = Mathf.Clamp(yaw, camNode.yawLimits.x, camNode.yawLimits.y);
        }

        // If the camera is being dragged, change the camera rotation via pitch and yaw values
        Quaternion camRotation = Quaternion.Euler(pitch, yaw, 0);

        RotateCamera(camRotation);
    }

    /*
     * Is called every frame.
     * If the screen is being dragged, or momentum is substantially large, the camera rotates accordingly.
     */
    public void HandleInput()
    {
        // Initialize inputDelta to 0
        Vector2 inputDelta = Vector2.zero; // (0, 0)

        // Checks if the mouse is pressed, and updates inputDelta and increases momentum if so.
        if (clickAndDragIA.IsPressed() && mouseDeltaIA.ReadValue<Vector2>().magnitude > 5 || clickAndDragIA.IsPressed() && state == CameraState.Dragging)
        {
            if (state != CameraState.Dragging)
            {
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = true;
            }
            state = CameraState.Dragging;
            inputDelta = mouseDeltaIA.ReadValue<Vector2>();
            momentum = Vector2.Lerp(momentum, inputDelta, 0.1f); // Linearly increase momentum in steps of 0.1
        }
        // Checks if momentum is substantially large, and updates inputDelta and decerases momentum if so.
        else if (momentum.sqrMagnitude > momentumThreshold)
        {
            if (state == CameraState.Dragging)
            {
                Cursor.lockState = CursorLockMode.None;
            }
            state = CameraState.Idle;
            inputDelta = momentum;
            momentum = Vector2.Lerp(momentum, Vector2.zero, momentumDropoff * Time.deltaTime);
        }
        // If none of these conditions are satisfied, set momentum to 0.
        else
        {
            if (state == CameraState.Dragging)
            {
                Cursor.lockState = CursorLockMode.None;
            }
            state = CameraState.Idle;
            momentum = Vector2.zero;
        }

        // Update yaw and pitch according to inputDelta and sensitivity
        yaw += inputDelta.x * sensitivity * Time.deltaTime;
        pitch -= inputDelta.y * sensitivity * Time.deltaTime;
    }

    /* 
     * Is called when double click is performed.
     * If the mouse double clicked on a camera node's collider, move to it.
     */
    public void DoubleClicked(InputAction.CallbackContext context)
    {
        if (state != CameraState.Idle)
        {
            return;
        }

        // Shoots out a ray from the camera through the mouse point
        LayerMask camMask = LayerMask.GetMask("CameraNode");
        LayerMask puzzleMask = LayerMask.GetMask("PuzzleObj");

        Ray ray = mainCam.ScreenPointToRay(GetMouseScreenPosition()/*mousePosIA.ReadValue<Vector2>()*/);
        RaycastHit hit;

        Debug.DrawLine(ray.origin, ray.origin + ray.direction * 100f, Color.red, 1f);

        // If a collider in the "CameraNode" layer is hit, re-enable the current camera's collider, and move the camera to the saved hit collider
        if (Physics.Raycast(ray, out hit, 1000f, puzzleMask))
        {
            PuzzleObj puzzle = hit.collider.GetComponentInParent<PuzzleObj>();

            if (puzzle == null)
            {
                Debug.LogError("Clicked puzzle object doesn't have a PuzzleObj script.");
                return;
            }

            PuzzleClickedOn(puzzle);
            return;
        }
        if (!Physics.Raycast(ray, out hit, 1000f, camMask)) return;

        Collider hitCollider = hit.collider;
        Debug.Log("Hit: " + hitCollider.name);

        // Get the collider's CameraNode script.
        targetNode = hitCollider.GetComponent<CameraNode>();

        if (targetNode == null) // If it doesn't exist, send an error log as that means a camera node is missing this script.
        {
            Debug.LogError("No Camera Node found");
            return;
        }

        foundConnection = default;
        isConnected = false;

        foreach (var connection in camNode.connections)
        {
            if (connection.targetNode == targetNode)
            {
                foundConnection = connection;
                isConnected = true;
                break;
            }
        }

        if (!isConnected)
        {
            Debug.Log("Target node is not connected to the current node");
            state = CameraState.Idle;
            return;
        }

        if (currentNodeCollider != null)
        {
            currentNodeCollider.enabled = true;
        }

        MoveCameraToHitNode(hitCollider);
    }

    /*
     * Is called when a camera node's collider is double clicked on.
     * Parameter is hit camera node's collider.
     * Move camera's position and rotation to camera Node, and save its collider.
     */
   void MoveCameraToHitNode(Collider hitColl)
    {
        state = CameraState.Moving;

        // Update camSpot immediately so camera knows where to move
        camSpot = targetNode.transform;

        SetNodePuzzleAccess(null);

        momentum = Vector2.zero;

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = false;

        startMovePos = mainCam.transform.position;
        startMoveRot = mainCam.transform.rotation;

        endMovePos = targetNode.getPosition();
        endMoveRot = targetNode.transform.rotation;

        // Assign new collider and disable it to avoid blocking raycasts
        currentNodeCollider = hitColl;
        if (currentNodeCollider != null)
        {
            currentNodeCollider.enabled = false;
        }
    }


    /*
     * Is called every frame.
     * Parameter is rotation, which is determined by pitch and yaw variables, which change with mouse drag.
     */
    void RotateCamera(Quaternion rotation)
    {
        // Do NOT override movement while transitioning
        if (state == CameraState.Moving || state == CameraState.PuzzleMoving)
            return;

        // Normal rotation behavior
        mainCam.transform.position = camSpot.position;
        mainCam.transform.rotation = rotation;
    }


    void PuzzleClickedOn(PuzzleObj p)
    {
        Debug.Log("Puzzle was double clicked on and is now moving to view");

        if (state == CameraState.ViewingPuzzle || state == CameraState.PuzzleMoving)
            return;

        currentPuzzle = p;
        SetNodePuzzleAccess(null);
        momentum = Vector2.zero;

        returningFromPuzzle = false;

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        if (p.viewMode == PuzzleViewMode.MoveCameraToPuzzle)
        {
            state = CameraState.PuzzleMoving;

            startMovePos = mainCam.transform.position;
            startMoveRot = mainCam.transform.rotation;

            endMovePos = p.puzzleViewSpot.position;
            endMoveRot = p.puzzleViewSpot.rotation;

            elapsedTime = 0f;
        }
        else if (p.viewMode == PuzzleViewMode.BringPuzzleToCamera)
        {
            p.SaveOriginalTransform();

            currentPuzzle = p;
            SetNodePuzzleAccess(null);
            momentum = Vector2.zero;

            returningFromPuzzle = false;
            state = CameraState.PuzzleMoving;

            puzzleStartPos = p.transform.position;
            puzzleStartRot = p.transform.rotation;

            puzzleEndPos =
                mainCam.transform.position +
                mainCam.transform.forward * p.pickupDistanceFromCamera +
                mainCam.transform.up * p.pickupVerticalOffset;

            puzzleEndRot = Quaternion.Euler(p.pickupRotation);

            elapsedTime = 0f;
        }
    }

    public void ExitPuzzleView()
    {
        if (currentPuzzle == null)
        {
            return;
        }

        currentPuzzle.DeactivatePuzzle();
        puzzleBackButton.SetActive(false);

        ResetCameraInputState();

        if (currentPuzzle.viewMode == PuzzleViewMode.BringPuzzleToCamera)
        {
            currentPuzzle.RestoreOriginalTransform();
            currentPuzzle = null;
            returningFromPuzzle = false;
            SetNodePuzzleAccess(camNode);
            state = CameraState.Idle;
            return;
        }

        returningFromPuzzle = true;
        state = CameraState.PuzzleMoving;

        startMovePos = mainCam.transform.position;
        startMoveRot = mainCam.transform.rotation;

        endMovePos = camNode.transform.position;
        endMoveRot = camNode.transform.rotation;

        elapsedTime = 0f;
    }

    private void SetNodePuzzleAccess(CameraNode node)
    {
        foreach (var puzzle in allPuzzles)
        {
            bool enable = node != null && node.puzzlesAtNode.Contains(puzzle);
            puzzle.SetAccessible(enable);
        }
    }

    private void ResetCameraInputState()
    {
        momentum = Vector2.zero;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        state = CameraState.Idle;
    }

    private Vector2 GetMouseScreenPosition()
    {
        return Mouse.current.position.ReadValue();
    }

    void OnDisable()
    {
        doubleClickIA.performed -= DoubleClicked;

        clickAndDragIA.Disable();
        doubleClickIA.Disable();
        mouseDeltaIA.Disable();
        mousePosIA.Disable();
    }
}
