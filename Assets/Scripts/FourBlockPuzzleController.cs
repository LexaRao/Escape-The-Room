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

        for (int i = 0; i < blocks.Length; i++)
        {
            if (blocks[i] != null)
            {
                startingPositions[i] = blocks[i].parent != null
                    ? blocks[i].localPosition
                    : blocks[i].position;
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

    private void TryBeginDrag()
    {
        if (mainCamera == null)
            return;

        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            for (int i = 0; i < blocks.Length; i++)
            {
                if (blocks[i] == null || blockLocked[i])
                    continue;

                if (hit.transform == blocks[i] || hit.transform.IsChildOf(blocks[i]))
                {
                    draggingIndex = i;
                    dragPlane = new Plane(Vector3.up, blocks[i].position);

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
        if (index < 0 || index >= blocks.Length)
            return;

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
        }
    }

    private void EndDrag(int index)
    {
        if (index < 0 || index >= blocks.Length)
        {
            draggingIndex = -1;
            return;
        }

        Transform block = blocks[index];

        if (block == null)
        {
            draggingIndex = -1;
            return;
        }

        if (IsBlockAtCorrectTarget(index))
        {
            SnapBlockToTarget(index);
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

        if (IsBlockAtCorrectTarget(index))
        {
            SnapBlockToTarget(index);
            blockLocked[index] = true;
        }
    }

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

        return distance <= snapDistance;
    }

    private void CheckPuzzleCompletion()
    {
        for (int i = 0; i < blocks.Length; i++)
        {
            if (!IsBlockAtCorrectTarget(i))
                return;
        }

        int stateObstacle1 = PlayerPrefs.GetInt("VentClicked", 0);
        int stateObstacle2 = PlayerPrefs.GetInt("PaintingClue", 0);

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
    }
}