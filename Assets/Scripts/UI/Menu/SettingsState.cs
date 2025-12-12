using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Gerencia o estado de Configurações.
/// Controla volume de música e contém botões de voltar ao menu.
/// </summary>
public partial class SettingsState : MenuState
{
    [Header("Sliders")]
    [SerializeField] private Slider masterSlider;
    [SerializeField] private Slider musicSlider;
    [SerializeField] private Slider sfxSlider;

    [Header("Labels")]
    [SerializeField] private TextMeshProUGUI masterLabel;
    [SerializeField] private TextMeshProUGUI musicLabel;
    [SerializeField] private TextMeshProUGUI sfxLabel;

    [SerializeField] private Button backButton;
    public override StateType GetStateType() => StateType.Settings;

    protected override void OnEnterComplete()
    {
        // Configura os sliders
        if (masterSlider != null)
        {
            // Remove listeners to avoid duplicates
            masterSlider.onValueChanged.RemoveAllListeners();
            float masterVal = AudioManager.Instance != null ? AudioManager.Instance.GetMasterSliderValue() : Mathf.Clamp01(masterSlider.value);
            masterSlider.SetValueWithoutNotify(masterVal);
            masterSlider.onValueChanged.AddListener(OnMasterChanged);
        }

        if (musicSlider != null)
        {
            musicSlider.onValueChanged.RemoveAllListeners();
            float musicVal = AudioManager.Instance != null ? AudioManager.Instance.GetMusicSliderValue() : Mathf.Clamp01(musicSlider.value);
            musicSlider.SetValueWithoutNotify(musicVal);
            musicSlider.onValueChanged.AddListener(OnMusicChanged);
        }

        if (sfxSlider != null)
        {
            sfxSlider.onValueChanged.RemoveAllListeners();
            float sfxVal = AudioManager.Instance != null ? AudioManager.Instance.GetSFXSliderValue() : Mathf.Clamp01(sfxSlider.value);
            sfxSlider.SetValueWithoutNotify(sfxVal);
            sfxSlider.onValueChanged.AddListener(OnSFXChanged);
        }

        // Configura o botão voltar
        if (backButton != null)
        {
            Debug.Log("[SettingsState] Adding listener to back button");
            backButton.onClick.AddListener(OnBackClicked);
        }
        else
        {
            Debug.LogError("[SettingsState] backButton is NULL in Inspector!");
        }

        UpdateVolumeLabels();
    }

    protected override void OnExitComplete()
    {
        // Remove os listeners
        if (masterSlider != null)
            masterSlider.onValueChanged.RemoveListener(OnMasterChanged);

        if (musicSlider != null)
            musicSlider.onValueChanged.RemoveListener(OnMusicChanged);

        if (sfxSlider != null)
            sfxSlider.onValueChanged.RemoveListener(OnSFXChanged);

        if (backButton != null)
            backButton.onClick.RemoveListener(OnBackClicked);
    }

    private void OnMasterChanged(float newVolume)
    {
        AudioManager.Instance.SetMasterVolume(newVolume, false);
        UpdateVolumeLabels();
    }

    private void OnMusicChanged(float newVolume)
    {
        AudioManager.Instance.SetMusicVolume(newVolume, false);
        UpdateVolumeLabels();
    }

    private void OnSFXChanged(float newVolume)
    {
        AudioManager.Instance.SetSFXVolume(newVolume, false);
        UpdateVolumeLabels();
    }

    private void UpdateVolumeLabels()
    {
        if (masterLabel != null && masterSlider != null)
        {
            int percent = Mathf.RoundToInt(masterSlider.value * 100f);
            masterLabel.text = $"Master: {percent}%";
        }

        if (musicLabel != null && musicSlider != null)
        {
            int percent = Mathf.RoundToInt(musicSlider.value * 100f);
            musicLabel.text = $"Music: {percent}%";
        }

        if (sfxLabel != null && sfxSlider != null)
        {
            int percent = Mathf.RoundToInt(sfxSlider.value * 100f);
            sfxLabel.text = $"SFX: {percent}%";
        }
    }

    private void OnBackClicked()
    {
        if (AudioManager.Instance != null)
            AudioManager.Instance.PlaySound("Sounds/menu_escolha", volume: 1f);
            
        Debug.Log("[SettingsState] Back button clicked");
        
        if (menuManager == null)
        {
            Debug.LogError("[SettingsState] menuManager is null!");
            return;
        }
        
        Debug.Log("[SettingsState] Calling GoToPreviousState");
        menuManager.GoToPreviousState();
    }
}
