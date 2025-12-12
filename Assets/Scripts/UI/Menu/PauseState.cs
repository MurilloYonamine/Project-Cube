using UnityEngine;
using System.Collections;

/// <summary>
/// Classe base abstrata para todos os estados do pause.
/// Define a interface que cada estado deve implementar.
/// </summary>
public abstract partial class PauseState : MonoBehaviour
{
    /// <summary>
    /// Tipo de estado para identificação.
    /// </summary>
    public enum StateType
    {
        PauseMenu,
        Settings,
        PhaseSelector,
        Confirm
    }

    /// <summary>
    /// Retorna o tipo de estado.
    /// </summary>
    public abstract StateType GetStateType();
    protected PauseManager pauseManager;
    protected CanvasGroup canvasGroup;

    /// <summary>
    /// Inicializa o estado com referências necessárias.
    /// </summary>
    public virtual void Initialize(PauseManager manager, CanvasGroup group)
    {
        pauseManager = manager;
        canvasGroup = group;
    }

    /// <summary>
    /// Chamado quando o estado se torna ativo.
    /// </summary>
    public virtual IEnumerator OnEnter()
    {
        // Fade in
        yield return pauseManager.FadeIn(canvasGroup);

        OnEnterComplete();
    }

    /// <summary>
    /// Chamado quando o estado está prestes a sair.
    /// </summary>
    public virtual IEnumerator OnExit()
    {
        // Fade out
        yield return pauseManager.FadeOut(canvasGroup);

        OnExitComplete();
    }

    /// <summary>
    /// Chamado após o término da animação de entrada.
    /// Pode ser sobrescrito para lógica adicional.
    /// </summary>
    protected virtual void OnEnterComplete() { }

    /// <summary>
    /// Chamado após o término da animação de saída.
    /// Pode ser sobrescrito para lógica adicional.
    /// </summary>
    protected virtual void OnExitComplete() { }

    /// <summary>
    /// Chamado a cada frame enquanto o estado está ativo.
    /// </summary>
    public virtual void StateUpdate() { }
}
