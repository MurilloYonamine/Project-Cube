using UnityEngine;
using System.Collections;

/// <summary>
/// Adapter que permite usar MenuStates (Settings, PhaseSelector) dentro do PauseManager.
/// Converte as chamadas de MenuState para funcionar com PauseState.
/// </summary>
public class MenuStateToPauseStateAdapter : PauseState
{
    private MenuState menuState;

    public MenuStateToPauseStateAdapter Initialize(MenuState state, PauseManager manager)
    {
        menuState = state;
        pauseManager = manager;
        return this;
    }

    public override StateType GetStateType()
    {
        // Mapeia MenuState.StateType para PauseState.StateType
        switch (menuState.GetStateType())
        {
            case MenuState.StateType.Settings:
                return StateType.Settings;
            case MenuState.StateType.PhaseSelector:
                return StateType.PhaseSelector;
            default:
                return StateType.PauseMenu;
        }
    }

    public override IEnumerator OnEnter()
    {
        Debug.Log($"[Adapter] OnEnter called for {menuState.GetStateType()}");
        
        // Chama o OnEnter do MenuState original para configurar tudo
        if (menuState != null)
        {
            // Substitui o fade normal pelo fade do PauseManager
            CanvasGroup group = GetCanvasGroup();
            if (group != null)
            {
                yield return pauseManager.FadeIn(group);
            }
            
            // Chama o setup do estado (OnEnterComplete)
            CallProtectedMethod(menuState, "OnEnterComplete");
        }
    }

    public override IEnumerator OnExit()
    {
        Debug.Log($"[Adapter] OnExit called for {menuState.GetStateType()}");
        
        // Chama o OnExitComplete antes do fade
        if (menuState != null)
        {
            CallProtectedMethod(menuState, "OnExitComplete");
            
            // Usa o fade do PauseManager (com unscaled time)
            CanvasGroup group = GetCanvasGroup();
            if (group != null)
            {
                yield return pauseManager.FadeOut(group);
            }
        }
    }

    public override void StateUpdate()
    {
        if (menuState != null)
        {
            menuState.StateUpdate();
        }
    }

    private CanvasGroup GetCanvasGroup()
    {
        // Acessa o canvasGroup através de reflection
        var field = typeof(MenuState).GetField("canvasGroup", 
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        return field?.GetValue(menuState) as CanvasGroup;
    }
    
    private void CallProtectedMethod(object obj, string methodName)
    {
        // Chama métodos protegidos através de reflection
        var method = obj.GetType().GetMethod(methodName, 
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        method?.Invoke(obj, null);
    }
}
