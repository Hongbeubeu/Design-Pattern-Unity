using UnityEditor;
using UnityEngine;
using System.Collections.Generic;
using System.Globalization;
using PlayerPrefs.Editor;

public class PlayerPrefsEditorWindow : EditorWindow
{
    private int _currentPage = 1;
    private const int ITEMS_PER_PAGE = 10;
    private Vector2 _scrollPosition;
    private string _newKey = "";
    private string _newValue = "";
    private PlayerPrefsManager.PlayerPrefType _newType = PlayerPrefsManager.PlayerPrefType.String;
    private string _searchQuery = "";

    // Enum for sorting options
    private enum SortOption
    {
        Key,
        Value,
        Created,
        Modified
    }

    private SortOption _currentSortOption = SortOption.Key; // Default sort option
    private bool _isAscending = true; // Default sorting order

    // Dictionary to hold editable values for each key
    private readonly Dictionary<string, string> _editableValues = new();

    [MenuItem("Tools/PlayerPrefs Editor")]
    public static void ShowWindow()
    {
        var window = GetWindow<PlayerPrefsEditorWindow>("PlayerPrefs Editor");
        window.minSize = new Vector2(780, 700);
        window.maxSize = new Vector2(780, 700);
        window.Show();
    }

    private bool _showAddNewPlayerPref = true;
    private readonly Color _btnColor = Color.green;

    private void OnEnable()
    {
        OnUpdateData();
    }

    private void OnGUI()
    {
        DrawTitle();
        DrawAddNewPlayerPref();
        DrawSearchBar();
        DrawClearAll();
        DrawPlayerPrefDatas();
    }

    private static void DrawTitle()
    {
        GUILayout.Space(10);
        GUILayout.Label("PlayerPrefs Manager", new GUIStyle(EditorStyles.boldLabel)
                                               {
                                                   fontSize = 30,
                                                   alignment = TextAnchor.MiddleCenter,
                                                   normal =
                                                   {
                                                       textColor = Color.green
                                                   },
                                                   hover =
                                                   {
                                                       textColor = Color.green
                                                   }
                                               });
        GUILayout.Space(10);
    }

    private void DrawAddNewPlayerPref()
    {
        using (new VerticalHelpBox())
        {
            // Add new PlayerPref
            _showAddNewPlayerPref = EditorGUILayout.Foldout(_showAddNewPlayerPref, "Add New PlayerPrefs", new GUIStyle(EditorStyles.foldout)
                                                                                                          {
                                                                                                              fontSize = 14,
                                                                                                              fontStyle = FontStyle.Bold,
                                                                                                              normal =
                                                                                                              {
                                                                                                                  textColor = Color.green
                                                                                                              },
                                                                                                              onNormal =
                                                                                                              {
                                                                                                                  textColor = Color.green
                                                                                                              }
                                                                                                          });
            if (!_showAddNewPlayerPref) return;
            using (new HorizontalHelpBox())
            {
                using (new VerticalLayout())
                {
                    using (new HorizontalLayout())
                    {
                        GUILayout.Label("Key ", new GUIStyle(EditorStyles.boldLabel)
                                                {
                                                    alignment = TextAnchor.MiddleCenter
                                                }, GUILayout.Width(80));
                        _newKey = EditorGUILayout.TextField(_newKey);
                    }

                    using (new HorizontalLayout())
                    {
                        GUILayout.Label("Value ", new GUIStyle(EditorStyles.boldLabel)
                                                  {
                                                      alignment = TextAnchor.MiddleCenter
                                                  }, GUILayout.Width(80));
                        _newType = (PlayerPrefsManager.PlayerPrefType)EditorGUILayout.EnumPopup(_newType, GUILayout.Width(60));
                        switch (_newType)
                        {
                            case PlayerPrefsManager.PlayerPrefType.String:
                                _newValue = EditorGUILayout.TextField(_newValue);
                                break;
                            case PlayerPrefsManager.PlayerPrefType.Int:
                                _newValue = int.TryParse(_newValue, out var intValue) ? intValue.ToString() : "0";
                                _newValue = EditorGUILayout.IntField(int.Parse(_newValue)).ToString();
                                break;
                            case PlayerPrefsManager.PlayerPrefType.Float:
                                _newValue = float.TryParse(_newValue, out var floatValue) ? floatValue.ToString(CultureInfo.InvariantCulture) : "0";
                                _newValue = EditorGUILayout.FloatField(float.Parse(_newValue)).ToString(CultureInfo.InvariantCulture);
                                break;
                        }
                    }
                }

                using (new VerticalLayout(false, GUILayout.Width(200)))
                {
                    using (new BackgroundColorScope(_btnColor))
                    {
                        if (GUILayout.Button(
                                "Save PlayerPref",
                                new GUIStyle(GUI.skin.button)
                                {
                                    fontSize = 15,
                                    alignment = TextAnchor.MiddleCenter
                                },
                                GUILayout.Height(EditorGUIUtility.singleLineHeight * 2 + 2),
                                GUILayout.Width(200)
                            )
                        )
                        {
                            if (!string.IsNullOrEmpty(_newKey))
                            {
                                var oldValue = PlayerPrefsManager.GetValue(_newKey);
                                if (oldValue.ToString() != _newValue)
                                {
                                    PlayerPrefsManager.SetValue(_newKey, _newValue, _newType);
                                    _editableValues.Clear();
                                    OnUpdateData();
                                }
                            }
                        }
                    }
                }
            }
        }
    }

