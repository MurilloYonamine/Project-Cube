#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using System.Collections.Generic;

namespace PROJECT_CUBE.LEVEL_GENERATION.EDITOR {
    public class LevelBlueprintEditorWindow : EditorWindow {
        private LevelBlueprint _currentBlueprint;
        private CubePalette _currentPalette;
        private Vector2 _scrollPosition;

        private float _cellSize = 30f;
        private const float MIN_CELL_SIZE = 15f;
        private const float MAX_CELL_SIZE = 60f;

        private int _selectedCubeType = 0;
        private bool _isPainting = false;
        private bool _isPlacingSpawn = false;

        private Vector2Int _dragStartPosition = Vector2Int.one * -1;
        private bool _isDragging = false;

        private bool _showSettings = true;
        private bool _showPalette = true;

        private string _previousScenePath = null;
        private bool _isPlayingBlueprint = false;
        private GameObject _cinemachinePrefab = null;
        private static GameObject _cinemachineToInstantiate = null;
        [MenuItem("Level Generation/Blueprint Editor Window")]
        public static void ShowWindow() {
            GetWindow<LevelBlueprintEditorWindow>("Blueprint Editor");
        }

        private void OnEnable() {
            EditorApplication.playModeStateChanged += OnPlayModeStateChanged;
        }

        private void OnDisable() {
            EditorApplication.playModeStateChanged -= OnPlayModeStateChanged;
        }

        private void OnPlayModeStateChanged(PlayModeStateChange state) {
            if (state == PlayModeStateChange.EnteredPlayMode && _cinemachineToInstantiate != null) {
                // Instancia a Cinemachine quando entra no Play Mode
                var cinemachineInstance = Instantiate(_cinemachineToInstantiate);
                cinemachineInstance.name = "Cinemachine";
                Debug.Log("Cinemachine carregada para a cena de teste");

                // Cria um GameObject auxiliar para setup
                var setupObj = new GameObject("_CinemachineSetup");
                setupObj.AddComponent<CinemachineSetup>();

                _cinemachineToInstantiate = null;
            } else if (state == PlayModeStateChange.EnteredEditMode && _isPlayingBlueprint) {
                _isPlayingBlueprint = false;
                
                // Volta para a cena anterior
                if (!string.IsNullOrEmpty(_previousScenePath)) {
                    EditorSceneManager.OpenScene(_previousScenePath, OpenSceneMode.Single);
                    Debug.Log($"Voltou para: {_previousScenePath}");
                } else {
                    Debug.Log("Nenhuma cena anterior para retornar");
                }
                
                _previousScenePath = null;
            }
        }

        private void OnGUI() {
            EditorGUILayout.LabelField("Level Blueprint Editor", EditorStyles.boldLabel);
            EditorGUILayout.Space();

            DrawLoadSection();
            EditorGUILayout.Space();

            if (_currentBlueprint == null) {
                EditorGUILayout.HelpBox("Load or create a Level Blueprint to start editing", MessageType.Info);
                return;
            }

            DrawToolbar();
            EditorGUILayout.Space();

            EditorGUILayout.BeginHorizontal();

            // Left Panel
            EditorGUILayout.BeginVertical(GUILayout.Width(250));
            DrawLeftPanel();
            EditorGUILayout.EndVertical();

            EditorGUILayout.Space(10);

            // Right Panel - Grid
            EditorGUILayout.BeginVertical();
            DrawGridEditor();
            EditorGUILayout.EndVertical();

            EditorGUILayout.EndHorizontal();
        }

        private void DrawLoadSection() {
            EditorGUILayout.BeginHorizontal(EditorStyles.helpBox);
            _currentBlueprint = EditorGUILayout.ObjectField("Blueprint", _currentBlueprint, typeof(LevelBlueprint), false) as LevelBlueprint;
            _currentPalette = EditorGUILayout.ObjectField("Palette", _currentPalette, typeof(CubePalette), false) as CubePalette;
            EditorGUILayout.EndHorizontal();
        }

