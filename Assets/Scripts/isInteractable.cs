using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Collider))]
public class isInteractable : MonoBehaviour
{
    [Header("Hover Color Settings")]
    public Color hoverColor = Color.cyan;
    public float lerpSpeed = 8f;

    [Header("Raycast Settings")]
    public Camera targetCamera;
    public float rayDistance = 1000f;

    private Renderer rend;
    private Material mat;
    private Color originalColor;
    private Color targetColor;

    private static isInteractable currentlyHovered;

    void Awake()
    {
        rend = GetComponentInChildren<Renderer>();

        if (rend == null)
        {
            Debug.LogError($"{gameObject.name} has no Renderer.");
            enabled = false;
            return;
        }

        mat = rend.material; // creates instance so it won’t affect others
        originalColor = mat.color;
        targetColor = originalColor;

        if (targetCamera == null)
            targetCamera = Camera.main;
    }

    void Update()
    {
        UpdateHoverState();
        UpdateColor();
    }

    private void UpdateHoverState()
    {
        if (targetCamera == null || Mouse.current == null)
            return;

        Vector2 mousePosition = Mouse.current.position.ReadValue();
        Ray ray = targetCamera.ScreenPointToRay(mousePosition);

        isInteractable hitHover = null;

        if (Physics.Raycast(ray, out RaycastHit hit, rayDistance))
        {
            hitHover = hit.collider.GetComponentInParent<isInteractable>();
        }

        if (currentlyHovered != hitHover)
        {
            if (currentlyHovered != null)
                currentlyHovered.SetHovered(false);

            if (hitHover != null)
                hitHover.SetHovered(true);

            currentlyHovered = hitHover;
        }
    }

    private void SetHovered(bool hovered)
    {
        targetColor = hovered ? hoverColor * 3f : originalColor;
    }

    private void UpdateColor()
    {
        mat.color = Color.Lerp(
            mat.color,
            targetColor,
            Time.deltaTime * lerpSpeed
        );
    }

    private void OnDisable()
    {
        if (currentlyHovered == this)
            currentlyHovered = null;

        if (mat != null)
            mat.color = originalColor;
    }
}