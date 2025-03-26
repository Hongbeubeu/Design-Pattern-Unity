using UnityEditor;
using UnityEngine;

namespace TileStack.Design.Editor
{
    public class LevelEditorWindow : EditorWindow
    {
        private LevelData levelData;
        private Vector2 scrollPosition;

        [MenuItem("Tools/Level Editor")]
        public static void ShowWindow()
        {
            GetWindow<LevelEditorWindow>("Level Editor");
        }

        private void OnGUI()
        {
            GUILayout.Label("Level Editor", EditorStyles.boldLabel);

            // Chọn hoặc tạo mới LevelData
            levelData = (LevelData)EditorGUILayout.ObjectField("Level Data", levelData, typeof(LevelData), false);

            if (levelData == null)
            {
                EditorGUILayout.HelpBox("Chọn một LevelData để chỉnh sửa", MessageType.Info);

                return;
            }

            // Chỉnh sửa kích thước board
            levelData.width = EditorGUILayout.IntField("Width", levelData.width);
            levelData.height = EditorGUILayout.IntField("Height", levelData.height);

            if (GUILayout.Button("Tạo Board"))
            {
                GenerateBoard();
            }

            DrawGrid();

            if (GUI.changed)
            {
                EditorUtility.SetDirty(levelData);
            }
        }

        private void GenerateBoard()
        {
            levelData.designedCellDatas = new DesignedCellData[levelData.width * levelData.height];

            for (int x = 0; x < levelData.width; x++)
            {
                for (int y = 0; y < levelData.height; y++)
                {
                    levelData.designedCellDatas[x + y * levelData.width] = new DesignedCellData
                                                                           {
                                                                               position = new Vector2Int(x, y),
                                                                               hasTile = false,
                                                                               moveDirection = MoveDirection.None
                                                                           };
                }
            }
        }

        private void DrawGrid()
        {
            if (levelData.designedCellDatas == null) return;

            scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);

            for (int y = 0; y < levelData.height; y++)
            {
                EditorGUILayout.BeginHorizontal();

                for (int x = 0; x < levelData.width; x++)
                {
                    int index = x + y * levelData.width;
                    var cell = levelData.designedCellDatas[index];

                    // Nút bật/tắt Tile
                    string label = cell.hasTile ? "X" : "O";

                    if (GUILayout.Button(label, GUILayout.Width(30), GUILayout.Height(30)))
                    {
                        cell.hasTile = !cell.hasTile;
                        levelData.designedCellDatas[index] = cell;
                    }
                }

                EditorGUILayout.EndHorizontal();
            }

            EditorGUILayout.EndScrollView();
        }
    }
}