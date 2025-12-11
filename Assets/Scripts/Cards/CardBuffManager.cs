using System.Collections;
using UnityEngine;

namespace PROJECT_CUBE.CARDS {
    public class CardBuffManager : MonoBehaviour {
        [Header("References")]
        private PLAYER.PlayerController _playerController;
        private PlayerHealth _playerHealth;
        
        [Header("Buff States")]
        private bool _hasDoubleJump = false;
        private int _remainingJumps = 0;
        private bool _isFloating = false;
        private float _jumpBoostMultiplier = 1f;
        private float _speedBoostMultiplier = 1f;
        private bool _isInvulnerable = false;
        private bool _isFlipped = false;
        
        [Header("Scale Settings")]
        [SerializeField] private Vector3 _originalScale;
        
        private void Awake() {
            _playerController = GetComponent<PLAYER.PlayerController>();
            _playerHealth = _playerController.PlayerHealth;
            _originalScale = transform.localScale;
        }
        
        public void ApplyCard(CardData card) {
            PlayerDebugManager.Instance?.AddLine($"Carta aplicada: {card.displayName}", "CardBuffManager");
            
            switch (card.type) {
                case CardType.Keyframe:
                    ApplyDoubleJump();
                    break;
                case CardType.Array:
                    ApplyExtraLife();
                    break;
                case CardType.Subdivision:
                    StartCoroutine(ApplyFloating());
                    break;
                case CardType.Screw:
                    ApplyJumpBoost();
                    break;
                case CardType.Skin:
                    ApplyScreenPause();
                    break;
                case CardType.Scale:
                    ApplyScale();
                    break;
                case CardType.Remesh:
                    StartCoroutine(ApplyInvulnerability());
                    break;
                case CardType.Particle:
                    ApplySpeedBoost();
                    break;
                case CardType.Mirror:
                    ApplyFlip();
                    break;
            }
        }
        
        // Keyframe - Double Jump
        private void ApplyDoubleJump() {
            _hasDoubleJump = true;
            _remainingJumps = 2;
            PlayerDebugManager.Instance?.AddLine("Double Jump ativado!", "CardBuffManager");
        }
        
        public bool CanJump(bool isGrounded) {
            if (isGrounded) {
                _remainingJumps = _hasDoubleJump ? 2 : 1;
                return true;
            }
            return _remainingJumps > 0;
        }
        
        public void ConsumeJump() {
            if (_remainingJumps > 0) _remainingJumps--;
        }
        
        // Array - +1 Vida
        private void ApplyExtraLife() {
            if (_playerHealth != null) {
                _playerHealth.AddLife(1);
                PlayerDebugManager.Instance?.AddLine("+1 Vida adicionada!", "CardBuffManager");
            }
        }
        
        // Subdivision - Flutua por 3s
        private IEnumerator ApplyFloating() {
            _isFloating = true;
            Rigidbody rb = GetComponent<Rigidbody>();
            
            if (rb != null) {
                float originalGravity = rb.linearVelocity.y;
                rb.useGravity = false;
                rb.linearVelocity = new Vector3(rb.linearVelocity.x, 0, rb.linearVelocity.z);
                
                PlayerDebugManager.Instance?.AddLine("Flutuando por 3 segundos!", "CardBuffManager");
                
                yield return new WaitForSeconds(3f);
                
                rb.useGravity = true;
                _isFloating = false;
                PlayerDebugManager.Instance?.AddLine("Flutuação terminou", "CardBuffManager");
            }
        }
        
        // Screw - Pulo maior
        private void ApplyJumpBoost() {
            _jumpBoostMultiplier = 1.5f;
            PlayerDebugManager.Instance?.AddLine("Pulo aumentado em 50%!", "CardBuffManager");
        }
        
        public float GetJumpMultiplier() => _jumpBoostMultiplier;
        
        // Skin - Pausa avanço da tela 1s
        private void ApplyScreenPause() {
            // Este método deve ser chamado por um gerenciador de câmera/tela
            StartCoroutine(PauseScreenAdvance());
        }
        
        private IEnumerator PauseScreenAdvance() {
            PlayerDebugManager.Instance?.AddLine("Avanço da tela pausado por 1s", "CardBuffManager");
            // Aqui você implementaria a pausa do avanço da câmera
            yield return new WaitForSeconds(1f);
            PlayerDebugManager.Instance?.AddLine("Avanço da tela retomado", "CardBuffManager");
        }
        
        // Scale - Muda tamanho
        private void ApplyScale() {
            float scaleMultiplier = Random.value > 0.5f ? 1.5f : 0.5f;
            transform.localScale = _originalScale * scaleMultiplier;
            
            string sizeChange = scaleMultiplier > 1f ? "aumentou" : "diminuiu";
            PlayerDebugManager.Instance?.AddLine($"Tamanho {sizeChange}!", "CardBuffManager");
        }
        
        // Remesh - Invulnerabilidade 5s
        private IEnumerator ApplyInvulnerability() {
            _isInvulnerable = true;
            PlayerDebugManager.Instance?.AddLine("Invulnerável por 5 segundos!", "CardBuffManager");
            
            yield return new WaitForSeconds(5f);
            
            _isInvulnerable = false;
            PlayerDebugManager.Instance?.AddLine("Invulnerabilidade terminou", "CardBuffManager");
        }
        
        public bool IsInvulnerable() => _isInvulnerable;
        
        // Particle - Aumento de velocidade
        private void ApplySpeedBoost() {
            _speedBoostMultiplier = 1.5f;
            PlayerDebugManager.Instance?.AddLine("Velocidade aumentada em 50%!", "CardBuffManager");
        }
        
        public float GetSpeedMultiplier() => _speedBoostMultiplier;
        
        // Mirror - Flip direção
        private void ApplyFlip() {
            _isFlipped = !_isFlipped;
            PlayerDebugManager.Instance?.AddLine($"Direção invertida! Flipped: {_isFlipped}", "CardBuffManager");
        }
        
        public bool IsFlipped() => _isFlipped;
        
        public bool IsFloating() => _isFloating;
    }
}
