using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class PauseController : MonoBehaviour
{
    public VisualElement ui;

    public VisualElement pausedPanel;

    public Button resumeButton;
    public Button mainMenuButton;
    public Button settingsButton;
    public Button quitButton;

    public VisualElement settingsModal;
    public Slider masterVolSlider;
    public Slider sfxVolSlider;
    public Slider musicVolSlider;
    public Slider fpsSlider;

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

        settingsButton = ui.Q<Button>("settings-button");
        settingsButton.clicked += OnSettingsButtonClicked;

        quitButton = ui.Q<Button>("quit-button");
        quitButton.clicked += OnQuitButtonClicked;

        InitSettingsModal();
    }

    void Update()
    {
        if (Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            bool paused = Time.timeScale == 0;
            Time.timeScale = paused ? 1 : 0;

            if (paused)
            {
                if (settingsModal.ClassListContains("open"))
                {
                    settingsModal.RemoveFromClassList("open");
                }
                pausedPanel.RemoveFromClassList("open");
            }
            else
            {
                pausedPanel.AddToClassList("open");
            }
        }
    }

    private void InitSettingsModal()
    {
        settingsModal = ui.Q<VisualElement>("settings-modal");

        //volume
        masterVolSlider = ui.Q<Slider>("master-slider");
        masterVolSlider.value = PlayerPrefs.GetFloat("masterVolume", 100);

        masterVolSlider.RegisterValueChangedCallback(evt =>
        {
            float newValue = evt.newValue;

            AudioManager.Instance.SetMasterVolume(newValue/100);

            PlayerPrefs.SetFloat("masterVolume", newValue);
            PlayerPrefs.Save();
        });

        sfxVolSlider = ui.Q<Slider>("sfx-slider");
        sfxVolSlider.value = PlayerPrefs.GetFloat("sfxVolume", 100);

        sfxVolSlider.RegisterValueChangedCallback(evt =>
        {
            float newValue = evt.newValue;

            AudioManager.Instance.SetSFXVolume(newValue/100);

            PlayerPrefs.SetFloat("sfxVolume", newValue);
            PlayerPrefs.Save();
        });

        musicVolSlider = ui.Q<Slider>("music-slider");
        musicVolSlider.value = PlayerPrefs.GetFloat("musicVolume", 100);

        musicVolSlider.RegisterValueChangedCallback(evt =>
        {
            float newValue = evt.newValue;

            AudioManager.Instance.SetMusicVolume(newValue/100);

            PlayerPrefs.SetFloat("musicVolume", newValue);
            PlayerPrefs.Save();
        });



        //fps
        fpsSlider = ui.Q<Slider>("fps-slider");
        fpsSlider.value = PlayerPrefs.GetInt("fps", 60);
        Application.targetFrameRate = (int)fpsSlider.value;

        PlayerPrefs.SetInt("fps", (int)fpsSlider.value);
        PlayerPrefs.Save();

        fpsSlider.RegisterValueChangedCallback(evt =>
        {
            float newValue = evt.newValue;
            Application.targetFrameRate = (int)newValue;

            PlayerPrefs.SetInt("fps", (int)newValue);
            PlayerPrefs.Save();
        });
    }

    private void OnResumeButtonClicked()
    {
        Time.timeScale = 1;
        if (settingsModal.ClassListContains("open"))
        {
            settingsModal.RemoveFromClassList("open");
        }
        pausedPanel.RemoveFromClassList("open");
    }

    private void onMainMenuButtonClicked()
    {
        SceneManager.LoadScene(0);
    }

    private void OnSettingsButtonClicked()
    {
        ToggleModal(settingsModal);
    }

    private void OnQuitButtonClicked()
    {
        Application.Quit();
    }

    private void ToggleModal(VisualElement modal)
    {
        if (modal.ClassListContains("open"))
        {
            modal.RemoveFromClassList("open");
            return;
        }

        modal.AddToClassList("open");
    }
}
