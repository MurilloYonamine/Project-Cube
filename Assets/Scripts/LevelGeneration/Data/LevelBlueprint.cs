using UnityEngine;
using System.Collections.Generic;
using PROJECT_CUBE.ATTRIBUTES;

namespace PROJECT_CUBE.LEVEL_GENERATION {
    [CreateAssetMenu(fileName = "New Level Blueprint", menuName = "Level Generation/Blueprint")]
    public class LevelBlueprint : ScriptableObject {
        [System.Serializable]
        public class CubeData {
            public int cubeTypeIndex;
            public Vector2Int gridPosition;
            
            public CubeData() {
                cubeTypeIndex = 0;
                gridPosition = Vector2Int.zero;
            }
            
            public CubeData(int typeIndex, Vector2Int position) {
                cubeTypeIndex = typeIndex;
                gridPosition = position;
            }
        }

        [Header("Grid Configuration")]
        [Tooltip("Tamanho do grid em células (largura x altura) - Proporção 16:9 da tela")]
        [SerializeField, ReadOnly] private Vector2Int gridSize = new Vector2Int(30, 17);
        [Tooltip("Espaçamento entre cubos em unidades do mundo")]
        [SerializeField] private Vector3 cubeSpacing = new Vector3(1f, 0f, 1f);

        [Header("Level Data")]
        [Tooltip("Nome identificador do nível")]
        [SerializeField] private string levelName = "New Level";
        [Tooltip("Descrição opcional do nível")]
        [SerializeField] private string description = "";
        [Tooltip("Array que armazena todos os cubos do nível")]
        [SerializeField] private CubeData[] cubes = new CubeData[0];

        [Header("Spawn Settings")]
        [Tooltip("Posição no grid onde o player irá aparecer")]
        [SerializeField] private Vector2Int playerSpawnPosition = Vector2Int.zero;
        [Tooltip("Se ativo, o player spawna automaticamente neste nível")]
        [SerializeField] private bool enablePlayerSpawn = true;

        #region Properties
        public Vector2Int GridSize {
            get => gridSize;
            set => gridSize = value;
        }

        public Vector3 CubeSpacing {
            get => cubeSpacing;
            set => cubeSpacing = value;
        }

        public string LevelName {
            get => levelName;
            set => levelName = value;
        }

        public string Description {
            get => description;
            set => description = value;
        }

        public CubeData[] Cubes {
            get => cubes;
            set => cubes = value;
        }

        public Vector2Int PlayerSpawnPosition {
            get => playerSpawnPosition;
            set => playerSpawnPosition = value;
        }

        public bool EnablePlayerSpawn {
            get => enablePlayerSpawn;
            set => enablePlayerSpawn = value;
        }
        #endregion

        #region Public Methods
        /// <summary>
        /// Obtém dados de um cubo em uma posição específica
        /// </summary>
        public CubeData GetCubeAt(Vector2Int position) {
            foreach (var cube in cubes) {
                if (cube.gridPosition == position)
                    return cube;
            }
            return null;
        }

        /// <summary>
        /// Calcula a posição mundo de uma posição no grid
        /// </summary>
        public Vector3 GridToWorldPosition(Vector2Int gridPos) {
            return new Vector3(
                gridPos.x * cubeSpacing.x,
                gridPos.y * cubeSpacing.y,
                0f
            );
        }

        /// <summary>
        /// Calcula a posição no grid a partir de uma posição mundo
        /// </summary>
        public Vector2Int WorldToGridPosition(Vector3 worldPos) {
            return new Vector2Int(
                Mathf.RoundToInt(worldPos.x / cubeSpacing.x),
                Mathf.RoundToInt(worldPos.y / cubeSpacing.y)
            );
        }
        #endregion
    }
}
