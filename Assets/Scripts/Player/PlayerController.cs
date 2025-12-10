using System;
using UnityEngine;

using PROJECT_CUBE.PLAYER.COMPONENTS;
using PROJECT_CUBE.TERRAIN;

namespace PROJECT_CUBE.PLAYER {
    public class PlayerController : MonoBehaviour {
        [Header("Components")]
        private PlayerComponent[] _playerComponents;
        [SerializeField] private PlayerMovement _playerMovement;
        private PlayerInputHandler _playerInputHandler;

        [Header("Terrain Settings")]
        [SerializeField] private float _blueTerrainSpeedMultiplier = 1.5f;
        [SerializeField] private float _redTerrainSpeedMultiplier = 0.5f;
        [SerializeField] private float _orangeBlockJumpMultiplier = 1.5f;
        
        private TerrainModifierManager _terrainModifierManager;

        #region Unity Methods
        private void Awake() {
            _playerComponents = new PlayerComponent[] {
                _playerInputHandler = new PlayerInputHandler(),
                _playerMovement = new PlayerMovement(),
            };

            _terrainModifierManager = new TerrainModifierManager
            {
                BlueTerrainSpeedMultiplier = _blueTerrainSpeedMultiplier,
                RedTerrainSpeedMultiplier = _redTerrainSpeedMultiplier,
                OrangeBlockJumpMultiplier = _orangeBlockJumpMultiplier
            };

            ForEachComponent(component => component.InitializeComponent(this));
            ForEachComponent(component => component.AwakeComponent());
        }
        private void Start() {
            ForEachComponent(component => component.StartComponent());
        }
        private void Update() {
            ForEachComponent(component => component.UpdateComponent());
        }
        private void OnEnable() {
            ForEachComponent(component => component.OnEnableComponent());
        }
        private void OnDisable() {
            ForEachComponent(component => component.OnDisableComponent());
        }
        private void OnDestroy() {
            ForEachComponent(component => component.OnDestroyComponent());
        }
        private void OnDrawGizmos() {
            ForEachComponent(component => component.OnDrawGizmosComponent());
        }
        #endregion

        #region Public Properties
        public PlayerMovement PlayerMovement => _playerMovement;
        public PlayerInputHandler PlayerInputHandler => _playerInputHandler;
        #endregion

        #region Public Methods
        public void EnterTerrain(TerrainType terrainType) {
            _terrainModifierManager?.OnTerrainChange(terrainType);
        }

        public void ExitTerrain() {
            _terrainModifierManager?.OnTerrainChange(TerrainType.None);
        }

        public void StopMovement() {
            _playerMovement.SetMovementEnabled(false);
        }

        public void ResumeMovement() {
            _playerMovement.SetMovementEnabled(true);
        }
        #endregion

        private void ForEachComponent(Action<PlayerComponent> action) {
            if (_playerComponents == null) return;

            foreach (PlayerComponent component in _playerComponents) {
                if (component == null) return;
                action(component);
            }
        }
    }
}