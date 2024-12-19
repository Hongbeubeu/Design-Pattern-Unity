using System.Reflection;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(MonoBehaviour), true)] // Applies to all MonoBehaviour scripts
public class ButtonAttributeEditor : Editor
{
    public override void OnInspectorGUI()
    {
        // Draw the default Inspector first
        DrawDefaultInspector();

        // Get the target MonoBehaviour
        MonoBehaviour monoBehaviour = (MonoBehaviour)target;

        // Get all methods in the target class
        MethodInfo[] methods = monoBehaviour.GetType().GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);

        foreach (MethodInfo method in methods)
        {
            // Check if the method has the Button attribute
            ButtonAttribute buttonAttribute = method.GetCustomAttribute<ButtonAttribute>();
            if (buttonAttribute != null)
            {
                string buttonLabel = buttonAttribute.ButtonName ?? method.Name;

                // Draw a button in the Inspector
                if (GUILayout.Button(buttonLabel))
                {
                    // Invoke the method when the button is clicked
                    method.Invoke(monoBehaviour, null);
                }
            }
        }
    }
}