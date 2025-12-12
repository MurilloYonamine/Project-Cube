using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Gerencia o sistema de Pause durante o gameplay com State Machine.
/// Controla o estado do pause menu com transições suaves.
/// Gerencia transições com fade in/out e controle de TimeScale.
/// </summary>
public class PauseManager : MonoBehaviour {
    [Header("Canvas Groups")]
    [SerializeField] private CanvasGroup pauseMenuCanvasGroup;
    [SerializeField] private CanvasGroup settingsCanvasGroup;
    [SerializeField] private CanvasGroup phaseSelectorCanvasGroup;

    [Header("States")]
    [SerializeField] private PauseMenuState pauseMenuState;
    [SerializeField] private SettingsState settingsState;
    [SerializeField] private PhaseSelectorState phaseSelectorState;

    [Header("Timings")]
    [SerializeField] private float fadeDuration = 0.3f;
    [SerializeField] private float bounceDuration = 0.4f;
    [SerializeField] private float bounceScale = 1.1f;

    [Header("Time Control")]
    [SerializeField] private float timeScaleWhenPaused = 0f;
    [SerializeField] private float timeScaleWhenPlaying = 1f;

    private PauseState currentState;
    private Dictionary<PauseState.StateType, PauseState> states;
    private bool isPaused = false;
    private bool isTransitioning = false;

    public static PauseManager Instance { get; private set; }

