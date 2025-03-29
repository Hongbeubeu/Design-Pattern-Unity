#if UNITY_EDITOR
using System.Linq;
using hcore.PlayerPrefsManager.Editor;
using UnityEditor;
using UnityEngine;

namespace TileStack.Design.Editor
{
    public class LevelEditorWindow : EditorWindow
    {
        private enum CellType
        {
            Empty,
            Cell
        }

        private Vector2 _scrollPosition;
        private readonly string[] _directions = { "None", "↑", "↓", "←", "→", "↖", "↗", "↙", "↘" };
        private readonly string[] _cellTypes = { "Empty", "Cell" };
        private int _directionSelected;
        private int _cellTypeSelected;
        private bool _hasTile;
        private GUIStyle _buttonStyle;
        private LevelData LevelData { get; set; }

        private MoveDirection CellMoveDirection
        {
            get
            {
                return _directionSelected switch
                       {
                           1 => MoveDirection.Forward,
                           2 => MoveDirection.Backward,
                           3 => MoveDirection.Left,
                           4 => MoveDirection.Right,
                           5 => MoveDirection.ForwardLeft,
                           6 => MoveDirection.ForwardRight,
                           7 => MoveDirection.BackwardLeft,
                           8 => MoveDirection.BackwardRight,
                           _ => MoveDirection.None
                       };
            }
        }

        [MenuItem("Tools/Level Editor")]
        public static void ShowWindow()
        {
            GetWindow<LevelEditorWindow>("Level Editor");
        }

        private static void OpenWindow(LevelData levelData)
        {
            var window = GetWindow<LevelEditorWindow>("Level Editor");
            window.LevelData = levelData;
        }

        [MenuItem("Assets/Edit Level")]
        public static void ShowLevelEditor()
        {
            var levelData = (LevelData)Selection.activeObject;

            if (levelData == null) return;

            OpenWindow(levelData);
        }

        [MenuItem("Assets/Edit Level", true)]
        private static bool ValidatePingAtlas()
        {
            return Selection.activeObject is LevelData;
        }

        private void OnEnable()
        {
            _hasTile = false;
            _directionSelected = 0;
        }

        private void OnGUI()
        {
            GUILayout.Label("Level Editor", EditorStyles.boldLabel);

            LevelData = (LevelData)EditorGUILayout.ObjectField("Level Data", LevelData, typeof(LevelData), false);

            if (!LevelData)
            {
                EditorGUILayout.HelpBox("Select LevelData to design", MessageType.Info);

                return;
            }

            _buttonStyle ??= new GUIStyle(GUI.skin.button)
                             {
                                 fontStyle = FontStyle.Bold,
                                 fontSize = 20
                             };

            LevelData.width = EditorGUILayout.IntField("Width", LevelData.width);
            LevelData.height = EditorGUILayout.IntField("Height", LevelData.height);

            var color = GUI.color;
            GUI.color = Color.red;

            if (GUILayout.Button("Reset Board", _buttonStyle, GUILayout.Height(30)))
            {
                ResetBoard();
            }

            GUI.color = Color.green;

            if (GUILayout.Button("Save Data", _buttonStyle, GUILayout.Height(40)))
            {
                AssetDatabase.SaveAssets();
            }

            GUI.color = color;

            DrawGrid();

            if (GUI.changed)
            {
                EditorUtility.SetDirty(LevelData);
            }
        }

        private void ResetBoard()
        {
            LevelData.designedCellDatas.Clear();
            _hasTile = false;
        }

        private void DrawGrid()
        {
            if (LevelData.designedCellDatas == null) return;

            DrawBoard();
            DrawBrushOptions();
        }

        private void DrawBoard()
        {
            _scrollPosition = EditorGUILayout.BeginScrollView(_scrollPosition);

            using (new VerticalLayout(true))
            {
                for (var y = LevelData.height - 1; y >= 0; y--)
                {
                    using (new HorizontalLayout(true))
                    {
                        for (var x = 0; x < LevelData.width; x++)
                        {
                            DrawCell(x, y);
                        }
                    }
                }
            }

            EditorGUILayout.EndScrollView();
        }

        private void DrawCell(int x, int y)
        {
            var gridPosition = new Vector2Int(x, y);
            var hasCell = LevelData.designedCellDatas.Any(d => d.position == gridPosition);
            DesignedCellData cell = null;
            if (hasCell) cell = LevelData.designedCellDatas.First(d => d.position == gridPosition);

            var originalColor = GUI.color;
            GUI.color = !hasCell ? originalColor : cell.hasTile ? Color.green : Color.yellow;
            var arrow = GetArrowLabel(cell);

            var label = !hasCell ? "" : arrow;

            if (GUILayout.Button(label, _buttonStyle, GUILayout.Width(50), GUILayout.Height(50)))
            {
                ApplyBrush(hasCell, cell, gridPosition);
            }

            GUI.color = originalColor;
        }

        private static string GetArrowLabel(DesignedCellData cell)
        {
            return cell?.moveDirection switch
                   {
                       MoveDirection.Forward => "↑",
                       MoveDirection.Backward => "↓",
                       MoveDirection.Left => "←",
                       MoveDirection.Right => "→",
                       MoveDirection.ForwardLeft => "↖",
                       MoveDirection.ForwardRight => "↗",
                       MoveDirection.BackwardLeft => "↙",
                       MoveDirection.BackwardRight => "↘",
                       _ => ""
                   };
        }

        private void DrawBrushOptions()
        {
            using (new VerticalHelpBox())
            {
                EditorGUILayout.LabelField("Select Brush", EditorStyles.boldLabel);
                _cellTypeSelected = EditorGUILayout.Popup("Cell Type", _cellTypeSelected, _cellTypes);

                if ((CellType)_cellTypeSelected == CellType.Empty) return;
                _hasTile = EditorGUILayout.Toggle("Has Tile", _hasTile);
                // _directionSelected = EditorGUILayout.Popup("Direction", _directionSelected, _directions);
                var style = new GUIStyle(GUI.skin.button)
                            {
                                fixedWidth = 60,
                                fixedHeight = 30,
                                fontSize = 16
                            };
                _directionSelected = GUILayout.SelectionGrid(_directionSelected, _directions, _directions.Length, style);
            }
        }

        private void ApplyBrush(bool hasCell, DesignedCellData cell, Vector2Int gridPosition)
        {
            if (hasCell)
            {
                if ((CellType)_cellTypeSelected == CellType.Empty)
                {
                    LevelData.designedCellDatas.Remove(cell);
                }
                else
                {
                    cell.hasTile = _hasTile;
                    cell.moveDirection = CellMoveDirection;
                }
            }
            else if ((CellType)_cellTypeSelected != CellType.Empty)
            {
                LevelData.designedCellDatas.Add(new DesignedCellData
                                                {
                                                    position = gridPosition,
                                                    hasTile = _hasTile,
                                                    moveDirection = CellMoveDirection
                                                });
            }
        }
    }
}
#endif