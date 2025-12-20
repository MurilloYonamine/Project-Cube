using UnityEngine;
using TMPro;

namespace PROJECT_CUBE.UI {
    /// <summary>
    /// Exibe a quantidade de moedas coletadas no formato "0x", "1x", "2x", etc.
    /// </summary>
    public class CoinDisplay : MonoBehaviour {
        [SerializeField] private TextMeshProUGUI coinText;
        
        private void Start() {
            UpdateDisplay();
        }
        
        private void OnEnable() {
            // Atualiza sempre que o display for ativado
            UpdateDisplay();
        }
        
        private void Update() {
            // Atualiza continuamente (pode ser otimizado com eventos se necessário)
            UpdateDisplay();
        }
        
        private void UpdateDisplay() {
            if (coinText != null && COLLECTIBLES.CoinManager.Instance != null) {
                int coins = COLLECTIBLES.CoinManager.Instance.CurrentCoins;
                coinText.text = $"{coins}x";
            }
        }
    }
}