    private void Awake() {
        // Singleton pattern
        if (Instance != null && Instance != this) {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        InitializeStates();
    }

    private void Start() {
        // Inicializa todos os canvas groups como invisíveis
        if (pauseMenuCanvasGroup != null) {
            pauseMenuCanvasGroup.alpha = 0f;
            pauseMenuCanvasGroup.interactable = false;
            pauseMenuCanvasGroup.blocksRaycasts = false;
        }

        if (settingsCanvasGroup != null) {
            settingsCanvasGroup.alpha = 0f;
            settingsCanvasGroup.interactable = false;
            settingsCanvasGroup.blocksRaycasts = false;
        }

        if (phaseSelectorCanvasGroup != null) {
            phaseSelectorCanvasGroup.alpha = 0f;
            phaseSelectorCanvasGroup.interactable = false;
            phaseSelectorCanvasGroup.blocksRaycasts = false;
        }
    }

    private void Update() {
        // Permite pausar com ESC durante gameplay
        if (Input.GetKeyDown(KeyCode.Escape) && !isTransitioning) {
            if (isPaused)
                Resume();
            else
                Pause();
        }

        // Atualiza o estado atual
        if (currentState != null && !isTransitioning) {
            currentState.StateUpdate();
        }
    }

    /// <summary>
    /// Inicializa todos os estados do pause.
    /// </summary>
    private void InitializeStates() {
        states = new Dictionary<PauseState.StateType, PauseState>();

        // Inicializa Pause Menu
        if (pauseMenuState != null && pauseMenuCanvasGroup != null) {
            pauseMenuState.Initialize(this, pauseMenuCanvasGroup);
            states.Add(PauseState.StateType.PauseMenu, pauseMenuState);
        }

        // Inicializa Settings (usa o state do menu normal)
        if (settingsState != null && settingsCanvasGroup != null) {
            settingsState.Initialize(null, settingsCanvasGroup, null);
            states.Add(PauseState.StateType.Settings, ConvertMenuStateToPauseState(settingsState));
        }

        // Inicializa Phase Selector (usa o state do menu normal)
        if (phaseSelectorState != null && phaseSelectorCanvasGroup != null) {
            phaseSelectorState.Initialize(null, phaseSelectorCanvasGroup, null);
            states.Add(PauseState.StateType.PhaseSelector, ConvertMenuStateToPauseState(phaseSelectorState));
        }
    }

    /// <summary>
    /// Converte um MenuState para uso como PauseState (adapter pattern).
    /// </summary>
    private PauseState ConvertMenuStateToPauseState(MenuState menuState) {
        return gameObject.AddComponent<MenuStateToPauseStateAdapter>().Initialize(menuState, this);
    }

    /// <summary>
    /// Pausa o jogo e entra no estado de pause menu.
    /// </summary>
    public void Pause() {
        if (isPaused || isTransitioning)
            return;

        StartCoroutine(PauseCoroutine());
    }

    /// <summary>
    /// Retoma o jogo e sai do estado de pause.
    /// </summary>
    public void Resume() {
        if (!isPaused || isTransitioning)
            return;

        StartCoroutine(ResumeCoroutine());
    }

    /// <summary>
    /// Altera o estado do pause menu com transição suave.
    /// </summary>
    public void ChangeState(PauseState.StateType newStateType) {
        if (isTransitioning || !states.ContainsKey(newStateType))
            return;

        AudioManager.Instance.PlaySound("Sounds/menu_escolha", volume: 1f);
        StartCoroutine(TransitionToState(newStateType));
    }

    /// <summary>
    /// Volta para o PauseMenu.
    /// </summary>
    public void GoToPreviousState() {
        if (isTransitioning)
            return;

        AudioManager.Instance.PlaySound("Sounds/menu_escolha", volume: 1f);
        StartCoroutine(TransitionToState(PauseState.StateType.PauseMenu));
    }

    /// <summary>
    /// Corrotina para pausar o jogo.
    /// </summary>
    private IEnumerator PauseCoroutine() {
        isTransitioning = true;
        isPaused = true;

        // Para o jogo
        Time.timeScale = timeScaleWhenPaused;

        // Ativa o GameObject do pause menu canvas se estiver desativado
        if (pauseMenuCanvasGroup != null && !pauseMenuCanvasGroup.gameObject.activeInHierarchy) {
            pauseMenuCanvasGroup.gameObject.SetActive(true);
        }

        // Entra no estado de pause menu
        if (states.ContainsKey(PauseState.StateType.PauseMenu)) {
            currentState = states[PauseState.StateType.PauseMenu];
            yield return StartCoroutine(currentState.OnEnter());
        }

        isTransitioning = false;

        Debug.Log("Game Paused");
    }

    /// <summary>
    /// Corrotina para retomar o jogo.
    /// </summary>
    private IEnumerator ResumeCoroutine() {
        isTransitioning = true;

        // Sai do estado atual
        if (currentState != null) {
            yield return StartCoroutine(currentState.OnExit());
        }

        // Retoma o jogo
        Time.timeScale = timeScaleWhenPlaying;

        currentState = null;
        isPaused = false;
        isTransitioning = false;

        Debug.Log("Game Resumed");
    }

    /// <summary>
    /// Corrotina que gerencia a transição entre estados do pause.
    /// </summary>
    private IEnumerator TransitionToState(PauseState.StateType newStateType) {
        isTransitioning = true;

        PauseState newState = states[newStateType];

        // Sai do estado anterior
        if (currentState != null) {
            yield return StartCoroutine(currentState.OnExit());
        }

        // Entra no novo estado
        currentState = newState;
        yield return StartCoroutine(currentState.OnEnter());

        isTransitioning = false;
    }

    /// <summary>
    /// Fade in com duração configurável (usando unscaled time para funcionar durante pause).
    /// </summary>
    public IEnumerator FadeIn(CanvasGroup canvasGroup) {
        float elapsedTime = 0f;

        // Garantir que o canvas não seja interativo enquanto faz fade in
        if (canvasGroup != null) {
            canvasGroup.interactable = false;
            canvasGroup.blocksRaycasts = false;
        }

        while (elapsedTime < fadeDuration) {
            elapsedTime += Time.unscaledDeltaTime; // Usa deltaTime não escalado
            if (canvasGroup != null)
                canvasGroup.alpha = Mathf.Clamp01(elapsedTime / fadeDuration);
            yield return null;
        }

        if (canvasGroup != null) {
            canvasGroup.alpha = 1f;
            // Tornar interativo somente após o fade in completo
            canvasGroup.interactable = true;
            canvasGroup.blocksRaycasts = true;
        }
    }

    /// <summary>
    /// Fade out com duração configurável (usando unscaled time para funcionar durante pause).
    /// </summary>
    public IEnumerator FadeOut(CanvasGroup canvasGroup) {
        float elapsedTime = 0f;

        // Desabilitar interação imediatamente ao iniciar o fade out
        if (canvasGroup != null) {
            canvasGroup.interactable = false;
            canvasGroup.blocksRaycasts = false;
        }

        while (elapsedTime < fadeDuration) {
            elapsedTime += Time.unscaledDeltaTime; // Usa deltaTime não escalado
            if (canvasGroup != null)
                canvasGroup.alpha = Mathf.Clamp01(1f - (elapsedTime / fadeDuration));
            yield return null;
        }

        if (canvasGroup != null)
            canvasGroup.alpha = 0f;
    }

    /// <summary>
    /// Efeito de bounce (scale animation) para entrada do pause.
    /// </summary>
    public IEnumerator BounceEffect(Transform target) {
        Vector3 originalScale = target.localScale;
        float elapsedTime = 0f;

        // Scale up
        while (elapsedTime < bounceDuration / 2f) {
            elapsedTime += Time.unscaledDeltaTime; // Usa deltaTime não escalado
            float t = elapsedTime / (bounceDuration / 2f);
            target.localScale = Vector3.Lerp(originalScale, originalScale * bounceScale, t);
            yield return null;
        }

        // Scale down de volta ao normal
        elapsedTime = 0f;
        while (elapsedTime < bounceDuration / 2f) {
            elapsedTime += Time.unscaledDeltaTime; // Usa deltaTime não escalado
            float t = elapsedTime / (bounceDuration / 2f);
            target.localScale = Vector3.Lerp(originalScale * bounceScale, originalScale, t);
            yield return null;
        }

        target.localScale = originalScale;
    }

    /// <summary>
    /// Retorna o estado atual do pause.
    /// </summary>
    public PauseState GetCurrentState() => currentState;

    /// <summary>
    /// Retorna se o jogo está pausado.
    /// </summary>
    public bool IsPaused() {
        return isPaused;
    }

    /// <summary>
    /// Retorna se está em transição de pause/resume.
    /// </summary>
    public bool IsTransitioning() {
        return isTransitioning;
    }

    /// <summary>
    /// Força o tempo a voltar ao normal (útil ao carregar cenas).
    /// </summary>
    public void ResetTimeScale() {
        Time.timeScale = timeScaleWhenPlaying;
        isPaused = false;
        isTransitioning = false;

        // Reseta todos os canvas groups
        if (pauseMenuCanvasGroup != null) {
            pauseMenuCanvasGroup.alpha = 0f;
            pauseMenuCanvasGroup.interactable = false;
            pauseMenuCanvasGroup.blocksRaycasts = false;
        }

        if (settingsCanvasGroup != null) {
            settingsCanvasGroup.alpha = 0f;
            settingsCanvasGroup.interactable = false;
            settingsCanvasGroup.blocksRaycasts = false;
        }

        if (phaseSelectorCanvasGroup != null) {
            phaseSelectorCanvasGroup.alpha = 0f;
            phaseSelectorCanvasGroup.interactable = false;
            phaseSelectorCanvasGroup.blocksRaycasts = false;
        }

        currentState = null;

        Debug.Log("TimeScale reset to normal");
    }
}
