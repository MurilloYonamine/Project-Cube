using UnityEngine;

namespace PROJECT_CUBE.LEVEL_GENERATION {
    /// <summary>
    /// Exemplo de setup do Level Generator para uma cena
    /// </summary>
    public class LevelGeneratorExample : MonoBehaviour {
        [Header("References")]
        [SerializeField] private CubePalette _cubePalette;
        [SerializeField] private LevelBlueprint[] _levelBlueprints;

        [Header("Player")]
        [SerializeField] private GameObject _playerPrefab;

        private LevelBuilder _levelBuilder;

        private void Start() {
            // Setup do LevelBuilder
            var config = new LevelGeneratorConfig {
                cubePalette = _cubePalette,
                levelParent = new GameObject("Level").transform,
                buildAsync = true,
                cubesPerFrame = 10,
                showDebugInfo = true,
                playerPrefab = _playerPrefab,
                playerParent = transform,
                autoSpawnPlayer = true,
                globalOffset = Vector3.zero,
                centerLevel = true
            };

            _levelBuilder = gameObject.AddComponent<LevelBuilder>();
            // Você precisaria expor a config para ser assignable, ou usar reflection

            // Alternativamente, configure pelo Inspector e use:
            if (_levelBlueprints.Length > 0) {
                _levelBuilder.BuildLevel(_levelBlueprints[0]);
            }
        }
    }
}
