# Sistema de Menu e Pause - Guia de Implementação

## 📋 Resumo da Estrutura

### Scripts Criados:
1. **MenuManager.cs** - State Machine central que gerencia os 3 estados de menu
2. **MenuState.cs** - Classe base abstrata para todos os estados
3. **MainMenuState.cs** - Estado do Menu Principal
4. **SettingsState.cs** - Estado de Configurações
5. **PhaseSelectorState.cs** - Estado do Seletor de Fases
6. **PauseManager.cs** - Gerenciador de Pause (independente, sem State Machine)
7. **AudioManager.cs** - Gerenciador de áudio e música
8. **PauseUIController.cs** - Controlador de UI para o Pause (exemplo)

---

## 🎯 Setup no Inspector

### 1. MenuManager (Objeto vazio "MenuManager")

Atribua os seguintes componentes:

```
Câmeras:
  - Main Menu Camera
  - Settings Camera
  - Phase Selector Camera

Canvas Groups:
  - Main Menu Canvas Group
  - Settings Canvas Group
  - Phase Selector Canvas Group

Estados:
  - Main Menu State (script MainMenuState)
  - Settings State (script SettingsState)
  - Phase Selector State (script PhaseSelectorState)

Configurações:
  - Fade Duration: 0.3
  - Bounce Duration: 0.4
  - Bounce Scale: 1.1
```

### 2. MainMenuState Canvas

Estrutura esperada:
```
Canvas (Main Menu)
  ├─ CanvasGroup
  ├─ Button (Play)
  ├─ Button (Settings)
  ├─ Button (Phase Selector)
  ├─ Button (Exit)
  └─ [Outros elementos]
```

**No script MainMenuState:**
- Drag & drop cada botão nos campos serializados

### 3. SettingsState Canvas

Estrutura esperada:
```
Canvas (Settings)
  ├─ CanvasGroup
  ├─ Text (Title)
  ├─ Slider (Volume)
  ├─ Text (Volume Label)
  ├─ Button (Back)
  └─ [Outros elementos]
```

**No script SettingsState:**
- Volume Slider
- Volume Label (Text)
- Back Button

### 4. PhaseSelectorState Canvas

Estrutura esperada:
```
Canvas (Phase Selector)
  ├─ CanvasGroup
  ├─ Button (Phase 1)
  ├─ Button (Phase 2)
  ├─ Button (Phase 3)
  ├─ Button (Phase N)
  ├─ Button (Back)
  └─ [Outros elementos]
```

**No script PhaseSelectorState:**
- Phase Buttons (array com quantas fases tiver)
- Back Button

### 5. AudioManager (Objeto vazio "AudioManager")

Setup:
```
Componentes:
  ✓ AudioSource (para música)
  ✓ AudioManager (script)

Configurações:
  - Music Source: (AudioSource deste objeto)
  - Main Menu Music: (AudioClip)
  - Settings Music: (AudioClip ou deixar vazio)
  - Gamplay Music: (AudioClip)
  - Master Volume: 0.8 (padrão)
```

### 6. PauseManager (Objeto vazio "PauseManager")

Setup:
```
Componentes:
  ✓ PauseManager (script)

Configurações:
  - Pause Canvas Group: (Canvas Group do pause)
  - Pause Canvas: (Canvas do pause)
  - Fade Duration: 0.2
  - Time Scale When Paused: 0
  - Time Scale When Playing: 1
```

### 7. Pause Canvas

Estrutura esperada:
```
Canvas (Pause)
  ├─ Image (Background com alpha)
  ├─ CanvasGroup
  ├─ Panel
  │  ├─ Text (Title "PAUSED")
  │  ├─ Button (Resume)
  │  ├─ Button (Settings)
  │  ├─ Button (Main Menu)
  │  └─ Button (Quit)
  └─ [Outros elementos]
```

**No PauseManager:**
- Pause Canvas Group: Drag o CanvasGroup do Pause Canvas
- Pause Canvas: Drag o Canvas do pause

**No PauseUIController:**
- Resume Button
- Settings Button (opcional)
- Main Menu Button
- Quit Button

---

## 🎮 Como Usar

### Iniciando o Menu
1. Crie uma cena chamada "MainMenu"
2. Adicione os GameObjects de MenuManager e AudioManager
3. Configure conforme instruções acima
4. Execute a cena

