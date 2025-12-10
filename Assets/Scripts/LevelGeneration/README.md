# Level Generator System (3D)

Sistema modular para geração de níveis em 3D com grid 2D (X, Y).

## Arquitetura

### Data Layer (`Data/`)
- **LevelBlueprint.cs** - ScriptableObject com dados do nível (grid, plataformas, spawn)
- **PlatformPalette.cs** - Mapeia tipos de plataforma para prefabs

### Components (`Components/`)
- **LevelGeneratorConfig.cs** - Configurações centralizadas
- **LevelGeneratorConstructor.cs** - Lógica de construção de níveis
- **LevelGeneratorPlayerManager.cs** - Gerencia spawn do player
- **LevelBuilder.cs** - Orquestra todo o processo

### Managers (`Managers/`)
- **LevelManager.cs** - Gerencia múltiplos níveis e transições

## Como Usar

### 1. Criar uma Paleta de Plataformas

```
Assets/ > Clique com botão direito > Level Generation > Platform Palette
```

Configure os prefabs para Platform e Player Spawn.

### 2. Criar um Blueprint de Nível

```
Assets/ > Clique com botão direito > Level Generation > Blueprint
```

Configure:
- Grid Size (X, Y)
- Platform Spacing
- Plataformas (posições e tipos)
- Player Spawn Position

### 3. Adicionar LevelBuilder à Cena

- Crie um GameObject vazio
- Adicione o script `LevelBuilder`
- Atribua a Palette e o Blueprint
- Configure as opções em `LevelGeneratorConfig`

### 4. (Opcional) Usar LevelManager

Se tiver múltiplos níveis:

- Adicione `LevelManager` ao GameObject do LevelBuilder
- Atribua os Blueprints na array
- Configure as opções de geração

## Fluxo de Execução

```
LevelManager.LoadLevel(index)
    ↓
LevelBuilder.BuildLevel(blueprint)
    ↓
LevelGeneratorConstructor.BuildLevel()
    - Síncrono: Instancia todos os tiles de uma vez
    - Assíncrono: Instancia em chunks por frame
    ↓
LevelGeneratorPlayerManager.SpawnPlayer()
    ↓
Eventos: OnLevelBuildCompleted
```

## Configuração Assíncrona

Para evitar lag em níveis grandes:

1. `LevelGeneratorConfig.buildAsync = true`
2. Ajuste `platformsPerFrame` conforme necessário

## Modularidade

Cada componente é independente e reutilizável:

- `LevelGeneratorConstructor` pode ser usado isoladamente
- `LevelGeneratorPlayerManager` pode ser usado em outro contexto
- `LevelBlueprint` é um ScriptableObject puro

Todos os componentes usam **System.Action** para eventos.
