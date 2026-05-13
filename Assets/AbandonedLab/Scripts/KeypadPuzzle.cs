using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class KeypadPuzzle : PuzzleInteractable
{
    [Header("Keypad Settings")]
    public string correctCode = "1947";

    private GameObject keypadUI;
    private TextMeshProUGUI displayText;
    private string currentInput = "";
    private bool isOpen = false;

    void Start()
    {
        puzzleNumber = 2;
        BuildKeypadUI();
    }

    private void BuildKeypadUI()
    {
        // Create Canvas
        GameObject canvasObj = new GameObject("KeypadCanvas");
        Canvas canvas = canvasObj.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = 10;
        canvasObj.AddComponent<CanvasScaler>();
        canvasObj.AddComponent<GraphicRaycaster>();

        // Create Panel
        keypadUI = new GameObject("KeypadPanel");
        keypadUI.transform.SetParent(canvasObj.transform, false);
        Image panelImg = keypadUI.AddComponent<Image>();
        panelImg.color = new Color(0.15f, 0.15f, 0.15f, 0.95f);
        RectTransform panelRect = keypadUI.GetComponent<RectTransform>();
        panelRect.sizeDelta = new Vector2(280, 420);
        panelRect.anchoredPosition = Vector2.zero;
        panelRect.anchorMin = new Vector2(0.5f, 0.5f);
        panelRect.anchorMax = new Vector2(0.5f, 0.5f);

        // Create Display
        GameObject displayObj = new GameObject("Display");
        displayObj.transform.SetParent(keypadUI.transform, false);
        Image displayBg = displayObj.AddComponent<Image>();
        displayBg.color = new Color(0.05f, 0.05f, 0.05f, 1f);
        RectTransform displayRect = displayObj.GetComponent<RectTransform>();
        displayRect.sizeDelta = new Vector2(240, 55);
        displayRect.anchoredPosition = new Vector2(0, 160);

        // Display Text
        GameObject textObj = new GameObject("DisplayText");
        textObj.transform.SetParent(displayObj.transform, false);
        displayText = textObj.AddComponent<TextMeshProUGUI>();
        displayText.text = "----";
        displayText.fontSize = 36;
        displayText.alignment = TextAlignmentOptions.Center;
        displayText.color = Color.green;
        RectTransform textRect = textObj.GetComponent<RectTransform>();
        textRect.sizeDelta = new Vector2(240, 55);
        textRect.anchoredPosition = Vector2.zero;

        // Create number buttons
        string[] labels = { "1","2","3","4","5","6","7","8","9","DEL","0","X" };
        Vector2[] positions = {
            new Vector2(-85, 90),  new Vector2(0, 90),   new Vector2(85, 90),
            new Vector2(-85, 20),  new Vector2(0, 20),   new Vector2(85, 20),
            new Vector2(-85, -50), new Vector2(0, -50),  new Vector2(85, -50),
            new Vector2(-85,-120), new Vector2(0,-120),  new Vector2(85,-120)
        };

        for (int i = 0; i < labels.Length; i++)
        {
            string label = labels[i];
            CreateButton(keypadUI.transform, label, positions[i], () => OnButtonPressed(label));
        }

        // Hide at start
        keypadUI.SetActive(false);
    }

    private void CreateButton(Transform parent, string label, Vector2 position, UnityEngine.Events.UnityAction onClick)
    {
        // Button object
        GameObject btnObj = new GameObject("Btn_" + label);
        btnObj.transform.SetParent(parent, false);

        Image img = btnObj.AddComponent<Image>();
        img.color = label == "X" ? new Color(0.6f, 0.1f, 0.1f, 1f) :
                    label == "DEL" ? new Color(0.4f, 0.3f, 0.1f, 1f) :
                    new Color(0.3f, 0.3f, 0.3f, 1f);

        Button btn = btnObj.AddComponent<Button>();
        btn.targetGraphic = img;

        ColorBlock colors = btn.colors;
        colors.highlightedColor = new Color(0.5f, 0.5f, 0.5f, 1f);
        colors.pressedColor = new Color(0.2f, 0.2f, 0.2f, 1f);
        btn.colors = colors;

        RectTransform rect = btnObj.GetComponent<RectTransform>();
        rect.sizeDelta = new Vector2(75, 60);
        rect.anchoredPosition = position;

        // Button label
        GameObject textObj = new GameObject("Text");
        textObj.transform.SetParent(btnObj.transform, false);
        TextMeshProUGUI text = textObj.AddComponent<TextMeshProUGUI>();
        text.text = label;
        text.fontSize = 24;
        text.alignment = TextAlignmentOptions.Center;
        text.color = Color.white;
        RectTransform textRect = textObj.GetComponent<RectTransform>();
        textRect.sizeDelta = new Vector2(75, 60);
        textRect.anchoredPosition = Vector2.zero;

        btn.onClick.AddListener(onClick);
    }

    private void OnButtonPressed(string label)
    {
        if (label == "X") { CloseKeypad(); return; }
        if (label == "DEL") { PressDelete(); return; }
        PressButton(label);
    }

    public override void Interact()
    {
        if (isOpen) return;
        isOpen = true;
        keypadUI.SetActive(true);
        UpdateDisplay();
    }

    public void PressButton(string digit)
    {
        if (currentInput.Length >= 4) return;
        currentInput += digit;
        UpdateDisplay();
        if (currentInput.Length == 4) CheckCode();
    }

    public void PressDelete()
    {
        if (currentInput.Length == 0) return;
        currentInput = currentInput.Substring(0, currentInput.Length - 1);
        UpdateDisplay();
    }

    private void CheckCode()
    {
        if (currentInput == correctCode)
        {
            displayText.text = "UNLOCKED";
            displayText.color = Color.green;
            Invoke("CloseKeypad", 1.5f);
            MarkSolved();
        }
        else
        {
            displayText.text = "WRONG";
            displayText.color = Color.red;
            Invoke("ResetInput", 1f);
        }
    }

    private void ResetInput()
    {
        currentInput = "";
        displayText.color = Color.green;
        UpdateDisplay();
    }

    private void UpdateDisplay()
    {
        if (displayText != null)
            displayText.text = currentInput.Length > 0 ? currentInput : "----";
    }

    public void CloseKeypad()
    {
        isOpen = false;
        currentInput = "";
        if (keypadUI != null) keypadUI.SetActive(false);
    }
}
