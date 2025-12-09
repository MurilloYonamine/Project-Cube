# Player Components

## Overview
Este diretório contém os componentes que gerenciam o comportamento do jogador. Cada componente é uma classe que herda de `PlayerComponent` e implementa funcionalidades específicas do sistema de movimento e input.

## Estrutura

### PlayerComponent (Base Class)
Classe abstrata que define a interface padrão para todos os componentes do jogador.

**Métodos disponíveis:**
- `InitializeComponent()` - Inicializa o componente com referência ao PlayerController
- `AwakeComponent()` - Chamado no Awake do MonoBehaviour
- `StartComponent()` - Chamado no Start do MonoBehaviour
- `UpdateComponent()` - Chamado no Update do MonoBehaviour
- `FixedUpdateComponent()` - Chamado no FixedUpdate do MonoBehaviour
- `LateUpdateComponent()` - Chamado no LateUpdate do MonoBehaviour
- `OnEnableComponent()` - Chamado quando o componente é ativado
- `OnDisableComponent()` - Chamado quando o componente é desativado
- `OnDestroyComponent()` - Chamado quando o componente é destruído
- `OnDrawGizmosComponent()` - Chamado para desenhar Gizmos de debug

---

## Componentes Implementados

### PlayerInputHandler
**Arquivo:** `PlayerInputHandler.cs`

Responsável pelo gerenciamento de input do jogador usando o novo Input System.

**Funcionalidades:**
- Captura inputs de movimento (W/A/S/D ou Analógico)
- Dispara eventos quando há movimento
- Gerencia o ciclo de vida do sistema de input

**Eventos:**
- `OnMovementInput` - Disparado quando há input de movimento (Vector2)

**Método de Inicialização:**
```csharp
AwakeComponent() {
    _inputControls = new InputControls();
    _moveAction = _inputControls.Player.Move;
    _moveAction.performed += OnMovePerformed;
}
```

---

### PlayerMovement
**Arquivo:** `PlayerMovement.cs`

Responsável pelo movimento e física do jogador.

**Funcionalidades:**
- Movimento horizontal automático
- Sistema de pulo com gravidade customizável
- Queda acelerada
- Detecção de solo via Raycast
- Restrições de rotação para manter o cubo sempre na posição correta

**Comportamento:**
- W ou Seta Para Cima - Pulo (se estiver no solo)
- S ou Seta Para Baixo - Queda acelerada (se estiver no ar)
- Movimento automático para frente constantemente

**Constraints do Rigidbody:**
- Congelada rotação nos eixos X e Z
- Permite rotação no eixo Y

**Debug:**
- Raycast visual via Gizmos (Verde = no solo, Vermelho = no ar)
- Logs de input via PlayerDebugManager

---

## Como Adicionar um Novo Componente

1. Crie uma nova classe que herde de `PlayerComponent`
2. Implemente os métodos desejados (não é necessário implementar todos)
3. Adicione a instância no array `_playerComponents` do `PlayerController`

**Exemplo:**
```csharp
[Serializable]
public class MeuComponente : PlayerComponent {
    public override void InitializeComponent(PlayerController playerController) {
        base.InitializeComponent(playerController);
        // Inicialize suas referências aqui
    }

    public override void UpdateComponent() {
        // Lógica a cada frame
    }
}
```

---

## Arquitetura

```
PlayerController (MonoBehaviour)
├── PlayerInputHandler (Component)
│   └── Gerencia Input
├── PlayerMovement (Component)
│   └── Gerencia Movimento e Física
└── Mais componentes...
```

Cada componente é inicializado no `Awake()` do `PlayerController` e todos os métodos são chamados de forma sincronizada através do sistema de `ForEachComponent()`.

---

## Notas Importantes

- Os componentes são instanciados via `[SerializeField]` no Inspector
- O sistema de eventos desacoplado permite comunicação entre componentes
- Todos os componentes têm acesso ao `PlayerController` através de `_playerController`
