## 🚀 QUICK START - Menu e Pause System

### Arquivos Criados:
```
Assets/Scripts/UI/Menu/
├── MenuManager.cs              (State Machine do menu)
├── MenuState.cs                (Classe base dos estados)
├── MainMenuState.cs            (Menu Principal)
├── SettingsState.cs            (Configurações + Volume)
├── PhaseSelectorState.cs       (Seletor de Fases)
├── PauseManager.cs             (Gerenciador de Pause)
├── AudioManager.cs             (Gerenciador de Áudio)
├── PauseUIController.cs        (Botões do Pause - Exemplo)
├── MenuEffects.cs              (Efeitos visuais reutilizáveis)
├── MenuSystemExamples.cs       (Exemplos de extensão)
├── MenuDebugger.cs             (Ferramentas de debug)
└── README.md                   (Documentação completa)
```

---

## ⚡ Setup Mínimo em 3 Passos

### 1️⃣ Crie a Hierarquia no Editor:
```
Canvas (Main Menu)
├─ Panel / Image
│  └─ Buttons: Play, Settings, Exit, etc.

Canvas (Settings)
├─ Panel / Image
│  └─ Slider (Volume)
│  └─ Buttons: Back

Canvas (Phase Selector)
├─ Panel / Image
│  └─ Buttons: Phase 1,2,3... Back

Canvas (Pause) [Desativado inicialmente]
├─ Panel / Image
│  └─ Buttons: Resume, Settings, Menu, Quit

GameObject "MenuManager" (vazio)
├─ Script: MenuManager.cs
├─ Atribua as 3 Cameras
├─ Atribua os 3 CanvasGroups
├─ Atribua os 3 Estados

GameObject "AudioManager" (vazio)
├─ Script: AudioManager.cs
├─ AudioSource (componente)

GameObject "PauseManager" (vazio)
├─ Script: PauseManager.cs
├─ Atribua o Pause Canvas
├─ Atribua o Pause CanvasGroup
```

### 2️⃣ Configure os Botões:
- MainMenuState: Drag buttons (Play, Settings, Exit)
- SettingsState: Drag slider e back button
- PhaseSelectorState: Drag phase buttons e back button
- PauseUIController: Drag pause buttons

### 3️⃣ Pronto! Use assim:
```csharp
// Menu
MenuManager.Instance.ChangeState(MenuState.StateType.Settings);

// Pause
PauseManager.Instance.Pause();
PauseManager.Instance.Resume();

// Áudio
AudioManager.Instance.SetVolume(0.8f);
AudioManager.Instance.PlayMainMenuMusic();
```

---

## 🎮 Atalhos de Debug (MenuDebugger.cs)
- **F1**: Mostrar informações do sistema no console
- **F2**: Toggle Pause (liga/desliga)
- **F3**: Ir para Main Menu
- **F4**: Ir para Settings
- **F5**: Ir para Phase Selector

---

## 📊 State Machine Flow
```
        ┌─────────────┐
        │  MainMenu   │
        └────────┬────┘
                 │
        ┌────────┴─────────┐
        │                  │
   ┌────▼─────┐      ┌─────▼──────────┐
   │ Settings │      │ PhaseSelector  │
   └──────────┘      └────────────────┘
        │                  │
        └────────┬─────────┘
                 │
           Back to MainMenu
```

---

## 🎨 Efeitos Disponíveis
```csharp
// Use em seus estados customizados:
MenuEffects.ShakeEffect(transform, 0.2f, 0.1f);
MenuEffects.PulseEffect(transform, 1f, 0.95f, 1.05f);
MenuEffects.SlideEffect(transform, from, to, 0.5f);
MenuEffects.RotateEffect(transform, 1f, 360f, false);
MenuEffects.PopInEffect(transform, 0.4f);
MenuEffects.WobbleEffect(transform, 0.3f, 10f);
```

---

## 🔧 Customizações Comuns

### Mudar duração de fade:
```csharp
[SerializeField] private float fadeDuration = 0.5f; // Padrão 0.3f
```

### Mudar intensidade do bounce:
```csharp
[SerializeField] private float bounceScale = 1.2f; // Padrão 1.1f
```

### Adicionar som ao clicar:
```csharp
AudioManager.Instance.PlaySFX(clickSound, 1f);
```

### Mudar música por estado:
```csharp
protected override void OnEnterComplete()
{
    AudioManager.Instance.PlayGameplayMusic();
}
```

---

## ✅ Checklist de Setup

- [ ] 3 Cameras criadas (Main, Settings, Selector)
- [ ] 3 Canvases com CanvasGroups
- [ ] MenuManager com todas as referências
- [ ] Estados (Main, Settings, Selector) configurados
- [ ] AudioManager com AudioSource
- [ ] Pause Canvas desativado inicialmente
- [ ] PauseManager com Canvas e CanvasGroup
- [ ] Botões com listeners configurados
- [ ] AudioClips atribuídos (opcional)

---

## 🐛 Problemas Comuns

| Problema | Solução |
|----------|---------|
| Botões não respondem | Verifique se CanvasGroup tem `interactable = true` |
| Fade não funciona | Certifique-se de que CanvasGroup existe |
| Música não toca | Atribua AudioClips e verifique volume > 0 |
| Transição bugada | Não faça 2 ChangeState() rapidamente |
| Pause não pausa | Verifique se Pause Canvas está no setup |

---

## 📚 Recursos Adicionais

- `README.md` - Documentação completa
- `MenuSystemExamples.cs` - 10 exemplos práticos
- `MenuDebugger.cs` - Ferramentas de debug
- `MenuEffects.cs` - Efeitos visuais reutilizáveis

---

**Tudo pronto para começar!** 🎉
