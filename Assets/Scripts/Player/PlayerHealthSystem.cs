using UnityEngine;
using System;

namespace PROJECT_CUBE {
    public class PlayerHealthSystem : MonoBehaviour {
        [Header("Health Settings")]
        [SerializeField] private int _maxHealth = 3;
        [SerializeField] private int _currentHealth;
        
        [Header("Damage Settings")]
        [SerializeField] private float _invulnerabilityDuration = 1f;
        private float _invulnerabilityTimer = 0f;
        
        public static event Action<int, int> OnHealthChanged; // current, max
        public static event Action OnPlayerDeath;
        
        public int CurrentHealth => _currentHealth;
        public int MaxHealth => _maxHealth;
        
        private void Awake() {
            _currentHealth = _maxHealth;
        }
        
        private void Update() {
            if (_invulnerabilityTimer > 0) {
                _invulnerabilityTimer -= Time.deltaTime;
            }
        }
        
        public void TakeDamage(int damage) {
            // Verifica invulnerabilidade temporária
            if (_invulnerabilityTimer > 0) return;
            
            // Verifica invulnerabilidade de carta
            CARDS.CardBuffManager buffManager = GetComponent<CARDS.CardBuffManager>();
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
            
            // Aqui você pode adicionar lógica de morte (reiniciar fase, etc)
            UnityEngine.SceneManagement.SceneManager.LoadScene(
                UnityEngine.SceneManagement.SceneManager.GetActiveScene().name
            );
        }
        
        public bool IsInvulnerable() {
            return _invulnerabilityTimer > 0;
        }
    }
}
