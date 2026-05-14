using UnityEngine;
using UnityEngine.Events;

public class FourBlockPuzzleController : MonoBehaviour
{
<<<<<<< HEAD
=======
<<<<<<< HEAD
=======
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
    public bool moveInLocalSpace = true;

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

    private int draggingIndex = -1;
    private Plane dragPlane;
    private Vector3 dragOffsetWorld;

    [Header("Puzzle Audio Source")]
    public AudioClip puzzleClick;
    private AudioSource puzzleClickSource;

    [Header("Debugging Mode Supported")]
    public bool debuggingMode = false;

    void Start()
    {
        if (debuggingMode)
        {
            PlayerPrefs.SetInt("FourBlockPuzzle", 0);
            puzzleComplete = false;
        }
        else
        {
            puzzleComplete = PlayerPrefs.GetInt("FourBlockPuzzle", 0) == 1;
        }

        mainCamera = Camera.main;

        if (mainCamera == null)
            Debug.LogWarning("[FourBlockPuzzleController] No main camera found. Raycasts will fail.");

        startingPositions = new Vector3[blocks.Length];
        blockLocked = new bool[blocks.Length];

=======
>>>>>>> 418fd7fd9f1e9ac4edd46e6f334b3ad52208b29c
>>>>>>> f0a8c5930e5995cdace1c13cf5804129b2d34f49
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
<<<<<<< HEAD
=======
<<<<<<< HEAD
=======
>>>>>>> Stashed changes
>>>>>>> 418fd7fd9f1e9ac4edd46e6f334b3ad52208b29c
>>>>>>> f0a8c5930e5995cdace1c13cf5804129b2d34f49
        for (int i = 0; i < blocks.Length; i++)
        {
            if (blocks[i] != null)
            {
<<<<<<< HEAD
=======
<<<<<<< HEAD
=======
<<<<<<< HEAD
                startingPositions[i] = blocks[i].parent != null
                    ? blocks[i].localPosition
                    : blocks[i].position;
=======
<<<<<<< Updated upstream
<<<<<<< HEAD
<<<<<<< HEAD
                startingPositions[i] = blocks[i].parent != null ? blocks[i].localPosition : blocks[i].position;
>>>>>>> Lexa-Room0
            }
        }

        if (targetPositions == null || targetPositions.Length != blocks.Length)
        {
            Transform[] fixedTargets = new Transform[blocks.Length];

            if (targetPositions != null)
            {
                for (int i = 0; i < Mathf.Min(fixedTargets.Length, targetPositions.Length); i++)
                {
                    fixedTargets[i] = targetPositions[i];
                }
            }

            targetPositions = fixedTargets;
        }

        puzzleClickSource = GetComponent<AudioSource>();
    }