    private string _prevSearchQuery = "";

    private void DrawSearchBar()
    {
        // Search bar
        using (new VerticalHelpBox())
        {
            using (new HorizontalHelpBox())
            {
                using (new HorizontalLayout(false, GUILayout.Width(80)))
                {
                    GUILayout.Label("Search", new GUIStyle(EditorStyles.boldLabel)
                                              {
                                                  alignment = TextAnchor.MiddleCenter,
                                                  fontSize = 15
                                              }, GUILayout.Width(70), GUILayout.Height(30));
                }

                _searchQuery = EditorGUILayout.TextField(_searchQuery, new GUIStyle(EditorStyles.textField)
                                                                       {
                                                                           alignment = TextAnchor.MiddleLeft
                                                                       }, GUILayout.Height(30));

                var currentEvent = Event.current;
                if (currentEvent.isKey)
                {
                    if (_searchQuery != _prevSearchQuery)
                    {
                        Debug.Log("Key pressed: " + Event.current.keyCode);
                        _prevSearchQuery = _searchQuery;
                        currentEvent.Use();
                        OnUpdateData();
                    }
                }

                using (new BackgroundColorScope(Color.yellow))
                {
                    if (GUILayout.Button("Clear Search", new GUIStyle(GUI.skin.button)
                                                         {
                                                             fontSize = 15
                                                         }, GUILayout.Width(200), GUILayout.Height(30)))
                    {
                        _searchQuery = "";
                        OnUpdateData();
                    }
                }
            }
        }
    }

    private void OnUpdateData()
    {
        _filteredKeys = PlayerPrefsManager.SearchPlayerPrefs(_searchQuery);
        _prefDataList = GetPlayerPrefsData(_filteredKeys);
        Repaint();
    }

    private void DrawClearAll()
    {
        GUILayout.Space(10);

        // Clear All PlayerPrefs Button
        using (new HorizontalHelpBox())
        {
            using (new BackgroundColorScope(Color.red))
            {
                if (GUILayout.Button("Clear All PlayerPrefs", new GUIStyle(GUI.skin.button)
                                                              {
                                                                  fontSize = 15
                                                              }, GUILayout.Height(30)))
                {
                    if (EditorUtility.DisplayDialog("Clear All PlayerPrefs",
                        "Are you sure you want to delete all PlayerPrefs? This action cannot be undone.", "Yes", "No"))
                    {
                        PlayerPrefsManager.ClearAll();
                        Debug.Log("All PlayerPrefs have been cleared.");
                    }
                }
            }
        }


        GUILayout.Space(10);
    }

    private List<string> _filteredKeys = new();
    private List<PlayerPrefData> _prefDataList = new();