        private void DrawToolbar() {
            EditorGUILayout.BeginHorizontal(EditorStyles.toolbar);

            if (GUILayout.Button("Paint", EditorStyles.toolbarButton, GUILayout.Width(60))) {
                _isPainting = !_isPainting;
                _isPlacingSpawn = false;
            }

            if (GUILayout.Button("Spawn", EditorStyles.toolbarButton, GUILayout.Width(60))) {
                _isPlacingSpawn = !_isPlacingSpawn;
                _isPainting = false;
            }

            GUILayout.FlexibleSpace();

            EditorGUILayout.LabelField("Zoom:", GUILayout.Width(50));
            _cellSize = GUILayout.HorizontalSlider(_cellSize, MIN_CELL_SIZE, MAX_CELL_SIZE, GUILayout.Width(100));

            if (GUILayout.Button("Save", EditorStyles.toolbarButton, GUILayout.Width(60))) {
                EditorUtility.SetDirty(_currentBlueprint);
                AssetDatabase.SaveAssets();
                Debug.Log("Blueprint saved!");
            }

            if (GUILayout.Button("Play", EditorStyles.toolbarButton, GUILayout.Width(60))) {
                PlayBlueprint();
            }

            EditorGUILayout.EndHorizontal();
        }

        private void DrawLeftPanel() {
            _scrollPosition = EditorGUILayout.BeginScrollView(_scrollPosition);

            // Settings
            _showSettings = EditorGUILayout.Foldout(_showSettings, "Settings", EditorStyles.foldoutHeader);
            if (_showSettings) {
                EditorGUI.indentLevel++;
                _currentBlueprint.LevelName = EditorGUILayout.TextField("Level Name", _currentBlueprint.LevelName);
                _currentBlueprint.Description = EditorGUILayout.TextArea(_currentBlueprint.Description, GUILayout.Height(60));

                EditorGUILayout.LabelField("Grid Size (Fixo: 30x17)", EditorStyles.boldLabel);
                EditorGUI.BeginDisabledGroup(true);
                EditorGUILayout.Vector2IntField("", _currentBlueprint.GridSize);
                EditorGUI.EndDisabledGroup();

                _currentBlueprint.CubeSpacing = EditorGUILayout.Vector3Field("Cube Spacing", _currentBlueprint.CubeSpacing);

                EditorGUILayout.Space();
                EditorGUILayout.LabelField("Cinemachine", EditorStyles.boldLabel);
                _cinemachinePrefab = EditorGUILayout.ObjectField("Camera Prefab", _cinemachinePrefab, typeof(GameObject), false) as GameObject;

                EditorGUILayout.Space();
                _currentBlueprint.EnablePlayerSpawn = EditorGUILayout.Toggle("Enable Player Spawn", _currentBlueprint.EnablePlayerSpawn);

                if (_currentBlueprint.EnablePlayerSpawn) {
                    EditorGUILayout.LabelField("Player Spawn Position");
                    _currentBlueprint.PlayerSpawnPosition = EditorGUILayout.Vector2IntField("", _currentBlueprint.PlayerSpawnPosition);
                }

                EditorGUI.indentLevel--;
                EditorGUILayout.Space();
            }

            // Palette
            if (_currentPalette != null) {
                _showPalette = EditorGUILayout.Foldout(_showPalette, "Cube Types", EditorStyles.foldoutHeader);
                if (_showPalette) {
                    EditorGUI.indentLevel++;
                    for (int i = 0; i < _currentPalette.CubeTypes.Length; i++) {
                        var cubeType = _currentPalette.CubeTypes[i];
                        EditorGUILayout.BeginHorizontal();

                        bool isSelected = _selectedCubeType == i;
                        GUI.backgroundColor = isSelected ? Color.cyan : Color.white;

                        if (GUILayout.Button($"{i}: {cubeType.TypeName}", EditorStyles.miniButton)) {
                            _selectedCubeType = i;
                        }

                        GUI.backgroundColor = Color.white;
                        EditorGUILayout.EndHorizontal();
                    }
                    EditorGUI.indentLevel--;
                }
                EditorGUILayout.Space();
            } else {
                EditorGUILayout.HelpBox("Nenhuma paleta carregada. Crie uma para começar a editar.", MessageType.Warning);
                if (GUILayout.Button("Criar Nova Paleta", GUILayout.Height(30))) {
                    CreateAndOpenPaletteEditor();
                }
                EditorGUILayout.Space();
            }

            // Info
            EditorGUILayout.LabelField("Info", EditorStyles.boldLabel);
            EditorGUILayout.LabelField($"Total Cubes: {_currentBlueprint.Cubes.Length}");
            EditorGUILayout.LabelField($"Grid: {_currentBlueprint.GridSize.x}x{_currentBlueprint.GridSize.y}");

            EditorGUILayout.Space();
            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("Clear All", GUILayout.Height(30))) {
                if (EditorUtility.DisplayDialog("Clear Level", "Remove all cubes?", "Clear", "Cancel")) {
                    _currentBlueprint.Cubes = new LevelBlueprint.CubeData[0];
                    EditorUtility.SetDirty(_currentBlueprint);
                }
            }
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.EndScrollView();
        }

