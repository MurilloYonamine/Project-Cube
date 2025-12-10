using UnityEngine;
using Unity.Cinemachine;

namespace PROJECT_CUBE.LEVEL_GENERATION {
    /// <summary>
    /// Helper para configurar Cinemachine no Play Mode
    /// </summary>
    public class CinemachineSetup : MonoBehaviour {
        private GameObject _spawnedPlayer;

        private void Start() {
            // Aguarda um frame para garantir que o Player foi criado
            StartCoroutine(SetupCinemachineNextFrame());
        }

        private System.Collections.IEnumerator SetupCinemachineNextFrame() {
            // Aguarda alguns frames para garantir que o Player foi criado
            yield return new WaitForSeconds(0.1f);

            // Encontra o Player na cena
            _spawnedPlayer = GameObject.FindWithTag("Player");
            if (_spawnedPlayer == null) {
                Debug.LogWarning("Player não encontrado na cena! Procurando por nome...");
                // Tenta encontrar pelo nome se não tiver tag
                var player = GameObject.Find("Player");
                if (player != null) {
                    _spawnedPlayer = player;
                    Debug.Log("Player encontrado pelo nome!");
                }
                else {
                    Debug.LogError("Nenhum Player encontrado na cena!");
                    Destroy(this);
                    yield break;
                }
            }

            Debug.Log($"Player encontrado: {_spawnedPlayer.name}");

            // Procura por Cinemachine Virtual Camera
            var virtualCamera = FindFirstObjectByType<CinemachineVirtualCamera>();
            if (virtualCamera != null) {
                virtualCamera.Follow = _spawnedPlayer.transform;
                virtualCamera.LookAt = _spawnedPlayer.transform;
                Debug.Log("Cinemachine configurada para seguir o Player");
            }
            else {
                Debug.LogWarning("Nenhuma Cinemachine Virtual Camera encontrada!");
            }

            // Adiciona Cinemachine Brain na Main Camera
            var mainCamera = Camera.main;
            if (mainCamera != null) {
                var cinemachineBrain = mainCamera.GetComponent<CinemachineBrain>();
                if (cinemachineBrain == null) {
                    cinemachineBrain = mainCamera.gameObject.AddComponent<CinemachineBrain>();
                    Debug.Log("Cinemachine Brain adicionado à Main Camera");
                }
            }
            else {
                Debug.LogWarning("Main Camera não encontrada!");
            }

            // Remove este script após completar a setup
            Destroy(this);
        }
    }
}

