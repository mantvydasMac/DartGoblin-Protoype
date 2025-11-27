using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.SocialPlatforms.Impl;
using UnityEngine.UIElements;

public class StageCompleteController : MonoBehaviour
{
    public VisualElement ui;

    public VisualElement pausedPanel;

    public Button nextSceneButton;
    public Button mainMenuButton;

    public GameObject activatedTrigger;
    private ITrigger trigger;

    public int nextScene;

    private void Awake()
    {
        ui = GetComponent<UIDocument>().rootVisualElement;
        Time.timeScale = 1;
    }

    private void OnEnable()
    {
        pausedPanel = ui.Q<VisualElement>("stage-complete-panel");

        nextSceneButton = ui.Q<Button>("next-scene-button");
        nextSceneButton.clicked += OnNextSceneButtonClicked;

        mainMenuButton = ui.Q<Button>("main-menu-button");
        mainMenuButton.clicked += onMainMenuButtonClicked;
    }

    private void Start()
    {
        trigger = activatedTrigger.GetComponent<ITrigger>();
    }

    void Update()
    {
        if (trigger.activated)
        {
            Time.timeScale = 0;
            pausedPanel.AddToClassList("open");
            if (PlayerPrefs.GetInt("completedLevels", 0) < nextScene)
            {
                PlayerPrefs.SetInt("completedLevels", nextScene - 1);
                PlayerPrefs.Save();
            }
        }
    }

    private void OnNextSceneButtonClicked()
    {
        Time.timeScale = 1;
        SceneManager.LoadScene(nextScene);
    }

    private void onMainMenuButtonClicked()
    {
        SceneManager.LoadScene(0);
    }
}
