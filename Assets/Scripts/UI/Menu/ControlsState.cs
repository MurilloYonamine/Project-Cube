using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using TMPro;
using UnityEngine.InputSystem;

/// <summary>
/// Gerencia o estado de Controles.
/// Exibe os controles do jogo e adapta dinamicamente baseado no último input usado.
/// </summary>
public partial class ControlsState : MenuState
{
    [SerializeField] private Button backButton;
    [SerializeField] private TextMeshProUGUI controlsText;
    [SerializeField] private TextMeshProUGUI controlsTextOutline;

    private bool isUsingGamepad = false;
    private string currentControlsText = "";

    private const string KEYBOARD_CONTROLS = 
        "W/Seta Cima = Pulo\n" +
        "S/Seta Baixo = Queda Rápida\n" +
        "A/Seta Esquerda = Esquerda\n" +
        "D/Seta Direita = Direita\n" +
        "R ou Ctrl+Z = Rewind";

    private const string GAMEPAD_CONTROLS = 
        "Analógico Esquerdo Cima = Pulo\n" +
        "Analógico Esquerdo Baixo = Queda Rápida\n" +
        "Analógico Esquerdo Esquerda = Esquerda\n" +
        "Analógico Esquerdo Direita = Direita";

    public override StateType GetStateType() => StateType.Controls;

    protected override void OnEnterComplete()
    {
        // Configura o botão de voltar
        if (backButton != null)
            backButton.onClick.AddListener(OnBackClicked);

        // Atualiza o texto dos controles
        UpdateControlsText();
    }

    protected override void OnExitComplete()
    {
        // Remove o listener
        if (backButton != null)
            backButton.onClick.RemoveListener(OnBackClicked);
    }

    public override void StateUpdate()
    {
        // Detecta qual dispositivo está sendo usado
        DetectActiveInputDevice();
        
        // Atualiza o texto se mudou o tipo de input
        UpdateControlsText();
    }

    private void DetectActiveInputDevice()
    {
        // Verifica se há algum input de gamepad
        if (Gamepad.current != null)
        {
            var gamepad = Gamepad.current;
            
            // Checa se qualquer botão do gamepad foi pressionado
            if (gamepad.leftStick.ReadValue().magnitude > 0.1f ||
                gamepad.rightStick.ReadValue().magnitude > 0.1f ||
                gamepad.buttonSouth.isPressed ||
                gamepad.buttonNorth.isPressed ||
                gamepad.buttonWest.isPressed ||
                gamepad.buttonEast.isPressed ||
                gamepad.leftShoulder.isPressed ||
                gamepad.rightShoulder.isPressed)
            {
                isUsingGamepad = true;
                return;
            }
        }

        // Verifica se há algum input de teclado ou mouse
        if (Keyboard.current != null)
        {
            if (Keyboard.current.anyKey.isPressed)
            {
                isUsingGamepad = false;
                return;
            }
        }

        if (Mouse.current != null)
        {
            if (Mouse.current.delta.ReadValue().magnitude > 0.1f ||
                Mouse.current.leftButton.isPressed ||
                Mouse.current.rightButton.isPressed)
            {
                isUsingGamepad = false;
                return;
            }
        }
    }

    private void UpdateControlsText()
    {
        string controlsString = GetControlsString();

        // Só atualiza se o texto mudou (otimização)
        if (controlsString != currentControlsText)
        {
            currentControlsText = controlsString;

            if (controlsText != null)
                controlsText.text = controlsString;

            if (controlsTextOutline != null)
                controlsTextOutline.text = controlsString;
        }
    }

    private string GetControlsString()
    {
        // Retorna baseado no último input detectado
        return isUsingGamepad ? GAMEPAD_CONTROLS : KEYBOARD_CONTROLS;
    }

    private void OnBackClicked()
    {
        if (AudioManager.Instance != null)
            AudioManager.Instance.PlaySound("Sounds/menu_escolha", null, 1f);
            
        menuManager.GoToPreviousState();
    }
}
