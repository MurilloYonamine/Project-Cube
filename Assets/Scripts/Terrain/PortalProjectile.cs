using UnityEngine;

namespace PROJECT_CUBE.TERRAIN {
    public class PortalProjectile : MonoBehaviour {
        [SerializeField] private ElevatorManager _elevatorManager;
        
        private void OnTriggerEnter(Collider collision) {
            if (_elevatorManager != null) {
                _elevatorManager.OnProjectileTrigger(collision);
            }
        }
    }
}