    private void DrawPlayerPrefDatas()
    {
        // Pagination Controls
        using (new VerticalHelpBox())
        {
            GUILayout.Space(10);
            GUILayout.Label("PlayerPrefs Data", new GUIStyle(EditorStyles.boldLabel)
                                                {
                                                    alignment = TextAnchor.MiddleCenter,
                                                    fontSize = 20,
                                                    normal =
                                                    {
                                                        textColor = Color.yellow
                                                    },
                                                    hover =
                                                    {
                                                        textColor = Color.yellow
                                                    }
                                                });
            GUILayout.Space(10);

            // Display table header with sorting buttons
            DisplayTableHeader();

            // Sort the list based on selected sorting option
            SortPlayerPrefs(_prefDataList);

            // Display PlayerPrefs Data
            DisplayPlayerPrefs(_prefDataList);
        }

        using (new HorizontalHelpBox(true))
        {
            var totalItems = _prefDataList.Count;
            var totalPages = Mathf.CeilToInt((float)totalItems / ITEMS_PER_PAGE);
            GUILayout.Label($"Page {_currentPage} of {totalPages}");

            if (GUILayout.Button("Previous Page") && _currentPage > 1)
            {
                _currentPage--;
            }

            if (GUILayout.Button("Next Page") && _currentPage < totalPages)
            {
                _currentPage++;
            }
        }
    }

    private void DisplayTableHeader()
    {
        using (new HorizontalLayout())
        {
            GUILayout.Space(3);

            using (new HorizontalHelpBox(false, GUILayout.Width(750)))
            {
                GUILayout.Space(53);

                // Key column button
                using (new HorizontalHelpBox(false, GUILayout.Width(100)))
                {
                    if (GUILayout.Button("Key " + (_currentSortOption == SortOption.Key && _isAscending ? "▼" : "▲")))
                    {
                        _currentSortOption = SortOption.Key;
                        _isAscending = !_isAscending;
                    }
                }

                // Value column button
                using (new HorizontalHelpBox(false, GUILayout.Width(250)))
                {
                    if (GUILayout.Button("Value " + (_currentSortOption == SortOption.Value && _isAscending ? "▼" : "▲")))
                    {
                        _currentSortOption = SortOption.Value;
                        _isAscending = !_isAscending;
                    }
                }

                // Created column button
                using (new HorizontalHelpBox(false, GUILayout.Width(150)))
                {
                    if (GUILayout.Button("Created " + (_currentSortOption == SortOption.Created && _isAscending ? "▼" : "▲")))
                    {
                        _currentSortOption = SortOption.Created;
                        _isAscending = !_isAscending;
                    }
                }

                // Modified column button
                using (new HorizontalHelpBox(false, GUILayout.Width(150)))
                {
                    if (GUILayout.Button("Modified " + (_currentSortOption == SortOption.Modified && _isAscending ? "▼" : "▲")))
                    {
                        _currentSortOption = SortOption.Modified;
                        _isAscending = !_isAscending;
                    }
                }

                GUILayout.Space(35);
            }
        }
    }

