using UnityEditor;
using UnityEngine;

public class ModelMaterialAssigner : EditorWindow
{
    private Material materialToAssign; // Material cần gán
    private Object[] selectedModels;  // Danh sách model được chọn
    private Vector2 scrollPosition;  // Vị trí cuộn thanh scroll

    [MenuItem("Tools/Model Material Assigner")]
    public static void ShowWindow()
    {
        GetWindow<ModelMaterialAssigner>("Model Material Assigner");
    }

    private void OnGUI()
    {
        GUILayout.Label("Material Assigner for Models", EditorStyles.boldLabel);

        // Chọn material
        materialToAssign = (Material)EditorGUILayout.ObjectField("Material to Assign", materialToAssign, typeof(Material), false);

        // Nút tìm model
        if (GUILayout.Button("Find All Models"))
        {
            FindAllModels();
        }

        // Hiển thị danh sách model trong thanh cuộn
        if (selectedModels != null && selectedModels.Length > 0)
        {
            EditorGUILayout.LabelField($"Found {selectedModels.Length} Models:");

            // Bắt đầu vùng cuộn
            scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition, GUILayout.Height(300)); // Chiều cao tối đa của danh sách là 300px
            foreach (Object model in selectedModels)
            {
                EditorGUILayout.ObjectField(model.name, model, typeof(GameObject), false);
            }
            EditorGUILayout.EndScrollView(); // Kết thúc vùng cuộn
        }

        // Áp dụng material
        if (selectedModels != null && materialToAssign != null && selectedModels.Length > 0)
        {
            if (GUILayout.Button("Assign Material to Models"))
            {
                AssignMaterialToModels();
            }
        }
    }

    private void FindAllModels()
    {
        string[] guids = AssetDatabase.FindAssets("t:Model");
        selectedModels = new Object[guids.Length];

        for (int i = 0; i < guids.Length; i++)
        {
            string path = AssetDatabase.GUIDToAssetPath(guids[i]);
            selectedModels[i] = AssetDatabase.LoadAssetAtPath<GameObject>(path);
        }

        Debug.Log($"Found {selectedModels.Length} models in the project.");
    }

    private void AssignMaterialToModels()
    {
        foreach (Object model in selectedModels)
        {
            string path = AssetDatabase.GetAssetPath(model);
            ModelImporter importer = AssetImporter.GetAtPath(path) as ModelImporter;

            if (importer != null)
            {
                importer.materialImportMode = ModelImporterMaterialImportMode.ImportViaMaterialDescription;
                AssetDatabase.ImportAsset(path, ImportAssetOptions.ForceUpdate);
                Debug.Log($"Assigned material to model: {model.name}");
            }
        }

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        Debug.Log("Material assignment complete!");
    }
}
