using UnityEngine;

namespace PROJECT_CUBE.COLLECTIBLES {
    public class CoinManager : MonoBehaviour {
        private static CoinManager _instance;
        public static CoinManager Instance {
            get {
                if (_instance == null) {
                    GameObject go = new GameObject("CoinManager");
                    _instance = go.AddComponent<CoinManager>();
                    DontDestroyOnLoad(go);
                }
                return _instance;
            }
        }
        
        private const string COIN_KEY = "TotalCoins";
        [SerializeField] private int _currentCoins = 0;
        
        public int CurrentCoins => _currentCoins;
        
        private void Awake() {
            if (_instance != null && _instance != this) {
                Destroy(gameObject);
                return;
            }
            _instance = this;
            DontDestroyOnLoad(gameObject);
            LoadCoins();
        }
        
        public void AddCoins(int amount) {
            _currentCoins += amount;
            SaveCoins();
            PlayerDebugManager.Instance?.AddLine($"Moedas coletadas: {_currentCoins}", "CoinManager");
        }
        
        public bool SpendCoins(int amount) {
            if (_currentCoins >= amount) {
                _currentCoins -= amount;
                SaveCoins();
                return true;
            }
            return false;
        }
        
        private void SaveCoins() {
            PlayerPrefs.SetInt(COIN_KEY, _currentCoins);
            PlayerPrefs.Save();
        }
        
        private void LoadCoins() {
            _currentCoins = PlayerPrefs.GetInt(COIN_KEY, 0);
        }
        
        public void ResetCoins() {
            _currentCoins = 0;
            SaveCoins();
        }
    }
}
