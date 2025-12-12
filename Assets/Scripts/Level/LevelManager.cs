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
        
        private const string LEVEL_KEY = "UnlockedLevel";
        private int _currentUnlockedLevel = 1;
        
        public int CurrentUnlockedLevel => _currentUnlockedLevel;
        
        private void Awake() {
            if (_instance != null && _instance != this) {
                Destroy(gameObject);
                return;
            }
            _instance = this;

            gameObject.transform.SetParent(null);

            DontDestroyOnLoad(gameObject);
            LoadProgress();
        }
        
        public void CompleteLevel(int levelNumber) {
            if (levelNumber >= _currentUnlockedLevel) {
                _currentUnlockedLevel = levelNumber + 1;
                SaveProgress();
                PlayerDebugManager.Instance?.AddLine($"Fase {levelNumber} completada! Fase {_currentUnlockedLevel} desbloqueada", "LevelManager");
            }
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
            SceneManager.LoadScene("LevelSelect");
        }
        
        private void SaveProgress() {
            PlayerPrefs.SetInt(LEVEL_KEY, _currentUnlockedLevel);
            PlayerPrefs.Save();
        }
        
        private void LoadProgress() {
            _currentUnlockedLevel = PlayerPrefs.GetInt(LEVEL_KEY, 1);
        }
        
        public void ResetProgress() {
            _currentUnlockedLevel = 1;
            SaveProgress();
        }

        public void Quit() {
            Application.Quit();
        }
    }
}
