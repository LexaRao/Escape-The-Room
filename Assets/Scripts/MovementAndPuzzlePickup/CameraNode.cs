/*
 * Purpose: Script to be attached to camera nodes (empty gameobjects). Decides limitations and connections for each node.
 */

using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class CameraNode : MonoBehaviour
{
    [System.Serializable]
    public struct CamConnections
    {
        public CameraNode targetNode; // Connected node
        public float moveDuration;    // Move time from node to node
        public float rotationDelay;   // Amount of seconds to wait during movement until camera begins to rotate
    }

    [Header("Rotational view limitations")]
    public Vector2 yawLimits;
    public Vector2 pitchLimits; // Typically (-90 to 90)

    // A boolean to signal that there's a limit on horizontal rotation. Typically false
    public bool hasYawLimits;

    [Header("Connected cameras")]
    public List<CamConnections> connections;

    [Header("Puzzles at this node")]
    public List<PuzzleObj> puzzlesAtNode;

    // Returns a given camera node's collider
    public Collider GetClickLocationCollider() { return GetComponent<Collider>(); }

    // Represent camera nodes by circles with a line shooting out to show default view for a given node
    void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawSphere(transform.position, 0.2f);

        // Show where the camera is facing
        Gizmos.color = Color.blue;
        Gizmos.DrawLine(transform.position, transform.position + transform.forward);
    }

    // Getters for a camera node's position and rotation in euler angles
    public Vector3 getPosition() {  return transform.position; }

    public Vector3 getRotation() { return transform.rotation.eulerAngles; }
}
