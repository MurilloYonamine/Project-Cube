using UnityEngine;

namespace PROJECT_CUBE.UTILITY {
    public class RotatingObject : MonoBehaviour {
        [Header("Rotation Settings")]
        [SerializeField] private Vector3 _rotationAxis = Vector3.up;
        [SerializeField] private float _rotationSpeed = 45f;
        
        private void Update() {
            transform.Rotate(_rotationAxis * _rotationSpeed * Time.deltaTime);
        }
    }
}
