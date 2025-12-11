using UnityEngine;

namespace PROJECT_CUBE.UI {
    public class HealthUI : MonoBehaviour {
        [SerializeField] private GameObject[] _healthIndicators;
        [SerializeField] private Material _visibleMaterial;
        [SerializeField] private Material _invisibleMaterial;
        
        private void OnEnable() {
            PlayerHealth.OnHealthChanged += UpdateHealthUI;
        }
        
        private void OnDisable() {
            PlayerHealth.OnHealthChanged -= UpdateHealthUI;
        }
        
        private void UpdateHealthUI(int currentHealth, int maxHealth) {
            for (int i = 0; i < _healthIndicators.Length; i++) {
                if (_healthIndicators[i] == null) continue;
                
                Renderer renderer = _healthIndicators[i].GetComponent<Renderer>();
                if (renderer != null) {
                    // Se está dentro da vida atual, usa material visível
                    if (i < currentHealth) {
                        renderer.material = _visibleMaterial;
                    } else {
                        renderer.material = _invisibleMaterial;
                    }
                }
            }
        }
    }
}
