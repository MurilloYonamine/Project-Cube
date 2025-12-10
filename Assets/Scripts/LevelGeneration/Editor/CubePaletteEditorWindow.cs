#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;

namespace PROJECT_CUBE.LEVEL_GENERATION.EDITOR {
    public class CubePaletteEditorWindow : EditorWindow {
        private CubePalette _currentPalette;
        private Vector2 _scrollPosition;
        private int _selectedCubeTypeIndex = -1;

        [MenuItem("Level Generation/Cube Palette Editor")]
        public static void ShowWindow() {
            GetWindow<CubePaletteEditorWindow>("Cube Palette Editor");
        }

        public static void ShowWindow(CubePalette palette) {
            var window = GetWindow<CubePaletteEditorWindow>("Cube Palette Editor");
            window._currentPalette = palette;
        }

        private void OnGUI() {
            EditorGUILayout.LabelField("Cube Palette Editor", EditorStyles.boldLabel);
            EditorGUILayout.Space();

            DrawLoadSection();
            EditorGUILayout.Space();

            if (_currentPalette == null) {
                EditorGUILayout.HelpBox("Selecione ou crie uma Cube Palette para editar", MessageType.Info);
                return;
            }

            DrawPaletteEditor();
        }

        private void DrawLoadSection() {
            EditorGUILayout.BeginHorizontal(EditorStyles.helpBox);
            var newPalette = EditorGUILayout.ObjectField("Palette", _currentPalette, typeof(CubePalette), false) as CubePalette;
            if (newPalette != _currentPalette) {
                _currentPalette = newPalette;
                _selectedCubeTypeIndex = -1;
            }
            EditorGUILayout.EndHorizontal();
        }

        private void DrawPaletteEditor() {
            _scrollPosition = EditorGUILayout.BeginScrollView(_scrollPosition);

            EditorGUILayout.LabelField("Tipos de Cubos", EditorStyles.boldLabel);
            EditorGUILayout.Space();

            var cubeTypes = _currentPalette.CubeTypes;

            for (int i = 0; i < cubeTypes.Length; i++) {
                DrawCubeTypeItem(i, cubeTypes[i]);
                EditorGUILayout.Space(5);
            }

            EditorGUILayout.Space(10);

            if (GUILayout.Button("+ Adicionar Novo Tipo", GUILayout.Height(30))) {
                AddNewCubeType();
            }

            EditorGUILayout.EndScrollView();
        }

        private void DrawCubeTypeItem(int index, CubePalette.CubeTypeData cubeType) {
            bool isSelected = _selectedCubeTypeIndex == index;
            GUI.backgroundColor = isSelected ? Color.cyan : Color.white;

            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            GUI.backgroundColor = Color.white;

            // Header
            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button($"Tipo {index}", EditorStyles.boldLabel, GUILayout.Width(100))) {
                _selectedCubeTypeIndex = isSelected ? -1 : index;
            }
            GUILayout.FlexibleSpace();
            if (GUILayout.Button("X", GUILayout.Width(30))) {
                RemoveCubeType(index);
                EditorGUILayout.EndHorizontal();
                EditorGUILayout.EndVertical();
                return;
            }
            EditorGUILayout.EndHorizontal();

            // Details (when selected)
            if (isSelected) {
                EditorGUI.indentLevel++;
                EditorGUILayout.LabelField("Nome", EditorStyles.label);
                cubeType.TypeName = EditorGUILayout.TextField(cubeType.TypeName);

                EditorGUILayout.LabelField("Prefab", EditorStyles.label);
                cubeType.Prefab = EditorGUILayout.ObjectField(cubeType.Prefab, typeof(GameObject), false) as GameObject;

                EditorGUILayout.LabelField("Tipo de Cubo", EditorStyles.label);
                cubeType.CubeType = (CubeType)EditorGUILayout.EnumPopup(cubeType.CubeType);

                EditorGUI.indentLevel--;

                EditorUtility.SetDirty(_currentPalette);
            }

            EditorGUILayout.EndVertical();
        }

        private void AddNewCubeType() {
            var cubeTypes = _currentPalette.CubeTypes;
            var newArray = new CubePalette.CubeTypeData[cubeTypes.Length + 1];
            System.Array.Copy(cubeTypes, newArray, cubeTypes.Length);
            newArray[cubeTypes.Length] = new CubePalette.CubeTypeData { _typeName = "New Cube Type" };

            // Use reflection para setar o array
            var field = _currentPalette.GetType().GetField("_cubeTypes", 
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            if (field != null) {
                field.SetValue(_currentPalette, newArray);
            }

            EditorUtility.SetDirty(_currentPalette);
            _selectedCubeTypeIndex = newArray.Length - 1;
        }

        private void RemoveCubeType(int index) {
            var cubeTypes = _currentPalette.CubeTypes;
            if (cubeTypes.Length <= 1) {
                EditorUtility.DisplayDialog("Erro", "Você deve ter pelo menos um tipo de cubo!", "OK");
                return;
            }

            var newArray = new CubePalette.CubeTypeData[cubeTypes.Length - 1];
            int newIndex = 0;
            for (int i = 0; i < cubeTypes.Length; i++) {
                if (i != index) {
                    newArray[newIndex] = cubeTypes[i];
                    newIndex++;
                }
            }

            var field = _currentPalette.GetType().GetField("_cubeTypes", 
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            if (field != null) {
                field.SetValue(_currentPalette, newArray);
            }

            EditorUtility.SetDirty(_currentPalette);
            _selectedCubeTypeIndex = -1;
        }
    }
}
#endif