### Pausando o Jogo During Gameplay
```csharp
// Pausa
PauseManager.Instance.Pause();

// Retoma
PauseManager.Instance.Resume();

// Verifica se está pausado
if (PauseManager.Instance.IsPaused())
{
    // Fazer algo
}

// Reseta timeScale (ao carregar cena)
PauseManager.Instance.ResetTimeScale();
```

### Controlando o AudioManager
```csharp
// Toca música do menu
AudioManager.Instance.PlayMainMenuMusic();

// Toca música de gameplay
AudioManager.Instance.PlayGameplayMusic();

// Define volume (0 a 1)
AudioManager.Instance.SetVolume(0.5f);

// Obtém volume atual
float currentVolume = AudioManager.Instance.GetVolume();

// Toca efeito sonoro
AudioManager.Instance.PlaySFX(mySFXClip, 1f);
```

### State Machine do Menu
```csharp
// Mudar para Settings
MenuManager.Instance.ChangeState(MenuState.StateType.Settings);

// Mudar para Phase Selector
MenuManager.Instance.ChangeState(MenuState.StateType.PhaseSelector);

// Mudar para Main Menu
MenuManager.Instance.ChangeState(MenuState.StateType.MainMenu);

// Verificar estado atual
MenuState currentState = MenuManager.Instance.GetCurrentState();

// Verificar se está em transição
if (MenuManager.Instance.IsTransitioning())
{
    // Aguardar transição
}
```

---

## 🎨 Personalizações

### Modificar Duração de Fade
No MenuManager, altere `fadeDuration`:
```csharp
[SerializeField] private float fadeDuration = 0.3f; // Padrão: 0.3s
```

### Modificar Efeito de Bounce
No MenuManager:
```csharp
[SerializeField] private float bounceDuration = 0.4f; // Duração do bounce
[SerializeField] private float bounceScale = 1.1f;    // Escala máxima (1.1 = 10% maior)
```

### Adicionar Novo Estado
1. Crie uma nova classe herdando de `MenuState`:
```csharp
public class MyNewState : MenuState
{
    protected override void OnEnterComplete()
    {
        // Lógica quando entra
    }

    protected override void OnExitComplete()
    {
        // Lógica quando sai
    }
}
```

2. Adicione um novo tipo no enum de MenuState
3. Configure no MenuManager

---

## ⚙️ Detalhes Técnicos

### State Machine (MenuManager)
- **Padrão:** State Pattern com Dictionary
- **Transições:** Gerenciadas por corrotinas
- **Segurança:** Impede múltiplas transições simultâneas
- **Singleton:** Acessível globalmente via `MenuManager.Instance`

### Pause System (PauseManager)
- **Padrão:** Simple Manager (sem State Machine)
- **Time Scale:** Controla `Time.timeScale` para pausar
- **CanvasGroup:** Controla alpha e raycast target
- **Singleton:** Acessível globalmente via `PauseManager.Instance`
- **Unscaled Time:** Usa `Time.unscaledDeltaTime` para funções durante pause

### Audio Manager
- **Padrão:** Singleton
- **Fade:** Transições suaves entre músicas
- **Volume:** Controle centralizado
- **Loop:** Música em loop por padrão

---

## 🐛 Troubleshooting

### Sons/Música não toca
- Verifique se os AudioClips estão atribuídos no Inspector
- Verifique o volume (Master Volume > 0)
- Verifique se o AudioSource está ativo

### Buttons não respondem
- Certifique-se de que GraphicRaycaster está no Canvas
- Verifique se o EventSystem existe na cena
- Verifique se o Canvas Group tem interactable = true

### Transições erráticas
- Verifique se há múltiplas instâncias de MenuManager ou PauseManager
- Sincronize as durações de fade e bounce

### Pause não funciona
- Certifique-se de que o Pause Canvas está desabilitado inicialmente
- Verifique se o CanvasGroup foi atribuído
- Teste com ESC se o PauseManager está escutando input

---

## 📝 Notas

- Todos os scripts usam boas práticas e comentários em português
- O sistema é totalmente extensível e modular
- Implementação sem dependências externas (sem DOTween necessário)
- Singletons com DontDestroyOnLoad para persistência entre cenas
- Uso de corrotinas para animações suaves

---

**Última atualização:** Dezembro 2025
