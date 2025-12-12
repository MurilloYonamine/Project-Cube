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
        {
            Debug.Log("[PhaseSelectorState] Adding listener to back button");
            backButton.onClick.AddListener(OnBackClicked);
        }
        else
        {
            Debug.LogError("[PhaseSelectorState] backButton is NULL in Inspector!");
        }

        // Configura os botões de fase
        for (int i = 0; i < phaseButtons.Length; i++)
        {
            int phaseIndex = i; // Captura para closure
            int phaseNumber = phaseIndex;
            
            phaseButtons[i].onClick.AddListener(() => OnPhaseSelected(phaseNumber));
            
            // Verifica se o nível está desbloqueado
            bool isUnlocked = LevelManager.Instance.IsLevelUnlocked(phaseNumber);
            phaseButtons[i].interactable = isUnlocked;
            
            // Atualiza o ícone de cadeado (se houver) de acordo com o progresso
            TryApplyLockSprite(phaseButtons[i], phaseNumber);
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
        
        // Reseta o timeScale se estiver vindo do pause
        if (PauseManager.Instance != null && PauseManager.Instance.IsPaused())
        {
            PauseManager.Instance.ResetTimeScale();
        }
        
        LevelManager.Instance.LoadLevel(phaseNumber);
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
        Debug.Log("[PhaseSelectorState] Back button clicked");
        
        if (menuManager == null)
        {
            Debug.LogError("[PhaseSelectorState] menuManager is null!");
            return;
        }
        
        Debug.Log("[PhaseSelectorState] Calling GoToPreviousState");
        menuManager.GoToPreviousState();
    }
}
