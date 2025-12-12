using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Script auxiliar para exemplo de configuração do Pause Canvas.
/// Adicione os botões de Pause com este padrão.
/// </summary>
public class PauseUIController : MonoBehaviour
{
    [SerializeField] private Button resumeButton;
    [SerializeField] private Button settingsButton;
    [SerializeField] private Button mainMenuButton;
    [SerializeField] private Button quitButton;

    private void Start()
    {
        // Configura os botões do pause
        if (resumeButton != null)
            resumeButton.onClick.AddListener(OnResumeClicked);

        if (settingsButton != null)
            settingsButton.onClick.AddListener(OnSettingsClicked);

        if (mainMenuButton != null)
            mainMenuButton.onClick.AddListener(OnMainMenuClicked);

        if (quitButton != null)
            quitButton.onClick.AddListener(OnQuitClicked);
    }

    private void OnResumeClicked()
    {
        Debug.Log("Resume clicked");
        PauseManager.Instance.Resume();
    }

    private void OnSettingsClicked()
    {
        Debug.Log("Settings clicked - Implement settings for paused state");
        // TODO: Implementar settings durante pause
    }

    private void OnMainMenuClicked()
    {
        Debug.Log("Main Menu clicked - Loading main menu scene");
        Time.timeScale = 1f; // Reseta o tempo antes de carregar a cena
        // TODO: Carregar a cena do menu principal
        // SceneManager.LoadScene("MainMenu");
    }

    private void OnQuitClicked()
    {
        Debug.Log("Quit clicked");
        Time.timeScale = 1f; // Reseta o tempo
        #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
        #else
            Application.Quit();
        #endif
    }

    private void OnDestroy()
    {
        // Remove listeners para evitar memory leaks
        if (resumeButton != null)
            resumeButton.onClick.RemoveListener(OnResumeClicked);

        if (settingsButton != null)
            settingsButton.onClick.RemoveListener(OnSettingsClicked);

        if (mainMenuButton != null)
            mainMenuButton.onClick.RemoveListener(OnMainMenuClicked);

        if (quitButton != null)
            quitButton.onClick.RemoveListener(OnQuitClicked);
    }
}
