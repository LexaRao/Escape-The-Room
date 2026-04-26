using System;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Renderer))]
[RequireComponent(typeof(Collider))]

public class isInteractable : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    [Header("Glow Settings")]
    [ColorUsage(true, true)]
    public Color glowColor = Color.cyan;
    public float glowIntensity = 2f;
    public float lerpSpeed = 8f;

    [Header("Raycast Settings")]
    public Camera targetCamera;
    public float rayDistance = 1000f;

    private Renderer rend;
    private Material mat;

    private Color originalEmission;
    private Color targetEmission;
    private bool isHovered;

    private static isInteractable currentlyHovered;

    void Awake()
    {
        rend = GetComponent<Renderer>();
        mat = rend.material;

        if (targetCamera == null)
            targetCamera = Camera.main;

        if (mat.HasProperty("_EmissionColor"))
        {
            mat.EnableKeyword("_EMISSION");
            originalEmission = mat.GetColor("_EmissionColor");
            targetEmission = originalEmission;
        }
        else
        {
            Debug.LogWarning($"{gameObject.name} material does not support _EmissionColor.");
        }
    }

    // Update is called once per frame
    void Update()
    {
        UpdateHoverState();
        UpdateGlow();
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
            hitHover = hit.collider.GetComponent<isInteractable>();
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
        isHovered = hovered;
        targetEmission = isHovered ? glowColor * glowIntensity : originalEmission;
    }

    private void UpdateGlow()
    {
        if (!mat.HasProperty("_EmissionColor"))
            return;

        Color currentEmission = mat.GetColor("_EmissionColor");
        Color newEmission = Color.Lerp(currentEmission, targetEmission, Time.deltaTime * lerpSpeed);
        mat.SetColor("_EmissionColor", newEmission);
    }

    void OnDisable()
    {
        if (currentlyHovered != null)
            currentlyHovered = null;

        if (mat != null && mat.HasProperty("_EmissionColor"))
        {
            mat.SetColor("_EmissionColor", originalEmission);
        }
    }

    private class HoverGlowInputSystem
    {
        bool isHovered;
        internal void SetHovered(bool v)
        {
            isHovered = v;
        }

        internal bool GetHovered()
        {
            return isHovered;
        }
    }
}

