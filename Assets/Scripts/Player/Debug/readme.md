# Debug System

## Overview
Sistema de debug centralizado para exibir informações de depuração na tela durante a execução do jogo.

## PlayerDebugManager

**Arquivo:** `PlayerDebugManager.cs`

Gerenciador singleton responsável por exibir mensagens de debug em tempo real na tela.

### Características:
- **Singleton Pattern** - Apenas uma instância em toda a cena
- **TextMeshPro Integration** - Usa TextMeshPro para renderizar texto na UI
- **Sistema de Slots** - Cada classe pode ter sua própria linha de debug
- **Atualização em Tempo Real** - Mensagens são atualizadas a cada frame

### Como Usar

#### 1. Setup Inicial
No seu script, obtenha a instância do `PlayerDebugManager`:

```csharp
PlayerDebugManager.Instance.AddLine("Sua mensagem", "Identificador");
```

#### 2. Adicionar Linhas de Debug
```csharp
// Exemplo do PlayerMovement
PlayerDebugManager.Instance.AddLine($"Está no chão: {_isGrounded}", "GroundCheck");
```

#### 3. Comportamento de Slots
- Primeira chamada com um identificador → **cria um novo slot** (nova linha)
- Chamadas subsequentes com o mesmo identificador → **atualiza a linha existente**

**Exemplo:**
```csharp
// Frame 1
AddLine("Velocidade: 5", "Movement");      // Cria slot 0
AddLine("Input: (1, 0)", "Input");         // Cria slot 1

// Frame 2
AddLine("Velocidade: 7", "Movement");      // Atualiza slot 0
AddLine("Input: (0, 1)", "Input");         // Atualiza slot 1

// Resultado na tela:
// Velocidade: 7
// Input: (0, 1)
```

### Métodos Públicos

#### `AddLine(string message, string classIdentifier)`
Adiciona ou atualiza uma linha de debug.

**Parâmetros:**
- `message` - Texto a ser exibido
- `classIdentifier` - Identificador único (geralmente nome da classe)

**Exemplo:**
```csharp
PlayerDebugManager.Instance.AddLine($"Input: Para cima ({movementInput.y})", nameof(PlayerMovement));
```

#### `ClearAll()`
Limpa todas as mensagens de debug.

**Exemplo:**
```csharp
PlayerDebugManager.Instance.ClearAll();
```

### Setup no Inspector

1. Crie um Canvas (UI) na cena
2. Crie um TextMeshPro Text element no Canvas
3. Selecione o GameObject com o `PlayerDebugManager`
4. Arraste o elemento TextMeshPro no campo `_debugText`

### Estrutura de Dados

```csharp
Dictionary<string, int> _debugSlots       // Mapeia identificador → índice do slot
List<string> _debugMessages                // Lista de mensagens (uma por linha)
int _nextSlotIndex                         // Próximo índice disponível
```

### Exemplo de Uso Completo

```csharp
public class MeuComponente : PlayerComponent {
    public override void UpdateComponent() {
        float velocidade = CalcularVelocidade();
        bool emMovimento = velocidade > 0;
        
        // Debug em tempo real
        PlayerDebugManager.Instance.AddLine(
            $"Velocidade: {velocidade:F2} | Em movimento: {emMovimento}",
            nameof(MeuComponente)
        );
    }
}
```

### Notas Importantes

- 📊 Ideal para monitorar valores em tempo real durante desenvolvimento
- 🧹 Lembre de remover ou comentar chamadas de debug antes de fazer build final
