using System.Collections.Generic;
using UnityEngine;

namespace PROJECT_CUBE.PLAYER.COMPONENTS.REWIND {
    [RequireComponent(typeof(TrailRenderer), typeof(MeshRenderer))]
    public class GhostEntity : MonoBehaviour {
        
        [Header("Components")]
        [SerializeField] private TrailRenderer _trail;
        [SerializeField] private MeshRenderer _mesh;

        private Vector3[] _trailBuffer;
        private List<Quaternion> _rotationBuffer = new List<Quaternion>();
        private bool _isRecording;

        public SnapshotData Data;

        private void Update() {
            if (_isRecording) {
                _rotationBuffer.Add(transform.rotation);
            }
        }

        public void InitializeGhost(SnapshotData data) {
            Data = data;

            if (_mesh) _mesh.enabled = false;

            if (_trail) {
                _trail.time = Mathf.Infinity;
                _trail.emitting = true;
                _trail.Clear();
            }

            _rotationBuffer.Clear();
            _isRecording = true;
        }

        public void DetachAndFreeze() {
            if (_mesh) _mesh.enabled = true;
            if (_trail) _trail.emitting = false;

            transform.SetParent(null);
            _isRecording = false;
        }

        public List<Vector3> GetReversePath() {
            if (_trail == null) return new List<Vector3>();

            _trailBuffer = new Vector3[_trail.positionCount];
            int count = _trail.GetPositions(_trailBuffer);

            List<Vector3> reversePath = new List<Vector3>();
            for (int i = count - 1; i >= 0; i--) {
                reversePath.Add(_trailBuffer[i]);
            }
            return reversePath;
        }

        public List<Quaternion> GetReverseRotation() {
            List<Quaternion> reverseRotation = new List<Quaternion>(_rotationBuffer);
            reverseRotation.Reverse();
            return reverseRotation;
        }
    }
}