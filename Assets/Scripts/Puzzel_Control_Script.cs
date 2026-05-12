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

<<<<<<< HEAD
    [Tooltip("Debugging Mode Supported")]
    public bool debuggingMode = false;

=======
>>>>>>> dbaf82884805f1e5cd70d4d43b7eb51d64eb1aea
    // Local data storing the keys typed so far.
    private string keysTyped = string.Empty;

    private GameObject lastClickedChild;
    private float lastClickTime;
    private GameObject lastSelectedChild;
    private float lastInputTime;
    private string lastAddedKey = string.Empty;

<<<<<<< HEAD
    // Created By: Lexa hope.
    // Note: Destructor used for delete elements of the scene after the scene has expired. 
    private void sysDestructor()
    {
        // Todo: Deconstructor that delete the data before visiting the next scene.
        PlayerPrefs.SetString("PuzzleControlSystemCode", string.Empty);
        PlayerPrefs.SetInt("VentClicked", 0);
        PlayerPrefs.SetInt("FourBlockPuzzle", 0);
        PlayerPrefs.SetInt("PaintingClue", 0);
    } 

    private void Start()
    {
        // If the debugging mode is set up call the debugger on program start.
        if (debuggingMode == true) {
            // On program start call the system destructor.
            sysDestructor();
        }

        // Create the system to ensure locking works as intended.
        PlayerPrefs.SetString("PuzzleControlSystemCode", string.Empty);
        PlayerPrefs.SetInt("PuzzleControlSystemInitialized", 1);
=======
    private void Start()
    {
        // Create the system to ensure locking works as intended.
        PlayerPrefs.SetInt("PuzzleControlSystemInitialized", 0);
>>>>>>> dbaf82884805f1e5cd70d4d43b7eb51d64eb1aea

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
<<<<<<< HEAD
            SearchForValidSequence();
        }
=======
        }

        SearchForValidSequence();
>>>>>>> dbaf82884805f1e5cd70d4d43b7eb51d64eb1aea
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

<<<<<<< HEAD
        bool hasValidPrefix = false;

=======
        string priorCodeString = "";

        bool validPrefix = false;
>>>>>>> dbaf82884805f1e5cd70d4d43b7eb51d64eb1aea
        foreach (string sequence in validKeySequences)
        {
            if (string.IsNullOrEmpty(sequence))
            {
                continue;
            }

<<<<<<< HEAD
            // Exact match → update stored code and load scene.
            if (sequence == keysTyped)
            {
                UpdateStoredCode(keysTyped);
                keysTyped = string.Empty;

                // If the key is in the correct range of older keys load the scene.
                string localPriorCodeString = PlayerPrefs.GetString("PuzzleControlSystemCode", string.Empty);
                string[] localPriorCodeSequence = localPriorCodeString.Split(',');

                // Print to console for debugging the prior code and valid sequences.
                Debug.Log($"Search Results - Prior code: {localPriorCodeString}, Valid sequences: {string.Join("", validKeySequences)}");

                // Todo: Accomplish all required updates before loading the scene to ensure the new scene can read the updated code.
                int ventOpened = PlayerPrefs.GetInt("VentClicked", 0);
                int paintingSolved = PlayerPrefs.GetInt("PaintingClue", 0);
                int puzzleSolved = PlayerPrefs.GetInt("FourBlockPuzzle", 0);

                // If the vent is opened, the painting clue, and, four block puzzle have been solved it is ok to load the next scene.
                bool priorLevelCompleted = false;
                if (ventOpened == 1 && paintingSolved == 1 && puzzleSolved == 1)
                {
                    priorLevelCompleted = true;
                }

                // Load the correct scene as a direct result.
                if (localPriorCodeString.Equals(string.Join("", validKeySequences)) && priorLevelCompleted)
                {
                    // Call destroyer before next scene.
                    sysDestructor();

                    // Go to next scene.
                    SceneManager.LoadScene(newScene);
                }
            }

            // Still a valid prefix of some sequence → keep accepting input.
            if (sequence.StartsWith(keysTyped))
            {
                hasValidPrefix = true;
            }
        }

        // If no valid sequence starts with what we have, reset the buffer.
        if (!hasValidPrefix)
        {
            keysTyped = string.Empty;
        }
    }

    private void UpdateStoredCode(string newKeys)
    {
        string priorCodeString = PlayerPrefs.GetString("PuzzleControlSystemCode", string.Empty);

        // Append new keys to the rolling code.
        priorCodeString += newKeys;

        // Print to console for debugging the key and prior code.
        Debug.Log($"New keys: {newKeys}, Prior code: {priorCodeString}, Valid sequences: {string.Join("", validKeySequences)}");

        // Limit stored length to the longest valid sequence.
        int maxLen = newKeys.Length;

        // Get the valid key in the system.
        if (maxLen > 0 && priorCodeString.Length > maxLen && priorCodeString.Length > 4)
        {
            priorCodeString = priorCodeString.Substring(maxLen);
        }

        // Save the current code string back to PlayerPrefs for the next scene to read.
        PlayerPrefs.SetString("PuzzleControlSystemCode", priorCodeString);
=======
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

>>>>>>> dbaf82884805f1e5cd70d4d43b7eb51d64eb1aea
    }
}
