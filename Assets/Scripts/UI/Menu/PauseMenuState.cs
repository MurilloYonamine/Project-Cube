using UnityEngine;
using UnityEngine.UI;
using System.Collections;

/// <summary>
/// Estado principal do menu de pause.
/// Gerencia os botões do pause menu.
/// </summary>
public class PauseMenuState : PauseState
{
    [Header("Buttons")]
    [SerializeField] private Button resumeButton;
    [SerializeField] private Button settingsButton;
    [SerializeField] private Button phaseSelectorButton;
    [SerializeField] private Button quitButton;

    public override StateType GetStateType() => StateType.PauseMenu;

    private void Start()
    {
        SetupButtons();
    }

    private void SetupButtons()
    {
        if (resumeButton != null)
            resumeButton.onClick.AddListener(OnResumeClicked);

        if (settingsButton != null)
            settingsButton.onClick.AddListener(OnSettingsClicked);

        if (phaseSelectorButton != null)
            phaseSelectorButton.onClick.AddListener(OnPhaseSelectorClicked);

        if (quitButton != null)
            quitButton.onClick.AddListener(OnQuitClicked);
    }

    protected override void OnEnterComplete()
    {
        Debug.Log("[PauseMenuState] Entered");
    }

    protected override void OnExitComplete()
    {
        Debug.Log("[PauseMenuState] Exited");
    }

    private void OnResumeClicked()
    {
        PlayClickSound();
        Debug.Log("Resume clicked");
        pauseManager.Resume();
    }

    private void OnSettingsClicked()
    {
        PlayClickSound();
        Debug.Log("Settings clicked from Pause");
        pauseManager.ChangeState(StateType.Settings);
    }

    private void OnPhaseSelectorClicked()
    {
        PlayClickSound();
        Debug.Log("Phase Selector clicked from Pause");
        pauseManager.ChangeState(StateType.PhaseSelector);
    }

    private void OnQuitClicked()
    {
        PlayClickSound();
        Debug.Log("Quit to Main Menu clicked");
        pauseManager.ResetTimeScale();
        // TODO: Carregar a cena do menu principal
        // SceneManager.LoadScene("MainMenu");
    }

    private void PlayClickSound()
    {
        if (AudioManager.Instance != null)
            AudioManager.Instance.PlaySound("Sounds/menu_escolha", null, 1f);
    }

    private void OnDestroy()
    {
        // Remove listeners para evitar memory leaks
        if (resumeButton != null)
            resumeButton.onClick.RemoveListener(OnResumeClicked);

        if (settingsButton != null)
            settingsButton.onClick.RemoveListener(OnSettingsClicked);

        if (phaseSelectorButton != null)
            phaseSelectorButton.onClick.RemoveListener(OnPhaseSelectorClicked);

        if (quitButton != null)
            quitButton.onClick.RemoveListener(OnQuitClicked);
    }
}
