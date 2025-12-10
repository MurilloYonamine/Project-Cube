using UnityEngine;
using System.Collections.Generic;

namespace PROJECT_CUBE.LEVEL_GENERATION {
    /// <summary>
    /// Manager principal que gerencia múltiplos níveis
    /// </summary>
    public class LevelManager : MonoBehaviour {
        [Header("Core")]
        [SerializeField] private LevelBuilder _levelBuilder;

        [Header("Levels")]
        [SerializeField] private LevelBlueprint[] _availableLevels;
        [SerializeField] private bool _autoLoadFirstLevel = true;

        [Header("Generation")]
        [SerializeField] private bool _preGenerateAllLevels = false;
        [SerializeField] private float _verticalSpacing = 15f;

        // Estado
        private int _currentLevelIndex = -1;
        private Dictionary<int, GameObject> _generatedLevels = new Dictionary<int, GameObject>();
        private bool _isGenerating = false;

        #region Properties
        public int CurrentLevelIndex => _currentLevelIndex;
        public int TotalLevels => _availableLevels?.Length ?? 0;
        public LevelBlueprint CurrentBlueprint => _levelBuilder?.CurrentBlueprint;
        public bool IsGenerating => _isGenerating;
        #endregion

        #region Events
        public System.Action<LevelBlueprint, int> OnLevelStarted;
        public System.Action<LevelBlueprint, int> OnLevelCompleted;
        public System.Action<int> OnLevelChanged;
        #endregion

        #region Unity Lifecycle
        private void Awake() {
            if (_levelBuilder == null) {
                _levelBuilder = GetComponent<LevelBuilder>();
                if (_levelBuilder == null) {
                    Debug.LogError("LevelManager: No LevelBuilder found!");
                }
            }

            ConnectEvents();
        }

        private void Start() {
            if (_preGenerateAllLevels && _availableLevels != null && _availableLevels.Length > 0) {
                GenerateAllLevels();
            } else if (_autoLoadFirstLevel && _availableLevels != null && _availableLevels.Length > 0) {
                LoadLevel(0);
            }
        }
        #endregion

        #region Events
        private void ConnectEvents() {
            if (_levelBuilder != null) {
                _levelBuilder.OnLevelBuildStarted += (bp) => OnLevelStarted?.Invoke(bp, _currentLevelIndex);
                _levelBuilder.OnLevelBuildCompleted += (bp) => OnLevelCompleted?.Invoke(bp, _currentLevelIndex);
            }
        }
        #endregion

        #region Public Methods
        /// <summary>
        /// Carrega um nível pelo índice
        /// </summary>
        public void LoadLevel(int levelIndex) {
            if (levelIndex < 0 || levelIndex >= _availableLevels.Length) {
                Debug.LogError($"LevelManager: Invalid level index {levelIndex}");
                return;
            }

            if (_isGenerating) {
                Debug.LogWarning("LevelManager: Already generating a level!");
                return;
            }

            _isGenerating = true;
            _currentLevelIndex = levelIndex;

            var blueprint = _availableLevels[levelIndex];
            _levelBuilder.BuildLevel(blueprint);

            OnLevelChanged?.Invoke(levelIndex);
            _isGenerating = false;
        }

        /// <summary>
        /// Carrega o próximo nível
        /// </summary>
        public void LoadNextLevel() {
            int nextIndex = _currentLevelIndex + 1;
            if (nextIndex < _availableLevels.Length) {
                LoadLevel(nextIndex);
            } else {
                Debug.Log("LevelManager: All levels completed!");
            }
        }

        /// <summary>
        /// Carrega o nível anterior
        /// </summary>
        public void LoadPreviousLevel() {
            int prevIndex = _currentLevelIndex - 1;
            if (prevIndex >= 0) {
                LoadLevel(prevIndex);
            }
        }

        /// <summary>
        /// Recarrega o nível atual
        /// </summary>
        public void ReloadCurrentLevel() {
            if (_currentLevelIndex >= 0) {
                LoadLevel(_currentLevelIndex);
            }
        }

        /// <summary>
        /// Gera todos os níveis previamente
        /// </summary>
        public void GenerateAllLevels() {
            if (_availableLevels == null || _availableLevels.Length == 0) {
                Debug.LogWarning("LevelManager: No levels available!");
                return;
            }

            Debug.Log($"LevelManager: Pre-generating {_availableLevels.Length} levels...");

            for (int i = 0; i < _availableLevels.Length; i++) {
                var blueprint = _availableLevels[i];
                if (blueprint == null) {
                    Debug.LogError($"LevelManager: Blueprint at index {i} is null!");
                    continue;
                }

                // Cria container para o nível
                GameObject levelContainer = new GameObject($"Level_{i:D2}_{blueprint.LevelName}");
                levelContainer.transform.SetParent(transform);
                levelContainer.transform.localPosition = new Vector3(0, -i * _verticalSpacing, 0);

                _generatedLevels[i] = levelContainer;

                if (_currentLevelIndex == -1) {
                    _currentLevelIndex = 0;
                }
            }

            Debug.Log("LevelManager: All levels pre-generated!");
        }
        #endregion
    }
}
