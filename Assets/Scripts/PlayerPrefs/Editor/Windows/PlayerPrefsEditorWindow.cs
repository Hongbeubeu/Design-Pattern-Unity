using UnityEditor;
using UnityEngine;
using System.Collections.Generic;

public class PlayerPrefsEditorWindow : EditorWindow
{
    private int currentPage = 1;
    private const int itemsPerPage = 10;
    private Vector2 scrollPosition;
    private string newKey = "";
    private string newValue = "";
    private string searchQuery = "";

    // Enum for sorting options
    private enum SortOption
    {
        Key,
        Value,
        Created,
        Modified
    }

    private SortOption currentSortOption = SortOption.Key; // Default sort option
    private bool isAscending = true; // Default sorting order

    // Dictionary to hold editable values for each key
    private Dictionary<string, string> editableValues = new Dictionary<string, string>();

    [MenuItem("Tools/PlayerPrefs Editor")]
    public static void ShowWindow()
    {
        GetWindow<PlayerPrefsEditorWindow>("PlayerPrefs Editor");
    }

    private void OnGUI()
    {
        GUILayout.Label("PlayerPrefs Manager", EditorStyles.boldLabel);

        // Add new PlayerPref
        GUILayout.BeginHorizontal();
        newKey = EditorGUILayout.TextField("Key", newKey);
        newValue = EditorGUILayout.TextField("Value", newValue);

        if (GUILayout.Button("Save PlayerPref", GUILayout.Width(120)))
        {
            if (!string.IsNullOrEmpty(newKey))
            {
                PlayerPrefsManager.SetString(newKey, newValue);
                Debug.Log($"PlayerPref '{newKey}' saved with value '{newValue}'");
            }
        }
        GUILayout.EndHorizontal();

        GUILayout.Space(10);

        // Search bar
        GUILayout.BeginHorizontal();
        GUILayout.Label("Search:", GUILayout.Width(50));
        searchQuery = EditorGUILayout.TextField(searchQuery, GUILayout.Width(300));

        if (GUILayout.Button("Clear Search", GUILayout.Width(100)))
        {
            searchQuery = "";
        }
        GUILayout.EndHorizontal();

        GUILayout.Space(10);

        // Clear All PlayerPrefs Button
        if (GUILayout.Button("Clear All PlayerPrefs", GUILayout.Width(200)))
        {
            if (EditorUtility.DisplayDialog("Clear All PlayerPrefs",
                "Are you sure you want to delete all PlayerPrefs? This action cannot be undone.", "Yes", "No"))
            {
                PlayerPrefsManager.ClearAll();
                Debug.Log("All PlayerPrefs have been cleared.");
            }
        }

        GUILayout.Space(10);

        // Display table header with sorting buttons
        GUILayout.Label("PlayerPrefs Data", EditorStyles.boldLabel);
        DisplayTableHeader();

        // Pagination Controls
        List<string> filteredKeys = PlayerPrefsManager.SearchPlayerPrefs(searchQuery);
        List<PlayerPrefData> prefDataList = GetPlayerPrefData(filteredKeys);

        // Sort the list based on selected sorting option
        SortPlayerPrefs(prefDataList);

        int totalItems = prefDataList.Count;
        int totalPages = Mathf.CeilToInt((float)totalItems / itemsPerPage);

        GUILayout.BeginHorizontal();
        GUILayout.FlexibleSpace();
        GUILayout.Label($"Page {currentPage} of {totalPages}");

        if (GUILayout.Button("Previous Page") && currentPage > 1)
        {
            currentPage--;
        }

        if (GUILayout.Button("Next Page") && currentPage < totalPages)
        {
            currentPage++;
        }
        GUILayout.FlexibleSpace();
        GUILayout.EndHorizontal();

        GUILayout.Space(10);

        // Display PlayerPrefs Data
        DisplayPlayerPrefs(prefDataList);
    }

