using UnityEngine;

public class PaintingAudioClue : MonoBehaviour
{
    [Header("Camera")]
    public Camera targetCamera;

    [Header("Audio Clue")]
    public AudioClip clueAudio;
    public AudioSource audioSource;

    [Header("Click Settings")]
    public float maxClickDistance = 100f;
    public float doubleClickTime = 0.3f;
    public bool allowReplay = true;

    private float lastClickTime = 0f;
    private bool hasPlayed = false;

    [Header("Debugging Mode Supported")]
    public bool debuggingMode = false;

    // Destructor for system.
    void sysDestructor()
    {
        PlayerPrefs.SetInt("PaintingClue", 0); // The different player clues.
        PlayerPrefs.SetInt("VentClicked", 0); // The status of the vents.
    }
    void Start() // Apply on start of the function.
    {
        // If the debug mode is on make the function more easy to debug.
        if (debuggingMode == true)
        {
            sysDestructor(); // Call the system destructor for internal data.
        }

        if (targetCamera == null)
            targetCamera = Camera.main;

        if (audioSource == null)
            audioSource = gameObject.AddComponent<AudioSource>();

        audioSource.playOnAwake = false;
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            TryDoubleClick();
        }
    }

    private void TryDoubleClick()
    {
        Ray ray = targetCamera.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out RaycastHit hit, maxClickDistance))
        {
            // Works for imported models (checks children too)
            if (hit.transform == transform || hit.transform.IsChildOf(transform))
            {
                float timeSinceLastClick = Time.time - lastClickTime;

                if (timeSinceLastClick <= doubleClickTime)
                {
                    PlayClueAudio();
                }

                lastClickTime = Time.time;
            }
        }
    }

    private void PlayClueAudio()
    {
        if (!allowReplay && hasPlayed)
            return;

        if (clueAudio == null)
        {
            Debug.LogWarning("No clue audio assigned.");
            return;
        }

        // Play the image right now.
        bool playNow = false;

        // The play now script has not been execute in the past then play now.
        int priorExecution2 = PlayerPrefs.GetInt("PaintingClue", 0);
        int priorExecution1 = PlayerPrefs.GetInt("VentClicked", 0);

        // If the last two execution have happen it is ok to play the script on the click.
        if (priorExecution1 == 1 && priorExecution2 == 0)
        {
            playNow = true;
        }

        if (playNow == true) { // The clue for the play now script.
            audioSource.Stop();
            audioSource.clip = clueAudio;
            audioSource.Play();

            hasPlayed = true;

            Debug.Log("Double-click detected: clue audio played.");
            PlayerPrefs.SetInt("PaintingClue", 1); // Make sure that the internal data is set so it will not play again.
        }
    }
}