using PROJECT_CUBE.PLAYER;
using UnityEngine;

namespace PROJECT_CUBE.TERRAIN {
    public class TerrainBlock : MonoBehaviour {
        [SerializeField] private TerrainType _terrainType = TerrainType.BlueTerrain;
        private bool _isPlayerOnTerrain = false;
        
        private void OnCollisionEnter(Collision collision) {
            PlayerController playerController = collision.gameObject.GetComponent<PlayerController>();
            if (playerController != null) {
                playerController.EnterTerrain(_terrainType);
                _isPlayerOnTerrain = true;
                PlayTerrainSound(true);
            }
        }

        private void OnCollisionExit(Collision collision) {
            PlayerController playerController = collision.gameObject.GetComponent<PlayerController>();
            if (playerController != null) {
                playerController.ExitTerrain();
                _isPlayerOnTerrain = false;
                PlayTerrainSound(false);
            }
        }
        
        private void PlayTerrainSound(bool isEntering) {
            switch (_terrainType) {
                case TerrainType.BlueTerrain:
                    if (isEntering) {
                        AudioManager.Instance?.PlaySound("BlueTerrain");
                    } else {
                        AudioManager.Instance?.StopSound("BlueTerrain");
                    }
                    break;
                    
                case TerrainType.RedTerrain:
                    if (isEntering) {
                        AudioManager.Instance?.PlaySound("RedTerrain");
                    } else {
                        AudioManager.Instance?.StopSound("RedTerrain");
                    }
                    break;
                    
                case TerrainType.OrangeBlock:
                    if (isEntering) {
                        AudioManager.Instance?.PlaySound("OrangeTerrain");
                    }
                    break;
            }
        }
    }
}
