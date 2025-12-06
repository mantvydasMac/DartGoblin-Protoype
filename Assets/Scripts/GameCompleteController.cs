using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class GameCompleteController : UIController
{
    public VisualElement ui;

    public VisualElement pausedPanel;

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
        pausedPanel = ui.Q<VisualElement>("game-complete-panel");

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

    private void onMainMenuButtonClicked()
    {
        SceneManager.LoadScene(0);
    }
}
