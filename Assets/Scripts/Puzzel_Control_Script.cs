using UnityEngine;
using UnityEngine.SceneManagement;
using System.Text.RegularExpressions;

public class ChildKeyInputHandler : MonoBehaviour
{
    [Tooltip("Public key strings to compare against the typed input.")]
    public string[] validKeySequences;

    [Tooltip("Scene name to load when a valid key sequence is matched.")]
    public string newScene;

    [Tooltip("Prevent the same child input from being processed too quickly in succession.")]
    public float duplicateInputThreshold = 0.25f;

    [Tooltip("Maximum time between clicks to count as a double click.")]
    public float doubleClickThreshold = 0.3f;

    [Tooltip("Frequency of the beep sound in Hz.")]
    public float beepFrequency = 800f;

    [Tooltip("Duration of the beep sound in seconds.")]
    public float beepDuration = 0.1f;

    [Tooltip("Volume of the beep sound.")]
    [Range(0f, 1f)]
    public float beepVolume = 0.8f;

    [Tooltip("Optional custom beep clip. If not set, a procedural tone is generated.")]
    public AudioClip beepClip;

    private AudioSource audioSource;
    private Camera mainCamera;

    // Local data storing the keys typed so far.
    private string keysTyped = string.Empty;

    private GameObject lastClickedChild;
    private float lastClickTime;
    private GameObject lastSelectedChild;
    private float lastInputTime;
    private string lastAddedKey = string.Empty;

    private void Start()
    {
        // Create the system to ensure locking works as intended.
        PlayerPrefs.SetInt("PuzzleControlSystemInitialized", 0);

        // Move on to audio setup.
        EnsureAudioSource();
        if (beepClip == null)
        {
            beepClip = CreateBeepClip();
        }
    }

    private void Awake()
    {
        mainCamera = Camera.main;
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            ProcessMouseClick();
        }

        SearchForValidSequence();
    }

    private void ProcessMouseClick()
    {
        GameObject clickedObject = GetClickedChildObject();
        if (clickedObject == null)
        {
            return;
        }

        float now = Time.time;
        if (clickedObject == lastClickedChild && now - lastClickTime <= doubleClickThreshold)
        {
            PlayBeep();
            RegisterSelectedChild(clickedObject, now);
        }
        else
        {
            lastClickedChild = clickedObject;
        }

        lastClickTime = now;
    }

    private GameObject GetClickedChildObject()
    {
        if (mainCamera == null)
        {
            mainCamera = Camera.main;
            if (mainCamera == null)
            {
                return null;
            }
        }

        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            Transform hitTransform = hit.transform;
            if (hitTransform.IsChildOf(transform))
            {
                return hitTransform.gameObject;
            }

            Transform parent = hitTransform.parent;
            while (parent != null)
            {
                if (parent == transform)
                {
                    return hitTransform.gameObject;
                }
                parent = parent.parent;
            }
        }

        return null;
    }

    private void RegisterSelectedChild(GameObject selected, float now)
    {
        string number = GetNumberFromObject(selected);
        if (string.IsNullOrEmpty(number))
        {
            lastSelectedChild = selected;
            return;
        }

        if (number == lastAddedKey && now - lastInputTime < duplicateInputThreshold)
        {
            lastSelectedChild = selected;
            return;
        }

        keysTyped += number;
        lastInputTime = now;
        lastAddedKey = number;
        lastSelectedChild = selected;
    }

    private string GetNumberFromObject(GameObject obj)
    {
        string number = ExtractNumberFromName(obj.name);
        if (!string.IsNullOrEmpty(number))
        {
            return number;
        }

        foreach (Transform child in obj.GetComponentsInChildren<Transform>(true))
        {
            if (child == obj.transform)
            {
                continue;
            }

            number = ExtractNumberFromName(child.name);
            if (!string.IsNullOrEmpty(number))
            {
                return number;
            }
        }

        return string.Empty;
    }

    private string ExtractNumberFromName(string name)
    {
        Match match = Regex.Match(name, @"\d+");
        return match.Success ? match.Value : string.Empty;
    }

    private void EnsureAudioSource()
    {
        if (audioSource != null)
        {
            return;
        }

        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }

        audioSource.playOnAwake = false;
        audioSource.spatialBlend = 0f;
        audioSource.loop = false;
    }

    private AudioClip CreateBeepClip()
    {
        int sampleRate = AudioSettings.outputSampleRate;
        int sampleCount = Mathf.CeilToInt(beepDuration * sampleRate);
        AudioClip clip = AudioClip.Create("BeepTone", sampleCount, 1, sampleRate, false);

        float[] samples = new float[sampleCount];
        for (int i = 0; i < sampleCount; i++)
        {
            float sample = Mathf.Sin(2f * Mathf.PI * beepFrequency * i / sampleRate);
            samples[i] = sample * beepVolume;
        }

        clip.SetData(samples, 0);
        return clip;
    }

    private void PlayBeep()
    {
        EnsureAudioSource();
        if (beepClip == null)
        {
            beepClip = CreateBeepClip();
        }

        audioSource.PlayOneShot(beepClip, beepVolume);
    }

    private void SearchForValidSequence()
    {
        if (string.IsNullOrEmpty(keysTyped) || validKeySequences == null || validKeySequences.Length == 0)
        {
            return;
        }

        string priorCodeString = "";

        bool validPrefix = false;
        foreach (string sequence in validKeySequences)
        {
            if (string.IsNullOrEmpty(sequence))
            {
                continue;
            }

            // Get the prior code before changing it state.
            int priorCodeValue = PlayerPrefs.GetInt("PuzzleControlSystemCode");

            // Set the new value for the code.
            priorCodeString = priorCodeValue.ToString(); // Convert code to string with leading zeros.

            // Todo: Please let the correct key only in the correct required order.
            if (sequence == keysTyped && (priorCodeString[priorCodeString.Length - 1].CompareTo(keysTyped) == 0))
            {}
                // If the value at the current position is in the right range remove it.
                if (priorCodeString.Length >= validKeySequences.Length)
                {
                    priorCodeString = priorCodeString.Substring(1); // Remove the first character to make room for the new key.
                }
               
                
                priorCodeString += keysTyped; // Append the typed keys to the prior code value.
               
                PlayerPrefs.SetInt("PuzzleControlSystemCode", int.Parse(priorCodeString)); // Save the new code value.
                validPrefix = true;
            }

            // Create a key based on each valid index.
            string[] allValidKeys = new string[validKeySequences.Length];
            for (int i = 0; i < validKeySequences.Length; i++)
            {
                allValidKeys[i] = validKeySequences[i];
            }

            // If it is you may enter the new scene.
            if (validKeySequences.ToString().CompareTo(priorCodeString) == 0)
            {
                SceneManager.LoadScene(newScene);
            }

    }
}