    private void DisplayTableHeader()
    {
        GUILayout.BeginHorizontal();
        GUILayout.Label("Index", EditorStyles.boldLabel, GUILayout.Width(50));

        // Key column button
        if (GUILayout.Button("Key " + (currentSortOption == SortOption.Key && isAscending ? "▲" : "▼"), GUILayout.Width(150)))
        {
            currentSortOption = SortOption.Key;
            isAscending = !isAscending;
        }

        // Value column button
        if (GUILayout.Button("Value " + (currentSortOption == SortOption.Value && isAscending ? "▲" : "▼"), GUILayout.Width(150)))
        {
            currentSortOption = SortOption.Value;
            isAscending = !isAscending;
        }

        // Created column button
        if (GUILayout.Button("Created " + (currentSortOption == SortOption.Created && isAscending ? "▲" : "▼"), GUILayout.Width(150)))
        {
            currentSortOption = SortOption.Created;
            isAscending = !isAscending;
        }

        // Modified column button
        if (GUILayout.Button("Modified " + (currentSortOption == SortOption.Modified && isAscending ? "▲" : "▼"), GUILayout.Width(150)))
        {
            currentSortOption = SortOption.Modified;
            isAscending = !isAscending;
        }

        GUILayout.Label("Actions", EditorStyles.boldLabel, GUILayout.Width(100));
        GUILayout.EndHorizontal();
    }

    private void DisplayPlayerPrefs(List<PlayerPrefData> prefDataList)
    {
        scrollPosition = GUILayout.BeginScrollView(scrollPosition);

        int start = (currentPage - 1) * itemsPerPage;
        int end = Mathf.Min(start + itemsPerPage, prefDataList.Count);

        for (int i = start; i < end; i++)
        {
            PlayerPrefData data = prefDataList[i];

            GUILayout.BeginHorizontal();
            GUILayout.Label((i + 1).ToString(), GUILayout.Width(50)); // Index
            GUILayout.Label(data.Key, GUILayout.Width(150));          // Key

            // Editable Value field
            if (!editableValues.ContainsKey(data.Key)) // Initialize editable value if not present
            {
                editableValues[data.Key] = data.Value;
            }
            string editedValue = EditorGUILayout.TextField(editableValues[data.Key], GUILayout.Width(150));

            // Save button for edited value
            if (GUILayout.Button("Save", GUILayout.Width(50))) 
            {
                if (data.Value != editedValue) // Only save if the value has changed
                {
                    PlayerPrefsManager.SetString(data.Key, editedValue);
                    editableValues[data.Key] = editedValue; // Update the editable value
                    Debug.Log($"PlayerPref '{data.Key}' updated to '{editedValue}'");
                }
            }

            GUILayout.Label(data.CreatedTime, GUILayout.Width(150));  // Created Time
            GUILayout.Label(data.ModifiedTime, GUILayout.Width(150)); // Modified Time

            if (GUILayout.Button("Delete", GUILayout.Width(50)))
            {
                PlayerPrefsManager.DeleteKey(data.Key);
                editableValues.Remove(data.Key); // Remove the editable value
                Debug.Log($"PlayerPref '{data.Key}' deleted.");
            }
            GUILayout.EndHorizontal();
        }

        GUILayout.EndScrollView();
    }

    // Get player pref data including timestamps
    private List<PlayerPrefData> GetPlayerPrefData(List<string> keys)
    {
        List<PlayerPrefData> prefDataList = new List<PlayerPrefData>();

        foreach (var key in keys)
        {
            string value = PlayerPrefsManager.GetString(key);
            string createdTime = PlayerPrefsManager.GetCreatedTime(key);
            string modifiedTime = PlayerPrefsManager.GetModifiedTime(key);
            prefDataList.Add(new PlayerPrefData(key, value, createdTime, modifiedTime));
        }

        return prefDataList;
    }

    // Sort PlayerPref data based on the current sort option
    private void SortPlayerPrefs(List<PlayerPrefData> prefDataList)
    {
        switch (currentSortOption)
        {
            case SortOption.Key:
                prefDataList.Sort((x, y) => isAscending 
                    ? string.Compare(x.Key, y.Key) 
                    : string.Compare(y.Key, x.Key));
                break;
            case SortOption.Value:
                prefDataList.Sort((x, y) => isAscending 
                    ? string.Compare(x.Value, y.Value) 
                    : string.Compare(y.Value, x.Value));
                break;
            case SortOption.Created:
                prefDataList.Sort((x, y) => isAscending 
                    ? string.Compare(x.CreatedTime, y.CreatedTime) 
                    : string.Compare(y.CreatedTime, x.CreatedTime));
                break;
            case SortOption.Modified:
                prefDataList.Sort((x, y) => isAscending 
                    ? string.Compare(x.ModifiedTime, y.ModifiedTime) 
                    : string.Compare(y.ModifiedTime, x.ModifiedTime));
                break;
        }
    }
}