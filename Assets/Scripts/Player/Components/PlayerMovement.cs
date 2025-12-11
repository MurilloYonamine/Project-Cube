using UnityEngine;
using System;
using PROJECT_CUBE.TERRAIN;
using PROJECT_CUBE.CARDS;

namespace PROJECT_CUBE.PLAYER.COMPONENTS {
    [Serializable]
    public class PlayerMovement : PlayerComponent {
        [Header("Unity Components")]
        private Rigidbody _rigidbody;
        private TerrainModifier _terrainModifierManager;
        private CardBuffManager _cardBuffManager;

        [Header("Movement Settings")]
        [SerializeField] private float _movementSpeed = 2f;
        [SerializeField] private float _jumpForce = 5f;
        private Vector2 _movementInput;

        [Header("Ground Detection")]
        [SerializeField] private float _groundDragDistance = 0.5f;
        private bool _isGrounded;
        private bool _wasGrounded;
        
        private bool _isMovementEnabled = true;
        
        [Header("Audio")]
        private bool _isPlayingWalkSound = false;

        public bool IsGrounded => _isGrounded;

        public override void AwakeComponent() {
            _rigidbody = _playerController.GetComponent<Rigidbody>();
            _terrainModifierManager = new TerrainModifier();
            _cardBuffManager = _playerController.GetComponent<CardBuffManager>();
            LogMovementInput(Vector2.zero);
        }

        public override void OnEnableComponent() {
            PlayerInputHandler.OnMovementInput += HandleMovement;
        }
        
        public override void OnDisableComponent() {
            PlayerInputHandler.OnMovementInput -= HandleMovement;
            StopWalkSound();
        }
        
        public override void UpdateComponent() {
            base.UpdateComponent();
            
            _wasGrounded = _isGrounded;
            CheckGroundStatus();
            
            // Detecta quando acabou de pousar no chão
            if (_isGrounded && !_wasGrounded) {
                AudioManager.Instance?.PlaySound("Fall");
            }
            
            if (_isMovementEnabled) {
                AutoMovement();
                UpdateWalkSound();
            }
        }
        
        private void HandleMovement(Vector2 movementInput) {
            _movementInput = movementInput;

            LogMovementInput(movementInput);

            // Pulo para cima - verifica double jump
            if (movementInput.y > 0) {
                bool canJump = _isGrounded;
                
                if (_cardBuffManager != null) {
                    canJump = _cardBuffManager.CanJump(_isGrounded);
                }
                
                if (canJump) {
                    Jump();
                    if (_cardBuffManager != null) {
                        _cardBuffManager.ConsumeJump();
                    }
                }
            }

            // Queda rápida para baixo (Dash Fall)
            else if (movementInput.y < 0 && !_isGrounded) {
                float fallMultiplier = 0.5f;
                _rigidbody.linearVelocity += _jumpForce * fallMultiplier * Vector3.down;
                
                AudioManager.Instance?.PlaySound("DashFall");
            }
        }
        
        private void Jump() {
            float jumpModifier = _terrainModifierManager != null ? _terrainModifierManager.CurrentJumpModifier : 1f;
            
            // Aplica buff de pulo da carta Screw
            if (_cardBuffManager != null) {
                jumpModifier *= _cardBuffManager.GetJumpMultiplier();
            }

            _rigidbody.linearVelocity = new Vector3(
                _rigidbody.linearVelocity.x,
                y: 0,
                _rigidbody.linearVelocity.z
            );

            _rigidbody.AddForce(
                _jumpForce * jumpModifier * Vector3.up,
                ForceMode.Impulse
            );
            
            AudioManager.Instance?.PlaySound("Jump");
        }
        
        private void AutoMovement() {
            // Não move se estiver flutuando
            if (_cardBuffManager != null && _cardBuffManager.IsFloating()) {
                return;
            }
            
            float speedModifier = _terrainModifierManager != null ? _terrainModifierManager.CurrentSpeedModifier : 1f;
            
            // Aplica buff de velocidade da carta Particle
            if (_cardBuffManager != null) {
                speedModifier *= _cardBuffManager.GetSpeedMultiplier();
            }
            
            float direction = _movementInput.x != 0 ? _movementInput.x : 1f;
            
            // Aplica flip da carta Mirror
            if (_cardBuffManager != null && _cardBuffManager.IsFlipped()) {
                direction *= -1f;
            }
            
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
        
        private void UpdateWalkSound() {
            if (_isGrounded && Mathf.Abs(_rigidbody.linearVelocity.x) > 0.1f) {
                if (!_isPlayingWalkSound) {
                    AudioManager.Instance?.PlaySound("Walk");
                    _isPlayingWalkSound = true;
                }
            } else {
                StopWalkSound();
            }
        }
        
        private void StopWalkSound() {
            if (_isPlayingWalkSound) {
                AudioManager.Instance?.StopSound("Walk");
                _isPlayingWalkSound = false;
            }
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
            PlayerDebugManager.Instance?.AddLine($"{message}", nameof(PlayerMovement));
        }

        public void SetMovementEnabled(bool enabled) {
            _isMovementEnabled = enabled;
            if (!enabled) {
                StopWalkSound();
            }
        }
    }
}
