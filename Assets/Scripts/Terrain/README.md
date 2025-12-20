# Terrain System

## Overview
O sistema de Terrain gerencia toda a lógica relacionada ao terreno, incluindo blocos modificáveis e portais infinitos.

## Estrutura de Diretórios

```
Terrain/
├── TerrainBlock.cs (Bloco individual)
├── TerrainType.cs (Definição de tipos de terreno)
├── TerrainModifier.cs (Modificador de terreno)
├── ElevatorManager.cs (Gerenciador de Portais)
├── PortalProjectile.cs (Script do Projétil nos Portais)
└── readme.md (Este arquivo)
```

---

## TerrainBlock

**Arquivo:** `TerrainBlock.cs`

Representa um bloco individual no terreno que pode ser modificado.

### Funcionalidades:
- Gerencia o tipo de terreno do bloco
- Suporta modificações dinâmicas
- Renderização e física do bloco

---

## TerrainType

**Arquivo:** `TerrainType.cs`

Define os diferentes tipos de terreno disponíveis no jogo.

### Funcionalidades:
- Enumeração de tipos de terreno
- Propriedades específicas de cada tipo
- Configurações visuais e físicas

---

## TerrainModifier

**Arquivo:** `TerrainModifier.cs`

Sistema que permite modificar blocos de terreno em tempo real.

### Funcionalidades:
- Destrói blocos
- Cria novos blocos
- Modifica propriedades de blocos existentes

---

## ElevatorManager

**Arquivo:** `ElevatorManager.cs`

Gerenciador de portais infinitos que simula a queda/movimento do jogo Portal.

### Funcionalidades:
- Controla movimento infinito entre dois portais
- Suporta movimento **Vertical** (queda) e **Horizontal** (movimento lateral)
- Teletransporte automático ao atingir o portal destino
- Detecção por trigger e por posição

### Configurações:

#### Modo Vertical (Queda Infinita)
```
Porta A (Topo) ↓
     ↓
     ↓ (queda)
     ↓
Porta B (Base) → Teletransporta para A
```

#### Modo Horizontal (Movimento Lateral)
```
Porta A → Movimento → Porta B → Teletransporta para A
```

### Propriedades Configuráveis:

| Propriedade | Descrição | Padrão |
|---|---|---|
| `_direction` | Escolhe entre Vertical ou Horizontal | Vertical |
| `_moveSpeed` | Velocidade de queda/movimento | 5f |
| `_teleportOffset` | Offset ao teletransportar | 0.1f |

### Enum PortalDirection:
```csharp
public enum PortalDirection 
{ 
    Vertical,    // Queda infinita (estilo Portal do jogo)
    Horizontal   // Movimento lateral infinito
}
```

### Métodos Públicos:

#### `OnProjectileTrigger(Collider collision)`
Chamado quando o projétil colide com um portal (trigger).
- **Parâmetro:** `collision` - O Collider do portal colidido
- **Comportamento:** Teletransporta o projétil para o portal oposto

---

## PortalProjectile

**Arquivo:** `PortalProjectile.cs`

Script anexado ao projétil que viaja pelos portais.

### Funcionalidades:
- Detecta colisão com triggers dos portais
- Comunica com `ElevatorManager` para teletransporte
- Controla o comportamento do projétil

### Configurações:
- Requer referência ao `ElevatorManager` no Inspector

---

## Como Usar

### Configurar Portais Verticais (Queda Infinita):

1. **Criar GameObjects dos Portais:**
   - Crie dois GameObjects vazios (ex: "Portal A" e "Portal B")
   - Adicione um Collider (Box/Sphere) com **"Is Trigger" ativado**
   - Posicione um acima do outro

2. **Configurar o Gerenciador:**
   - Crie um GameObject vazio (ex: "ElevatorManager")
   - Adicione o script `ElevatorManager.cs`
   - Atribua os Portais A e B nos slots corretos
   - Defina `_direction = Vertical`

3. **Configurar o Projétil:**
   - Crie um GameObject para o projétil (um cubo ou esfera)
   - Adicione um **Collider (não trigger)**
   - Adicione o script `PortalProjectile.cs`
   - Atribua a referência do `ElevatorManager`
   - Atribua o Projétil no `ElevatorManager`

4. **Ajustar Velocidade:**
   - Modifique `_moveSpeed` para controlar a velocidade de queda
   - Valores recomendados: 3-10

---

### Configurar Portais Horizontais (Movimento Lateral):

1. **Criar GameObjects dos Portais:**
   - Crie dois GameObjects vazios (ex: "Portal A" e "Portal B")
   - Adicione um Collider com **"Is Trigger" ativado**
   - Posicione um ao lado do outro

2. **Configurar o Gerenciador:**
   - Use o mesmo `ElevatorManager`
   - Defina `_direction = Horizontal`

3. **Resto igual ao processo vertical**

---

## Arquitetura do Sistema

```
ElevatorManager
    ├── Gerencia Portais (A e B)
    ├── Controla movimento do projétil
    └── Detecta colisões com portais
    
PortalProjectile (no Projétil)
    ├── Detecta OnTriggerEnter
    └── Comunica com ElevatorManager
```

---

## Ciclo de Vida do Projétil

```
Start() → Posicionado no Portal A
       ↓
Update() → Movimento (queda/horizontal)
       ↓
Colide com Portal B → OnTriggerEnter()
       ↓
Teletransporta para Portal A
       ↓
Volta ao Update() e repete
```

---

## Gizmos de Debug

O `ElevatorManager` desenha gizmos para ajudar na visualização:
- **Verde:** Linha conectando os dois portais
- **Amarelo:** Esferas de detecção dos portais
- **Azul:** Posições exatas dos portais

Visível no Scene view durante desenvolvimento.

---

## Notas de Desenvolvimento

- O sistema usa **Transform** para movimento (sem Rigidbody necessário)
- Detecção de colisão via **Trigger Colliders**
- Suporta expansão para mais direções (diagonais, etc.)
- O delay de 0.2s entre teletransportes evita múltiplos teletransportes consecutivos