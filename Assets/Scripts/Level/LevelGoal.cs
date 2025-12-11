using UnityEngine;

namespace PROJECT_CUBE.LEVEL {
    public class LevelGoal : MonoBehaviour {
        [SerializeField] private int _currentLevelNumber;
        
        private void OnTriggerEnter(Collider other) {
            if (other.GetComponent<PLAYER.PlayerController>() != null) {
                LevelManager.Instance.CompleteLevel(_currentLevelNumber);
                LevelManager.Instance.LoadLevelSelect();
            }
        }
    }
}
