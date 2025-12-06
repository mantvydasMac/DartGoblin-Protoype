using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class MainMenuController : UIController
{
    public VisualElement ui;

    public Button playButton;
    public Button statsButton;
    public Button creditsButton;
    public Button quitButton;

    public VisualElement stagesModal;
    public Button stage1Button;
    public Button stage2Button;

    public VisualElement creditsModal;
    public VisualElement statsModal;

    public List<VisualElement> modals = new List<VisualElement>();

    private void Awake()
    {
        ui = GetComponent<UIDocument>().rootVisualElement;
    }

    private void OnEnable()
    {
        InitMainButtons();
        InitStagesModal();
        InitCreditsModal();
        InitStatsModal();
    }

    private void InitMainButtons()
    {
        playButton = ui.Q<Button>("play-button");
        playButton.clicked += OnPlayButtonClicked;

        statsButton = ui.Q<Button>("stats-button");
        statsButton.clicked += OnStatsButtonClicked;

        creditsButton = ui.Q<Button>("credits-button");
        creditsButton.clicked += OnCreditsButtonClicked;

        quitButton = ui.Q<Button>("quit-button");
        quitButton.clicked += OnQuitButtonClicked;
    }

    private void InitStagesModal()
    {
        stagesModal = ui.Q<VisualElement>("stages-modal");
        modals.Add(stagesModal);

        stage1Button = ui.Q<Button>("stage1-button");
        stage1Button.clicked += OnStage1Clicked;

        stage2Button = ui.Q<Button>("stage2-button");
        stage2Button.SetEnabled(PlayerPrefs.GetInt("completedLevels", 0) >= 1);
        stage2Button.clicked += OnStage2Clicked;
    }

    private void InitCreditsModal()
    {
        creditsModal = ui.Q<VisualElement>("credits-modal");
        modals.Add(creditsModal);
    }

    private void InitStatsModal()
    {
        statsModal = ui.Q<VisualElement>("stats-modal");
        modals.Add(statsModal);

        statsModal.Q<Label>("stage-one-deaths").text = PlayerPrefs.GetInt("stage1Deaths", 0).ToString();
        statsModal.Q<Label>("stage-two-deaths").text = PlayerPrefs.GetInt("stage2Deaths", 0).ToString();
        statsModal.Q<Label>("total-deaths").text = PlayerPrefs.GetInt("totalDeaths", 0).ToString();
        statsModal.Q<Label>("completed-stages").text = PlayerPrefs.GetInt("completedLevels", 0).ToString();
    }

    private void OnPlayButtonClicked()
    {
        ToggleModal(stagesModal);
    }

    private void OnStatsButtonClicked()
    {
        ToggleModal(statsModal);
    }

    private void OnCreditsButtonClicked()
    {
        ToggleModal(creditsModal);
    }

    private void OnQuitButtonClicked()
    {
        Application.Quit();
    }

    private void OnStage1Clicked()
    {
        SceneManager.LoadScene(1);
    }

    private void OnStage2Clicked()
    {
        SceneManager.LoadScene(2);
    }

    private void CloseAllModals()
    {
        foreach (var modal in modals)
        {
            modal.RemoveFromClassList("open");
        }
    }

    private void ToggleModal(VisualElement modal)
    {
        if (modal.ClassListContains("open"))
        {
            CloseAllModals();
            return;
        }

        CloseAllModals();
        modal.AddToClassList("open");
    }
}
