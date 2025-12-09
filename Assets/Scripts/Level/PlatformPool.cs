using PROJECT_CUBE.ATTRIBUTES;
using UnityEngine;

namespace PROJECT_CUBE.LEVEL {
    public class PlatformPool : MonoBehaviour {
        [Header("Pool Settings")]
        [SerializeField] private GameObject _platformPrefab;
        [SerializeField, ReadOnly] private int _poolSize;
        [SerializeField] private Transform _poolParent;

        private GameObject[] _platforms;
        private int _currentIndex = 0;

        private void Awake() {
            InitializePool();
        }

        private void InitializePool() {
            _platforms = new GameObject[_poolSize];
            for (int i = 0; i < _poolSize; i++) {
                _platforms[i] = Instantiate(_platformPrefab, _poolParent);
                _platforms[i].SetActive(false);
            }
        }
        public GameObject GetPlatform(Transform parent) {
            GameObject platform = _platforms[_currentIndex];
            _currentIndex = (_currentIndex + 1) % _poolSize;

            platform.SetActive(true);
            platform.transform.SetParent(parent);
            
            return platform;
        }

        public void ReturnPlatform(GameObject platform) {
            platform.SetActive(false);
            platform.transform.SetParent(_poolParent);
        }
        public void ReturnAllPlatforms() {
            foreach (var platform in _platforms) {
                ReturnPlatform(platform);
            }
        }
    }
}