using UnityEngine;
using PROJECT_CUBE.PLAYER;

namespace PROJECT_CUBE.LEVEL_GENERATION {
    /// <summary>
    /// Componente que gerencia a instanciação do player
    /// </summary>
    public class LevelGeneratorPlayerManager {
        private GameObject _spawnedPlayer;
        private LevelGeneratorConfig _config;

        public GameObject SpawnedPlayer => _spawnedPlayer;

        public void Initialize(LevelGeneratorConfig config) {
            _config = config;
        }

        /// <summary>
        /// Spawna o player na posição definida no blueprint
        /// </summary>
        public void SpawnPlayer(LevelBlueprint blueprint) {
            if (_config.playerPrefab == null) {
                Debug.LogWarning("LevelGenerator: No player prefab assigned!");
                return;
            }

            if (!blueprint.EnablePlayerSpawn) {
                if (_config.showDebugInfo)
                    Debug.Log("LevelGenerator: Player spawn disabled for this level");
                return;
            }

            DespawnPlayer();

            // Calcula posição
            Vector3 spawnPos = blueprint.GridToWorldPosition(blueprint.PlayerSpawnPosition);
            spawnPos += _config.globalOffset;

            // Instancia player
            _spawnedPlayer = Object.Instantiate(
                _config.playerPrefab,
                spawnPos,
                Quaternion.identity,
                _config.playerParent
            );
            _spawnedPlayer.name = "Player";

            if (_config.showDebugInfo)
                Debug.Log($"Player spawned at grid position {blueprint.PlayerSpawnPosition}");
        }

        /// <summary>
        /// Remove o player da cena
        /// </summary>
        public void DespawnPlayer() {
            if (_spawnedPlayer != null) {
                if (Application.isPlaying)
                    Object.Destroy(_spawnedPlayer);
                else
                    Object.DestroyImmediate(_spawnedPlayer);
                _spawnedPlayer = null;
            }
        }

        public bool ShouldAutoSpawnPlayer(LevelBlueprint blueprint) {
            return _config.autoSpawnPlayer && blueprint.EnablePlayerSpawn;
        }
    }
}
