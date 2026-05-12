using UnityEngine;
using UnityEngine.UIElements;

public class MainMenuEvents : MonoBehaviour
{
    private UIDocument _document;
    private Button _startButton;
    private Button _quitButton;

    private void Awake()
    {
        _document = GetComponent<UIDocument>();

        if (_document == null)
        {
            Debug.LogError("UIDocument not found.");
            return;
        }

        VisualElement root = _document.rootVisualElement;
        _startButton = root.Q<Button>("StartGameButton");
        _quitButton = root.Q<Button>("DesktopButton");

        if (_startButton == null)
        {
            Debug.LogError("Could not find Button named StartGameButton.");
            return;
        }
        if (_startButton != null)
        {
            _startButton.RegisterCallback<ClickEvent>(OnPlayGameClick);
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

        
    }

    private void OnDisable()
    {
        if (_startButton != null)
        {
            _startButton.UnregisterCallback<ClickEvent>(OnPlayGameClick);
        }
        if (_quitButton != null)
        {
            _quitButton.UnregisterCallback<ClickEvent>(OnQuitClick);
        }
    }

    private void OnPlayGameClick(ClickEvent evt)
    {
        Debug.Log("Start button pressed.");
        _document.enabled = false;
    }

    private void OnQuitClick(ClickEvent evt)
    {
        Debug.Log("Quit button pressed.");
        Application.Quit();
    #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
    #endif
    }
}