    private void DisplayPlayerPrefs(List<PlayerPrefData> prefDataList)
    {
        using (new BeginScrollView(ref _scrollPosition))
        {
            var start = (_currentPage - 1) * ITEMS_PER_PAGE;
            var end = Mathf.Min(start + ITEMS_PER_PAGE, prefDataList.Count);

            for (var i = start; i < end; i++)
            {
                var data = prefDataList[i];

                using (new HorizontalHelpBox(false, GUILayout.Width(750)))
                {
                    // Index
                    using (new HorizontalHelpBox(false, GUILayout.Width(50), GUILayout.Height(25)))
                    {
                        GUILayout.Label($"{i + 1}.");
                    }

                    // Key
                    using (new HorizontalHelpBox(false, GUILayout.Width(100), GUILayout.Height(25)))
                    {
                        GUILayout.Label(data.Key);
                    }


                    // Editable Value field
                    using (new HorizontalHelpBox(false, GUILayout.Width(250), GUILayout.Height(25)))
                    {
                        if (!_editableValues.ContainsKey(data.Key))
                            _editableValues[data.Key] = data.Value;

                        var dataType = PlayerPrefsManager.GetKeyType(data.Key);
                        switch (dataType)
                        {
                            case PlayerPrefsManager.PlayerPrefType.String:
                                _editableValues[data.Key] = EditorGUILayout.TextField(_editableValues[data.Key]);
                                break;
                            case PlayerPrefsManager.PlayerPrefType.Int:
                                _editableValues[data.Key] = int.TryParse(_editableValues[data.Key], out var intValue) ? intValue.ToString() : "0";
                                _editableValues[data.Key] = EditorGUILayout.IntField(int.Parse(_editableValues[data.Key])).ToString();
                                break;
                            case PlayerPrefsManager.PlayerPrefType.Float:
                                _editableValues[data.Key] = float.TryParse(_editableValues[data.Key], out var floatValue) ? floatValue.ToString(CultureInfo.InvariantCulture) : "0";
                                _editableValues[data.Key] = EditorGUILayout.FloatField(float.Parse(_editableValues[data.Key])).ToString(CultureInfo.InvariantCulture);
                                break;
                        }

                        EditorGUILayout.LabelField(PlayerPrefsManager.GetKeyType(data.Key).ToString(), GUILayout.Width(40));

                        // Save button for edited value
                        using (new DisabledGUI(data.Value == _editableValues[data.Key]))
                        {
                            using (new BackgroundColorScope(Color.green))
                            {
                                if (GUILayout.Button("✔", GUILayout.Width(30)))
                                {
                                    if (data.Value != _editableValues[data.Key]) // Only save if the value has changed
                                    {
                                        PlayerPrefsManager.SetValue(data.Key, _editableValues[data.Key], PlayerPrefsManager.GetKeyType(data.Key));
                                        Debug.Log($"PlayerPref <b>'{data.Key}'</b> updated to <b>'{_editableValues[data.Key]}'</b>");
                                    }
                                }
                            }
                        }

                        if (GUILayout.Button("...", GUILayout.Width(20)))
                        {
                            DataViewWindow.Init(data.Key, data.Value);
                        }
                    }

                    // Created Time
                    using (new HorizontalHelpBox(false, GUILayout.Width(150), GUILayout.Height(25)))
                    {
                        GUILayout.Label(data.CreatedTime);
                    }

                    // Modified Time
                    using (new HorizontalHelpBox(false, GUILayout.Width(150), GUILayout.Height(25)))
                    {
                        GUILayout.Label(data.ModifiedTime);
                    }

                    // Remove player pref button
                    using (new HorizontalHelpBox(false, GUILayout.Width(20), GUILayout.Height(25)))
                    {
                        using (new BackgroundColorScope(Color.red))
                        {
                            if (GUILayout.Button("✘"))
                            {
                                PlayerPrefsManager.DeleteKey(data.Key);
                                _editableValues.Remove(data.Key);
                                Debug.Log($"PlayerPref '{data.Key}' deleted.");
                            }
                        }
                    }
                }
            }
        }
    }

    // Get player pref data including timestamps
    private List<PlayerPrefData> GetPlayerPrefsData(List<string> keys)
    {
        var prefDataList = new List<PlayerPrefData>();

        foreach (var key in keys)
        {
            var value = PlayerPrefsManager.GetValue(key);
            var createdTime = PlayerPrefsManager.GetCreatedTime(key);
            var modifiedTime = PlayerPrefsManager.GetModifiedTime(key);
            prefDataList.Add(new PlayerPrefData(key, value.ToString(), createdTime, modifiedTime));
        }

        return prefDataList;
    }

    // Sort PlayerPref data based on the current sort option
    private void SortPlayerPrefs(List<PlayerPrefData> prefDataList)
    {
        switch (_currentSortOption)
        {
            case SortOption.Key:
                prefDataList.Sort((x, y) => _isAscending
                                                ? string.CompareOrdinal(x.Key, y.Key)
                                                : string.CompareOrdinal(y.Key, x.Key));
                break;
            case SortOption.Value:
                prefDataList.Sort((x, y) => _isAscending
                                                ? string.CompareOrdinal(x.Value, y.Value)
                                                : string.CompareOrdinal(y.Value, x.Value));
                break;
            case SortOption.Created:
                prefDataList.Sort((x, y) => _isAscending
                                                ? string.CompareOrdinal(x.CreatedTime, y.CreatedTime)
                                                : string.CompareOrdinal(y.CreatedTime, x.CreatedTime));
                break;
            case SortOption.Modified:
                prefDataList.Sort((x, y) => _isAscending
                                                ? string.CompareOrdinal(x.ModifiedTime, y.ModifiedTime)
                                                : string.CompareOrdinal(y.ModifiedTime, x.ModifiedTime));
                break;
        }
    }
}