using PROJECT_CUBE.PLAYER;
using UnityEngine;

namespace PROJECT_CUBE.DEBUG {
    public class HealthTestButton : MonoBehaviour {
        [SerializeField] private int _damageAmount = 1;
        [SerializeField] private KeyCode _damageKey = KeyCode.T;
        [SerializeField] private KeyCode _healKey = KeyCode.H;
        
        private PlayerHealth _playerHealth;
        
        private void Start() {
            // Tenta encontrar o PlayerHealth
            _playerHealth = FindFirstObjectByType<PlayerController>().PlayerHealth;

            if (_playerHealth == null) {
                PlayerDebugManager.Instance?.AddLine("PlayerHealth não encontrado!", "HealthTestButton");
            }
        }
        
        private void Update() {
            if (_playerHealth == null) return;
            
            if (Input.GetKeyDown(_damageKey)) {
                _playerHealth.TakeDamage(_damageAmount);
                PlayerDebugManager.Instance?.AddLine($"Teste: {_damageAmount} de dano aplicado!", "HealthTestButton");
            }
            
            if (Input.GetKeyDown(_healKey)) {
                _playerHealth.Heal(_damageAmount);
                PlayerDebugManager.Instance?.AddLine($"Teste: {_damageAmount} de cura aplicado!", "HealthTestButton");
            }
        }
    }
}
