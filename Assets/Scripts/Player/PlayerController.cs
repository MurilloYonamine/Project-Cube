using System;
using UnityEngine;

using PROJECT_CUBE.PLAYER.COMPONENTS;
using PROJECT_CUBE.PLAYER.COMPONENTS.REWIND;

namespace PROJECT_CUBE.PLAYER {
    public class PlayerController : MonoBehaviour {
        [Header("Components")]
        private PlayerComponent[] _playerComponents;
        [SerializeField] private PlayerMovement _playerMovement;
        [SerializeField] private PlayerRewind _playerRewind;
        
        private PlayerInputHandler _playerInputHandler;

        #region Unity Methods
        private void Awake() {
            _playerComponents = new PlayerComponent[] {
                _playerInputHandler = new PlayerInputHandler(),
                _playerMovement = new PlayerMovement(),
                _playerRewind,
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