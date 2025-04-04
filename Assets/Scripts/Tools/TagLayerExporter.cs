﻿using UnityEngine;
using UnityEditor;
using System.IO;

namespace hcore.Tool
{
    public class TagLayerExporter : EditorWindow
    {
        private string _exportPath = "Assets/Scripts/Generated/TagLayerData.cs";

        [MenuItem("Tools/Export Tags and Layers")]
        public static void ShowWindow()
        {
            GetWindow<TagLayerExporter>("Export Tags and Layers");
        }

        private void OnGUI()
        {
            // Hiển thị đường dẫn hiện tại
            GUILayout.Label("Export Path:", EditorStyles.boldLabel);
            _exportPath = EditorGUILayout.TextField(_exportPath);

            // Nút để chọn đường dẫn
            if (GUILayout.Button("Choose Export Path"))
            {
                string selectedPath = EditorUtility.SaveFilePanel(
                    "Choose Export Path",
                    Path.GetDirectoryName(_exportPath),
                    Path.GetFileName(_exportPath),
                    "cs");

                if (!string.IsNullOrEmpty(selectedPath))
                {
                    // Chuyển đổi đường dẫn tuyệt đối thành đường dẫn tương đối trong Unity
                    _exportPath = "Assets" + selectedPath.Substring(Application.dataPath.Length);
                }
            }

            // Nút để export
            if (GUILayout.Button("Export Tags and Layers"))
            {
                ExportTagsAndLayers();
            }
        }

        private void ExportTagsAndLayers()
        {
            // Tạo nội dung script
            string scriptContent = GenerateScriptContent();

            // Đảm bảo thư mục tồn tại
            string directory = Path.GetDirectoryName(_exportPath);
            if (!Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }

            // Ghi vào file
            File.WriteAllText(_exportPath, scriptContent);

            // Làm mới Asset Database để Unity nhận biết file mới
            AssetDatabase.Refresh();

            Debug.Log($"Tags and Layers exported to {_exportPath}");
        }

        private static string GenerateScriptContent()
        {
            // Lấy danh sách tags và layers từ Unity
            string[] tags = UnityEditorInternal.InternalEditorUtility.tags;
            string[] layers = UnityEditorInternal.InternalEditorUtility.layers;

            // Tạo nội dung script
            string script = @"
// AUTO-GENERATED FILE. DO NOT MODIFY.
// This file is generated by TagLayerExporter tool.

public static class TagLayerData
{
    // Tags
    public static class Tags
    {";

            // Thêm các tags vào script
            foreach (string tag in tags)
            {
                script += $@"
        public const string {SanitizeName(tag)} = ""{tag}"";";
            }

            script += @"
    }

    // Layers
    public static class Layers
    {";

            // Thêm các layers vào script
            foreach (string layer in layers)
            {
                script += $@"
        public const int {SanitizeName(layer)} = {LayerMask.NameToLayer(layer)};";
            }

            script += @"
    }

    // Layer Masks
    public static class LayerMasks
    {";

            // Thêm các layer masks vào script
            foreach (string layer in layers)
            {
                script += $@"
        public const int {SanitizeName(layer)}_MASK = 1 << {LayerMask.NameToLayer(layer)};";
            }

            script += @"
    }
}";

            return script;
        }

        private static string SanitizeName(string name)
        {
            name = name.ToUpper();
            return name.Replace(" ", "_").Replace("-", "_").Replace(".", "_");
        }
    }
}