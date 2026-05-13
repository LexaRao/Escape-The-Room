using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(isInteractable))]
public abstract class PuzzleInteractable : MonoBehaviour
{
    [Header("Puzzle Settings")]
    public int puzzleNumber;
    public string promptMessage = "Press E to Interact";

    protected bool hasBeenSolved = false;
    private Camera mainCam;

    void Awake()
    {
        mainCam = Camera.main;
    }

    protected virtual void Update()
    {
        if (hasBeenSolved) return;

        // Use mouse click instead of E key — fits point and click style
        if (Mouse.current.leftButton.wasPressedThisFrame)
        {
            if (IsMouseOver())
                Interact();
        }
    }

    private bool IsMouseOver()
    {
        // Raycast from mouse position instead of screen center
        Ray ray = mainCam.ScreenPointToRay(
            Mouse.current.position.ReadValue());

        if (Physics.Raycast(ray, out RaycastHit hit, 100f))
            return hit.collider.gameObject == gameObject;

        return false;
    }

    public abstract void Interact();

    protected void MarkSolved()
    {
        if (hasBeenSolved) return;
        hasBeenSolved = true;
        LabRoomManager.Instance.SolvePuzzle(puzzleNumber);
        Debug.Log($"Puzzle {puzzleNumber} solved!");
    }
}