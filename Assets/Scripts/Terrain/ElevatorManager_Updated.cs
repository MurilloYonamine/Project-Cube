using UnityEngine;

namespace PROJECT_CUBE.TERRAIN {
    public class ElevatorManager : MonoBehaviour {
        public enum PortalDirection { Vertical, Horizontal }

        [Header("Configuração dos Portais")]
        [SerializeField] private Transform _portalA;
        [SerializeField] private Transform _portalB;
        [SerializeField] private PortalDirection _direction = PortalDirection.Vertical;

        [Header("Configuração do Projétil")]
        [SerializeField] private Transform _projectile;

        [Header("Configurações de Movimento")]
        [SerializeField] private float _moveSpeed = 5f; 
        [SerializeField] private float _teleportOffset = 0.1f;

        private Vector3 _currentVelocity = Vector3.zero;
        private bool _canTeleport = true;

        private void Start() {
            if (_projectile != null) {
                _projectile.gameObject.SetActive(true);
                _projectile.position = _portalA.position;
                _currentVelocity = Vector3.zero;
            }
        }

        private void Update() {
            if (_projectile == null) return;

            if (_direction == PortalDirection.Vertical) {
                _currentVelocity.y -= _moveSpeed * Time.deltaTime;
                _projectile.position += _currentVelocity * Time.deltaTime;

                if (_projectile.position.y < _portalB.position.y && _canTeleport) {
                    TeleportToPortal(_portalA);
                }
            } else {
                _currentVelocity.x = _moveSpeed;
                _projectile.position += _currentVelocity * Time.deltaTime;

                if (_projectile.position.x > _portalB.position.x && _canTeleport) {
                    TeleportToPortal(_portalA);
                }
            }
        }

        public void OnProjectileTrigger(Collider collision) {
            if (collision.transform == _portalA) {
                if (_direction == PortalDirection.Horizontal) {
                    TeleportToPortal(_portalB);
                }
            } else if (collision.transform == _portalB) {
                if (_direction == PortalDirection.Horizontal) {
                    TeleportToPortal(_portalA);
                } else if (_direction == PortalDirection.Vertical) {
                    TeleportToPortal(_portalA);
                }
            }
        }

        private void TeleportToPortal(Transform destination) {
            if (!_canTeleport) return;

            _canTeleport = false;

            if (_direction == PortalDirection.Vertical) {
                _projectile.position = destination.position + (destination.up * _teleportOffset);
            } else {
                _projectile.position = destination.position + (destination.right * _teleportOffset);
            }
            
            _currentVelocity = Vector3.zero;
            
            // Tocar som de portal
            AudioManager.Instance?.PlaySound("Portal");

            Invoke(nameof(EnableTeleport), 0.2f);
        }

        private void EnableTeleport() {
            _canTeleport = true;
        }

        private void OnDrawGizmos() {
            if (_portalA != null && _portalB != null) {
                Gizmos.color = Color.green;
                Gizmos.DrawLine(_portalA.position, _portalB.position);
                
                Gizmos.color = Color.yellow;
                Gizmos.DrawWireSphere(_portalA.position, 0.5f);
                Gizmos.DrawWireSphere(_portalB.position, 0.5f);
                
                Gizmos.color = Color.blue;
                Gizmos.DrawWireSphere(_portalA.position, 0.3f);
                Gizmos.DrawWireSphere(_portalB.position, 0.3f);
            }
        }
    }
}
