using PROJECT_CUBE.PLAYER;
using UnityEngine;

namespace PROJECT_CUBE.TERRAIN {
    public class Portal : MonoBehaviour {
        [SerializeField] private PortalManager _portalManager;

        private void OnCollisionEnter(Collision collision) {
            PlayerController playerController = collision.gameObject.GetComponent<PlayerController>();
            if (playerController != null && _portalManager != null) {
                playerController.EnterTerrain(TerrainType.Portal);
                _portalManager.StartTransport(playerController, transform);
            }
        }
    }
}
