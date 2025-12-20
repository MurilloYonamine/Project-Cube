using UnityEngine;
using System.Collections;
using UnityEngine.UI;

/// <summary>
/// Gerencia o estado de Créditos.
/// Exibe os créditos do jogo.
/// </summary>
public partial class CreditsState : MenuState
{
    [SerializeField] private Button backButton;

    public override StateType GetStateType() => StateType.Credits;

    protected override void OnEnterComplete()
    {
        // Configura o botão de voltar
        if (backButton != null)
            backButton.onClick.AddListener(OnBackClicked);
    }

    protected override void OnExitComplete()
    {
        // Remove o listener
        if (backButton != null)
            backButton.onClick.RemoveListener(OnBackClicked);
    }

    public override void StateUpdate()
    {
        // Nada especial aqui
    }

    private void OnBackClicked()
    {
        if (AudioManager.Instance != null)
            AudioManager.Instance.PlaySound("Sounds/menu_escolha", null, 1f);
            
        menuManager.GoToPreviousState();
    }
}
