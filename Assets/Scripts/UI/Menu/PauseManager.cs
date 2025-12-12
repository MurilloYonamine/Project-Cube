using UnityEngine;
using System.Collections;

/// <summary>
/// Gerencia o sistema de Pause durante o gameplay.
/// Não utiliza State Machine - controla o pause através de CanvasGroup.
/// Usa alpha para fade in/out e desativa raycast target quando pausado.
/// </summary>
public class PauseManager : MonoBehaviour
{
    [SerializeField] private CanvasGroup pauseCanvasGroup;
    [SerializeField] private Canvas pauseCanvas;

    [SerializeField] private float fadeDuration = 0.2f;
    [SerializeField] private float timeScaleWhenPaused = 0f;
    [SerializeField] private float timeScaleWhenPlaying = 1f;

    private bool isPaused = false;
    private bool isTransitioning = false;

    public static PauseManager Instance { get; private set; }

    private void Awake()
    {
        // Singleton pattern
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        // Inicializa o pause canvas como invisível e inativo
        if (pauseCanvasGroup == null)
        {
            Debug.LogError("PauseCanvasGroup not assigned!");
            return;
        }

        pauseCanvasGroup.alpha = 0f;
        pauseCanvasGroup.interactable = false;
        pauseCanvasGroup.blocksRaycasts = false;

        if (pauseCanvas != null)
        {
            pauseCanvas.enabled = false;
        }
    }

    private void Update()
    {
        // Permite pausar com ESC durante gameplay
        if (Input.GetKeyDown(KeyCode.Escape) && !isTransitioning)
        {
            if (isPaused)
                Resume();
            else
                Pause();
        }
    }

    /// <summary>
    /// Pausa o jogo com fade in do canvas de pause.
    /// </summary>
    public void Pause()
    {
        if (isPaused || isTransitioning)
            return;

        StartCoroutine(PauseCoroutine());
    }

    /// <summary>
    /// Retoma o jogo com fade out do canvas de pause.
    /// </summary>
    public void Resume()
    {
        if (!isPaused || isTransitioning)
            return;

        StartCoroutine(ResumeCoroutine());
    }

    /// <summary>
    /// Corrotina para pausar o jogo.
    /// </summary>
    private IEnumerator PauseCoroutine()
    {
        isTransitioning = true;
        isPaused = true;

        // Ativa o canvas
        if (pauseCanvas != null)
        {
            pauseCanvas.enabled = true;
        }

        // Fade in do pause canvas
        yield return FadeInPauseCanvas();

        // Para o jogo (Time.timeScale = 0)
        Time.timeScale = timeScaleWhenPaused;

        // Ativa interação com o canvas de pause
        pauseCanvasGroup.interactable = true;
        pauseCanvasGroup.blocksRaycasts = true;

        isTransitioning = false;

        Debug.Log("Game Paused");
    }

    /// <summary>
    /// Corrotina para retomar o jogo.
    /// </summary>
    private IEnumerator ResumeCoroutine()
    {
        isTransitioning = true;

        // Desativa interação com o canvas de pause
        pauseCanvasGroup.interactable = false;
        pauseCanvasGroup.blocksRaycasts = false;

        // Retoma o jogo (Time.timeScale = 1)
        Time.timeScale = timeScaleWhenPlaying;

        // Fade out do pause canvas
        yield return FadeOutPauseCanvas();

        // Desativa o canvas
        if (pauseCanvas != null)
        {
            pauseCanvas.enabled = false;
        }

        isPaused = false;
        isTransitioning = false;

        Debug.Log("Game Resumed");
    }

    /// <summary>
    /// Fade in do canvas de pause.
    /// </summary>
    private IEnumerator FadeInPauseCanvas()
    {
        float elapsedTime = 0f;

        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.unscaledDeltaTime; // Usa deltaTime não escalado para funcionar durante pause
            pauseCanvasGroup.alpha = Mathf.Clamp01(elapsedTime / fadeDuration);
            yield return null;
        }

        pauseCanvasGroup.alpha = 1f;
    }

    /// <summary>
    /// Fade out do canvas de pause.
    /// </summary>
    private IEnumerator FadeOutPauseCanvas()
    {
        float elapsedTime = 0f;

        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.unscaledDeltaTime; // Usa deltaTime não escalado
            pauseCanvasGroup.alpha = Mathf.Clamp01(1f - (elapsedTime / fadeDuration));
            yield return null;
        }

        pauseCanvasGroup.alpha = 0f;
    }

    /// <summary>
    /// Retorna se o jogo está pausado.
    /// </summary>
    public bool IsPaused()
    {
        return isPaused;
    }

    /// <summary>
    /// Retorna se está em transição de pause/resume.
    /// </summary>
    public bool IsTransitioning()
    {
        return isTransitioning;
    }

    /// <summary>
    /// Força o tempo a voltar ao normal (útil ao carregar cenas).
    /// </summary>
    public void ResetTimeScale()
    {
        Time.timeScale = timeScaleWhenPlaying;
        isPaused = false;
        isTransitioning = false;
        pauseCanvasGroup.alpha = 0f;
        pauseCanvasGroup.interactable = false;
        pauseCanvasGroup.blocksRaycasts = false;

        if (pauseCanvas != null)
        {
            pauseCanvas.enabled = false;
        }

        Debug.Log("TimeScale reset to normal");
    }
}
