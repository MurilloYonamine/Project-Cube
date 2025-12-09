using System;
using UnityEngine;

using PROJECT_CUBE.PLAYER.COMPONENTS;

namespace PROJECT_CUBE.PLAYER {
    public class PlayerController : MonoBehaviour {
        [Header("Components")]
        private PlayerComponent[] _playerComponents;
        [SerializeField] private PlayerMovement _playerMovement;
        private PlayerInputHandler _playerInputHandler;

        #region Unity Methods
        private void Awake() {
            _playerComponents = new PlayerComponent[] {
                _playerInputHandler = new PlayerInputHandler(),
                _playerMovement,
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

        private void ForEachComponent(Action<PlayerComponent> action) {
            if (_playerComponents == null) return;

            foreach (PlayerComponent component in _playerComponents) {
                if (component == null) return;
                action(component);
            }
        }
    }
}