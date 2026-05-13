/*
 * Purpose: Script to be attached to interactable puzzles (preferably empty parent). Handles how puzzles respond to being double clicked on and activating their scripts.
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
    public Transform puzzleViewSpot; // Position that the camera will move to
    public float moveDuration = 1f;  // Amount of time it'll take to move there

    [Header("Pickup View Settings")]
    public float pickupDistanceFromCamera = 0f; // How far the puzzle will appear before camera
    public float pickupVerticalOffset = 0f;     // How high or low the puzzle is held before camera
    public Vector3 pickupRotation = Vector3.zero; // The rotation the object will initially be at upon picking up

    [Header("Scripts To Enable")]
    public MonoBehaviour[] puzzleScripts;

    // This is the node that you must be at in order to interact with the puzzle
    public CameraNode necessaryNode;
    private Collider interactionCollider; // The collider on the puzzle that is detected from a double click

    private Vector3 originalPosition;
    private Quaternion originalRotation;

    void Start()
    {
        interactionCollider = GetComponent<Collider>();
    }

    // These functions save the puzzle's original location and returns it to that location in the case that you pick it up
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

    // Enables the colliders of accessible puzzles, defined by the node you're currently visiting
    public void SetAccessible(bool accessible)
    {
        if (interactionCollider != null)
        {
            interactionCollider.enabled = accessible;
        }
    }

    // Activate all puzzles within the list
    public void ActivatePuzzle()
    {
        foreach (MonoBehaviour script in puzzleScripts)
        {
            script.enabled = true;
        }
    }

    // Deactivate all puzzles within the list
    public void DeactivatePuzzle()
    {
        foreach (MonoBehaviour script in puzzleScripts)
        {
            script.enabled = false;
        }
    }
}