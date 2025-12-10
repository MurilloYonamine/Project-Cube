using PROJECT_CUBE.PLAYER;
using UnityEngine;

namespace PROJECT_CUBE.TERRAIN {
    public class TerrainBlock : MonoBehaviour {
        [SerializeField] private TerrainType _terrainType = TerrainType.BlueTerrain;
        
        private void OnCollisionEnter(Collision collision) {
            PlayerController playerController = collision.gameObject.GetComponent<PlayerController>();
            if (playerController != null) {
                playerController.EnterTerrain(_terrainType);
            }
        }

        private void OnCollisionExit(Collision collision) {
            PlayerController playerController = collision.gameObject.GetComponent<PlayerController>();
            if (playerController != null) {
                playerController.ExitTerrain();
            }
        }
    }
}
