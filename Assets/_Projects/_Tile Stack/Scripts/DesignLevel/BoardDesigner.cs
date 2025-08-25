using System.Linq;
using UnityEditor;
using UnityEngine;

#if UNITY_EDITOR
namespace TileStack.Design
{
    public class BoardDesigner : MonoBehaviour
    {
        [SerializeField] private LevelData _levelData;

        private void DrawArrow(Vector3 position, Vector3 direction)
        {
            // Vẽ thân mũi tên
            Gizmos.DrawLine(position, position + direction * 0.2f);

            // Vẽ đầu mũi tên (hai đoạn nhỏ tạo thành tam giác)
            var right = Quaternion.LookRotation(direction) * Quaternion.Euler(0, 150, 0) * Vector3.forward;
            var left = Quaternion.LookRotation(direction) * Quaternion.Euler(0, -150, 0) * Vector3.forward;

            Gizmos.DrawLine(position + direction * 0.2f, position + direction * 0.25f + right * 0.2f);
            Gizmos.DrawLine(position + direction * 0.2f, position + direction * 0.25f + left * 0.2f);
        }

        private void OnDrawGizmos()
        {
            var forwardPosition = new Vector3((_levelData.width - 1) / 2f, 0, _levelData.height);
            Handles.Label(forwardPosition, "Forward");

            var backwardPosition = new Vector3((_levelData.width - 1) / 2f, 0, -1);
            Handles.Label(backwardPosition, "Backward");

            var leftPosition = new Vector3(-1, 0, (_levelData.height - 1) / 2f);
            Handles.Label(leftPosition, "Left");

            var rightPosition = new Vector3(_levelData.width, 0, (_levelData.height - 1) / 2f);
            Handles.Label(rightPosition, "Right");

            if (_levelData == null) return;

            for (var i = 0; i < _levelData.width; i++)
            {
                for (var j = 0; j < _levelData.height; j++)
                {
                    Gizmos.color = Color.white;
                    var position = new Vector2Int(i, j);
                    var worldPosition = new Vector3(i, 0, j);
                    Gizmos.DrawWireCube(worldPosition, Vector3.one);

                    if (_levelData.designedCellDatas.Any(d => d.position == position))
                    {
                        var designedCell = _levelData.designedCellDatas.FirstOrDefault(x => x.position == position);
                        Gizmos.color = Color.yellow;
                        Gizmos.DrawWireCube(new Vector3(i, 0, j), Vector3.one * 0.9f);

                        if (designedCell.hasTile)
                        {
                            Gizmos.color = Color.green;
                            Gizmos.DrawWireCube(new Vector3(i, 0, j), Vector3.one * 0.8f);

                            if (designedCell.moveDirection != MoveDirection.None)
                            {
                                var directionVectorInt = designedCell.moveDirection.GetDirectionVector();
                                var directionVector3 = new Vector3(directionVectorInt.x, 0, directionVectorInt.y);
                                DrawArrow(worldPosition, directionVector3);
                            }
                        }
                        else
                        {
                            if (designedCell.moveDirection != MoveDirection.None)
                            {
                                Gizmos.color = Color.yellow;
                                var directionVectorInt = designedCell.moveDirection.GetDirectionVector();
                                var directionVector3 = new Vector3(directionVectorInt.x, 0, directionVectorInt.y);
                                DrawArrow(worldPosition, directionVector3);
                            }
                        }
                    }
                }
            }
        }
    }
}
#endif