using UnityEngine;

namespace PROJECT_CUBE.LEVEL_GENERATION {
    /// <summary>
    /// Configurações centralizadas do LevelGenerator
    /// </summary>
    [System.Serializable]
    public class LevelGeneratorConfig {
        [Header("Core")]
        [Tooltip("Paleta de cubos que define os prefabs")]
        public CubePalette cubePalette;
        [Tooltip("Transform pai onde as plataformas serão criadas")]
        public Transform levelParent;

        [Header("Build Settings")]
        [Tooltip("Se ativo, constrói de forma assíncrona")]
        public bool buildAsync = true;
        [Tooltip("Quantidade de cubos por frame quando assíncrono")]
        public int cubesPerFrame = 10;
        [Tooltip("Se ativo, mostra logs no console")]
        public bool showDebugInfo = true;

        [Header("Player Settings")]
        [Tooltip("Prefab do player")]
        public GameObject playerPrefab;
        [Tooltip("Transform pai do player")]
        public Transform playerParent;
        [Tooltip("Se ativo, spawna player automaticamente")]
        public bool autoSpawnPlayer = true;

        [Header("Position")]
        [Tooltip("Offset global aplicado a todas as plataformas")]
        public Vector3 globalOffset = Vector3.zero;
        [Tooltip("Se ativo, centraliza o level no transform")]
        public bool centerLevel = false;
    }
}
