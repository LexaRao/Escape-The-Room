using UnityEngine;
using UnityEngine.Events;

public class FourBlockPuzzleController : MonoBehaviour
{
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
        for (int i = 0; i < blocks.Length; i++)
        {
            if (blocks[i] != null)
            {
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

    private void TryBeginDrag()
    {
        if (mainCamera == null) return;

        // Raycast from mouse position
        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            // Loop through blocks to find which was clicked
            for (int i = 0; i < blocks.Length; i++)
            {
                if (blocks[i] == null || blockLocked[i])
                    continue;

                // Accept clicks on block or its children
                if (hit.transform == blocks[i] || hit.transform.IsChildOf(blocks[i]))
                {
                    draggingIndex = i; // Mark block as being dragged

                    // Create drag plane at block height
                    dragPlane = new Plane(Vector3.up, blocks[i].position);

                    // Compute offset between block center and mouse hit
                    if (dragPlane.Raycast(ray, out float enter))
                    {
                        Vector3 hitPoint = ray.GetPoint(enter);
                        dragOffsetWorld = blocks[i].position - hitPoint;
                    }
                    else
                    {
                        dragOffsetWorld = Vector3.zero;
                    }

                    break;
                }
            }
        }
    }

    private void ContinueDrag(int index)
    {
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
        }
    }

    private void EndDrag(int index)
    {
        Transform block = blocks[index];
        if (block == null)
        {
            draggingIndex = -1;
            return;
        }

        // If block is close enough to target, snap and lock it
        if (IsBlockAtCorrectTarget(index))
        {
            if (snapToTargetWhenCorrect && targetPositions[index] != null)
            {
                if (block.parent != null && targetPositions[index].parent == block.parent)
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
        if (IsBlockAtCorrectTarget(index))
        {
            if (snapToTargetWhenCorrect && targetPositions[index] != null)
            {
                if (block.parent != null && targetPositions[index].parent == block.parent)
                    block.localPosition = targetPositions[index].localPosition;
                else
                    block.position = targetPositions[index].position;
            }

            blockLocked[index] = true;
        }
    }

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

        return distance <= snapDistance;
    }

    // ============================
    //  PUZZLE COMPLETION LOGIC
    // ============================

    private void CheckPuzzleCompletion()
    {
        // Ensure all blocks are correctly placed
        for (int i = 0; i < blocks.Length; i++)
        {
            if (!IsBlockAtCorrectTarget(i))
                return;
        }

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
    }
}