    void Update()
    {
        if (puzzleComplete)
            return;

        if (mainCamera == null)
            mainCamera = Camera.main;

        if (Input.GetMouseButtonDown(0))
        {
            TryBeginDrag();
        }

        if (draggingIndex != -1 && Input.GetMouseButton(0))
        {
            ContinueDrag(draggingIndex);
        }

        if (draggingIndex != -1 && Input.GetMouseButtonUp(0))
        {
            EndDrag(draggingIndex);
        }

        CheckPuzzleCompletion();
    }

<<<<<<< HEAD
=======
<<<<<<< HEAD
<<<<<<< HEAD
=======
>>>>>>> 418fd7fd9f1e9ac4edd46e6f334b3ad52208b29c
>>>>>>> f0a8c5930e5995cdace1c13cf5804129b2d34f49
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

<<<<<<< HEAD
=======
<<<<<<< HEAD
>>>>>>> f0a8c5930e5995cdace1c13cf5804129b2d34f49
    private void TryBeginDrag()
    {
        if (mainCamera == null) return;

<<<<<<< HEAD
=======
=======
>>>>>>> Stashed changes
>>>>>>> Lexa-Room0
    private void TryBeginDrag()
    {
        if (mainCamera == null)
            return;

<<<<<<< Updated upstream
        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out RaycastHit hit))
        {
=======
>>>>>>> 418fd7fd9f1e9ac4edd46e6f334b3ad52208b29c
>>>>>>> f0a8c5930e5995cdace1c13cf5804129b2d34f49
        // Raycast from mouse position
        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            // Loop through blocks to find which was clicked
<<<<<<< HEAD
=======
<<<<<<< HEAD
=======
>>>>>>> Stashed changes
>>>>>>> 418fd7fd9f1e9ac4edd46e6f334b3ad52208b29c
>>>>>>> f0a8c5930e5995cdace1c13cf5804129b2d34f49
            for (int i = 0; i < blocks.Length; i++)
            {
                if (blocks[i] == null || blockLocked[i])
                    continue;

<<<<<<< HEAD
=======
<<<<<<< HEAD
=======
<<<<<<< HEAD
=======
<<<<<<< Updated upstream
                // Accept clicks on the block itself or any child collider of the block
>>>>>>> Lexa-Room0
                if (hit.transform == blocks[i] || hit.transform.IsChildOf(blocks[i]))
                {
                    draggingIndex = i;
                    dragPlane = new Plane(Vector3.up, blocks[i].position);

<<<<<<< HEAD
=======
                    // Use a horizontal plane (top-down puzzle). Plane normal is up, passing through block world position.
                    Vector3 planeNormal = Vector3.up;
                    dragPlane = new Plane(planeNormal, blocks[i].position);

                    // Compute offset between block position and mouse hit point on plane
=======
>>>>>>> 418fd7fd9f1e9ac4edd46e6f334b3ad52208b29c
>>>>>>> f0a8c5930e5995cdace1c13cf5804129b2d34f49
                // Accept clicks on block or its children
                if (hit.transform == blocks[i] || hit.transform.IsChildOf(blocks[i]))
                {
                    draggingIndex = i; // Mark block as being dragged

                    // Create drag plane at block height
                    dragPlane = new Plane(Vector3.up, blocks[i].position);

                    // Compute offset between block center and mouse hit
<<<<<<< HEAD
=======
<<<<<<< HEAD
=======
>>>>>>> Stashed changes
>>>>>>> Lexa-Room0
>>>>>>> 418fd7fd9f1e9ac4edd46e6f334b3ad52208b29c
>>>>>>> f0a8c5930e5995cdace1c13cf5804129b2d34f49
                    if (dragPlane.Raycast(ray, out float enter))
                    {
                        Vector3 hitPoint = ray.GetPoint(enter);
                        dragOffsetWorld = blocks[i].position - hitPoint;
                    }
                    else
                    {
                        dragOffsetWorld = Vector3.zero;
                    }

<<<<<<< HEAD
=======
<<<<<<< HEAD
=======
<<<<<<< HEAD
=======
<<<<<<< Updated upstream
                    // Stop after selecting first matching block
=======
>>>>>>> Stashed changes
>>>>>>> Lexa-Room0
>>>>>>> 418fd7fd9f1e9ac4edd46e6f334b3ad52208b29c
>>>>>>> f0a8c5930e5995cdace1c13cf5804129b2d34f49
                    break;
                }
            }
        }
    }

    private void ContinueDrag(int index)
    {
<<<<<<< HEAD
=======
<<<<<<< HEAD
=======
<<<<<<< HEAD
        if (index < 0 || index >= blocks.Length)
            return;

=======
<<<<<<< Updated upstream
        if (index < 0 || index >= blocks.Length) return;
>>>>>>> Lexa-Room0
        Transform block = blocks[index];

        if (block == null)
            return;

        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);

