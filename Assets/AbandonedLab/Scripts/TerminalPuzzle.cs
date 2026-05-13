using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class TerminalPuzzle : PuzzleInteractable
{
    [Header("Terminal Settings")]
    public GameObject finalDoor;

    private GameObject terminalUI;
    private TextMeshProUGUI terminalText;
    private bool isOpen = false;

    void Start()
    {
        puzzleNumber = 4;
        BuildTerminalUI();
    }

    public override void Interact()
    {
        if (isOpen) return;
        if (hasBeenSolved) return;

        // Check if all previous puzzles are solved
        bool p1 = LabRoomManager.Instance.IsSolved(1);
        bool p2 = LabRoomManager.Instance.IsSolved(2);
        bool p3 = LabRoomManager.Instance.IsSolved(3);

        Debug.Log($"P1:{p1} P2:{p2} P3:{p3}");

        isOpen = true;
        terminalUI.SetActive(true);
        Camera.main.GetComponent<MainCamera>().enabled = false;

        if (p1 && p2 && p3)
        {
            // All puzzles solved!
            terminalText.text = "> SYSTEM ONLINE\n> ALL CHECKS PASSED\n> UNLOCKING EXIT...";
            terminalText.color = Color.green;
            Invoke("OpenFinalDoor", 2f);
            MarkSolved();
        }
        else
        {
            // Show what's missing
            string status = "> SYSTEM CHECK:\n";
            status += p1 ? "> [OK] Lab Access\n" : "> [!!] Lab Access incomplete\n";
            status += p2 ? "> [OK] Experiment Code\n" : "> [!!] Experiment Code incomplete\n";
            status += p3 ? "> [OK] Equipment Setup\n" : "> [!!] Equipment Setup incomplete\n";
            status += "\n> RESOLVE ERRORS FIRST";
            terminalText.text = status;
            terminalText.color = Color.red;
            Invoke("CloseTerminal", 3f);
        }
    }

    private void OpenFinalDoor()
    {
        CloseTerminal();

        if (finalDoor != null)
        {
            // Simple version: door disappears
            finalDoor.SetActive(false);
            Debug.Log("ESCAPED! Final door open!");
        }
        else
        {
            Debug.LogWarning("Final door not assigned in TerminalPuzzle!");
        }
    }

    private void CloseTerminal()
    {
        isOpen = false;
        if (terminalUI != null)
            terminalUI.SetActive(false);
        Camera.main.GetComponent<MainCamera>().enabled = true;
    }

    private void BuildTerminalUI()
    {
        // Canvas
        GameObject canvasObj = new GameObject("TerminalCanvas");
        Canvas canvas = canvasObj.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = 10;
        canvasObj.AddComponent<CanvasScaler>();
        canvasObj.AddComponent<GraphicRaycaster>();

        // Panel
        terminalUI = new GameObject("TerminalPanel");
        terminalUI.transform.SetParent(canvasObj.transform, false);
        Image panelImg = terminalUI.AddComponent<Image>();
        panelImg.color = new Color(0.05f, 0.05f, 0.05f, 0.95f);
        RectTransform panelRect = terminalUI.GetComponent<RectTransform>();
        panelRect.sizeDelta = new Vector2(400, 300);
        panelRect.anchoredPosition = Vector2.zero;
        panelRect.anchorMin = new Vector2(0.5f, 0.5f);
        panelRect.anchorMax = new Vector2(0.5f, 0.5f);

        // Terminal Text
        GameObject textObj = new GameObject("TerminalText");
        textObj.transform.SetParent(terminalUI.transform, false);
        terminalText = textObj.AddComponent<TextMeshProUGUI>();
        terminalText.text = "";
        terminalText.fontSize = 16;
        terminalText.alignment = TextAlignmentOptions.TopLeft;
        terminalText.color = Color.green;
        RectTransform textRect = textObj.GetComponent<RectTransform>();
        textRect.sizeDelta = new Vector2(370, 270);
        textRect.anchoredPosition = new Vector2(0, 0);

        // Close button
        GameObject btnObj = new GameObject("CloseBtn");
        btnObj.transform.SetParent(terminalUI.transform, false);
        Image btnImg = btnObj.AddComponent<Image>();
        btnImg.color = new Color(0.4f, 0.1f, 0.1f, 1f);
        Button btn = btnObj.AddComponent<Button>();
        btn.targetGraphic = btnImg;
        btn.onClick.AddListener(CloseTerminal);
        RectTransform btnRect = btnObj.GetComponent<RectTransform>();
        btnRect.sizeDelta = new Vector2(80, 30);
        btnRect.anchoredPosition = new Vector2(150, -125);

        GameObject btnText = new GameObject("Text");
        btnText.transform.SetParent(btnObj.transform, false);
        TextMeshProUGUI txt = btnText.AddComponent<TextMeshProUGUI>();
        txt.text = "CLOSE";
        txt.fontSize = 14;
        txt.alignment = TextAlignmentOptions.Center;
        txt.color = Color.white;
        RectTransform btnTextRect = btnText.GetComponent<RectTransform>();
        btnTextRect.sizeDelta = new Vector2(80, 30);
        btnTextRect.anchoredPosition = Vector2.zero;

        terminalUI.SetActive(false);
    }
}
