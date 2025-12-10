using PROJECT_CUBE.TERRAIN;

namespace PROJECT_CUBE.PLAYER.COMPONENTS {
    public class TerrainModifierManager {
        public float BlueTerrainSpeedMultiplier = 1.5f;
        public float RedTerrainSpeedMultiplier = 0.5f;
        public float OrangeBlockJumpMultiplier = 1.5f;

        private TerrainType _currentTerrainType = TerrainType.None;
        public float CurrentSpeedModifier { get; private set; } = 1f;
        public float CurrentJumpModifier { get; private set; } = 1f;

        public void OnTerrainChange(TerrainType terrainType) {
            _currentTerrainType = terrainType;

            CurrentSpeedModifier = terrainType switch {
                TerrainType.BlueTerrain => BlueTerrainSpeedMultiplier,
                TerrainType.RedTerrain => RedTerrainSpeedMultiplier,
                _ => 1f
            };

            CurrentJumpModifier = terrainType == TerrainType.OrangeBlock ? OrangeBlockJumpMultiplier : 1f;

            string terrainName = terrainType switch {
                TerrainType.BlueTerrain => "Chão Azul (+50% velocidade)",
                TerrainType.RedTerrain => "Chão Vermelho (-50% velocidade)",
                TerrainType.OrangeBlock => "Bloco Laranja (+50% salto)",
                _ => "Terreno Normal"
            };

            PlayerDebugManager.Instance?.AddLine($"Terreno: {terrainName}", nameof(TerrainModifierManager));
        }
    }
}
