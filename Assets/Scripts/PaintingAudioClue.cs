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

    [Header("Debugging Mode Supported")]
    public bool debuggingMode = false;

    private float lastClickTime = 0f;
    private bool hasPlayed = false;

    void Start()
    {
        if (debuggingMode)
        {
            ResetPuzzleState();
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

    private void ResetPuzzleState()
    {
        PlayerPrefs.SetInt("PaintingClue", 0);
        PlayerPrefs.SetInt("VentClicked", 0);
        PlayerPrefs.Save();
    }

    private void TryDoubleClick()
    {
        if (targetCamera == null)
            targetCamera = Camera.main;

        if (targetCamera == null)
            return;

        Ray ray = targetCamera.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out RaycastHit hit, maxClickDistance))
        {
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

        int ventClicked = PlayerPrefs.GetInt("VentClicked", 0);
        int paintingCluePlayed = PlayerPrefs.GetInt("PaintingClue", 0);

        // Gate: vent must be clicked first, and clue not already played
        if (ventClicked != 1 || paintingCluePlayed == 1)
            return;

        audioSource.Stop();
        audioSource.clip = clueAudio;
        audioSource.Play();

        hasPlayed = true;

        PlayerPrefs.SetInt("PaintingClue", 1);
        PlayerPrefs.Save();

        Debug.Log("Double-click detected: clue audio played.");
    }
}