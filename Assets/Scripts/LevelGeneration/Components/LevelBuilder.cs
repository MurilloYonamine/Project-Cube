using UnityEngine;

namespace PROJECT_CUBE.LEVEL_GENERATION {
    /// <summary>
    /// LevelBuilder modular - orquestra a construção de níveis
    /// </summary>
    public class LevelBuilder : MonoBehaviour {
        [SerializeField] private LevelGeneratorConfig _config = new LevelGeneratorConfig();
        [SerializeField] private LevelBlueprint _blueprintToLoad;

        // Componentes modulares
        private LevelGeneratorConstructor _constructor;
        private LevelGeneratorPlayerManager _playerManager;

        // Eventos públicos
        public System.Action<LevelBlueprint> OnLevelBuildStarted;
        public System.Action<LevelBlueprint> OnLevelBuildCompleted;
        public System.Action<Vector2Int, GameObject> OnCubeBuilt;
        public System.Action<GameObject> OnPlayerSpawned;

        #region Properties
        public LevelBlueprint CurrentBlueprint => _constructor?.CurrentBlueprint;
        public bool IsBuilding => _constructor?.IsBuilding ?? false;
        public GameObject SpawnedPlayer => _playerManager?.SpawnedPlayer;
        public CubePalette CubePalette {
            get => _config.cubePalette;
            set => _config.cubePalette = value;
        }
        #endregion

        #region Unity Lifecycle
        private void Awake() {
            InitializeComponents();
            SetupLevelParent();
        }

        private void Start() {
            if (_blueprintToLoad != null) {
                BuildLevel(_blueprintToLoad);
            }
        }
        #endregion

        #region Initialization
        private void InitializeComponents() {
            _constructor = new LevelGeneratorConstructor();
            _playerManager = new LevelGeneratorPlayerManager();

            _constructor.Initialize(_config, this);
            _playerManager.Initialize(_config);

            ConnectEvents();
        }

        private void ConnectEvents() {
            _constructor.OnBuildStarted += (blueprint) => OnLevelBuildStarted?.Invoke(blueprint);
            _constructor.OnBuildCompleted += (blueprint) => {
                OnLevelBuildCompleted?.Invoke(blueprint);
                if (_playerManager.ShouldAutoSpawnPlayer(blueprint)) {
                    _playerManager.SpawnPlayer(blueprint);
                    OnPlayerSpawned?.Invoke(_playerManager.SpawnedPlayer);
                }
            };
            _constructor.OnCubeBuilt += (pos, obj) => OnCubeBuilt?.Invoke(pos, obj);
        }

        private void SetupLevelParent() {
            if (_config.levelParent == null) {
                GameObject levelParentObj = new GameObject("Level");
                levelParentObj.transform.SetParent(transform);
                _config.levelParent = levelParentObj.transform;

                if (_config.centerLevel) {
                    levelParentObj.transform.localPosition = Vector3.zero;
                }

                if (_config.showDebugInfo)
                    Debug.Log("LevelBuilder: Created level parent automatically");
            }
        }
        #endregion

        #region Public Methods
        /// <summary>
        /// Constrói um nível baseado no blueprint
        /// </summary>
        public void BuildLevel(LevelBlueprint blueprint) {
            if (blueprint == null) {
                Debug.LogError("LevelBuilder: Cannot build with null blueprint!");
                return;
            }

            _constructor.BuildLevel(blueprint);
        }

        /// <summary>
        /// Remove o player da cena
        /// </summary>
        public void RemovePlayer() {
            _playerManager.DespawnPlayer();
        }

        /// <summary>
        /// Retorna o blueprint carregado atualmente
        /// </summary>
        public LevelBlueprint GetCurrentBlueprint() {
            return _constructor.CurrentBlueprint;
        }
        #endregion
    }
}
