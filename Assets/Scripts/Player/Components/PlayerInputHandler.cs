using PROJECT_CUBE.INPUT;
using PROJECT_CUBE.PLAYER.COMPONENTS.REWIND;
using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace PROJECT_CUBE.PLAYER.COMPONENTS {
    public class PlayerInputHandler : PlayerComponent {
        private InputControls _inputControls;
        private InputAction _moveAction;
        private InputAction _rewindAction;

        public static event Action<Vector2> OnMovementInput;
        public static event Action OnRewindInput;


        public override void AwakeComponent() {
            _inputControls = new InputControls();

            _moveAction = _inputControls.Player.Move;
            _moveAction.performed += OnMovePerformed;

            _rewindAction = _inputControls.Player.Rewind;
            _rewindAction.performed += OnRewindPerformed;
        }
        public override void OnEnableComponent() {
            _inputControls.Enable();
        }
        public override void OnDisableComponent() {
            _inputControls.Disable();
        }
        private void OnMovePerformed(InputAction.CallbackContext context) {
            Vector2 movementInput = context.ReadValue<Vector2>();
            OnMovementInput?.Invoke(movementInput);
        }
        private void OnRewindPerformed(InputAction.CallbackContext context) {
            if (context.performed) {
                OnRewindInput?.Invoke();
            }
        }
        public override void OnDestroyComponent() {
            _moveAction.performed -= OnMovePerformed;
            _rewindAction.performed -= OnRewindPerformed;
        }
    }
}