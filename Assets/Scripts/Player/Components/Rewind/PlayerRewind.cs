using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PROJECT_CUBE.PLAYER.COMPONENTS.REWIND {
    [Serializable]
    public class PlayerRewind : PlayerComponent {

        [Header("Settings")]
        [SerializeField] private GhostEntity _ghostPrefab;
        [SerializeField] private float _snapshotInterval = 5f;
        [SerializeField] private float _rewindSpeed = 20f;
        [SerializeField] private int _maxSnapshots = 5;

        private List<GhostEntity> _pastGhosts;
        private GhostEntity _currentActiveGhost;
        
        private float _snapshotTimer;
        private bool _isRewinding;
        private bool _shouldCreateNewGhost;
        private float _ghostCreationTime;

        public override void AwakeComponent() {
            _pastGhosts = new List<GhostEntity>();
            _shouldCreateNewGhost = true;
            StartNewGhostCycle();
        }

        public override void FixedUpdateComponent() {
            if (_isRewinding) return;

            if (_currentActiveGhost == null && _shouldCreateNewGhost) {
                StartNewGhostCycle();
                _shouldCreateNewGhost = false;
            }

            if (_currentActiveGhost != null) {
                _snapshotTimer += Time.fixedDeltaTime;

                if (_snapshotTimer >= _snapshotInterval) {
                    Snapshot(); 
                    _snapshotTimer = 0f;
                }
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

            if (_pastGhosts.Count > _maxSnapshots) {
                GhostEntity oldestGhost = _pastGhosts[0];
                _pastGhosts.RemoveAt(0);
                if (oldestGhost != null) UnityEngine.Object.Destroy(oldestGhost.gameObject);
                PlayerDebugManager.Instance.AddLine($"Ghost mais antigo removido", "PlayerRewind");
            }

            _currentActiveGhost = null;
            StartNewGhostCycle();
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
            
            PlayerDebugManager.Instance.AddLine($"Novo ghost criado", "PlayerRewind");
        }

        private void TriggerRewind() {
            if (_isRewinding) return;
            
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
            if (rigidbody) rigidbody.linearVelocity = Vector3.zero;

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
            
            _playerController.PlayerMovement.enabled = true;
            _isRewinding = false;
            
            _shouldCreateNewGhost = true;
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