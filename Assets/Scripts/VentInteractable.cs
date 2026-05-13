using UnityEngine;

public class VentInteractable : MonoBehaviour
{
    [Header("Camera Check")]
    public Camera targetCamera;

    [Header("Double Click Settings")]
    public float doubleClickTime = 0.3f;

    [Header("State")]
    public bool ventClicked = false;
<<<<<<< Updated upstream
<<<<<<< HEAD
<<<<<<< HEAD
=======

>>>>>>> dbaf82884805f1e5cd70d4d43b7eb51d64eb1aea
=======

>>>>>>> dbaf82884805f1e5cd70d4d43b7eb51d64eb1aea
=======
>>>>>>> Stashed changes
    private float lastClickTime = 0f;

    private Renderer objectRenderer;

<<<<<<< Updated upstream
<<<<<<< HEAD
<<<<<<< HEAD
=======
>>>>>>> Stashed changes
    [Header("Audio Settings")]
    public AudioSource ventAudioSource;

    [Header("Debugging Mode Supported")]
    public bool debuggingMode = false;

<<<<<<< Updated upstream
=======
>>>>>>> dbaf82884805f1e5cd70d4d43b7eb51d64eb1aea
=======
>>>>>>> dbaf82884805f1e5cd70d4d43b7eb51d64eb1aea
=======
>>>>>>> Stashed changes
    void Start()
    {
        if (targetCamera == null)
            targetCamera = Camera.main;

        objectRenderer = GetComponentInChildren<Renderer>();

        // Load saved state
<<<<<<< Updated upstream
<<<<<<< HEAD
<<<<<<< HEAD
=======
>>>>>>> Stashed changes
        if (debuggingMode == true) // If debugging mode is turned on load the game at start state.
        {
            PlayerPrefs.SetInt("VentClicked", 0);
            ventClicked = false;
        } else { // Otherwise, restore the game history.
            if (PlayerPrefs.GetInt("VentClicked", 0) == 1)
            {
                ventClicked = true;
            }
<<<<<<< Updated upstream
=======
        if (PlayerPrefs.GetInt("VentClicked", 0) == 1)
        {
            ventClicked = true;
>>>>>>> dbaf82884805f1e5cd70d4d43b7eb51d64eb1aea
=======
        if (PlayerPrefs.GetInt("VentClicked", 0) == 1)
        {
            ventClicked = true;
>>>>>>> dbaf82884805f1e5cd70d4d43b7eb51d64eb1aea
=======
>>>>>>> Stashed changes
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
<<<<<<< Updated upstream
<<<<<<< HEAD
<<<<<<< HEAD
=======
>>>>>>> Stashed changes

        // Todo: Play back scene of a cracking of a vent and then the noise of a character talking back.  
        AudioSource ventPlayer = ventAudioSource.GetComponent<AudioSource>();
        if (ventPlayer != null)
        {
            ventPlayer.Play(); // Play back the vent shadder noise on click.
        }
<<<<<<< Updated upstream
=======
>>>>>>> dbaf82884805f1e5cd70d4d43b7eb51d64eb1aea
=======
>>>>>>> dbaf82884805f1e5cd70d4d43b7eb51d64eb1aea
=======
>>>>>>> Stashed changes
    }
}