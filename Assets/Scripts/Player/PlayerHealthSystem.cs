using UnityEngine;
using System;
using PROJECT_CUBE.PLAYER.COMPONENTS;
using PROJECT_CUBE.CARDS;

namespace PROJECT_CUBE {
    public class PlayerHealthSystem : PlayerComponent {
        [Header("Health Settings")]
        private int _maxHealth = 3;
        private int _currentHealth;
        
        [Header("Damage Settings")]
        private float _invulnerabilityDuration = 1f;
        private float _invulnerabilityTimer = 0f;
        
        public static event Action<int, int> OnHealthChanged; // current, max
        public static event Action OnPlayerDeath;
        
        public int CurrentHealth => _currentHealth;
        public int MaxHealth => _maxHealth;
        
        public override void AwakeComponent() {
            _currentHealth = _maxHealth;
        }
        
        public override void UpdateComponent() {
            base.UpdateComponent();
            if (!enabled) return;
            
            if (_invulnerabilityTimer > 0) {
                _invulnerabilityTimer -= Time.deltaTime;
            }
        }
        
        public void TakeDamage(int damage) {
            // Verifica invulnerabilidade temporária
            if (_invulnerabilityTimer > 0) return;
            
            // Verifica invulnerabilidade de carta
            CardBuffManager buffManager = _playerController.GetComponent<CardBuffManager>();
            if (buffManager != null && buffManager.IsInvulnerable()) {
                PlayerDebugManager.Instance?.AddLine("Dano bloqueado por invulnerabilidade!", "PlayerHealthSystem");
                return;
            }
            
            _currentHealth -= damage;
            _currentHealth = Mathf.Max(_currentHealth, 0);
            
            _invulnerabilityTimer = _invulnerabilityDuration;
            
            OnHealthChanged?.Invoke(_currentHealth, _maxHealth);
            
            // Tocar som de dano
            AudioManager.Instance?.PlaySound("TakeDamage");
            
            PlayerDebugManager.Instance?.AddLine($"Dano recebido! Vida: {_currentHealth}/{_maxHealth}", "PlayerHealthSystem");
            
            if (_currentHealth <= 0) {
                Die();
            }
        }
        
        public void Heal(int amount) {
            _currentHealth += amount;
            _currentHealth = Mathf.Min(_currentHealth, _maxHealth);
            
            OnHealthChanged?.Invoke(_currentHealth, _maxHealth);
            
            PlayerDebugManager.Instance?.AddLine($"Curado! Vida: {_currentHealth}/{_maxHealth}", "PlayerHealthSystem");
        }
        
        public void AddLife(int amount) {
            _maxHealth += amount;
            _currentHealth += amount;
            
            OnHealthChanged?.Invoke(_currentHealth, _maxHealth);
            
            PlayerDebugManager.Instance?.AddLine($"Vida máxima aumentada! Vida: {_currentHealth}/{_maxHealth}", "PlayerHealthSystem");
        }
        
        private void Die() {
            OnPlayerDeath?.Invoke();
            PlayerDebugManager.Instance?.AddLine("Jogador morreu!", "PlayerHealthSystem");
            
            UnityEngine.SceneManagement.SceneManager.LoadScene(
                UnityEngine.SceneManagement.SceneManager.GetActiveScene().name
            );
        }
        
        public bool IsInvulnerable() {
            return _invulnerabilityTimer > 0;
        }
    }
}
