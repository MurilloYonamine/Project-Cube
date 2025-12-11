using UnityEngine;

namespace PROJECT_CUBE.HAZARDS {
    public class CannonProjectile : MonoBehaviour {
        [SerializeField] private int _damage = 1;
        [SerializeField] private float _lifetime = 10f;
        
        private Vector3 _direction;
        private float _speed;
        
        public void Initialize(Vector3 direction, float speed) {
            _direction = direction.normalized;
            _speed = speed;
            Destroy(gameObject, _lifetime);
        }
        
        private void Update() {
            transform.position += _direction * _speed * Time.deltaTime;
        }
        
        private void OnTriggerEnter(Collider other) {
            PLAYER.PlayerController player = other.GetComponent<PLAYER.PlayerController>();
            if (player != null) {
                PlayerHealthSystem health = player.GetComponent<PlayerHealthSystem>();
                if (health != null) {
                    health.TakeDamage(_damage);
                }
                Destroy(gameObject);
                return;
            }
            
            if (!other.isTrigger) {
                Destroy(gameObject);
            }
        }
    }
}