        if (dragPlane.Raycast(ray, out float enter))
        {
            Vector3 hitPoint = ray.GetPoint(enter);
            Vector3 desiredWorldPos = hitPoint + dragOffsetWorld;

            if (moveInLocalSpace && block.parent != null)
            {
                block.localPosition = block.parent.InverseTransformPoint(desiredWorldPos);
            }
            else
            {
                block.position = desiredWorldPos;
            }
=======
>>>>>>> 418fd7fd9f1e9ac4edd46e6f334b3ad52208b29c
>>>>>>> f0a8c5930e5995cdace1c13cf5804129b2d34f49
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
<<<<<<< HEAD
=======
<<<<<<< HEAD
=======
>>>>>>> Stashed changes
>>>>>>> 418fd7fd9f1e9ac4edd46e6f334b3ad52208b29c
>>>>>>> f0a8c5930e5995cdace1c13cf5804129b2d34f49
        }
    }

    private void EndDrag(int index)
    {
<<<<<<< HEAD
        Transform block = blocks[index];
=======
<<<<<<< HEAD
        Transform block = blocks[index];
=======
<<<<<<< HEAD
        if (index < 0 || index >= blocks.Length)
        {
            draggingIndex = -1;
            return;
        }

=======
<<<<<<< Updated upstream
        if (index < 0 || index >= blocks.Length) return;
=======
>>>>>>> Stashed changes
>>>>>>> Lexa-Room0
        Transform block = blocks[index];

>>>>>>> 418fd7fd9f1e9ac4edd46e6f334b3ad52208b29c
>>>>>>> f0a8c5930e5995cdace1c13cf5804129b2d34f49
        if (block == null)
        {
            draggingIndex = -1;
            return;
        }

<<<<<<< HEAD
        // If block is close enough to target, snap and lock it
=======
<<<<<<< HEAD
        // If block is close enough to target, snap and lock it
=======
<<<<<<< HEAD
        if (IsBlockAtCorrectTarget(index))
        {
            SnapBlockToTarget(index);
=======
<<<<<<< Updated upstream
        // On release, check snapping and locking
=======
        // If block is close enough to target, snap and lock it
>>>>>>> Stashed changes
>>>>>>> 418fd7fd9f1e9ac4edd46e6f334b3ad52208b29c
>>>>>>> f0a8c5930e5995cdace1c13cf5804129b2d34f49
        if (IsBlockAtCorrectTarget(index))
        {
            if (snapToTargetWhenCorrect && targetPositions[index] != null)
            {
                if (block.parent != null && targetPositions[index].parent == block.parent)
<<<<<<< HEAD
=======
<<<<<<< HEAD
=======
<<<<<<< Updated upstream
                {
                    block.localPosition = targetPositions[index].localPosition;
                }
                else
                {
                    block.position = targetPositions[index].position;
                }
            }

>>>>>>> Lexa-Room0
            blockLocked[index] = true;
        }

        draggingIndex = -1;
    }

    public void MoveBlock(int index)
    {
        if (index < 0 || index >= blocks.Length)
            return;

        Transform block = blocks[index];

        if (block == null)
            return;

        Vector3 moveAmount = useObjectDiameterInstead
            ? new Vector3(objectDiameter, 0f, 0f)
            : new Vector3(moveWidth, 0f, moveLength);

        if (moveInLocalSpace && block.parent != null)
        {
            block.localPosition += moveAmount;
        }
        else
        {
            block.position += moveAmount;
        }

=======
>>>>>>> 418fd7fd9f1e9ac4edd46e6f334b3ad52208b29c
>>>>>>> f0a8c5930e5995cdace1c13cf5804129b2d34f49
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
<<<<<<< HEAD
=======
<<<<<<< HEAD
>>>>>>> f0a8c5930e5995cdace1c13cf5804129b2d34f49
        if (IsBlockAtCorrectTarget(index))
        {
            if (snapToTargetWhenCorrect && targetPositions[index] != null)
            {
                if (block.parent != null && targetPositions[index].parent == block.parent)
                    block.localPosition = targetPositions[index].localPosition;
                else
                    block.position = targetPositions[index].position;
            }

<<<<<<< HEAD
=======
=======
>>>>>>> Stashed changes
        if (IsBlockAtCorrectTarget(index))
        {
<<<<<<< HEAD
            SnapBlockToTarget(index);
=======
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

>>>>>>> Lexa-Room0
>>>>>>> 418fd7fd9f1e9ac4edd46e6f334b3ad52208b29c
>>>>>>> f0a8c5930e5995cdace1c13cf5804129b2d34f49
            blockLocked[index] = true;
        }
    }

<<<<<<< HEAD
=======
<<<<<<< HEAD
=======
<<<<<<< HEAD
    private void SnapBlockToTarget(int index)
    {
        if (!snapToTargetWhenCorrect)
            return;

        if (index < 0 || index >= blocks.Length || index >= targetPositions.Length)
            return;

        Transform block = blocks[index];
        Transform target = targetPositions[index];

        if (block == null || target == null)
            return;

        if (block.parent != null && target.parent == block.parent)
        {
            block.localPosition = target.localPosition;
        }
        else
        {
            block.position = target.position;
        }
    }

=======
<<<<<<< Updated upstream
>>>>>>> Lexa-Room0
    private bool IsBlockAtCorrectTarget(int index)
    {
        if (index < 0 || index >= blocks.Length)
            return false;

        if (targetPositions == null || index >= targetPositions.Length)
            return false;

        if (blocks[index] == null || targetPositions[index] == null)
            return false;

        float distance;

        if (blocks[index].parent != null && targetPositions[index].parent == blocks[index].parent)
        {
            distance = Vector3.Distance(blocks[index].localPosition, targetPositions[index].localPosition);
        }
        else
        {
            distance = Vector3.Distance(blocks[index].position, targetPositions[index].position);
        }
<<<<<<< HEAD
=======
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
>>>>>>> 418fd7fd9f1e9ac4edd46e6f334b3ad52208b29c
>>>>>>> f0a8c5930e5995cdace1c13cf5804129b2d34f49
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
<<<<<<< HEAD
=======
<<<<<<< HEAD
=======
>>>>>>> Stashed changes
>>>>>>> Lexa-Room0
>>>>>>> 418fd7fd9f1e9ac4edd46e6f334b3ad52208b29c
>>>>>>> f0a8c5930e5995cdace1c13cf5804129b2d34f49

        return distance <= snapDistance;
    }

<<<<<<< HEAD
=======
<<<<<<< HEAD
=======
<<<<<<< Updated upstream
    private void CheckPuzzleCompletion()
    {
=======
>>>>>>> 418fd7fd9f1e9ac4edd46e6f334b3ad52208b29c
>>>>>>> f0a8c5930e5995cdace1c13cf5804129b2d34f49
    // ============================
    //  PUZZLE COMPLETION LOGIC
    // ============================

    private void CheckPuzzleCompletion()
    {
        // Ensure all blocks are correctly placed
<<<<<<< HEAD
=======
<<<<<<< HEAD
=======
>>>>>>> Stashed changes
>>>>>>> 418fd7fd9f1e9ac4edd46e6f334b3ad52208b29c
>>>>>>> f0a8c5930e5995cdace1c13cf5804129b2d34f49
        for (int i = 0; i < blocks.Length; i++)
        {
            if (!IsBlockAtCorrectTarget(i))
                return;
        }

<<<<<<< HEAD
=======
<<<<<<< HEAD
=======
<<<<<<< HEAD
        int stateObstacle1 = PlayerPrefs.GetInt("VentClicked", 0);
        int stateObstacle2 = PlayerPrefs.GetInt("PaintingClue", 0);
=======
<<<<<<< Updated upstream
<<<<<<< HEAD
<<<<<<< HEAD
        // Todo: Maje sure that all other obsticals have been complete.
        int stateObstical1 = PlayerPrefs.GetInt("VentClicked", 0);
        int stateObstical2 = PlayerPrefs.GetInt("PaintingClue", 0);
>>>>>>> Lexa-Room0

        if (stateObstacle1 == 1 && stateObstacle2 == 1)
        {
            puzzleComplete = true;

            PlayerPrefs.SetInt("FourBlockPuzzle", 1);
            PlayerPrefs.Save();

            if (puzzleClickSource != null && puzzleClick != null)
            {
                puzzleClickSource.PlayOneShot(puzzleClick);
            }

            onPuzzleComplete?.Invoke();

            Debug.Log("Puzzle Complete! Moving to next phase.");
        }
<<<<<<< HEAD
=======
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
>>>>>>> 418fd7fd9f1e9ac4edd46e6f334b3ad52208b29c
>>>>>>> f0a8c5930e5995cdace1c13cf5804129b2d34f49
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
<<<<<<< HEAD
=======
<<<<<<< HEAD
=======
>>>>>>> Stashed changes
>>>>>>> Lexa-Room0
>>>>>>> 418fd7fd9f1e9ac4edd46e6f334b3ad52208b29c
>>>>>>> f0a8c5930e5995cdace1c13cf5804129b2d34f49
    }
}