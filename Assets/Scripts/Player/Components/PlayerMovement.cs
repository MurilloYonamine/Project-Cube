using UnityEngine;
using System;

namespace PROJECT_CUBE.PLAYER.COMPONENTS {
    [Serializable]
    public class PlayerMovement : PlayerComponent {
        [Header("Unity Components")]
        private Rigidbody _rigidbody;

        [Header("Movement Settings")]
        [SerializeField] private float _movementSpeed = 2f;
        [SerializeField] private float _jumpForce = 5f;

        [Header("Ground Detection")]
        [SerializeField] private float _groundDragDistance = 0.5f;
        private bool _isGrounded;

        public override void AwakeComponent() {
            _rigidbody = _playerController.GetComponent<Rigidbody>();
        }

        public override void OnEnableComponent() {
            PlayerInputHandler.OnMovementInput += HandleMovement;
        }
        public override void OnDisableComponent() {
            PlayerInputHandler.OnMovementInput -= HandleMovement;
        }
        public override void UpdateComponent() {
            CheckGroundStatus();
            AutoMovement();
        }
        private void HandleMovement(Vector2 movementInput) {
            LogMovementInput(movementInput);

            // Pulo para cima
            if (movementInput.y > 0 && _isGrounded) {
                Jump();
            }

            // Queda para baixo
            else if (movementInput.y < 0 && !_isGrounded) {
                float fallMultiplier = 0.5f;
                _rigidbody.linearVelocity += _jumpForce * fallMultiplier * Vector3.down;
            }
        }
        private void Jump() {
            _rigidbody.linearVelocity = new Vector3(_rigidbody.linearVelocity.x, 0, _rigidbody.linearVelocity.z);
            _rigidbody.AddForce(Vector3.up * _jumpForce, ForceMode.Impulse);
        }
        private void AutoMovement() {
            Vector2 movement = new Vector2(1, 0) * _movementSpeed;
            _rigidbody.linearVelocity = new Vector2(movement.x, _rigidbody.linearVelocity.y);
        }
        private void CheckGroundStatus() {
            Vector3 raycastOrigin = _rigidbody.position + Vector3.down * 0.5f;
            _isGrounded = Physics.Raycast(raycastOrigin, Vector3.down, _groundDragDistance);

            PlayerDebugManager.Instance.AddLine($"Está no chão: {_isGrounded}", "GroundCheck");
        }
        public override void OnDrawGizmosComponent() {
            if (_rigidbody == null) return;

            Vector3 raycastOrigin = _rigidbody.position + Vector3.down * 0.5f;

            Gizmos.color = _isGrounded ? Color.green : Color.red;
            Gizmos.DrawLine(
                raycastOrigin,
                raycastOrigin + Vector3.down * _groundDragDistance
            );
        }
        private void LogMovementInput(Vector2 movementInput) {
            string message = string.Empty;
            switch (movementInput.y) {
                case > 0:
                    message = $"Input: Para cima ({movementInput.y})";
                    break;
                case < 0:
                    message = $"Input: Para baixo ({movementInput.y})";
                    break;
                default:
                    message = $"Input: Ignorado";
                    break;
            }

            PlayerDebugManager.Instance.AddLine($"{message}", nameof(PlayerMovement));
        }
    }
}