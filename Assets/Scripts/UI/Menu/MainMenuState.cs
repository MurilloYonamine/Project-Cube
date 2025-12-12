using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using PROJECT_CUBE.LEVEL;

/// <summary>
/// Gerencia o estado do Menu Principal.
/// Controla botões de Play, Configurações, Seletor de Fases e Sair.
/// </summary>
public partial class MainMenuState : MenuState
{
    [SerializeField] private Button playButton;
    [SerializeField] private Button settingsButton;
    [SerializeField] private Button phaseSelectorButton;
    [SerializeField] private Button controlsButton;
    [SerializeField] private Button creditsButton;
    [SerializeField] private Button exitButton;

    public override StateType GetStateType() => StateType.MainMenu;

    protected override void OnEnterComplete()
    {
        // Configura os botões
        if (playButton != null)
            playButton.onClick.AddListener(OnPlayClicked);

        if (settingsButton != null)
            settingsButton.onClick.AddListener(OnSettingsClicked);

        if (phaseSelectorButton != null)
            phaseSelectorButton.onClick.AddListener(OnPhaseSelectorClicked);

        if (controlsButton != null)
            controlsButton.onClick.AddListener(OnControlsClicked);

        if (creditsButton != null)
            creditsButton.onClick.AddListener(OnCreditsClicked);

        if (exitButton != null)
            exitButton.onClick.AddListener(OnExitClicked);
    }

    protected override void OnExitComplete()
    {
        // Remove os listeners para evitar múltiplas chamadas
        if (playButton != null)
            playButton.onClick.RemoveListener(OnPlayClicked);

        if (controlsButton != null)
            controlsButton.onClick.RemoveListener(OnControlsClicked);

        if (creditsButton != null)
            creditsButton.onClick.RemoveListener(OnCreditsClicked);

        if (settingsButton != null)
            settingsButton.onClick.RemoveListener(OnSettingsClicked);

        if (phaseSelectorButton != null)
            phaseSelectorButton.onClick.RemoveListener(OnPhaseSelectorClicked);

        if (exitButton != null)
            exitButton.onClick.RemoveListener(OnExitClicked);
    }

    private void OnPlayClicked()
    {
        PlayClickSound();
        Debug.Log("Play button clicked - Loading last unlocked level");
        LevelManager.Instance.LoadLevel(0);
    }

    private void OnSettingsClicked()
    {
        PlayClickSound();
        Debug.Log("Settings button clicked - Transitioning to Settings");
        menuManager.ChangeState(MenuState.StateType.Settings);
    }

    private void OnControlsClicked()
    {
        PlayClickSound();
        Debug.Log("Controls button clicked");
        menuManager.ChangeState(MenuState.StateType.Controls);
    }

    private void OnCreditsClicked()
    {
        PlayClickSound();
        Debug.Log("Credits button clicked");
        menuManager.ChangeState(MenuState.StateType.Credits);
    }

    private void OnPhaseSelectorClicked()
    {
        PlayClickSound();
        Debug.Log("Phase Selector button clicked");
        menuManager.ChangeState(MenuState.StateType.PhaseSelector);
    }

    private void OnExitClicked()
    {
        PlayClickSound();
        Debug.Log("Exit button clicked - Closing application");
        #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
        #else
            Application.Quit();
        #endif
    }

    private void PlayClickSound()
    {
        if (AudioManager.Instance != null)
            AudioManager.Instance.PlaySound("Sounds/menu_escolha", null, 1f);
    }
}
