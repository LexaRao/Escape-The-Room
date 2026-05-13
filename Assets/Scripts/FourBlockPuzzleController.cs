using UnityEngine;
using UnityEngine.Events;

public class FourBlockPuzzleController : MonoBehaviour
{
<<<<<<< Updated upstream
    [Header("Puzzle Blocks")]
    public Transform[] blocks = new Transform[4];

    [Header("Correct Target Positions")]
    public Transform[] targetPositions = new Transform[4];

    [Header("Movement Settings")]
    public float moveWidth = 2f;
    public float moveLength = 2f;
    public bool useObjectDiameterInstead = false;
    public float objectDiameter = 1f;
<<<<<<< HEAD
<<<<<<< HEAD
    public bool moveInLocalSpace = true; // move relative to parent/local axes
=======
>>>>>>> dbaf82884805f1e5cd70d4d43b7eb51d64eb1aea
=======
>>>>>>> dbaf82884805f1e5cd70d4d43b7eb51d64eb1aea

    [Header("Completion Settings")]
    public float snapDistance = 0.25f;
    public bool snapToTargetWhenCorrect = true;

    [Header("Puzzle State")]
    public bool puzzleComplete = false;

    [Header("Events")]
    public UnityEvent onPuzzleComplete;

    private Camera mainCamera;
    private Vector3[] startingPositions;
    private bool[] blockLocked;

<<<<<<< HEAD
<<<<<<< HEAD
    // Dragging state
    private int draggingIndex = -1;
    private Plane dragPlane;
    private Vector3 dragOffsetWorld;

    // Created by Lexa Hope.
    // Description: Add functionality for click on puzzle completion.
    [Header("Puzzle Audio Source")]
    public AudioClip puzzleClick;
    private AudioSource puzzleClickSource;

    // Created by: Lexa Hope.
    // Description: Add functionality for debugging mode to the program.
    [Header("Debugging Mode Supported")]
    public bool debuggingMode = false;

