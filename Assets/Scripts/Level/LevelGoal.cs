using UnityEngine;

namespace PROJECT_CUBE.LEVEL {
    public class LevelGoal : MonoBehaviour {
        [SerializeField] private int _currentLevelNumber;
        [SerializeField] private bool isLastLevel = false; // Marque true na última fase
        
        private void OnTriggerEnter(Collider other) {
            Debug.Log($"LevelGoal: Trigger entered by {other.gameObject.name} with tag {other.tag}");
            
            if (other.GetComponent<PLAYER.PlayerController>() != null) {
                Debug.Log($"LevelGoal: Player detected! Completing level {_currentLevelNumber}");
                LevelManager.Instance.CompleteLevel(_currentLevelNumber);
                
                // Se for a última fase, mostra o crash falso
                if (isLastLevel && FakeCrashSystem.Instance != null)
                {
                    Debug.Log("LevelGoal: Last level - triggering fake crash");
                    FakeCrashSystem.Instance.TriggerFakeCrash();
                }
                else
                {
                    // Carrega o próximo nível
                    int nextLevel = _currentLevelNumber + 1;
                    Debug.Log($"LevelGoal: Loading next level {nextLevel}");
                    LevelManager.Instance.LoadLevel(nextLevel);
                }
            }
            else
            {
                Debug.Log($"LevelGoal: Object has no PlayerController component");
            }
        }
    }
}
