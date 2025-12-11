using System;
using UnityEngine;

using PROJECT_CUBE.PLAYER.COMPONENTS;
using PROJECT_CUBE.PLAYER.COMPONENTS.REWIND;
using PROJECT_CUBE.TERRAIN;

namespace PROJECT_CUBE.PLAYER {
    public class PlayerController : MonoBehaviour {
        [Header("Components")]
        private PlayerComponent[] _playerComponents;
        [SerializeField] private PlayerMovement _playerMovement;
        [SerializeField] private PlayerRewind _playerRewind;
        [SerializeField] private PlayerHealth _playerHealth;
        private PlayerInputHandler _playerInputHandler;
        
        private TerrainModifier _terrainModifier;

        #region Unity Methods
        private void Awake() {
            _playerComponents = new PlayerComponent[] {
                _playerInputHandler = new PlayerInputHandler(),
                _playerMovement,
                _playerRewind,
                _playerHealth,
            };

            _terrainModifier = new TerrainModifier();

            ForEachComponent(component => component.InitializeComponent(this));
            ForEachComponent(component => component.AwakeComponent());
        }
        private void Start() {
            ForEachComponent(component => component.StartComponent());
        }
        private void Update() {
            ForEachComponent(component => component.UpdateComponent());
        }
        private void FixedUpdate() {
            ForEachComponent(component => component.FixedUpdateComponent());
        }
        private void LateUpdate() {
            ForEachComponent(component => component.LateUpdateComponent());
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
        public PlayerRewind PlayerRewind => _playerRewind;
        public PlayerHealth PlayerHealth => _playerHealth;
        #endregion

        #region Public Methods
        public void EnterTerrain(TerrainType terrainType) {
            _terrainModifier?.OnTerrainChange(terrainType);
        }

        public void ExitTerrain() {
            _terrainModifier?.OnTerrainChange(TerrainType.None);
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