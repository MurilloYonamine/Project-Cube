#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;

namespace PROJECT_CUBE.LEVEL_GENERATION.EDITOR {
    public class LevelGeneratorMenu {
        [MenuItem("Level Generation/Cube Palette Editor", false, 10)]
        public static void OpenCubePaletteEditor() {
            CubePaletteEditorWindow.ShowWindow();
        }

        [MenuItem("Level Generation/Blueprint Editor Window", false, 15)]
        public static void OpenBlueprintEditor() {
            LevelBlueprintEditorWindow.ShowWindow();
        }
    }
}
#endif
