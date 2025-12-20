using UnityEngine;
using UnityEngine.SceneManagement;

namespace PROJECT_CUBE.LEVEL {
    public class LevelManager : MonoBehaviour {
        private static LevelManager _instance;
        public static LevelManager Instance {
            get {
                if (_instance == null) {
                    GameObject go = new GameObject("LevelManager");
                    _instance = go.AddComponent<LevelManager>();
                    DontDestroyOnLoad(go);
                }
                return _instance;
            }
        }
        
        private int _currentUnlockedLevel = 999;
        
        public int CurrentUnlockedLevel => _currentUnlockedLevel;
        
        private void Awake() {
            if (_instance != null && _instance != this) {
                Destroy(gameObject);
                return;
            }
            _instance = this;

            gameObject.transform.SetParent(null);

            DontDestroyOnLoad(gameObject);
        }
        
        public void CompleteLevel(int levelNumber) {
            PlayerDebugManager.Instance?.AddLine($"Fase {levelNumber} completada!", "LevelManager");
        }
        
        public bool IsLevelUnlocked(int levelNumber) {
            return levelNumber <= _currentUnlockedLevel;
        }
        
        public void LoadLevel(int levelNumber) {
            if (IsLevelUnlocked(levelNumber)) {
                // Reseta as moedas quando começar da fase 0
                if (levelNumber == 0) {
                    COLLECTIBLES.CoinManager.Instance.ResetCoins();
                    PlayerDebugManager.Instance?.AddLine("Moedas resetadas (Fase 0 iniciada)", "LevelManager");
                }
                
                SceneManager.LoadScene($"Level {levelNumber}");
            } else {
                PlayerDebugManager.Instance?.AddLine($"Fase {levelNumber} está bloqueada!", "LevelManager");
            }
        }
        
        public void LoadLevelSelect() {
            SceneManager.LoadScene("Menu");
        }
        
        public void ResetProgress() {
            // Todos os níveis estão sempre desbloqueados
        }

        public void Quit() {
            Application.Quit();
        }
    }
}
