using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class ClueInteractable : MonoBehaviour
{
    [Header("Clue Settings")]
    public string clueTitle = "Research Note";
    public string clueText = "Enter your clue text here";

    private GameObject clueUI;
    private bool isOpen = false;
    private Camera mainCam;

    void Start()
    {
        mainCam = Camera.main;
        BuildClueUI();
    }

    void Update()
    {
        if (isOpen) return;

        if (IsMouseOver() && 
            UnityEngine.InputSystem.Mouse.current.leftButton.wasPressedThisFrame)
            ShowClue();
    }

    private bool IsMouseOver()
    {
        Ray ray = mainCam.ScreenPointToRay(
            UnityEngine.InputSystem.Mouse.current.position.ReadValue());

        if (Physics.Raycast(ray, out RaycastHit hit, 100f))
            return hit.collider.gameObject == gameObject;

        return false;
    }

    private void ShowClue()
    {
        isOpen = true;
        clueUI.SetActive(true);
        Camera.main.GetComponent<MainCamera>().enabled = false;
    }

    public void CloseClue()
    {
        isOpen = false;
        clueUI.SetActive(false);
        Camera.main.GetComponent<MainCamera>().enabled = true;
    }

    private void BuildClueUI()
    {
        // Canvas
        GameObject canvasObj = new GameObject("ClueCanvas_" + clueTitle);
        Canvas canvas = canvasObj.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = 10;
        canvasObj.AddComponent<CanvasScaler>();
        canvasObj.AddComponent<GraphicRaycaster>();

        // Panel — looks like a note/paper
        clueUI = new GameObject("CluePanel");
        clueUI.transform.SetParent(canvasObj.transform, false);
        Image panelImg = clueUI.AddComponent<Image>();
        panelImg.color = new Color(0.9f, 0.85f, 0.7f, 0.97f); // yellowish paper
        RectTransform panelRect = clueUI.GetComponent<RectTransform>();
        panelRect.sizeDelta = new Vector2(380, 300);
        panelRect.anchoredPosition = Vector2.zero;
        panelRect.anchorMin = new Vector2(0.5f, 0.5f);
        panelRect.anchorMax = new Vector2(0.5f, 0.5f);

        // Title
        GameObject titleObj = new GameObject("Title");
        titleObj.transform.SetParent(clueUI.transform, false);
        TextMeshProUGUI titleText = titleObj.AddComponent<TextMeshProUGUI>();
        titleText.text = clueTitle;
        titleText.fontSize = 20;
        titleText.fontStyle = FontStyles.Bold;
        titleText.color = new Color(0.2f, 0.1f, 0.0f);
        titleText.alignment = TextAlignmentOptions.Center;
        RectTransform titleRect = titleObj.GetComponent<RectTransform>();
        titleRect.sizeDelta = new Vector2(320, 40);
        titleRect.anchoredPosition = new Vector2(0, 90);

        // Divider line
        GameObject line = new GameObject("Line");
        line.transform.SetParent(clueUI.transform, false);
        Image lineImg = line.AddComponent<Image>();
        lineImg.color = new Color(0.4f, 0.2f, 0.0f, 0.5f);
        RectTransform lineRect = line.GetComponent<RectTransform>();
        lineRect.sizeDelta = new Vector2(300, 2);
        lineRect.anchoredPosition = new Vector2(0, 65);

        // Body text
        GameObject bodyObj = new GameObject("Body");
        bodyObj.transform.SetParent(clueUI.transform, false);
        TextMeshProUGUI bodyText = bodyObj.AddComponent<TextMeshProUGUI>();
        bodyText.text = clueText;
        bodyText.fontSize = 16;
        bodyText.color = new Color(0.15f, 0.1f, 0.05f);
        bodyText.alignment = TextAlignmentOptions.TopLeft;
        bodyText.enableWordWrapping = true;
        bodyText.overflowMode = TextOverflowModes.Overflow;
        RectTransform bodyRect = bodyObj.GetComponent<RectTransform>();
        bodyRect.sizeDelta = new Vector2(300, 150);
        bodyRect.anchoredPosition = new Vector2(0, 0);

        // Close button
        GameObject btnObj = new GameObject("CloseBtn");
        btnObj.transform.SetParent(clueUI.transform, false);
        Image btnImg = btnObj.AddComponent<Image>();
        btnImg.color = new Color(0.5f, 0.2f, 0.05f);
        Button btn = btnObj.AddComponent<Button>();
        btn.targetGraphic = btnImg;
        btn.onClick.AddListener(CloseClue);
        RectTransform btnRect = btnObj.GetComponent<RectTransform>();
        btnRect.sizeDelta = new Vector2(100, 30);
        btnRect.anchoredPosition = new Vector2(0, -90);

        GameObject btnText = new GameObject("Text");
        btnText.transform.SetParent(btnObj.transform, false);
        TextMeshProUGUI txt = btnText.AddComponent<TextMeshProUGUI>();
        txt.text = "Close";
        txt.fontSize = 14;
        txt.alignment = TextAlignmentOptions.Center;
        txt.color = Color.white;
        RectTransform btnTextRect = btnText.GetComponent<RectTransform>();
        btnTextRect.sizeDelta = new Vector2(100, 30);
        btnTextRect.anchoredPosition = Vector2.zero;

        clueUI.SetActive(false);
    }
}
