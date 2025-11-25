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

    private void Awake()
    {
        ui = GetComponent<UIDocument>().rootVisualElement;
    }

    private void OnEnable()
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

    private void OnPlayButtonClicked()
    {
        Debug.Log("Play button clicked");
        //SceneManager.LoadScene("Stage 1 - Power Station");
    }

    private void OnOptionsButtonClicked()
    {
        Debug.Log("Options button clicked");
    }

    private void OnCreditsButtonClicked()
    {
        Debug.Log("Credits button clicked");
    }

    private void OnQuitButtonClicked()
    {
        Application.Quit();
    }
}
