using UnityEngine;
using PROJECT_CUBE.PLAYER;

namespace PROJECT_CUBE.TERRAIN {
    public class PortalManager : MonoBehaviour {
        [SerializeField] private Transform _pointA;
        [SerializeField] private Transform _pointB;
        [SerializeField] private float _transportDuration = 2f;

        private PlayerController _currentPlayer;
        private bool _isTransporting = false;
        private float _transportTimer = 0f;
        private Transform _targetPoint;

        private void Update() {
            if (_isTransporting && _currentPlayer != null) {
                _transportTimer += Time.deltaTime;
                float progress = Mathf.Clamp01(_transportTimer / _transportDuration);

                Vector3 startPos = _currentPlayer.transform.position;
                Vector3 targetPos = _targetPoint.position;
                _currentPlayer.transform.position = Vector3.Lerp(startPos, targetPos, progress);

                if (progress >= 1f) {
                    _isTransporting = false;
                    _currentPlayer.ResumeMovement();
                }
            }
        }

        public void StartTransport(PlayerController player, Transform currentPoint) {
            if (_isTransporting) return;

            _currentPlayer = player;
            _isTransporting = true;
            _transportTimer = 0f;

            _targetPoint = currentPoint == _pointA ? _pointB : _pointA;

            _currentPlayer.StopMovement();
        }

        public Transform GetPointA() => _pointA;
        public Transform GetPointB() => _pointB;

        private void OnDrawGizmosSelected() {
            if (_pointA != null && _pointB != null) {
                Gizmos.color = Color.cyan;
                Gizmos.DrawLine(_pointA.position, _pointB.position);
                Gizmos.DrawWireSphere(_pointA.position, 0.3f);
                Gizmos.DrawWireSphere(_pointB.position, 0.3f);
            }
        }
    }
}
