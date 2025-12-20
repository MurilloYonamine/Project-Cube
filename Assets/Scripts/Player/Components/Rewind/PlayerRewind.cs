using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PROJECT_CUBE.PLAYER.COMPONENTS.REWIND {
    [Serializable]
    public class PlayerRewind : PlayerComponent {

        [Header("Settings")]
        [SerializeField] private GhostEntity _ghostPrefab;
        [SerializeField] private float _rewindSpeed = 20f;
        [SerializeField] private int _maxSnapshots = 5;
        [Tooltip("Delay after landing before creating a snapshot (seconds)")]
        [SerializeField] private float _landingSnapshotDelay = 0.25f;

        private List<GhostEntity> _pastGhosts;
        private GhostEntity _currentActiveGhost;

        private float _snapshotTimer;
        private bool _previousIsGrounded = false;
        private bool _landingSnapshotScheduled = false;
        private Coroutine _landingSnapshotCoroutine;
        private bool _isRewinding;
        private bool _shouldCreateNewGhost;
        private float _ghostCreationTime;

        public override void AwakeComponent() {
            _pastGhosts = new List<GhostEntity>();
            // Não criar um ghost imediatamente aqui — aguardar até o jogador estar no chão.
            _shouldCreateNewGhost = true;
        }

        public override void FixedUpdateComponent() {
            if (_isRewinding) return;

            if (_currentActiveGhost == null && _shouldCreateNewGhost && _playerController.PlayerMovement.IsGrounded) {
                StartNewGhostCycle();
                _shouldCreateNewGhost = false;
            }

            if (_currentActiveGhost != null) {
                bool isGrounded = _playerController.PlayerMovement.IsGrounded;

                // detect landing (was not grounded, now grounded)
                if (!_previousIsGrounded && isGrounded) {
                    if (!_landingSnapshotScheduled) {
                        _landingSnapshotScheduled = true;
                        if (_landingSnapshotCoroutine != null)
                            _playerController.StopCoroutine(_landingSnapshotCoroutine);
                        _landingSnapshotCoroutine =
                        _playerController.StartCoroutine(LandingSnapshotRoutine());
                    }
                }

                _previousIsGrounded = isGrounded;
            }

            PlayerDebugManager.Instance.AddLine($"{GetAvailableRewinds()}/{_maxSnapshots} rewinds disponíveis", "PlayerRewind");
        }

        public override void OnEnableComponent() {
            PlayerInputHandler.OnRewindInput += TriggerRewind;
        }

        public override void OnDisableComponent() {
            PlayerInputHandler.OnRewindInput -= TriggerRewind;
        }

        private void Snapshot() {
            if (_currentActiveGhost == null) return;

            _currentActiveGhost.DetachAndFreeze();
            _pastGhosts.Add(_currentActiveGhost);

            PlayerDebugManager.Instance.AddLine($"Snapshot! Total ghosts: {_pastGhosts.Count}", "PlayerRewind");

            // Mantém sempre o primeiro ghost (nunca remove o índice 0)
            if (_pastGhosts.Count > _maxSnapshots) {
                // Remove o ghost no índice 1 (segundo mais antigo), mantendo o primeiro
                if (_pastGhosts.Count > 1) {
                    GhostEntity ghostToRemove = _pastGhosts[1];
                    _pastGhosts.RemoveAt(1);
                    if (ghostToRemove != null) UnityEngine.Object.Destroy(ghostToRemove.gameObject);
                    PlayerDebugManager.Instance.AddLine($"Ghost antigo removido (primeiro mantido)", "PlayerRewind");
                }
            }

            _currentActiveGhost = null;
            StartNewGhostCycle();
        }

        private IEnumerator LandingSnapshotRoutine() {
            yield return new WaitForSeconds(_landingSnapshotDelay);
            Snapshot();
            _landingSnapshotScheduled = false;
            _landingSnapshotCoroutine = null;
        }

        private void StartNewGhostCycle() {
            if (_ghostPrefab == null) {
                PlayerDebugManager.Instance.AddLine($"ERRO: Ghost Prefab não está configurado!", "PlayerRewind");
                return;
            }

            _currentActiveGhost = UnityEngine.Object.Instantiate(_ghostPrefab, _playerController.transform);
            _ghostCreationTime = Time.time;

            _currentActiveGhost.transform.localPosition = Vector3.zero;
            _currentActiveGhost.transform.localRotation = Quaternion.identity;

            SnapshotData newData = new SnapshotData {
                position = _playerController.transform.position,
                rotation = _playerController.transform.rotation,
                ghostInstance = _currentActiveGhost.gameObject
            };

            _currentActiveGhost.InitializeGhost(newData);

            if (PlayerDebugManager.Instance != null) {
                PlayerDebugManager.Instance.AddLine($"Novo ghost criado", "PlayerRewind");
            }
        }

        private void TriggerRewind() {
            if (_isRewinding) return;
            
            // Verifica se o PlayerController ainda existe
            if (_playerController == null) {
                PlayerDebugManager.Instance?.AddLine("PlayerController foi destruído!", "PlayerRewind");
                return;
            }

            if (_currentActiveGhost != null || _pastGhosts.Count > 0) {
                _playerController.StartCoroutine(RewindRoutine());
            }
        }

        private int GetAvailableRewinds() {
            int availableRewinds = _pastGhosts.Count;
            if (_currentActiveGhost != null && Time.time - _ghostCreationTime > 0.5f) availableRewinds++;
            return availableRewinds;
        }

        private IEnumerator RewindRoutine() {
            _isRewinding = true;
            _playerController.PlayerMovement.enabled = false;

            Rigidbody rigidbody = _playerController.GetComponent<Rigidbody>();
            if (rigidbody) {
                rigidbody.linearVelocity = Vector3.zero;
                rigidbody.useGravity = false; // Desativa gravidade durante rewind
            }

            GhostEntity ghostToRewind = null;

            if (_currentActiveGhost != null) {
                bool isGhostTooYoung = (Time.time - _ghostCreationTime < 0.5f);

                if (isGhostTooYoung && _pastGhosts.Count > 0) {
                    UnityEngine.Object.Destroy(_currentActiveGhost.gameObject);
                    _currentActiveGhost = null;
                }
                else {
                    ghostToRewind = _currentActiveGhost;
                    _currentActiveGhost = null;
                }
            }

            if (ghostToRewind == null && _pastGhosts.Count > 0) {
                ghostToRewind = _pastGhosts[_pastGhosts.Count - 1];
                _pastGhosts.RemoveAt(_pastGhosts.Count - 1);
            }

            SnapshotData finalDataToRestore = new SnapshotData();
            bool hasData = false;

            if (ghostToRewind != null) {
                ghostToRewind.transform.SetParent(null);

                finalDataToRestore = ghostToRewind.Data;
                hasData = true;

                List<Vector3> pathPositions = ghostToRewind.GetReversePath();
                List<Quaternion> pathRotations = ghostToRewind.GetReverseRotation();

                yield return _playerController.StartCoroutine(FollowPathBackwards(pathPositions, pathRotations));

                UnityEngine.Object.Destroy(ghostToRewind.gameObject);
            }

            if (hasData) {
                _playerController.transform.position = finalDataToRestore.position;
                _playerController.transform.rotation = finalDataToRestore.rotation;
            }

            _snapshotTimer = 0f;

            // Reativa gravidade
            if (rigidbody != null) {
                rigidbody.useGravity = true;
            }

            _playerController.PlayerMovement.enabled = true;
            _isRewinding = false;

            _shouldCreateNewGhost = true;
        }

        public void RewindToFirstCheckpoint() {
            if (_isRewinding) return;
            
            if (_playerController == null) {
                PlayerDebugManager.Instance?.AddLine("PlayerController foi destruído!", "PlayerRewind");
                return;
            }

            // Volta para o primeiro ghost se existir
            if (_pastGhosts.Count > 0) {
                _playerController.StartCoroutine(RewindToFirstRoutine());
            }
        }

        private IEnumerator RewindToFirstRoutine() {
            _isRewinding = true;
            _playerController.PlayerMovement.enabled = false;

            Rigidbody rigidbody = _playerController.GetComponent<Rigidbody>();
            if (rigidbody) {
                rigidbody.linearVelocity = Vector3.zero;
                rigidbody.useGravity = false;
            }

            // Destroi o ghost atual se existir
            if (_currentActiveGhost != null) {
                UnityEngine.Object.Destroy(_currentActiveGhost.gameObject);
                _currentActiveGhost = null;
            }

            // Volta por TODOS os ghosts do mais recente ao mais antigo (exceto o primeiro)
            for (int i = _pastGhosts.Count - 1; i >= 1; i--) {
                GhostEntity ghost = _pastGhosts[i];
                
                if (ghost != null) {
                    ghost.transform.SetParent(null);
                    
                    List<Vector3> pathPositions = ghost.GetReversePath();
                    List<Quaternion> pathRotations = ghost.GetReverseRotation();

                    yield return _playerController.StartCoroutine(FollowPathBackwards(pathPositions, pathRotations));

                    UnityEngine.Object.Destroy(ghost.gameObject);
                }
            }

            // Agora volta pelo caminho do primeiro ghost
            if (_pastGhosts.Count > 0) {
                GhostEntity firstGhost = _pastGhosts[0];
                
                if (firstGhost != null) {
                    firstGhost.transform.SetParent(null);
                    
                    List<Vector3> pathPositions = firstGhost.GetReversePath();
                    List<Quaternion> pathRotations = firstGhost.GetReverseRotation();

                    yield return _playerController.StartCoroutine(FollowPathBackwards(pathPositions, pathRotations));

                    // Garante posição final exata
                    _playerController.transform.position = firstGhost.Data.position;
                    _playerController.transform.rotation = firstGhost.Data.rotation;
                    
                    // O primeiro ghost permanece congelado no mundo, não re-anexado ao player
                }
            }

            // Remove todos os ghosts exceto o primeiro
            for (int i = _pastGhosts.Count - 1; i >= 1; i--) {
                if (_pastGhosts[i] != null) {
                    UnityEngine.Object.Destroy(_pastGhosts[i].gameObject);
                }
                _pastGhosts.RemoveAt(i);
            }

            // Reativa gravidade
            if (rigidbody != null) {
                rigidbody.useGravity = true;
            }

            _playerController.PlayerMovement.enabled = true;
            _isRewinding = false;

            _shouldCreateNewGhost = true;

            PlayerDebugManager.Instance?.AddLine("Voltou ao primeiro checkpoint!", "PlayerRewind");
        }

        private IEnumerator FollowPathBackwards(List<Vector3> pathPositions, List<Quaternion> pathRotations) {
            int pathIndex = 0;

            float rotationsPerPathPoint = pathRotations.Count > 0 ? (float)pathRotations.Count / pathPositions.Count : 0;
            int rotationIndex = 0;

            while (pathIndex < pathPositions.Count) {
                Vector3 targetPosition = pathPositions[pathIndex];

                rotationIndex = Mathf.Min((int)(pathIndex * rotationsPerPathPoint), pathRotations.Count - 1);

                while (Vector3.Distance(_playerController.transform.position, targetPosition) > 0.1f) {
                    _playerController.transform.position = Vector3.MoveTowards(
                        _playerController.transform.position,
                        targetPosition,
                        _rewindSpeed * Time.deltaTime
                    );

                    if (rotationIndex >= 0 && rotationIndex < pathRotations.Count) {
                        _playerController.transform.rotation = Quaternion.Slerp(
                            _playerController.transform.rotation,
                            pathRotations[rotationIndex],
                            10 * Time.deltaTime
                        );
                    }

                    yield return null;
                }
                pathIndex++;
            }
        }
    }
}