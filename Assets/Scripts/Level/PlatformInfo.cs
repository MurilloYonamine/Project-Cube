using System;
using PROJECT_CUBE.ATTRIBUTES;
using UnityEngine;

namespace PROJECT_CUBE.LEVEL {

    public class PlatformInfo : MonoBehaviour {
        [Header("Platform Info")]
        [SerializeField, ReadOnly] private int _platformID;
        [SerializeField, ReadOnly] private Vector2 coordinates;

        [Header("Platform Renderer")]
        [SerializeField] private MeshFilter _platformMesh;
        [SerializeField] private MeshRenderer _platformRenderer;
        [SerializeField] private MeshCollider _platformCollider;

        [Serializable]
        private class PlatFormObjects {
            [field: SerializeField] public GameObject RightPlatform { get; private set; }
            [field: SerializeField] public GameObject LeftPlatform { get; private set; }
            [field: SerializeField] public GameObject ConnectingPlatform { get; private set; }
            [field: SerializeField] public GameObject UniquePlatform { get; private set; }

            public Mesh GetMesh(GameObject platform) {
                return platform.GetComponent<MeshFilter>().sharedMesh;
            }
            public Material GetMaterial(GameObject platform) {
                return platform.GetComponent<MeshRenderer>().sharedMaterial;
            }
        }
        [SerializeField] private PlatFormObjects _platforms;
        public void ChoosePrefabs(PlatformType platformType) {
            switch (platformType) {
                case PlatformType.Right:
                    AssignPlatformRenderer(_platforms.RightPlatform);
                    gameObject.name = $"platform_({coordinates.x}, {coordinates.y}) Right Platform";
                    break;
                case PlatformType.Left:
                    AssignPlatformRenderer(_platforms.LeftPlatform);
                    gameObject.name = $"platform_({coordinates.x}, {coordinates.y}) Left Platform";
                    break;
                case PlatformType.Connecting:
                    AssignPlatformRenderer(_platforms.ConnectingPlatform);
                    gameObject.name = $"platform_({coordinates.x}, {coordinates.y}) Connecting Platform";
                    break;
                case PlatformType.Unique:
                    AssignPlatformRenderer(_platforms.UniquePlatform);
                    gameObject.name = $"platform_({coordinates.x}, {coordinates.y}) Unique Platform";
                    break;
                default:
                    AssignPlatformRenderer(_platforms.UniquePlatform);
                    gameObject.name = $"platform_({coordinates.x}, {coordinates.y}) Unique Platform";
                    break;
            }
        }
        private void AssignPlatformRenderer(GameObject platform) {
            _platformRenderer.material = _platforms.GetMaterial(platform);
            _platformMesh.mesh = _platforms.GetMesh(platform);
            _platformCollider.sharedMesh = _platforms.GetMesh(platform);
        }
        public void AssignPlatformCoordinates(int x, int y) {
            coordinates = new Vector2(x, y);
        }
        public void AssignPlatformID(int id) {
            _platformID = id;
        }
    }
}