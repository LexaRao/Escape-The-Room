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

<<<<<<< HEAD
=======
    private float lastClickTime = 0f;
    private bool hasPlayed = false;

<<<<<<< Updated upstream
<<<<<<< HEAD
<<<<<<< HEAD
=======
>>>>>>> Stashed changes
>>>>>>> Lexa-Room0
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

<<<<<<< HEAD
=======
<<<<<<< Updated upstream
=======
    void Start()
    {
>>>>>>> dbaf82884805f1e5cd70d4d43b7eb51d64eb1aea
=======
    void Start()
    {
>>>>>>> dbaf82884805f1e5cd70d4d43b7eb51d64eb1aea
=======
>>>>>>> Stashed changes
>>>>>>> Lexa-Room0
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

<<<<<<< HEAD
        int ventClicked = PlayerPrefs.GetInt("VentClicked", 0);
        int paintingCluePlayed = PlayerPrefs.GetInt("PaintingClue", 0);
=======
<<<<<<< Updated upstream
<<<<<<< HEAD
<<<<<<< HEAD
=======
>>>>>>> Stashed changes
        // Play the image right now.
        bool playNow = false;
>>>>>>> Lexa-Room0

        if (ventClicked != 1 || paintingCluePlayed == 1)
            return;

<<<<<<< HEAD
=======
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
<<<<<<< Updated upstream
=======
=======
>>>>>>> dbaf82884805f1e5cd70d4d43b7eb51d64eb1aea
>>>>>>> Lexa-Room0
        audioSource.Stop();
        audioSource.clip = clueAudio;
        audioSource.Play();

        hasPlayed = true;

        PlayerPrefs.SetInt("PaintingClue", 1);
        PlayerPrefs.Save();

        Debug.Log("Double-click detected: clue audio played.");
<<<<<<< HEAD
=======
<<<<<<< HEAD
>>>>>>> dbaf82884805f1e5cd70d4d43b7eb51d64eb1aea
=======
>>>>>>> dbaf82884805f1e5cd70d4d43b7eb51d64eb1aea
=======
>>>>>>> Stashed changes
>>>>>>> Lexa-Room0
    }
}