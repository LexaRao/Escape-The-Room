using UnityEngine;
using UnityEngine.InputSystem;

public class RotateCipherA : MonoBehaviour
{
    [SerializeField] private float rotationSpeed = 0.3f;

    private bool _isDragging;

    private void Update()
    {
        if (Mouse.current == null)
            return;

        if (Mouse.current.leftButton.wasPressedThisFrame)
            _isDragging = true;

        if (Mouse.current.leftButton.wasReleasedThisFrame)
            _isDragging = false;

        if (_isDragging)
        {
            float mouseX = Mouse.current.delta.ReadValue().x;

            // Rotate around LOCAL up axis (respects Y = 90 rotation)
            transform.Rotate(Vector3.up, -mouseX * rotationSpeed, Space.Self);
        }
    }
}