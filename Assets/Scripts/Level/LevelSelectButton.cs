using UnityEngine;

namespace PROJECT_CUBE.LEVEL {
    public class LevelSelectButton : MonoBehaviour {
        [SerializeField] private int _levelNumber;
        [SerializeField] private GameObject _lockedIcon;
        [SerializeField] private GameObject _unlockedIcon;
        
        private void Start() {
            UpdateButtonState();
        }
        
        private void UpdateButtonState() {
            bool isUnlocked = LevelManager.Instance.IsLevelUnlocked(_levelNumber);
            
            if (_lockedIcon) _lockedIcon.SetActive(!isUnlocked);
            if (_unlockedIcon) _unlockedIcon.SetActive(isUnlocked);
            
            var button = GetComponent<UnityEngine.UI.Button>();
            if (button) button.interactable = isUnlocked;
        }
        
        public void OnButtonClicked() {
            LevelManager.Instance.LoadLevel(_levelNumber);
        }
    }
}
