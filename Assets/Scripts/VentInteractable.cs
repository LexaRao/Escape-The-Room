using UnityEngine;

public class VentInteractable : MonoBehaviour
{
    [Header("Camera Check")]
    public Camera targetCamera;

    [Header("Double Click Settings")]
    public float doubleClickTime = 0.3f;

    [Header("State")]
    public bool ventClicked = false;

    private float lastClickTime = 0f;

    private Renderer objectRenderer;

    void Start()
    {
        if (targetCamera == null)
            targetCamera = Camera.main;

        objectRenderer = GetComponentInChildren<Renderer>();

        // Load saved state
        if (PlayerPrefs.GetInt("VentClicked", 0) == 1)
        {
            ventClicked = true;
        }
    }

    void Update()
    {
        if (ventClicked)
            return;

        if (Input.GetMouseButtonDown(0))
        {
            if (IsMouseOverObject() && IsVisibleToCamera())
            {
                DetectDoubleClick();
            }
        }
    }

    private bool IsMouseOverObject()
    {
        Ray ray = targetCamera.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            return hit.transform == transform || hit.transform.IsChildOf(transform);
        }

        return false;
    }

    private bool IsVisibleToCamera()
    {
        if (objectRenderer == null)
            return false;

        Plane[] planes = GeometryUtility.CalculateFrustumPlanes(targetCamera);
        return GeometryUtility.TestPlanesAABB(planes, objectRenderer.bounds);
    }

    private void DetectDoubleClick()
    {
        float timeSinceLastClick = Time.time - lastClickTime;

        if (timeSinceLastClick <= doubleClickTime)
        {
            ActivateVent();
        }

        lastClickTime = Time.time;
    }

    private void ActivateVent()
    {
        ventClicked = true;

        // Save state
        PlayerPrefs.SetInt("VentClicked", 1);
        PlayerPrefs.Save();

        Debug.Log("Vent clicked! Player can now exit scene.");

        // Optional: trigger next phase here
        // Example:
        // SceneManager.LoadScene("NextScene");
        // or enable movement script
    }
}