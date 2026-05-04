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

    void Start()
    {
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

        audioSource.Stop();
        audioSource.clip = clueAudio;
        audioSource.Play();

        hasPlayed = true;

        Debug.Log("Double-click detected: clue audio played.");
    }
}