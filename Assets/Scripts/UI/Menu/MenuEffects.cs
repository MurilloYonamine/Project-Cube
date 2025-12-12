using UnityEngine;
using UnityEngine.UI;
using System.Collections;

/// <summary>
/// Classe utilitária para efeitos visuais comuns no sistema de menu.
/// Pode ser usada como referência ou estendida para outros efeitos.
/// </summary>
public static class MenuEffects
{
    /// <summary>
    /// Efeito de shake (vibração) em um objeto.
    /// Útil para feedback visual em botões.
    /// </summary>
    public static IEnumerator ShakeEffect(Transform target, float duration = 0.2f, float magnitude = 0.1f)
    {
        Vector3 originalPosition = target.localPosition;
        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;

            float x = Random.Range(-1f, 1f) * magnitude;
            float y = Random.Range(-1f, 1f) * magnitude;

            target.localPosition = originalPosition + new Vector3(x, y, 0f);

            yield return null;
        }

        target.localPosition = originalPosition;
    }

    /// <summary>
    /// Efeito de pulsação (scale up and down repetidamente).
    /// Útil para indicar que algo é interativo.
    /// </summary>
    public static IEnumerator PulseEffect(Transform target, float duration = 1f, float minScale = 0.95f, float maxScale = 1.05f)
    {
        Vector3 originalScale = target.localScale;
        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;

            // Oscila entre min e max
            float t = Mathf.PingPong(elapsedTime / (duration / 2f), 1f);
            float scale = Mathf.Lerp(minScale, maxScale, t);

            target.localScale = originalScale * scale;

            yield return null;
        }

        target.localScale = originalScale;
    }

    /// <summary>
    /// Efeito de slide (movimento suave de uma posição para outra).
    /// Útil para menus que entram de um lado da tela.
    /// </summary>
    public static IEnumerator SlideEffect(Transform target, Vector3 startPosition, Vector3 endPosition, float duration = 0.5f, AnimationCurve curve = null)
    {
        if (curve == null)
            curve = AnimationCurve.EaseInOut(0, 0, 1, 1);

        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            float t = curve.Evaluate(elapsedTime / duration);

            target.localPosition = Vector3.Lerp(startPosition, endPosition, t);

            yield return null;
        }

        target.localPosition = endPosition;
    }

    /// <summary>
    /// Efeito de rotação suave.
    /// Útil para spinners de loading ou efeitos decorativos.
    /// </summary>
    public static IEnumerator RotateEffect(Transform target, float duration = 1f, float totalRotation = 360f, bool infinite = false)
    {
        if (infinite)
        {
            float elapsedTime = 0f;
            while (true)
            {
                elapsedTime += Time.deltaTime;
                float rotation = (elapsedTime / duration) * totalRotation;
                target.localRotation = Quaternion.Euler(0, 0, rotation);
                yield return null;
            }
        }
        else
        {
            float elapsedTime = 0f;
            while (elapsedTime < duration)
            {
                elapsedTime += Time.deltaTime;
                float rotation = (elapsedTime / duration) * totalRotation;
                target.localRotation = Quaternion.Euler(0, 0, rotation);
                yield return null;
            }

            target.localRotation = Quaternion.Euler(0, 0, totalRotation);
        }
    }

    /// <summary>
    /// Efeito de fade em uma Image.
    /// </summary>
    public static IEnumerator FadeImage(Image image, float startAlpha, float endAlpha, float duration = 0.5f)
    {
        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / duration;
            float alpha = Mathf.Lerp(startAlpha, endAlpha, t);

            Color color = image.color;
            color.a = alpha;
            image.color = color;

            yield return null;
        }

        Color finalColor = image.color;
        finalColor.a = endAlpha;
        image.color = finalColor;
    }

    /// <summary>
    /// Efeito de "pop in" - escalas de 0 a 1 com bounce.
    /// Mais dramático que o bounce simples.
    /// </summary>
    public static IEnumerator PopInEffect(Transform target, float duration = 0.4f)
    {
        Vector3 originalScale = target.localScale;
        target.localScale = Vector3.zero;

        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / duration;

            // Usando ease out bounce
            float scale = EaseOutBounce(t);
            target.localScale = originalScale * scale;

            yield return null;
        }

        target.localScale = originalScale;
    }

    /// <summary>
    /// Efeito de "wobble" (oscilação)
    /// Útil para indicar que algo está errado (ex: input inválido)
    /// </summary>
    public static IEnumerator WobbleEffect(Transform target, float duration = 0.3f, float angle = 10f)
    {
        Quaternion originalRotation = target.localRotation;
        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;

            float progress = (elapsedTime / duration);
            float oscillation = Mathf.Sin(progress * Mathf.PI * 4f); // 2 oscilações completas
            float currentAngle = oscillation * angle * (1f - progress);

            target.localRotation = originalRotation * Quaternion.Euler(0, 0, currentAngle);

            yield return null;
        }

        target.localRotation = originalRotation;
    }

    /// <summary>
    /// Função auxiliar para calcular ease out bounce (easing function)
    /// </summary>
    private static float EaseOutBounce(float t)
    {
        float n1 = 7.5625f;
        float d1 = 2.75f;

        if (t < 1f / d1)
        {
            return n1 * t * t;
        }
        else if (t < 2f / d1)
        {
            return n1 * (t -= 1.5f / d1) * t + 0.75f;
        }
        else if (t < 2.5f / d1)
        {
            return n1 * (t -= 2.25f / d1) * t + 0.9375f;
        }
        else
        {
            return n1 * (t -= 2.625f / d1) * t + 0.984375f;
        }
    }
}

