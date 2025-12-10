using UnityEngine;
using System.Collections.Generic;

namespace PROJECT_CUBE.LEVEL_GENERATION {
    public enum CubeType {
        Cube = 0,
        Player = 1
    }

    [CreateAssetMenu(fileName = "New Cube Palette", menuName = "Level Generation/Cube Palette")]
    public class CubePalette : ScriptableObject {
        [System.Serializable]
        public class CubeTypeData {
            [Tooltip("Nome descritivo deste tipo de cubo")]
            [SerializeField] public string _typeName = "New Cube";
            [Tooltip("Prefab que será instanciado")]
            [SerializeField] private GameObject _prefab;
            [Tooltip("Tipo de cubo")]
            [SerializeField] private CubeType _cubeType = CubeType.Cube;

            public string TypeName {
                get => _typeName;
                set => _typeName = value;
            }

            public GameObject Prefab {
                get => _prefab;
                set => _prefab = value;
            }

            public CubeType CubeType {
                get => _cubeType;
                set => _cubeType = value;
            }
        }

        [SerializeField] private CubeTypeData[] _cubeTypes = new CubeTypeData[2];

        #region Properties
        public CubeTypeData[] CubeTypes => _cubeTypes;

        public CubeTypeData GetCubeTypeData(int index) {
            if (index >= 0 && index < _cubeTypes.Length)
                return _cubeTypes[index];
            return null;
        }

        public GameObject GetPrefab(int typeIndex) {
            var data = GetCubeTypeData(typeIndex);
            return data?.Prefab;
        }

        public CubeType GetCubeType(int typeIndex) {
            var data = GetCubeTypeData(typeIndex);
            return data?.CubeType ?? CubeType.Cube;
        }
        #endregion

        #region Public Methods
        public void Initialize() {
            if (_cubeTypes == null || _cubeTypes.Length == 0) {
                _cubeTypes = new CubeTypeData[2];
                _cubeTypes[0] = new CubeTypeData { _typeName = "Cube" };
                _cubeTypes[1] = new CubeTypeData { _typeName = "Player Spawn" };
            }
        }
        #endregion
    }
}