        private void DrawGridEditor() {
            EditorGUILayout.LabelField("Grid Editor", EditorStyles.boldLabel);

            // Calculate grid dimensions
            int gridWidth = _currentBlueprint.GridSize.x;
            int gridHeight = _currentBlueprint.GridSize.y;

            float totalWidth = gridWidth * _cellSize;
            float totalHeight = gridHeight * _cellSize;

            // Grid background
            Rect gridRect = GUILayoutUtility.GetRect(totalWidth + 50, totalHeight + 50);
            GUI.Box(gridRect, "", EditorStyles.helpBox);

            DrawGridLines(gridRect, gridWidth, gridHeight);
            DrawCubes(gridRect, gridWidth, gridHeight);
            HandleGridInput(gridRect, gridWidth, gridHeight);
        }

        private void DrawGridLines(Rect gridRect, int width, int height) {
            Handles.BeginGUI();
            Handles.color = Color.gray;

            float startX = gridRect.x + 5;
            float startY = gridRect.y + 5;

            // Vertical lines
            for (int x = 0; x <= width; x++) {
                float lineX = startX + x * _cellSize;
                Handles.DrawLine(new Vector3(lineX, startY), new Vector3(lineX, startY + height * _cellSize));
            }

            // Horizontal lines
            for (int y = 0; y <= height; y++) {
                float lineY = startY + y * _cellSize;
                Handles.DrawLine(new Vector3(startX, lineY), new Vector3(startX + width * _cellSize, lineY));
            }

            Handles.color = Color.white;
            Handles.EndGUI();
        }

        private void DrawCubes(Rect gridRect, int width, int height) {
            float startX = gridRect.x + 5;
            float startY = gridRect.y + 5;

            // Draw existing cubes
            foreach (var cube in _currentBlueprint.Cubes) {
                if (cube.gridPosition.x >= 0 && cube.gridPosition.x < width &&
                    cube.gridPosition.y >= 0 && cube.gridPosition.y < height) {
                    DrawCubeCell(startX, startY, cube.gridPosition, cube.cubeTypeIndex);
                }
            }

            // Draw player spawn
            if (_currentBlueprint.EnablePlayerSpawn) {
                DrawPlayerSpawnCell(startX, startY, _currentBlueprint.PlayerSpawnPosition);
            }
        }

        private void DrawCubeCell(float startX, float startY, Vector2Int position, int typeIndex) {
            float x = startX + position.x * _cellSize;
            float y = startY + position.y * _cellSize;
            Rect cellRect = new Rect(x, y, _cellSize, _cellSize);

            Color cubeColor = GetCubeColor(typeIndex);
            GUI.color = cubeColor;
            GUI.Box(cellRect, "");
            GUI.color = Color.white;

            GUI.Label(cellRect, typeIndex.ToString(), EditorStyles.miniLabel);
        }

        private void DrawPlayerSpawnCell(float startX, float startY, Vector2Int position) {
            float x = startX + position.x * _cellSize;
            float y = startY + position.y * _cellSize;
            Rect cellRect = new Rect(x, y, _cellSize, _cellSize);

            GUI.color = new Color(0, 1, 0, 0.5f);
            GUI.Box(cellRect, "");
            GUI.color = Color.green;
            GUI.Label(cellRect, "P", EditorStyles.miniLabel);
            GUI.color = Color.white;
        }

        private void HandleGridInput(Rect gridRect, int width, int height) {
            Event e = Event.current;
            float startX = gridRect.x + 5;
            float startY = gridRect.y + 5;

            if (gridRect.Contains(e.mousePosition)) {
                Vector2 mousePos = e.mousePosition;
                int gridX = Mathf.FloorToInt((mousePos.x - startX) / _cellSize);
                int gridY = Mathf.FloorToInt((mousePos.y - startY) / _cellSize);

                if (gridX >= 0 && gridX < width && gridY >= 0 && gridY < height) {
                    Vector2Int gridPos = new Vector2Int(gridX, gridY);

                    if (e.type == EventType.MouseDown && e.button == 0) {
                        if (_isPainting) {
                            AddOrUpdateCube(gridPos, _selectedCubeType);
                            e.Use();
                        } else if (_isPlacingSpawn) {
                            _currentBlueprint.PlayerSpawnPosition = gridPos;
                            e.Use();
                        }
                    } else if (e.type == EventType.MouseDown && e.button == 1) {
                        RemoveCube(gridPos);
                        e.Use();
                    }
                }
            }

            if (e.type == EventType.MouseMove) {
                Repaint();
            }
        }

