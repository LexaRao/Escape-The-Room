/*
 * Purpose: Handles how puzzles respond to be double clicked on and activating their scripts
 */

using UnityEngine;

// Enumerator that defines whether puzzle will be moved to or picked up. To be defined in inspector
public enum PuzzleViewMode
{
    MoveCameraToPuzzle,
    BringPuzzleToCamera
}

public class PuzzleObj : MonoBehaviour
{
    public PuzzleViewMode viewMode;

    [Header("Camera View Target")]
    public Transform puzzleViewSpot;
    public float moveDuration = 1f;

    [Header("Pickup View Settings")]
    public float pickupDistanceFromCamera = 2f;
    public float pickupVerticalOffset = -0.2f;
    public Vector3 pickupRotation = Vector3.zero;

    [Header("Scripts To Enable")]
    public MonoBehaviour[] puzzleScripts;

    // This is the node that you must be at in order to interact with the puzzle
    public CameraNode necessaryNode;
    //public Collider interactionCollider;
    private Collider interactionCollider;

    //
    private Vector3 originalPosition;
    private Quaternion originalRotation;

    void Start()
    {
        interactionCollider = GetComponent<Collider>();
    }

    public void SaveOriginalTransform()
    {
        originalPosition = transform.position;
        originalRotation = transform.rotation;
    }

    public void RestoreOriginalTransform()
    {
        transform.position = originalPosition;
        transform.rotation = originalRotation;
    }

    public void SetAccessible(bool accessible)
    {
        if (interactionCollider != null)
        {
            interactionCollider.enabled = accessible;
        }
    }

    public void ActivatePuzzle()
    {
        foreach (MonoBehaviour script in puzzleScripts)
        {
            script.enabled = true;
        }
    }

    public void DeactivatePuzzle()
    {
        foreach (MonoBehaviour script in puzzleScripts)
        {
            script.enabled = false;
        }
    }
}