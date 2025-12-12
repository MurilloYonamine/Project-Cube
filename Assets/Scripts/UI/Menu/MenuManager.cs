using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Gerencia o sistema de Menu principal com State Machine.
/// Controla 3 câmeras diferentes (Main Menu, Settings, Phase Selector).
/// Gerencia transições com fade in/out e bounce effects.
/// </summary>
public class MenuManager : MonoBehaviour {
    [Header("Cameras")]
    [SerializeField] private Camera mainMenuCamera;
    [SerializeField] private Camera settingsCamera;
    [SerializeField] private Camera phaseSelectorCamera;

    [Header("Canvas Groups")]
    [SerializeField] private CanvasGroup mainMenuCanvasGroup;
    [SerializeField] private CanvasGroup settingsCanvasGroup;
    [SerializeField] private CanvasGroup phaseSelectorCanvasGroup;

    [Header("States")]
    [SerializeField] private MainMenuState mainMenuState;
    [SerializeField] private SettingsState settingsState;
    [SerializeField] private PhaseSelectorState phaseSelectorState;

    [Header("Timings")]
    [SerializeField] private float fadeDuration = 0.3f;
    [SerializeField] private float bounceDuration = 0.4f;
    [SerializeField] private float bounceScale = 1.1f;

    private MenuState currentState;
    private Dictionary<MenuState.StateType, MenuState> states;
    private bool isTransitioning = false;

    [SerializeField] private AudioClip _backgroundMusic;

    public static MenuManager Instance { get; private set; }

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
        // Inicia no estado de Main Menu
        ChangeState(MenuState.StateType.MainMenu);
        AudioManager.Instance.PlayTrack(_backgroundMusic);
    }

    private void Update() {
        if (currentState != null && !isTransitioning) {
            currentState.StateUpdate();
        }
    }

    /// <summary>
    /// Inicializa todos os estados do menu.
    /// </summary>
    private void InitializeStates() {
        states = new Dictionary<MenuState.StateType, MenuState>();

        // Inicializa Main Menu
        if (mainMenuState != null) {
            mainMenuState.Initialize(this, mainMenuCanvasGroup, mainMenuCamera);
            states.Add(MenuState.StateType.MainMenu, mainMenuState);
            mainMenuCanvasGroup.alpha = 0f;
            mainMenuCamera.enabled = false;
        }

        // Inicializa Settings
        if (settingsState != null) {
            settingsState.Initialize(this, settingsCanvasGroup, settingsCamera);
            states.Add(MenuState.StateType.Settings, settingsState);
            settingsCanvasGroup.alpha = 0f;
            settingsCamera.enabled = false;
        }

        // Inicializa Phase Selector
        if (phaseSelectorState != null) {
            phaseSelectorState.Initialize(this, phaseSelectorCanvasGroup, phaseSelectorCamera);
            states.Add(MenuState.StateType.PhaseSelector, phaseSelectorState);
            phaseSelectorCanvasGroup.alpha = 0f;
            phaseSelectorCamera.enabled = false;
        }
    }

    /// <summary>
    /// Altera o estado do menu com transição suave.
    /// </summary>
    public void ChangeState(MenuState.StateType newStateType) {
        if (isTransitioning || !states.ContainsKey(newStateType))
            return;

        StartCoroutine(TransitionToState(newStateType));
    }

    /// <summary>
    /// Volta para o MainMenu.
    /// </summary>
    public void GoToPreviousState()
    {
        if (isTransitioning)
            return;

        StartCoroutine(TransitionToState(MenuState.StateType.MainMenu));
    }

    /// <summary>
    /// Corrotina que gerencia a transição entre estados.
    /// </summary>
    private IEnumerator TransitionToState(MenuState.StateType newStateType) {
        isTransitioning = true;

        MenuState newState = states[newStateType];

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
    /// Fade in com duração configurável.
    /// </summary>
    public IEnumerator FadeIn(CanvasGroup canvasGroup) {
        float elapsedTime = 0f;

        // Garantir que o canvas não seja interativo enquanto faz fade in
        if (canvasGroup != null) {
            canvasGroup.interactable = false;
            canvasGroup.blocksRaycasts = false;
        }

        while (elapsedTime < fadeDuration) {
            elapsedTime += Time.deltaTime;
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
    /// Fade out com duração configurável.
    /// </summary>
    public IEnumerator FadeOut(CanvasGroup canvasGroup) {
        float elapsedTime = 0f;

        // Desabilitar interação imediatamente ao iniciar o fade out
        if (canvasGroup != null) {
            canvasGroup.interactable = false;
            canvasGroup.blocksRaycasts = false;
        }

        while (elapsedTime < fadeDuration) {
            elapsedTime += Time.deltaTime;
            if (canvasGroup != null)
                canvasGroup.alpha = Mathf.Clamp01(1f - (elapsedTime / fadeDuration));
            yield return null;
        }

        if (canvasGroup != null)
            canvasGroup.alpha = 0f;
    }

    /// <summary>
    /// Efeito de bounce (scale animation) para entrada do menu.
    /// </summary>
    public IEnumerator BounceEffect(Transform target) {
        Vector3 originalScale = target.localScale;
        float elapsedTime = 0f;

        // Scale up
        while (elapsedTime < bounceDuration / 2f) {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / (bounceDuration / 2f);
            target.localScale = Vector3.Lerp(originalScale, originalScale * bounceScale, t);
            yield return null;
        }

        // Scale down de volta ao normal
        elapsedTime = 0f;
        while (elapsedTime < bounceDuration / 2f) {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / (bounceDuration / 2f);
            target.localScale = Vector3.Lerp(originalScale * bounceScale, originalScale, t);
            yield return null;
        }

        target.localScale = originalScale;
    }

    /// <summary>
    /// Retorna o estado atual do menu.
    /// </summary>
    public MenuState GetCurrentState() => currentState;

    /// <summary>
    /// Retorna se está em transição no momento.
    /// </summary>
    public bool IsTransitioning() => isTransitioning;
}
