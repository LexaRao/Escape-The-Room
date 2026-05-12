using UnityEngine;
using UnityEngine.Events;

public class FourBlockPuzzleController : MonoBehaviour
{
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
    public bool moveInLocalSpace = true; // move relative to parent/local axes
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
    void Start()
    {
        mainCamera = Camera.main;

>>>>>>> dbaf82884805f1e5cd70d4d43b7eb51d64eb1aea
        startingPositions = new Vector3[blocks.Length];
        blockLocked = new bool[blocks.Length];

        for (int i = 0; i < blocks.Length; i++)
        {
            if (blocks[i] != null)
            {
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
    }

    void Update()
    {
        if (puzzleComplete)
            return;

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
        }

        CheckPuzzleCompletion();
    }

<<<<<<< HEAD
    private void TryBeginDrag()
    {
        if (mainCamera == null) return;

        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            for (int i = 0; i < blocks.Length; i++)
            {
                if (blocks[i] == null || blockLocked[i])
                    continue;

                // Accept clicks on the block itself or any child collider of the block
                if (hit.transform == blocks[i] || hit.transform.IsChildOf(blocks[i]))
                {
                    // Start dragging this block
                    draggingIndex = i;

                    // Use a horizontal plane (top-down puzzle). Plane normal is up, passing through block world position.
                    Vector3 planeNormal = Vector3.up;
                    dragPlane = new Plane(planeNormal, blocks[i].position);

                    // Compute offset between block position and mouse hit point on plane
                    if (dragPlane.Raycast(ray, out float enter))
                    {
                        Vector3 hitPoint = ray.GetPoint(enter);
                        dragOffsetWorld = blocks[i].position - hitPoint;
                    }
                    else
                    {
                        dragOffsetWorld = Vector3.zero;
                    }

                    // Stop after selecting first matching block
                    break;
                }
            }
        }
    }

    private void ContinueDrag(int index)
    {
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
        }
    }

    private void EndDrag(int index)
    {
        if (index < 0 || index >= blocks.Length) return;
        Transform block = blocks[index];
        if (block == null)
        {
            draggingIndex = -1;
            return;
        }

        // On release, check snapping and locking
        if (IsBlockAtCorrectTarget(index))
        {
            if (snapToTargetWhenCorrect && targetPositions[index] != null)
            {
                if (block.parent != null && targetPositions[index].parent == block.parent)
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
        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            for (int i = 0; i < blocks.Length; i++)
            {
                if (blocks[i] == null || blockLocked[i])
                    continue;

<<<<<<< HEAD
                if (hit.transform == blocks[i] || hit.transform.IsChildOf(blocks[i]))
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
    public void MoveBlock(int index)
    {
        if (index < 0 || index >= blocks.Length) return;
        Transform block = blocks[index];
        if (block == null) return;

        Vector3 moveAmount;
=======
    private void MoveBlock(int index)
    {
        Transform block = blocks[index];

        Vector3 moveAmount;

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
        if (moveInLocalSpace && block.parent != null)
        {
            block.localPosition += moveAmount;
        }
        else
        {
            block.position += moveAmount;
        }

        if (IsBlockAtCorrectTarget(index))
        {
            if (snapToTargetWhenCorrect && targetPositions[index] != null)
            {
                if (block.parent != null && targetPositions[index].parent == block.parent)
                {
                    block.localPosition = targetPositions[index].localPosition;
                }
                else
                {
                    block.position = targetPositions[index].position;
                }
=======
        block.position += moveAmount;

        if (IsBlockAtCorrectTarget(index))
        {
            if (snapToTargetWhenCorrect)
            {
                block.position = targetPositions[index].position;
>>>>>>> dbaf82884805f1e5cd70d4d43b7eb51d64eb1aea
            }

            blockLocked[index] = true;
        }
    }

    private bool IsBlockAtCorrectTarget(int index)
    {
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
        if (blocks[index] == null || targetPositions[index] == null)
            return false;

        float distance = Vector3.Distance(
            blocks[index].position,
            targetPositions[index].position
        );
>>>>>>> dbaf82884805f1e5cd70d4d43b7eb51d64eb1aea

        return distance <= snapDistance;
    }

    private void CheckPuzzleCompletion()
    {
        for (int i = 0; i < blocks.Length; i++)
        {
            if (!IsBlockAtCorrectTarget(i))
                return;
        }

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
        puzzleComplete = true;
        onPuzzleComplete.Invoke();

        Debug.Log("Puzzle Complete! Moving to next phase.");
>>>>>>> dbaf82884805f1e5cd70d4d43b7eb51d64eb1aea
    }
}