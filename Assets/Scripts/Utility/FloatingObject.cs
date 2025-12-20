using UnityEngine;

namespace PROJECT_CUBE.UTILITY {
    public class FloatingObject : MonoBehaviour {
        [Header("Floating Settings")]
        [SerializeField] private float _floatSpeed = 2f;
        [SerializeField] private float _floatHeight = 1f;
        
        private Vector3 _startPosition;
        
        private void Start() {
            _startPosition = transform.position;
        }
        
        private void Update() {
            float newY = _startPosition.y + Mathf.Sin(Time.time * _floatSpeed) * _floatHeight;
            transform.position = new Vector3(_startPosition.x, newY, _startPosition.z);
        }
    }
}
