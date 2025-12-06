using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class PauseController : UIController
{
    public VisualElement ui;

    public VisualElement pausedPanel;

    public Button resumeButton;
    public Button mainMenuButton;
    public Button quitButton;

    private void Awake()
    {
        ui = GetComponent<UIDocument>().rootVisualElement;
        Time.timeScale = 1;
    }

    private void OnEnable()
    {
        pausedPanel = ui.Q<VisualElement>("paused-panel");

        resumeButton = ui.Q<Button>("resume-button");
        resumeButton.clicked += OnResumeButtonClicked;

        mainMenuButton = ui.Q<Button>("main-menu-button");
        mainMenuButton.clicked += onMainMenuButtonClicked;
    }

    void Update()
    {
        if (Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            bool paused = Time.timeScale == 0;
            Time.timeScale = paused ? 1 : 0;

            if (paused)
                pausedPanel.RemoveFromClassList("open");
            else
                pausedPanel.AddToClassList("open");
        }
    }

    private void OnResumeButtonClicked()
    {
        Time.timeScale = 1;
        pausedPanel.RemoveFromClassList("open");
    }

    private void onMainMenuButtonClicked()
    {
        SceneManager.LoadScene(0);
    }
}
