using UnityEngine;
using System.Collections;

/// <summary>
/// Classe base abstrata para todos os estados do menu.
/// Define a interface que cada estado deve implementar.
/// </summary>
public abstract partial class MenuState : MonoBehaviour
{
    /// <summary>
    /// Tipo de estado para identificação.
    /// </summary>
    public enum StateType
    {
        MainMenu,
        Settings,
        PhaseSelector,
        Credits,
        Controls
    }

    /// <summary>
    /// Retorna o tipo de estado.
    /// </summary>
    public abstract StateType GetStateType();
    protected MenuManager menuManager;
    protected CanvasGroup canvasGroup;
    protected Camera stateCamera;

    /// <summary>
    /// Inicializa o estado com referências necessárias.
    /// </summary>
    public virtual void Initialize(MenuManager manager, CanvasGroup group, Camera camera)
    {
        menuManager = manager;
        canvasGroup = group;
        stateCamera = camera;
    }

    /// <summary>
    /// Chamado quando o estado se torna ativo.
    /// </summary>
    public virtual IEnumerator OnEnter()
    {
        // Ativa a câmera do estado
        if (stateCamera != null)
            stateCamera.enabled = true;

        // Fade in
        yield return menuManager.FadeIn(canvasGroup);

        OnEnterComplete();
    }

    /// <summary>
    /// Chamado quando o estado está prestes a sair.
    /// </summary>
    public virtual IEnumerator OnExit()
    {
        // Fade out
        yield return menuManager.FadeOut(canvasGroup);

        // Desativa a câmera do estado
        if (stateCamera != null)
            stateCamera.enabled = false;

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
