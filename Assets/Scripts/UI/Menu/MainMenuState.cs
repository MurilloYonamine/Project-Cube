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

        if (exitButton != null)
            exitButton.onClick.AddListener(OnExitClicked);
    }

    protected override void OnExitComplete()
    {
        // Remove os listeners para evitar múltiplas chamadas
        if (playButton != null)
            playButton.onClick.RemoveListener(OnPlayClicked);

        if (settingsButton != null)
            settingsButton.onClick.RemoveListener(OnSettingsClicked);

        if (phaseSelectorButton != null)
            phaseSelectorButton.onClick.RemoveListener(OnPhaseSelectorClicked);

        if (exitButton != null)
            exitButton.onClick.RemoveListener(OnExitClicked);
    }

    private void OnPlayClicked()
    {
        Debug.Log("Play button clicked - Loading last unlocked level");
        LevelManager.Instance.LoadLevel(LevelManager.Instance.CurrentUnlockedLevel);
    }

    private void OnSettingsClicked()
    {
        Debug.Log("Settings button clicked - Transitioning to Settings");
        menuManager.ChangeState(MenuState.StateType.Settings);
    }

    private void OnPhaseSelectorClicked()
    {
        Debug.Log("Phase Selector button clicked");
        menuManager.ChangeState(MenuState.StateType.PhaseSelector);
    }

    private void OnExitClicked()
    {
        Debug.Log("Exit button clicked - Closing application");
        #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
        #else
            Application.Quit();
        #endif
    }
}
