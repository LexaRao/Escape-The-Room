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

    void Start()
    {
        mainCamera = Camera.main;

        startingPositions = new Vector3[blocks.Length];
        blockLocked = new bool[blocks.Length];

        for (int i = 0; i < blocks.Length; i++)
        {
            if (blocks[i] != null)
            {
                startingPositions[i] = blocks[i].position;
            }
        }
    }

    void Update()
    {
        if (puzzleComplete)
            return;

        if (Input.GetMouseButtonDown(0))
        {
            TryClickBlock();
        }

        CheckPuzzleCompletion();
    }

    private void TryClickBlock()
    {
        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            for (int i = 0; i < blocks.Length; i++)
            {
                if (blocks[i] == null || blockLocked[i])
                    continue;

                if (hit.transform == blocks[i])
                {
                    MoveBlock(i);
                    break;
                }
            }
        }
    }

    private void MoveBlock(int index)
    {
        Transform block = blocks[index];

        Vector3 moveAmount;

        if (useObjectDiameterInstead)
        {
            moveAmount = new Vector3(objectDiameter, 0f, 0f);
        }
        else
        {
            moveAmount = new Vector3(moveWidth, 0f, moveLength);
        }

        block.position += moveAmount;

        if (IsBlockAtCorrectTarget(index))
        {
            if (snapToTargetWhenCorrect)
            {
                block.position = targetPositions[index].position;
            }

            blockLocked[index] = true;
        }
    }

    private bool IsBlockAtCorrectTarget(int index)
    {
        if (blocks[index] == null || targetPositions[index] == null)
            return false;

        float distance = Vector3.Distance(
            blocks[index].position,
            targetPositions[index].position
        );

        return distance <= snapDistance;
    }

    private void CheckPuzzleCompletion()
    {
        for (int i = 0; i < blocks.Length; i++)
        {
            if (!IsBlockAtCorrectTarget(i))
                return;
        }

        puzzleComplete = true;
        onPuzzleComplete.Invoke();

        Debug.Log("Puzzle Complete! Moving to next phase.");
    }
}