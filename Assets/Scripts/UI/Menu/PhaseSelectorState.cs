using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using PROJECT_CUBE.LEVEL;

/// <summary>
/// Gerencia o estado do Seletor de Fases.
/// Permite ao jogador selecionar qual fase jogar.
/// </summary>
public partial class PhaseSelectorState : MenuState
{
    [SerializeField] private Button backButton;
    [SerializeField] private Button[] phaseButtons;
    [Header("Lock Sprites (assign in Inspector)")]
    [SerializeField] private Sprite lockClosedSprite;
    [SerializeField] private Sprite lockOpenSprite;

    public override StateType GetStateType() => StateType.PhaseSelector;

    // Override OnEnter to skip the bounce effect (prevents zoom-in when opening phase selector)
    public override IEnumerator OnEnter()
    {
        // Ativa a câmera do estado
        if (stateCamera != null)
            stateCamera.enabled = true;

        // Apenas fade in (sem bounce)
        yield return menuManager.FadeIn(canvasGroup);

        OnEnterComplete();
    }

    protected override void OnEnterComplete()
    {
        // Configura o botão voltar
        if (backButton != null)
            backButton.onClick.AddListener(OnBackClicked);

        // Configura os botões de fase
        for (int i = 0; i < phaseButtons.Length; i++)
        {
            int phaseIndex = i; // Captura para closure
            phaseButtons[i].onClick.AddListener(() => OnPhaseSelected(phaseIndex + 1));
            // Atualiza o ícone de cadeado (se houver) de acordo com o progresso
            TryApplyLockSprite(phaseButtons[i], phaseIndex + 1);
        }
    }

    protected override void OnExitComplete()
    {
        // Remove os listeners
        if (backButton != null)
            backButton.onClick.RemoveListener(OnBackClicked);

        foreach (Button btn in phaseButtons)
        {
            btn.onClick.RemoveAllListeners();
        }
    }

    private void OnPhaseSelected(int phaseNumber)
    {
        Debug.Log($"Phase {phaseNumber} selected - Loading scene...");
        // TODO: Implementar carregamento de cena da fase
        // SceneManager.LoadScene($"Level_{phaseNumber}");
    }

    private void TryApplyLockSprite(Button btn, int phaseNumber)
    {
        if (btn == null)
            return;

        // Procura por um Image filho chamado "LockIcon" primeiro
        Image lockImage = null;
        Transform lockTransform = btn.transform.Find("LockIcon");
        if (lockTransform != null)
            lockImage = lockTransform.GetComponent<Image>();

        // Se não encontrar, tenta pegar a primeira Image filho que não pertence ao próprio Button (evita o background)
        if (lockImage == null)
        {
            foreach (var img in btn.GetComponentsInChildren<Image>(true))
            {
                if (img.gameObject == btn.gameObject)
                    continue;
                lockImage = img;
                break;
            }
        }

        if (lockImage == null)
            return;

        bool unlocked = true;
        try
        {
            unlocked = LevelManager.Instance.IsLevelUnlocked(phaseNumber);
        }
        catch
        {
            // Se LevelManager não estiver disponível, assume desbloqueado
            unlocked = true;
        }

        if (unlocked && lockOpenSprite != null)
            lockImage.sprite = lockOpenSprite;
        else if (!unlocked && lockClosedSprite != null)
            lockImage.sprite = lockClosedSprite;
    }

    private void OnBackClicked()
    {
        Debug.Log("Back button clicked - Returning to Main Menu");
        menuManager.ChangeState(MenuState.StateType.MainMenu);
    }
}
