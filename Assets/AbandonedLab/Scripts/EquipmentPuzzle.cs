using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class EquipmentPuzzle : MonoBehaviour
{
    [Header("Equipment Settings")]
    public GameObject[] equipment;        // Drag E1, E2, E3 here in order
    public int[] correctSequence = {0, 2, 1}; // E1 → E3 → E2 (zero-indexed)
    public Material activeMaterial;       // Glowing material when activated
    public Material defaultMaterial;      // Original material
    private bool hasBeenSolved = false;
    private int puzzleNumber = 3;

    private int[] playerSequence;
    private int currentStep = 0;
    private bool[] isActivated;

    // Monitor UI (built automatically)
    private GameObject monitorUI;
    private TextMeshProUGUI monitorText;

    void Start()
    {
        puzzleNumber = 3;
        playerSequence = new int[correctSequence.Length];
        isActivated = new bool[equipment.Length];
        BuildMonitorUI();
    }

    // Called by each EquipmentPiece script
    public void EquipmentClicked(int index)
    {
        if (hasBeenSolved) return;
        if (isActivated[index]) return; // Already activated

        // Activate this piece
        isActivated[index] = true;
        playerSequence[currentStep] = index;
        currentStep++;

        // Change material to show activation
        if (activeMaterial != null)
            equipment[index].GetComponent<Renderer>().material = activeMaterial;

        UpdateMonitor();

        // Check if sequence is complete
        if (currentStep == correctSequence.Length)
            CheckSequence();
    }

    private void CheckSequence()
    {
        for (int i = 0; i < correctSequence.Length; i++)
        {
            if (playerSequence[i] != correctSequence[i])
            {
                // Wrong sequence
                Invoke("ResetSequence", 1f);
                monitorText.text = "WRONG\nSEQUENCE";
                monitorText.color = Color.red;
                return;
            }
        }

        // Correct!
        monitorText.text = "SYSTEMS\nONLINE";
        monitorText.color = Color.green;
        hasBeenSolved = true;
        LabRoomManager.Instance.SolvePuzzle(3);
        Debug.Log("Puzzle 3 solved!");
    }

    private void ResetSequence()
    {
        currentStep = 0;
        isActivated = new bool[equipment.Length];
        playerSequence = new int[correctSequence.Length];

        // Reset materials
        if (defaultMaterial != null)
        {
            foreach (GameObject eq in equipment)
                eq.GetComponent<Renderer>().material = defaultMaterial;
        }

        monitorText.text = "ACTIVATE\nSEQUENCE";
        monitorText.color = Color.white;
    }

    private void UpdateMonitor()
    {
        string display = "";
        for (int i = 0; i < equipment.Length; i++)
            display += isActivated[i] ? "[ON] " : "[--] ";
        monitorText.text = display;
    }

    private void BuildMonitorUI()
    {
        // Screen space monitor display
        GameObject canvasObj = new GameObject("MonitorCanvas");
        Canvas canvas = canvasObj.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvasObj.AddComponent<CanvasScaler>();
        canvasObj.AddComponent<GraphicRaycaster>();

        monitorUI = new GameObject("MonitorPanel");
        monitorUI.transform.SetParent(canvasObj.transform, false);
        Image bg = monitorUI.AddComponent<Image>();
        bg.color = new Color(0.05f, 0.2f, 0.05f, 0.9f);
        RectTransform rect = monitorUI.GetComponent<RectTransform>();
        rect.sizeDelta = new Vector2(200, 100);
        rect.anchorMin = new Vector2(0, 1);
        rect.anchorMax = new Vector2(0, 1);
        rect.anchoredPosition = new Vector2(120, -60);

        GameObject textObj = new GameObject("MonitorText");
        textObj.transform.SetParent(monitorUI.transform, false);
        monitorText = textObj.AddComponent<TextMeshProUGUI>();
        monitorText.text = "ACTIVATE\nSEQUENCE";
        monitorText.fontSize = 18;
        monitorText.alignment = TextAlignmentOptions.Center;
        monitorText.color = Color.white;
        RectTransform textRect = textObj.GetComponent<RectTransform>();
        textRect.sizeDelta = new Vector2(200, 100);
        textRect.anchoredPosition = Vector2.zero;
    }
}
