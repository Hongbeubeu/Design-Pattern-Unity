using UnityEditor;
using UnityEngine;

#if UNITY_EDITOR

[CustomEditor(typeof(BoardController))]
public class BoardEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        var board = (BoardController)target;

        if (GUILayout.Button("Generate Board"))
        {
            board.GenerateBoard();
            EditorUtility.SetDirty(board);
        }
    }
}

#endif