using UnityEngine;
using System.Collections;

namespace PROJECT_CUBE.LEVEL_GENERATION {
    /// <summary>
    /// Responsável pela construção de um nível baseado no blueprint
    /// </summary>
    public class LevelGeneratorConstructor {
        private bool _isBuilding = false;
        private LevelBlueprint _currentBlueprint;
        private LevelGeneratorConfig _config;
        private MonoBehaviour _monoBehaviourContext;
        private Transform _levelParent;
        private CubePalette _palette;

        public System.Action<LevelBlueprint> OnBuildStarted;
        public System.Action<LevelBlueprint> OnBuildCompleted;
        public System.Action<Vector2Int, GameObject> OnCubeBuilt;

        public bool IsBuilding => _isBuilding;
        public LevelBlueprint CurrentBlueprint => _currentBlueprint;

        public void Initialize(LevelGeneratorConfig config, MonoBehaviour context) {
            _config = config;
            _monoBehaviourContext = context;
            _palette = config.cubePalette;
            _levelParent = config.levelParent;
        }

        /// <summary>
        /// Constrói um nível
        /// </summary>
        public void BuildLevel(LevelBlueprint blueprint) {
            if (blueprint == null) {
                Debug.LogError("LevelGenerator: Cannot build with null blueprint!");
                return;
            }
            if (_isBuilding) {
                Debug.LogWarning("LevelGenerator: Already building!");
                return;
            }
            if (_palette == null) {
                Debug.LogError("LevelGenerator: No palette assigned!");
                return;
            }

            _currentBlueprint = blueprint;

            if (_config.buildAsync) {
                _monoBehaviourContext.StartCoroutine(BuildLevelAsync(blueprint));
            } else {
                BuildLevelImmediate(blueprint);
            }
        }

        private void BuildLevelImmediate(LevelBlueprint blueprint) {
            _isBuilding = true;
            OnBuildStarted?.Invoke(blueprint);

            ClearLevel();

            if (_config.showDebugInfo)
                Debug.Log($"Building level '{blueprint.LevelName}' with {blueprint.Cubes.Length} cubes");

            // Constrói cubos
            foreach (var cubeData in blueprint.Cubes) {
                InstantiateCube(cubeData, blueprint);
            }

            _isBuilding = false;
            OnBuildCompleted?.Invoke(blueprint);

            if (_config.showDebugInfo)
                Debug.Log($"Level build completed! Built {blueprint.Cubes.Length} cubes.");
        }

        private IEnumerator BuildLevelAsync(LevelBlueprint blueprint) {
            _isBuilding = true;
            OnBuildStarted?.Invoke(blueprint);

            ClearLevel();

            if (_config.showDebugInfo)
                Debug.Log($"Building level '{blueprint.LevelName}' asynchronously...");

            int cubesBuilt = 0;

            foreach (var cubeData in blueprint.Cubes) {
                InstantiateCube(cubeData, blueprint);
                cubesBuilt++;

                if (cubesBuilt >= _config.cubesPerFrame) {
                    cubesBuilt = 0;
                    yield return null;
                }
            }

            _isBuilding = false;
            OnBuildCompleted?.Invoke(blueprint);

            if (_config.showDebugInfo)
                Debug.Log($"Async level build completed! Built {blueprint.Cubes.Length} cubes.");
        }

        private void InstantiateCube(LevelBlueprint.CubeData cubeData, LevelBlueprint blueprint) {
            var prefab = _palette.GetPrefab(cubeData.cubeTypeIndex);
            if (prefab == null) {
                Debug.LogWarning($"Cube prefab not found for type {cubeData.cubeTypeIndex}");
                return;
            }

            // Calcula posição mundo
            Vector3 worldPos = blueprint.GridToWorldPosition(cubeData.gridPosition);
            worldPos += _config.globalOffset;

            // Instancia cubo
            GameObject cubeObj = Object.Instantiate(prefab, worldPos, Quaternion.identity, _levelParent);
            cubeObj.name = $"Cube_{cubeData.gridPosition.x}_{cubeData.gridPosition.y}";

            OnCubeBuilt?.Invoke(cubeData.gridPosition, cubeObj);
        }

        private void ClearLevel() {
            if (_levelParent != null) {
                for (int i = _levelParent.childCount - 1; i >= 0; i--) {
                    Transform child = _levelParent.GetChild(i);
                    if (Application.isPlaying)
                        Object.Destroy(child.gameObject);
                    else
                        Object.DestroyImmediate(child.gameObject);
                }
            }
        }
    }
}
