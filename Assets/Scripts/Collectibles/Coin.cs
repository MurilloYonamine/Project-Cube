using UnityEngine;

namespace PROJECT_CUBE.COLLECTIBLES {
    public class Coin : MonoBehaviour {
        [Header("Settings")]
        [SerializeField] private int _coinValue = 1;
        [SerializeField] private float _rotationSpeed = 100f;
        
        private void Update() {
            transform.Rotate(Vector3.up, _rotationSpeed * Time.deltaTime);
        }
        
        private void OnTriggerEnter(Collider other) {
            if (other.GetComponent<PLAYER.PlayerController>() != null) {
                CoinManager.Instance.AddCoins(_coinValue);
                Destroy(gameObject);
            }
        }
    }
}
