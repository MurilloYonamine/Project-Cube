using UnityEngine;
using System;
using PROJECT_CUBE.TERRAIN;

namespace PROJECT_CUBE.PLAYER.COMPONENTS {
    [Serializable]
    public class PlayerMovement : PlayerComponent {
        [Header("Unity Components")]
        private Rigidbody _rigidbody;
        private TerrainModifier _terrainModifierManager;

        [Header("Movement Settings")]
        [SerializeField] private float _movementSpeed = 2f;
        [SerializeField] private float _jumpForce = 5f;
        private Vector2 _movementInput;

        [Header("Ground Detection")]
        [SerializeField] private float _groundDragDistance = 0.5f;
        private bool _isGrounded;
        
        private bool _isMovementEnabled = true;

        public override void AwakeComponent() {
            _rigidbody = _playerController.GetComponent<Rigidbody>();
            _terrainModifierManager = new TerrainModifier();
            LogMovementInput(Vector2.zero);
        }

        public override void OnEnableComponent() {
            PlayerInputHandler.OnMovementInput += HandleMovement;
        }
        public override void OnDisableComponent() {
            PlayerInputHandler.OnMovementInput -= HandleMovement;
        }
        public override void UpdateComponent() {
            base.UpdateComponent();
            
            CheckGroundStatus();
            if (_isMovementEnabled) {
                AutoMovement();
            }
        }
        private void HandleMovement(Vector2 movementInput) {
            _movementInput = movementInput;

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
            float jumpModifier = _terrainModifierManager != null ? _terrainModifierManager.CurrentJumpModifier : 1f;

            _rigidbody.linearVelocity = new Vector3(
                _rigidbody.linearVelocity.x,
                y: 0,
                _rigidbody.linearVelocity.z
            );

            _rigidbody.AddForce(
                _jumpForce * jumpModifier * Vector3.up,
                ForceMode.Impulse
            );
        }
        private void AutoMovement() {
            float speedModifier = _terrainModifierManager != null ? _terrainModifierManager.CurrentSpeedModifier : 1f;
            float direction = _movementInput.x != 0 ? _movementInput.x : 1f;
            float movement = Mathf.Sign(direction) * _movementSpeed * speedModifier;

            _rigidbody.linearVelocity = new Vector3(
                movement, 
                _rigidbody.linearVelocity.y, 
                _rigidbody.linearVelocity.z
            );
        }
        private void CheckGroundStatus() {
            Vector3 raycastOrigin = _rigidbody.position + Vector3.down * 0.5f;

            _isGrounded = Physics.Raycast(
                raycastOrigin, 
                Vector3.down, 
                _groundDragDistance
            );

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
            message = movementInput switch {
                Vector2 input when input.x > 0 => "Movendo para a direita",
                Vector2 input when input.x < 0 => "Movendo para a esquerda",
                Vector2 input when input.y > 0 => "Pulando",
                Vector2 input when input.y < 0 => "Caindo mais rápido",
                _ => "Nenhum Input de Movimento",
            };
            PlayerDebugManager.Instance.AddLine($"{message}", nameof(PlayerMovement));
        }

        public void SetMovementEnabled(bool enabled) {
            _isMovementEnabled = enabled;
        }
    }
}