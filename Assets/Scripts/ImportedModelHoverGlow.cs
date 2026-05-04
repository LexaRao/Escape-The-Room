using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections.Generic;

public class ImportedModelHoverGlow : MonoBehaviour
{
    [Header("Glow Settings")]
    [ColorUsage(true, true)]
    public Color glowColor = Color.cyan;
    public float glowIntensity = 2f;
    public float lerpSpeed = 8f;

    [Header("Raycast Settings")]
    public Camera targetCamera;
    public float rayDistance = 1000f;
    public LayerMask raycastLayers = ~0;

    [Header("Model Settings")]
    public bool includeInactiveChildren = false;

    private Renderer[] renderersToGlow;
    private readonly List<Material> materials = new List<Material>();
    private readonly Dictionary<Material, Color> originalEmissionColors = new Dictionary<Material, Color>();

    private bool isHovered;

    private static ImportedModelHoverGlow currentlyHovered;
    private static readonly Dictionary<Collider, ImportedModelHoverGlow> colliderOwnerMap = new Dictionary<Collider, ImportedModelHoverGlow>();

    void Awake()
    {
        if (targetCamera == null)
            targetCamera = Camera.main;

        CacheRenderers();
        CacheMaterials();
        RegisterColliders();
    }

    void Update()
    {
        UpdateHoverState();
        UpdateGlow();
    }

    private void CacheRenderers()
    {
        renderersToGlow = GetComponentsInChildren<Renderer>(includeInactiveChildren);

        if (renderersToGlow == null || renderersToGlow.Length == 0)
        {
            Debug.LogWarning($"No Renderers found on {gameObject.name} or its children.");
        }
    }

    private void CacheMaterials()
    {
        materials.Clear();
        originalEmissionColors.Clear();

        foreach (Renderer rend in renderersToGlow)
        {
            if (rend == null)
                continue;

            foreach (Material mat in rend.materials)
            {
                if (mat == null || materials.Contains(mat))
                    continue;

                materials.Add(mat);

                if (mat.HasProperty("_EmissionColor"))
                {
                    mat.EnableKeyword("_EMISSION");
                    originalEmissionColors[mat] = mat.GetColor("_EmissionColor");
                }
            }
        }
    }

    private void RegisterColliders()
    {
        Collider[] colliders = GetComponentsInChildren<Collider>(includeInactiveChildren);

        if (colliders.Length == 0)
        {
            Debug.LogWarning(
                $"No Colliders found on {gameObject.name} or its children. " +
                $"Add a Collider to the model or one of its child meshes so hover detection can work."
            );
            return;
        }

        foreach (Collider col in colliders)
        {
            if (col != null)
                colliderOwnerMap[col] = this;
        }
    }

    private void UpdateHoverState()
    {
        if (targetCamera == null || Mouse.current == null)
            return;

        Vector2 mousePosition = Mouse.current.position.ReadValue();
        Ray ray = targetCamera.ScreenPointToRay(mousePosition);

        ImportedModelHoverGlow hitGlow = null;

        if (Physics.Raycast(ray, out RaycastHit hit, rayDistance, raycastLayers))
        {
            colliderOwnerMap.TryGetValue(hit.collider, out hitGlow);

            if (hitGlow == null)
            {
                hitGlow = hit.collider.GetComponentInParent<ImportedModelHoverGlow>();
            }
        }

        if (currentlyHovered != hitGlow)
        {
            if (currentlyHovered != null)
                currentlyHovered.SetHovered(false);

            if (hitGlow != null)
                hitGlow.SetHovered(true);

            currentlyHovered = hitGlow;
        }
    }

    private void SetHovered(bool hovered)
    {
        isHovered = hovered;
    }

    private void UpdateGlow()
    {
        foreach (Material mat in materials)
        {
            if (mat == null || !mat.HasProperty("_EmissionColor"))
                continue;

            Color originalColor = originalEmissionColors.ContainsKey(mat)
                ? originalEmissionColors[mat]
                : Color.black;

            Color targetColor = isHovered
                ? glowColor * glowIntensity
                : originalColor;

            Color currentColor = mat.GetColor("_EmissionColor");
            Color newColor = Color.Lerp(currentColor, targetColor, Time.deltaTime * lerpSpeed);

            mat.SetColor("_EmissionColor", newColor);
        }
    }

    void OnDisable()
    {
        if (currentlyHovered == this)
            currentlyHovered = null;

        foreach (Material mat in materials)
        {
            if (mat == null || !mat.HasProperty("_EmissionColor"))
                continue;

            if (originalEmissionColors.TryGetValue(mat, out Color originalColor))
            {
                mat.SetColor("_EmissionColor", originalColor);
            }
        }
    }

    void OnDestroy()
    {
        Collider[] colliders = GetComponentsInChildren<Collider>(includeInactiveChildren);

        foreach (Collider col in colliders)
        {
            if (col != null && colliderOwnerMap.ContainsKey(col) && colliderOwnerMap[col] == this)
            {
                colliderOwnerMap.Remove(col);
            }
        }
    }
}