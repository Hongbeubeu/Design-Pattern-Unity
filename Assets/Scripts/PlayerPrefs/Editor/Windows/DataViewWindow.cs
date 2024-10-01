using PlayerPrefs.Editor;
using Unity.Plastic.Newtonsoft.Json;
using Unity.Plastic.Newtonsoft.Json.Linq;
using UnityEditor;
using UnityEngine;

public class DataViewWindow : EditorWindow
{
    private string _data;
    private Vector2 _scrollPos;

    public static void Init(string key, string value)
    {
        var window = (DataViewWindow)GetWindow(typeof(DataViewWindow), true, $"View Data: {key}");
        window._data = FormatAsJsonIfApplicable(value);
        window.Show();
    }

    private static string FormatAsJsonIfApplicable(string data)
    {
        if (!IsJson(data)) return data;
        try
        {
            // Parse the JSON to ensure it's valid
            var parsedJson = JToken.Parse(data);
            // Beautify and pretty-print JSON
            return parsedJson.ToString(Formatting.Indented);
        }
        catch
        {
            // If parsing fails, just return the raw data
            return data;
        }
    }

    private static bool IsJson(string data)
    {
        // Trim the data to avoid whitespace issues
        data = data.Trim();

        // Check if it starts with a valid JSON object or array
        if ((!data.StartsWith("{") || !data.EndsWith("}")) && (!data.StartsWith("[") || !data.EndsWith("]"))) return false; // Not JSON format
        try
        {
            // Try parsing to check if it's valid JSON
            JToken.Parse(data);
            return true;
        }
        catch (JsonReaderException)
        {
            return false; // Invalid JSON format
        }
    }

    private void OnGUI()
    {
        using (new BeginScrollView(ref _scrollPos))
        {
            EditorGUILayout.TextArea(_data, GUILayout.ExpandHeight(true));
        }
    }
}