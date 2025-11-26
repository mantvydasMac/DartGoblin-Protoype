using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class MainMenuController : MonoBehaviour
{
    public VisualElement ui;

    public Button playButton;
    public Button optionsButton;
    public Button creditsButton;
    public Button quitButton;

    public VisualElement stagesModal;
    public Button stage1Button;
    public Button stage2Button;

    public VisualElement creditsModal;

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
    }

    private void InitMainButtons()
    {
        playButton = ui.Q<Button>("play-button");
        playButton.clicked += OnPlayButtonClicked;

        optionsButton = ui.Q<Button>("options-button");
        optionsButton.clicked += OnOptionsButtonClicked;

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
        stage2Button.clicked += OnStage2Clicked;
    }

    private void InitCreditsModal()
    {
        creditsModal = ui.Q<VisualElement>("credits-modal");
        modals.Add(creditsModal);
    }

    private void OnPlayButtonClicked()
    {
        ToggleModal(stagesModal);
    }

    private void OnOptionsButtonClicked()
    {
        CloseAllModals();
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