        private void AddOrUpdateCube(Vector2Int position, int typeIndex) {
            var cubes = new List<LevelBlueprint.CubeData>(_currentBlueprint.Cubes);

            // Check if cube already exists
            bool found = false;
            for (int i = 0; i < cubes.Count; i++) {
                if (cubes[i].gridPosition == position) {
                    cubes[i].cubeTypeIndex = typeIndex;
                    found = true;
                    break;
                }
            }

            if (!found) {
                cubes.Add(new LevelBlueprint.CubeData(typeIndex, position));
            }

            _currentBlueprint.Cubes = cubes.ToArray();
            EditorUtility.SetDirty(_currentBlueprint);
        }

        private void RemoveCube(Vector2Int position) {
            var cubes = new List<LevelBlueprint.CubeData>(_currentBlueprint.Cubes);

            for (int i = cubes.Count - 1; i >= 0; i--) {
                if (cubes[i].gridPosition == position) {
                    cubes.RemoveAt(i);
                    break;
                }
            }

            _currentBlueprint.Cubes = cubes.ToArray();
            EditorUtility.SetDirty(_currentBlueprint);
        }

        private Color GetCubeColor(int typeIndex) {
            if (_currentPalette != null && typeIndex >= 0 && typeIndex < _currentPalette.CubeTypes.Length) {
                return Color.HSVToRGB((typeIndex * 0.1f) % 1f, 0.7f, 0.8f);
            }
            return Color.gray;
        }

        private void CreateAndOpenPaletteEditor() {
            var palette = ScriptableObject.CreateInstance<CubePalette>();
            palette.Initialize();

            string path = EditorUtility.SaveFilePanelInProject(
                "Salvar Nova Paleta de Cubos",
                "New Cube Palette",
                "asset",
                "Escolha um local para salvar a paleta"
            );

            if (!string.IsNullOrEmpty(path)) {
                AssetDatabase.CreateAsset(palette, path);
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();

                _currentPalette = palette;

                // Abre o editor de paleta em uma nova janela
                CubePaletteEditorWindow.ShowWindow(palette);

                Debug.Log($"Paleta criada em {path}");
            }
        }

        private void PlayBlueprint() {
            if (_currentBlueprint == null) {
                EditorUtility.DisplayDialog("Erro", "Carregue um Blueprint primeiro!", "OK");
                return;
            }

            if (_currentPalette == null) {
                EditorUtility.DisplayDialog("Erro", "Carregue uma Paleta para jogar!", "OK");
                return;
            }

            // Salva o blueprint antes de jogar
            EditorUtility.SetDirty(_currentBlueprint);
            AssetDatabase.SaveAssets();

            // Salva a cena atual antes de criar a nova
            _previousScenePath = EditorSceneManager.GetActiveScene().path;
            _isPlayingBlueprint = true;

            // Cria uma cena temporária
            var scene = EditorSceneManager.NewScene(NewSceneSetup.DefaultGameObjects, NewSceneMode.Single);

            // Cria o LevelBuilder PRIMEIRO
            GameObject levelBuilderObj = new GameObject("LevelBuilder");
            var levelBuilder = levelBuilderObj.AddComponent<LevelBuilder>();

            // Cria o LevelManager e configura a referência ao LevelBuilder
            GameObject levelManagerObj = new GameObject("LevelManager");
            var levelManager = levelManagerObj.AddComponent<LevelManager>();
            
            // Usa reflection para setar a referência do LevelBuilder no LevelManager
            var levelBuilderField = levelManager.GetType().GetField("_levelBuilder", 
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            if (levelBuilderField != null) {
                levelBuilderField.SetValue(levelManager, levelBuilder);
            }

            // Configura o LevelBuilder com o Blueprint e Paleta
            levelBuilder.SetupAndBuildLevel(_currentBlueprint, _currentPalette);

            // Armazena a Cinemachine para instanciar quando entrar no Play Mode
            if (_cinemachinePrefab != null) {
                _cinemachineToInstantiate = _cinemachinePrefab;
            }

            // Inicia o playmode
            EditorSceneManager.MarkSceneDirty(scene);
            EditorApplication.isPlaying = true;

            Debug.Log($"Iniciando Blueprint: {_currentBlueprint.LevelName}");
        }
    }
}
#endif