/// <summary>
/// Classe auxiliar para controlar e debugar o sistema de menu.
/// Adicione este script a um GameObject para usar as funções de debug.
/// </summary>
public class MenuDebugger : MonoBehaviour
{
    private void Update()
    {
        // Pressione F1 para debug info
        if (Input.GetKeyDown(KeyCode.F1))
        {
            PrintDebugInfo();
        }

        // Pressione F2 para simular pause
        if (Input.GetKeyDown(KeyCode.F2))
        {
            if (PauseManager.Instance.IsPaused())
                PauseManager.Instance.Resume();
            else
                PauseManager.Instance.Pause();
        }

        // Pressione F3 para ir ao Main Menu
        if (Input.GetKeyDown(KeyCode.F3))
        {
            MenuManager.Instance.ChangeState(MenuState.StateType.MainMenu);
        }

        // Pressione F4 para ir a Settings
        if (Input.GetKeyDown(KeyCode.F4))
        {
            MenuManager.Instance.ChangeState(MenuState.StateType.Settings);
        }

        // Pressione F5 para ir a Phase Selector
        if (Input.GetKeyDown(KeyCode.F5))
        {
            MenuManager.Instance.ChangeState(MenuState.StateType.PhaseSelector);
        }
    }

    private void PrintDebugInfo()
    {
        Debug.Log("========== MENU SYSTEM DEBUG INFO ==========");

        // Menu Manager Info
        if (MenuManager.Instance != null)
        {
            MenuState current = MenuManager.Instance.GetCurrentState();
            bool isTransitioning = MenuManager.Instance.IsTransitioning();

            Debug.Log($"[MENU] Current State: {(current != null ? current.GetType().Name : "None")}");
            Debug.Log($"[MENU] Is Transitioning: {isTransitioning}");
        }
        else
        {
            Debug.LogWarning("[MENU] MenuManager instance not found!");
        }

        // Pause Manager Info
        if (PauseManager.Instance != null)
        {
            bool isPaused = PauseManager.Instance.IsPaused();
            bool isTransitioning = PauseManager.Instance.IsTransitioning();

            Debug.Log($"[PAUSE] Is Paused: {isPaused}");
            Debug.Log($"[PAUSE] Is Transitioning: {isTransitioning}");
            Debug.Log($"[PAUSE] Time Scale: {Time.timeScale}");
        }
        else
        {
            Debug.LogWarning("[PAUSE] PauseManager instance not found!");
        }

        // Audio Manager Info
        if (AudioManager.Instance != null)
        {
            Debug.Log($"[AUDIO] Master Volume: Check mixer");
            Debug.Log($"[AUDIO] Music Channels: {AudioManager.Instance.channels.Count}");
        }
        else
        {
            Debug.LogWarning("[AUDIO] AudioManager instance not found!");
        }

        Debug.Log("==========================================");
    }
}
