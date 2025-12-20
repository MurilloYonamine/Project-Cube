using UnityEngine;
using System.Collections;

/// <summary>
/// Exemplos de como estender e usar o sistema de Menu e Pause.
/// Descomente e adapte conforme necessário.
/// </summary>
public class MenuSystemExamples : MonoBehaviour
{
    // ============================================================
    // EXEMPLO 1: Como criar um novo estado de menu
    // ============================================================
    /*
    public class CreditsState : MenuState
    {
        [SerializeField] private Button backButton;
        [SerializeField] private ScrollRect creditsScroll;

        protected override void OnEnterComplete()
        {
            if (backButton != null)
                backButton.onClick.AddListener(OnBackClicked);
        }

        protected override void OnExitComplete()
        {
            if (backButton != null)
                backButton.onClick.RemoveListener(OnBackClicked);
        }

        private void OnBackClicked()
        {
            menuManager.ChangeState(MenuState.StateType.MainMenu);
        }
    }
    */

    // ============================================================
    // EXEMPLO 2: Como usar MenuManager em outro script
    // ============================================================
    /*
    public class GameScene : MonoBehaviour
    {
        private void Update()
        {
            // Ir para Main Menu ao pressionar M
            if (Input.GetKeyDown(KeyCode.M))
            {
                MenuManager.Instance.ChangeState(MenuState.StateType.MainMenu);
            }
        }
    }
    */

    // ============================================================
    // EXEMPLO 3: Controlar música ao trocar de estado
    // ============================================================
    /*
    public class MainMenuState : MenuState
    {
        protected override void OnEnterComplete()
        {
            // Toca música do menu
            AudioManager.Instance.PlayMainMenuMusic();
            
            base.OnEnterComplete();
        }
    }
    */

    // ============================================================
    // EXEMPLO 4: Sistema de SFX ao clicar
    // ============================================================
    /*
    public class SFXClickSound : MonoBehaviour
    {
        [SerializeField] private AudioClip clickSFX;
        private Button button;

        private void Start()
        {
            button = GetComponent<Button>();
            button.onClick.AddListener(PlayClickSound);
        }

        private void PlayClickSound()
        {
            AudioManager.Instance.PlaySFX(clickSFX, 1f);
        }
    }
    */

    // ============================================================
    // EXEMPLO 5: Carregamento de cena ao selecionar fase
    // ============================================================
    /*
    public class PhaseSelectorState : MenuState
    {
        [SerializeField] private Button[] phaseButtons;

        protected override void OnEnterComplete()
        {
            for (int i = 0; i < phaseButtons.Length; i++)
            {
                int phaseIndex = i;
                phaseButtons[i].onClick.AddListener(() => OnPhaseSelected(phaseIndex + 1));
            }
        }

        private void OnPhaseSelected(int phaseNumber)
        {
            // Fade out do menu
            StartCoroutine(menuManager.FadeOut(canvasGroup));

            // Carregar cena da fase
            UnityEngine.SceneManagement.SceneManager.LoadScene($"Level_{phaseNumber}");
        }
    }
    */

    // ============================================================
    // EXEMPLO 6: Pausar ao entrar em menu durante gameplay
    // ============================================================
    /*
    public class InGameMenuState : MenuState
    {
        protected override IEnumerator OnEnter()
        {
            // Pausa o jogo
            PauseManager.Instance.Pause();
            
            // Executa fade in
            yield return menuManager.FadeIn(canvasGroup);
            
            OnEnterComplete();
        }

        protected override IEnumerator OnExit()
        {
            // Retoma o jogo
            PauseManager.Instance.Resume();
            
            // Executa fade out
            yield return menuManager.FadeOut(canvasGroup);
            
            OnExitComplete();
        }
    }
    */

    // ============================================================
    // EXEMPLO 7: Verifica volume antes de tocar SFX
    // ============================================================
    /*
    public void PlaySoundIfVolumeAllow(AudioClip clip)
    {
        float currentVolume = AudioManager.Instance.GetVolume();
        
        if (currentVolume > 0.1f)
        {
            AudioManager.Instance.PlaySFX(clip, currentVolume);
        }
    }
    */

    // ============================================================
    // EXEMPLO 8: Sistema de confirmação antes de sair
    // ============================================================
    /*
    public class ConfirmationPanel : MonoBehaviour
    {
        [SerializeField] private Button yesButton;
        [SerializeField] private Button noButton;

        private void Start()
        {
            yesButton.onClick.AddListener(() => 
            {
                #if UNITY_EDITOR
                    UnityEditor.EditorApplication.isPlaying = false;
                #else
                    Application.Quit();
                #endif
            });

            noButton.onClick.AddListener(() => gameObject.SetActive(false));
        }
    }
    */

    // ============================================================
    // EXEMPLO 9: Customizar duração de fade por estado
    // ============================================================
    /*
    public class CustomFadeState : MenuState
    {
        public override IEnumerator OnEnter()
        {
            if (stateCamera != null)
                stateCamera.enabled = true;

            // Fade in com duração customizada
            yield return StartCoroutine(CustomFadeIn(canvasGroup, 0.5f)); // 0.5s

            OnEnterComplete();
        }

        private IEnumerator CustomFadeIn(CanvasGroup group, float duration)
        {
            float elapsedTime = 0f;

            while (elapsedTime < duration)
            {
                elapsedTime += Time.deltaTime;
                group.alpha = Mathf.Clamp01(elapsedTime / duration);
                yield return null;
            }

            group.alpha = 1f;
        }
    }
    */

    // ============================================================
    // EXEMPLO 10: Debug - Mostrar estado atual do menu
    // ============================================================
    /*
    public class DebugMenuState : MonoBehaviour
    {
        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.D))
            {
                MenuState current = MenuManager.Instance.GetCurrentState();
                bool isTransitioning = MenuManager.Instance.IsTransitioning();
                bool isPaused = PauseManager.Instance.IsPaused();

                Debug.Log($"Current State: {current.GetType().Name}");
                Debug.Log($"Is Transitioning: {isTransitioning}");
                Debug.Log($"Is Game Paused: {isPaused}");
                Debug.Log($"Master Volume: {AudioManager.Instance.GetVolume()}");
            }
        }
    }
    */
}

/*
 * INSTRUÇÕES GERAIS:
 * 
 * 1. Para estender o sistema, copie o código desejado do exemplo
 * 2. Descomente e coloque em um novo arquivo
 * 3. Ajuste os tipos de dados e nomes conforme necessário
 * 4. Registre novos estados no MenuManager
 * 
 * 5. Use Always:
 *    - MenuManager.Instance para acessar o gerenciador de menu
 *    - PauseManager.Instance para acessar o pause
 *    - AudioManager.Instance para acessar áudio
 * 
 * 6. Lembre-se:
 *    - Estados devem herdar de MenuState
 *    - Use corrotinas para animações
 *    - Sempre remova listeners para evitar memory leaks
 *    - Use Time.unscaledDeltaTime durante pause
 * 
 * 7. Boas práticas:
 *    - Sempre verifique se referências não são null
 *    - Use StartCoroutine em vez de Coroutine.Start
 *    - Remova listeners em OnDisable ou no destrutor
 *    - Documente estados customizados com XML comments
 */
