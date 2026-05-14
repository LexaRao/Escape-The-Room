using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class PauseMenuEvents : MonoBehaviour
{
    private UIDocument _document;
    private Button _resumeButton;
    private Button _quitButton;
    private Button _mainMenuButton;

    [SerializeField] private UIDocument _inventoryUI;

    private void Awake()
    {
        _document = GetComponent<UIDocument>();

        if (_document == null)
        {
            Debug.LogError("UIDocument not found.");
            return;
        }

        VisualElement root = _document.rootVisualElement;

        _resumeButton = root.Q<Button>("ResumeGameButton");
        _quitButton = root.Q<Button>("DesktopButton");
        _mainMenuButton = root.Q<Button>("MainMenuButton");

        if (_resumeButton == null)
        {
            Debug.LogError("Could not find Button named ResumeGameButton.");
            return;
        }
        if (_resumeButton != null)
        {
            _resumeButton.RegisterCallback<ClickEvent>(OnResumeClick);
        }

        if (_quitButton == null)
        {
            Debug.LogError("Could not find Button named DesktopButton.");
            return;
        }
        if (_quitButton != null)
        {
            _quitButton.RegisterCallback<ClickEvent>(OnQuitClick);
        }

        if (_mainMenuButton == null)
        {
            Debug.LogError("Could not find Button named MainMenuButton.");
            return;
        }
        if (_mainMenuButton != null)
        {
            _mainMenuButton.RegisterCallback<ClickEvent>(OnMainMenuClick);
        }

        root.style.display = DisplayStyle.None; // start hidden
    }

    private void Update()
    {
        if (Keyboard.current != null && Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            TogglePauseMenu();
        }
    }

    private void OnDisable()
    {
        if (_resumeButton != null)
        {
            _resumeButton.UnregisterCallback<ClickEvent>(OnResumeClick);
        }
        if (_quitButton != null)
        {
            _quitButton.UnregisterCallback<ClickEvent>(OnQuitClick);
        }
        if (_mainMenuButton != null)
        {
            _mainMenuButton.UnregisterCallback<ClickEvent>(OnMainMenuClick);
        }
    }

    private void OnResumeClick(ClickEvent evt)
    {
        Debug.Log("Resume button pressed.");
        TogglePauseMenu();
    }

    private void OnQuitClick(ClickEvent evt)
    {
        Debug.Log("Quit button pressed.");
        Application.Quit();

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }

    private void OnMainMenuClick(ClickEvent evt)
    {
        Debug.Log("Main menu button pressed.");

        Time.timeScale = 1f;
        SceneManager.LoadScene("PlacedInventoryUI");
    }

    private void TogglePauseMenu()
    {
        VisualElement root = _document.rootVisualElement;

        bool isActive = root.style.display == DisplayStyle.None;

        root.style.display = isActive ? DisplayStyle.Flex : DisplayStyle.None;
        Time.timeScale = isActive ? 0f : 1f;

        if (_inventoryUI != null)
        {
            _inventoryUI.rootVisualElement.style.display = isActive
                ? DisplayStyle.None
                : DisplayStyle.Flex;
        }
    }
}