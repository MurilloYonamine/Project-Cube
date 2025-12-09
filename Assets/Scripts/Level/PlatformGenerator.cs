using UnityEngine;

namespace PROJECT_CUBE.LEVEL {
    [RequireComponent(typeof(PlatformPool))]
    public class PlatformGenerator : MonoBehaviour {

        [Header("Generation Settings")]
        [SerializeField] private int amount = 20;

        [Header("Offsets (Ranges)")]
        [SerializeField] private Vector2Int xOffsetRange;
        [SerializeField] private Vector2Int yOffsetRange;

        [Header("Start Position")]
        [SerializeField] private Vector2 startPosition = Vector2.zero;

        private PlatformPool _platformPool;
        [SerializeField] private Transform _platformSpawnParent;

        private void Awake() {
            _platformPool = GetComponent<PlatformPool>();
        }

        private void Start() {
            GeneratePlatforms();
        }

        private void GeneratePlatforms() {
            Vector2 currentPos = startPosition;

            for (int i = 0; i < amount; i++) {

                GameObject platform = _platformPool.GetPlatform(_platformSpawnParent);

                platform.transform.localPosition = new Vector3(
                    currentPos.x,
                    currentPos.y,
                    0f
                );

                currentPos.x = Random.Range(xOffsetRange.x, xOffsetRange.y);

                currentPos.y = Random.Range(yOffsetRange.x, yOffsetRange.y);

                PlatformInfo info = platform.GetComponent<PlatformInfo>();
                info.ChoosePrefabs(PickPlatformType());
            }
        }


        private PlatformType PickPlatformType() {
            return (PlatformType)Random.Range(0, 4);
        }
    }
}