    void Start()
    {
        // If debugging mode is supported make sure to clear local level on game start.
        if (debuggingMode == true)
        {
            PlayerPrefs.SetInt("FourBlockPuzzle", 0); // Set the current state of the puzzle completion to zero.
            puzzleComplete = false; // Set puzzle complete to false by default.
        } else // Otherwise restore last program state.
        {
            // Declare the last state.
            int lastState = PlayerPrefs.GetInt("FourBlockPuzzle", 0);

            // If the last state is true set true to state otherwise set false.
            if (lastState == 1)
            {
                puzzleComplete = true;
            } else
            {
                puzzleComplete = false;
            }
        }

        mainCamera = Camera.main;
        if (mainCamera == null)
            Debug.LogWarning("[FourBlockPuzzleController] No main camera found. Raycasts will fail.");

        int len = Mathf.Max((blocks != null ? blocks.Length : 0), (targetPositions != null ? targetPositions.Length : 0));
        if (len == 0)
        {
            Debug.LogWarning("[FourBlockPuzzleController] No blocks or targets assigned.");
            return;
        }

        if (blocks == null) blocks = new Transform[0];
=======
=======
>>>>>>> dbaf82884805f1e5cd70d4d43b7eb51d64eb1aea
    void Start()
    {
        mainCamera = Camera.main;

<<<<<<< HEAD
>>>>>>> dbaf82884805f1e5cd70d4d43b7eb51d64eb1aea
=======
>>>>>>> dbaf82884805f1e5cd70d4d43b7eb51d64eb1aea
        startingPositions = new Vector3[blocks.Length];
        blockLocked = new bool[blocks.Length];

=======
    // ============================
    //  PUBLIC INSPECTOR FIELDS
    // ============================

    [Header("Puzzle Blocks")]
    public Transform[] blocks = new Transform[4]; // Array of puzzle block transforms

    [Header("Correct Target Positions")]
    public Transform[] targetPositions = new Transform[4]; // Matching target positions for each block

    [Header("Movement Settings")]
    public float moveWidth = 2f; // X movement amount for click-move mode
    public float moveLength = 2f; // Z movement amount for click-move mode
    public bool useObjectDiameterInstead = false; // Whether to use diameter instead of width/length
    public float objectDiameter = 1f; // Diameter used if above is true
    public bool moveInLocalSpace = true; // Whether movement is local or world space

    [Header("Completion Settings")]
    public float snapDistance = 0.25f; // Distance threshold for snapping to target
    public bool snapToTargetWhenCorrect = true; // Whether to snap when correct

    [Header("Puzzle State")]
    public bool puzzleComplete = false; // Whether puzzle is solved

    [Header("Events")]
    public UnityEvent onPuzzleComplete; // Event fired when puzzle completes

    // ============================
    //  PRIVATE RUNTIME FIELDS
    // ============================

    private Camera mainCamera; // Cached main camera reference
    private Vector3[] startingPositions; // Original block positions
    private bool[] blockLocked; // Whether each block is locked after correct placement

    // Dragging state
    private int draggingIndex = -1; // Index of block currently being dragged
    private Plane dragPlane; // Plane used for raycast dragging
    private Vector3 dragOffsetWorld; // Offset between mouse hit and block center

    // Audio
    [Header("Puzzle Audio Source")]
    public AudioClip puzzleClick = null; // Sound to play on puzzle completion
    private AudioSource puzzleClickSource = null; // AudioSource used to play the clip

    // Debugging
    [Header("Debugging Mode Supported")]
    public bool debuggingMode = false; // Whether debugging mode is enabled

    // ============================
    //  UNITY START METHOD
    // ============================

    void Start()
    {
        // If debugging mode is enabled, reset puzzle state
        if (debuggingMode)
        {
            PlayerPrefs.SetInt("FourBlockPuzzle", 0); // Reset saved state
            puzzleComplete = false; // Reset runtime state
        }
        else
        {
            // Load saved puzzle state
            int lastState = PlayerPrefs.GetInt("FourBlockPuzzle", 0);
            puzzleComplete = lastState == 1; // Convert int to bool
        }

        // Ensure AudioSource exists on this GameObject
        puzzleClickSource = GetComponent<AudioSource>();
        if (puzzleClickSource == null)
            puzzleClickSource = gameObject.AddComponent<AudioSource>();

        puzzleClickSource.playOnAwake = false; // Prevent auto-play
        puzzleClickSource.clip = puzzleClick; // Assign clip

        // Cache main camera
        mainCamera = Camera.main;
        if (mainCamera == null)
            Debug.LogWarning("No main camera found — dragging will not work.");

        // Initialize arrays
        startingPositions = new Vector3[blocks.Length];
        blockLocked = new bool[blocks.Length];

        // Store starting positions
>>>>>>> Stashed changes
        for (int i = 0; i < blocks.Length; i++)
        {
            if (blocks[i] != null)
            {
<<<<<<< Updated upstream
<<<<<<< HEAD
<<<<<<< HEAD
                startingPositions[i] = blocks[i].parent != null ? blocks[i].localPosition : blocks[i].position;
            }
        }

        if (targetPositions == null || targetPositions.Length != blocks.Length)
        {
            var tmp = new Transform[blocks.Length];
            if (targetPositions != null)
            {
                for (int i = 0; i < Mathf.Min(tmp.Length, targetPositions.Length); i++)
                    tmp[i] = targetPositions[i];
            }
            targetPositions = tmp;
        }
=======
                startingPositions[i] = blocks[i].position;
            }
        }
>>>>>>> dbaf82884805f1e5cd70d4d43b7eb51d64eb1aea
=======
                startingPositions[i] = blocks[i].position;
            }
        }
>>>>>>> dbaf82884805f1e5cd70d4d43b7eb51d64eb1aea
    }

    void Update()
    {
        if (puzzleComplete)
            return;

<<<<<<< HEAD
<<<<<<< HEAD
        if (mainCamera == null)
            mainCamera = Camera.main;

        // Mouse down: try to start dragging a block
        if (Input.GetMouseButtonDown(0))
        {
            TryBeginDrag();
        }

        // While holding mouse: update drag position
        if (draggingIndex != -1 && Input.GetMouseButton(0))
        {
            ContinueDrag(draggingIndex);
        }

        // Mouse up: release drag and snap/lock if needed
        if (draggingIndex != -1 && Input.GetMouseButtonUp(0))
        {
            EndDrag(draggingIndex);
=======
        if (Input.GetMouseButtonDown(0))
        {
            TryClickBlock();
>>>>>>> dbaf82884805f1e5cd70d4d43b7eb51d64eb1aea
=======
        if (Input.GetMouseButtonDown(0))
        {
            TryClickBlock();
>>>>>>> dbaf82884805f1e5cd70d4d43b7eb51d64eb1aea
        }

        CheckPuzzleCompletion();
    }

<<<<<<< HEAD
<<<<<<< HEAD
=======
                startingPositions[i] =
                    blocks[i].parent != null ? blocks[i].localPosition : blocks[i].position;
            }
        }

        // Ensure targetPositions matches block count
        if (targetPositions.Length != blocks.Length)
        {
            Transform[] tmp = new Transform[blocks.Length];
            for (int i = 0; i < Mathf.Min(tmp.Length, targetPositions.Length); i++)
                tmp[i] = targetPositions[i];
            targetPositions = tmp;
        }
    }

    // ============================
    //  UNITY UPDATE LOOP
    // ============================

    void Update()
    {
        // Stop all interaction if puzzle is complete
        if (puzzleComplete)
            return;

        // Ensure camera reference exists
        if (mainCamera == null)
            mainCamera = Camera.main;

        // Begin drag on mouse down
        if (Input.GetMouseButtonDown(0))
            TryBeginDrag();

        // Continue dragging while mouse held
        if (draggingIndex != -1 && Input.GetMouseButton(0))
            ContinueDrag(draggingIndex);

        // End drag on mouse release
        if (draggingIndex != -1 && Input.GetMouseButtonUp(0))
            EndDrag(draggingIndex);

        // Check if puzzle is solved
        CheckPuzzleCompletion();
    }

    // ============================
    //  DRAGGING LOGIC
    // ============================

