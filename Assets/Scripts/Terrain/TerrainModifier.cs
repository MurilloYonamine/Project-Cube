using PROJECT_CUBE.TERRAIN;

namespace PROJECT_CUBE.TERRAIN {
    public class TerrainModifier {
        public const float BLUE_TERRAIN_SPEED_MULTIPLIER = 1.5f;
        public const float RED_TERRAIN_SPEED_MULTIPLIER = 0.5f;
        public const float ORANGE_BLOCK_JUMP_MULTIPLIER = 1.5f;

        private TerrainType _currentTerrainType = TerrainType.None;
        public float CurrentSpeedModifier { get; private set; } = 1f;
        public float CurrentJumpModifier { get; private set; } = 1f;

        public void OnTerrainChange(TerrainType terrainType) {
            _currentTerrainType = terrainType;

            CurrentSpeedModifier = terrainType switch {
                TerrainType.BlueTerrain => BLUE_TERRAIN_SPEED_MULTIPLIER,
                TerrainType.RedTerrain => RED_TERRAIN_SPEED_MULTIPLIER,
                _ => 1f
            };

            CurrentJumpModifier = terrainType == TerrainType.OrangeBlock ? ORANGE_BLOCK_JUMP_MULTIPLIER : 1f;

            string terrainName = terrainType switch {
                TerrainType.BlueTerrain => "Chão Azul (+50% velocidade)",
                TerrainType.RedTerrain => "Chão Vermelho (-50% velocidade)",
                TerrainType.OrangeBlock => "Bloco Laranja (+50% salto)",
                _ => "Terreno Normal"
            };

            PlayerDebugManager.Instance?.AddLine($"Terreno: {terrainName}", nameof(TerrainModifier));
        }
    }
}
