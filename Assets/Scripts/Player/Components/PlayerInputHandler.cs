using PROJECT_CUBE.INPUT;

using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace PROJECT_CUBE.PLAYER.COMPONENTS {
    public class PlayerInputHandler : PlayerComponent {
        private InputControls _inputControls;
        private InputAction _moveAction;

        public static event Action<Vector2> OnMovementInput;
        public override void AwakeComponent() {
            _inputControls = new InputControls();

            _moveAction = _inputControls.Player.Move;
            _moveAction.performed += OnMovePerformed;
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
        public override void OnDestroyComponent() {
            _moveAction.performed -= OnMovePerformed;
        }
    }
}