>>>>>>> Stashed changes
    private void TryBeginDrag()
    {
        if (mainCamera == null) return;

<<<<<<< Updated upstream
        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hit))
        {
=======
        // Raycast from mouse position
        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            // Loop through blocks to find which was clicked
>>>>>>> Stashed changes
            for (int i = 0; i < blocks.Length; i++)
            {
                if (blocks[i] == null || blockLocked[i])
                    continue;

<<<<<<< Updated upstream
                // Accept clicks on the block itself or any child collider of the block
                if (hit.transform == blocks[i] || hit.transform.IsChildOf(blocks[i]))
                {
                    // Start dragging this block
                    draggingIndex = i;

                    // Use a horizontal plane (top-down puzzle). Plane normal is up, passing through block world position.
                    Vector3 planeNormal = Vector3.up;
                    dragPlane = new Plane(planeNormal, blocks[i].position);

                    // Compute offset between block position and mouse hit point on plane
=======
                // Accept clicks on block or its children
                if (hit.transform == blocks[i] || hit.transform.IsChildOf(blocks[i]))
                {
                    draggingIndex = i; // Mark block as being dragged

                    // Create drag plane at block height
                    dragPlane = new Plane(Vector3.up, blocks[i].position);

                    // Compute offset between block center and mouse hit
>>>>>>> Stashed changes
                    if (dragPlane.Raycast(ray, out float enter))
                    {
                        Vector3 hitPoint = ray.GetPoint(enter);
                        dragOffsetWorld = blocks[i].position - hitPoint;
                    }
                    else
                    {
                        dragOffsetWorld = Vector3.zero;
                    }

<<<<<<< Updated upstream
                    // Stop after selecting first matching block
=======
>>>>>>> Stashed changes
                    break;
                }
            }
        }
    }

    private void ContinueDrag(int index)
    {
<<<<<<< Updated upstream
        if (index < 0 || index >= blocks.Length) return;
        Transform block = blocks[index];
        if (block == null) return;

        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
        if (dragPlane.Raycast(ray, out float enter))
        {
            Vector3 hitPoint = ray.GetPoint(enter);
            Vector3 desiredWorldPos = hitPoint + dragOffsetWorld;

            if (moveInLocalSpace && block.parent != null)
            {
                // Convert desired world position to parent's local space and assign
                Vector3 localPos = block.parent.InverseTransformPoint(desiredWorldPos);
                block.localPosition = localPos;
            }
            else
            {
                block.position = desiredWorldPos;
            }
=======
        Transform block = blocks[index];
        if (block == null) return;

        // Raycast from mouse
        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);

        if (dragPlane.Raycast(ray, out float enter))
        {
            // Compute desired world position
            Vector3 hitPoint = ray.GetPoint(enter);
            Vector3 desiredWorldPos = hitPoint + dragOffsetWorld;

            // Move block in correct space
            if (moveInLocalSpace && block.parent != null)
                block.localPosition = block.parent.InverseTransformPoint(desiredWorldPos);
            else
                block.position = desiredWorldPos;
>>>>>>> Stashed changes
        }
    }

    private void EndDrag(int index)
    {
<<<<<<< Updated upstream
        if (index < 0 || index >= blocks.Length) return;
=======
>>>>>>> Stashed changes
        Transform block = blocks[index];
        if (block == null)
        {
            draggingIndex = -1;
            return;
        }

<<<<<<< Updated upstream
        // On release, check snapping and locking
=======
        // If block is close enough to target, snap and lock it
>>>>>>> Stashed changes
        if (IsBlockAtCorrectTarget(index))
        {
            if (snapToTargetWhenCorrect && targetPositions[index] != null)
            {
                if (block.parent != null && targetPositions[index].parent == block.parent)
<<<<<<< Updated upstream
                {
                    block.localPosition = targetPositions[index].localPosition;
                }
                else
                {
                    block.position = targetPositions[index].position;
                }
            }

            blockLocked[index] = true;
        }

        draggingIndex = -1;
    }

    // Optional: quick keyboard move (discrete step) if user still wants click-to-move
    private void TryClickBlock()
    {
        // kept for backward compatibility if you want to use discrete moves instead of drag
        if (mainCamera == null) return;

=======
    private void TryClickBlock()
    {
>>>>>>> dbaf82884805f1e5cd70d4d43b7eb51d64eb1aea
=======
    private void TryClickBlock()
    {
>>>>>>> dbaf82884805f1e5cd70d4d43b7eb51d64eb1aea
        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            for (int i = 0; i < blocks.Length; i++)
            {
                if (blocks[i] == null || blockLocked[i])
                    continue;

<<<<<<< HEAD
<<<<<<< HEAD
                if (hit.transform == blocks[i] || hit.transform.IsChildOf(blocks[i]))
=======
                if (hit.transform == blocks[i])
>>>>>>> dbaf82884805f1e5cd70d4d43b7eb51d64eb1aea
=======
                if (hit.transform == blocks[i])
>>>>>>> dbaf82884805f1e5cd70d4d43b7eb51d64eb1aea
                {
                    MoveBlock(i);
                    break;
                }
            }
        }
    }

<<<<<<< HEAD
<<<<<<< HEAD
    public void MoveBlock(int index)
    {
        if (index < 0 || index >= blocks.Length) return;
        Transform block = blocks[index];
        if (block == null) return;

        Vector3 moveAmount;
=======
=======
>>>>>>> dbaf82884805f1e5cd70d4d43b7eb51d64eb1aea
    private void MoveBlock(int index)
    {
        Transform block = blocks[index];

        Vector3 moveAmount;

<<<<<<< HEAD
>>>>>>> dbaf82884805f1e5cd70d4d43b7eb51d64eb1aea
=======
>>>>>>> dbaf82884805f1e5cd70d4d43b7eb51d64eb1aea
        if (useObjectDiameterInstead)
        {
            moveAmount = new Vector3(objectDiameter, 0f, 0f);
        }
        else
        {
            moveAmount = new Vector3(moveWidth, 0f, moveLength);
        }

<<<<<<< HEAD
<<<<<<< HEAD
        if (moveInLocalSpace && block.parent != null)
        {
            block.localPosition += moveAmount;
        }
        else
        {
            block.position += moveAmount;
        }

=======
                    block.localPosition = targetPositions[index].localPosition;
                else
                    block.position = targetPositions[index].position;
            }

            blockLocked[index] = true; // Prevent further movement
        }

        draggingIndex = -1; // Stop dragging
    }

    // ============================
    //  CLICK-MOVE MODE (OPTIONAL)
    // ============================

    public void MoveBlock(int index)
    {
        Transform block = blocks[index];
        if (block == null) return;

        // Determine movement amount
        Vector3 moveAmount = useObjectDiameterInstead
            ? new Vector3(objectDiameter, 0f, 0f)
            : new Vector3(moveWidth, 0f, moveLength);

        // Move block
        if (moveInLocalSpace && block.parent != null)
            block.localPosition += moveAmount;
        else
            block.position += moveAmount;

        // Snap if correct
>>>>>>> Stashed changes
        if (IsBlockAtCorrectTarget(index))
        {
            if (snapToTargetWhenCorrect && targetPositions[index] != null)
            {
                if (block.parent != null && targetPositions[index].parent == block.parent)
<<<<<<< Updated upstream
                {
                    block.localPosition = targetPositions[index].localPosition;
                }
                else
                {
                    block.position = targetPositions[index].position;
                }
=======
=======
>>>>>>> dbaf82884805f1e5cd70d4d43b7eb51d64eb1aea
        block.position += moveAmount;

        if (IsBlockAtCorrectTarget(index))
        {
            if (snapToTargetWhenCorrect)
            {
                block.position = targetPositions[index].position;
<<<<<<< HEAD
>>>>>>> dbaf82884805f1e5cd70d4d43b7eb51d64eb1aea
=======
>>>>>>> dbaf82884805f1e5cd70d4d43b7eb51d64eb1aea
=======
                    block.localPosition = targetPositions[index].localPosition;
                else
                    block.position = targetPositions[index].position;
>>>>>>> Stashed changes
            }

            blockLocked[index] = true;
        }
    }

<<<<<<< Updated upstream
    private bool IsBlockAtCorrectTarget(int index)
    {
<<<<<<< HEAD
<<<<<<< HEAD
        if (index < 0 || index >= blocks.Length) return false;
        if (blocks[index] == null) return false;
        if (targetPositions == null || index >= targetPositions.Length) return false;
        if (targetPositions[index] == null) return false;

        float distance;
        if (blocks[index].parent != null && targetPositions[index].parent == blocks[index].parent)
        {
            distance = Vector3.Distance(blocks[index].localPosition, targetPositions[index].localPosition);
        }
        else
        {
            distance = Vector3.Distance(blocks[index].position, targetPositions[index].position);
        }
=======
=======
>>>>>>> dbaf82884805f1e5cd70d4d43b7eb51d64eb1aea
        if (blocks[index] == null || targetPositions[index] == null)
            return false;

        float distance = Vector3.Distance(
            blocks[index].position,
            targetPositions[index].position
        );
<<<<<<< HEAD
>>>>>>> dbaf82884805f1e5cd70d4d43b7eb51d64eb1aea
=======
>>>>>>> dbaf82884805f1e5cd70d4d43b7eb51d64eb1aea
=======
    // ============================
    //  TARGET CHECKING
    // ============================

    private bool IsBlockAtCorrectTarget(int index)
    {
        if (blocks[index] == null || targetPositions[index] == null)
            return false;

        // Compare local or world positions depending on parent
        float distance =
            (blocks[index].parent != null &&
             targetPositions[index].parent == blocks[index].parent)
            ? Vector3.Distance(blocks[index].localPosition, targetPositions[index].localPosition)
            : Vector3.Distance(blocks[index].position, targetPositions[index].position);
>>>>>>> Stashed changes

        return distance <= snapDistance;
    }

<<<<<<< Updated upstream
    private void CheckPuzzleCompletion()
    {
=======
    // ============================
    //  PUZZLE COMPLETION LOGIC
    // ============================

    private void CheckPuzzleCompletion()
    {
        // Ensure all blocks are correctly placed
>>>>>>> Stashed changes
        for (int i = 0; i < blocks.Length; i++)
        {
            if (!IsBlockAtCorrectTarget(i))
                return;
        }

<<<<<<< Updated upstream
<<<<<<< HEAD
<<<<<<< HEAD
        // Todo: Maje sure that all other obsticals have been complete.
        int stateObstical1 = PlayerPrefs.GetInt("VentClicked", 0);
        int stateObstical2 = PlayerPrefs.GetInt("PaintingClue", 0);

        // If both of the prior states are true then mark the puzzle as completed.
        if (stateObstical1 == 1 && stateObstical2 == 1) {
            puzzleComplete = true; // Return the true state.

            // If the puzzle has been complete create a click noise on it's completion.


            // Save data stating the puzzel is completed.
            PlayerPrefs.SetInt("FourBlockPuzzle", 1);

            // Change the puzzle state to completed.
            onPuzzleComplete?.Invoke();
            Debug.Log("Puzzle Complete! Moving to next phase.");
        }
=======
=======
>>>>>>> dbaf82884805f1e5cd70d4d43b7eb51d64eb1aea
        puzzleComplete = true;
        onPuzzleComplete.Invoke();

        Debug.Log("Puzzle Complete! Moving to next phase.");
<<<<<<< HEAD
>>>>>>> dbaf82884805f1e5cd70d4d43b7eb51d64eb1aea
=======
>>>>>>> dbaf82884805f1e5cd70d4d43b7eb51d64eb1aea
=======
        // Check external obstacles
        int stateObstical1 = PlayerPrefs.GetInt("VentClicked", 0);
        int stateObstical2 = PlayerPrefs.GetInt("PaintingClue", 0);

        // Only complete puzzle if all conditions met
        if (stateObstical1 == 1 && stateObstical2 == 1)
        {
            puzzleComplete = true;

            // Play completion sound
            if (puzzleClickSource != null)
                puzzleClickSource.Play();

            // Save puzzle completion
            PlayerPrefs.SetInt("FourBlockPuzzle", 1);

            // Fire event
            onPuzzleComplete?.Invoke();

            Debug.Log("Puzzle Complete! Moving to next phase.");
        }
>>>>>>> Stashed changes
    }
}