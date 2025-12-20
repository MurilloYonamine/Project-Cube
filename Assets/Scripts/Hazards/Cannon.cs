using UnityEngine;

namespace PROJECT_CUBE.HAZARDS {
    public class Cannon : MonoBehaviour {
        [Header("References")]
        [SerializeField] private Transform _barrelTip;
        [SerializeField] private GameObject _projectilePrefab;
        
        [Header("Shooting Settings")]
        [SerializeField] private float _shootInterval = 2f;
        [SerializeField] private float _projectileSpeed = 10f;
        
        [Header("Rotation Settings")]
        [SerializeField] private float _rotationSpeed = 45f;
        [SerializeField] private float _rotationAmount = 90f;
        
        private float _shootTimer;
        private float _currentRotation = 0f;
        private int _rotationDirection = 1;
        
        private void Start() {
            if (_barrelTip == null) {
                _barrelTip = transform;
            }
        }
        
        private void Update() {
            RotateCannon();
            
            _shootTimer += Time.deltaTime;
            if (_shootTimer >= _shootInterval) {
                Shoot();
                _shootTimer = 0f;
            }
        }
        
        private void RotateCannon() {
            float rotationThisFrame = _rotationSpeed * _rotationDirection * Time.deltaTime;
            _currentRotation += rotationThisFrame;
            
            if (Mathf.Abs(_currentRotation) >= _rotationAmount) {
                _rotationDirection *= -1;
                _currentRotation = Mathf.Clamp(_currentRotation, -_rotationAmount, _rotationAmount);
            }
            
            transform.localRotation = Quaternion.Euler(0, _currentRotation, 0);
        }
        
        private void Shoot() {
            if (_projectilePrefab == null) return;
            
            GameObject projectile = Instantiate(_projectilePrefab, _barrelTip.position, _barrelTip.rotation);
            
            CannonProjectile projectileScript = projectile.GetComponent<CannonProjectile>();
            if (projectileScript != null) {
                projectileScript.Initialize(Vector3.up, _projectileSpeed);
            }
            
            AudioManager.Instance?.PlaySound("Cannon");
            
            PlayerDebugManager.Instance?.AddLine("Canhão atirou!", "Cannon");
        }
        
        private void OnDrawGizmos() {
            if (_barrelTip == null) return;
            
            Gizmos.color = Color.red;
            Gizmos.DrawLine(_barrelTip.position, _barrelTip.position + _barrelTip.forward * 2f);
            
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(_barrelTip.position, 0.2f);
        }
    }
}
