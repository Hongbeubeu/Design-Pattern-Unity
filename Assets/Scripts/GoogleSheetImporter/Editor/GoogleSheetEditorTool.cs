using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Sheets.v4;
using Google.Apis.Services;

namespace GoogleSheetImporter.Editor
{
    public class GoogleSheetEditorTool : EditorWindow
    {
        private string _credentialsPath = "Path/To/Your/Credentials.json"; // Đường dẫn tới file JSON
        private string _spreadsheetId = ""; // ID của Google Sheets
        private string _range = "Sheet1!A1:C10"; // Dải dữ liệu
        private List<IList<object>> _sheetData = new();

        private SheetsService _sheetsService;

        [MenuItem("Tools/Google Sheets Importer")]
        public static void ShowWindow()
        {
            GetWindow<GoogleSheetEditorTool>("Google Sheets Importer");
        }

        private void OnEnable()
        {
            _credentialsPath = EditorPrefs.GetString("GoogleSheetImporter_CredentialsPath", _credentialsPath);
            _spreadsheetId = EditorPrefs.GetString("GoogleSheetImporter_SpreadsheetId", _spreadsheetId);
        }

        private void OnDisable()
        {
            EditorPrefs.SetString("GoogleSheetImporter_CredentialsPath", _credentialsPath);
            EditorPrefs.SetString("GoogleSheetImporter_SpreadsheetId", _spreadsheetId);
        }

        private void OnGUI()
        {
            GUILayout.Label("Google Sheets Importer", EditorStyles.boldLabel);

            // Nút chọn file credentials.json
            GUILayout.BeginHorizontal();
            GUILayout.Label("Credentials Path:", GUILayout.Width(100));
            _credentialsPath = GUILayout.TextField(_credentialsPath, GUILayout.Width(position.width - 130 - 4 * EditorGUIUtility.standardVerticalSpacing));
            if (GUILayout.Button("...", GUILayout.Width(25)))
            {
                _credentialsPath = EditorUtility.OpenFilePanel("Select Credentials File", "", "json");
            }

            GUILayout.EndHorizontal();

            // Nhập Spreadsheet ID
            GUILayout.Label("Spreadsheet ID:", EditorStyles.label);
            _spreadsheetId = EditorGUILayout.TextField(_spreadsheetId);

            // Nhập phạm vi dữ liệu (Range)
            GUILayout.Label("Range:", EditorStyles.label);
            _range = EditorGUILayout.TextField(_range);

            // Nút "Initialize"
            if (GUILayout.Button("Initialize Google Sheets API"))
            {
                InitializeGoogleSheetsAPI();
            }

            // Nút "Fetch Data"
            if (GUILayout.Button("Fetch Data"))
            {
                FetchSheetData();
            }

            // Hiển thị dữ liệu đã tải
            GUILayout.Space(10);
            GUILayout.Label("Fetched Data:", EditorStyles.boldLabel);

            if (_sheetData is { Count: > 0 })
            {
                foreach (var row in _sheetData)
                {
                    GUILayout.Label(string.Join(", ", row));
                }
            }
        }

        private void InitializeGoogleSheetsAPI()
        {
            if (!File.Exists(_credentialsPath))
            {
                Debug.LogError("Credentials file not found at path: " + _credentialsPath);
                return;
            }

            GoogleCredential credential;

            using (var stream = new FileStream(_credentialsPath, FileMode.Open, FileAccess.Read))
            {
                credential = GoogleCredential.FromStream(stream).CreateScoped(SheetsService.Scope.SpreadsheetsReadonly);
            }

            _sheetsService = new SheetsService(new BaseClientService.Initializer
                                               {
                                                   HttpClientInitializer = credential,
                                                   ApplicationName = "Google Sheets Importer"
                                               });

            Debug.Log("Google Sheets API initialized successfully.");
        }

        private void FetchSheetData()
        {
            if (_sheetsService == null)
            {
                InitializeGoogleSheetsAPI();
            }

            if (_sheetsService == null)
            {
                Debug.LogError("The Google Sheets API could not be initialized.");
                return;
            }

            if (string.IsNullOrEmpty(_spreadsheetId) || string.IsNullOrEmpty(_range))
            {
                Debug.LogError("Spreadsheet ID or Range is empty.");
                return;
            }

            var request = _sheetsService.Spreadsheets.Values.Get(_spreadsheetId, _range);

            try
            {
                var response = request.Execute();
                _sheetData = response.Values != null ? new List<IList<object>>(response.Values) : new List<IList<object>>();
                Debug.Log("Data fetched successfully.");
            }
            catch (Exception ex)
            {
                Debug.LogError("Error fetching data: " + ex.Message);
            }
        }
    }
}