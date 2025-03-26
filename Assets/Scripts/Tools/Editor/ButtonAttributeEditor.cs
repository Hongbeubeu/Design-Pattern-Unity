using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace hcore.Tool
{
    [CustomEditor(typeof(MonoBehaviour), true)] // Applies to all MonoBehaviour scripts
    public class ButtonAttributeEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            // Draw the default Inspector first
            DrawDefaultInspector();

            // Get the target MonoBehaviour
            var monoBehaviour = (MonoBehaviour)target;

            // Get all methods in the target class
            var methods = monoBehaviour.GetType().GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);

            foreach (var method in methods)
            {
                // Check if the method has the Button attribute
                var buttonAttribute = method.GetCustomAttribute<ButtonAttribute>();
                if (buttonAttribute == null) continue;
                var buttonLabel = buttonAttribute.ButtonName ?? method.Name;
                var buttonHeight = buttonAttribute.ButtonHeight;

                var height = EditorGUIUtility.singleLineHeight * ((int)buttonHeight + 1) + EditorGUIUtility.standardVerticalSpacing;

                // Draw a button in the Inspector
                if (GUILayout.Button(buttonLabel, GUILayout.Height(height)))
                {
                    // Invoke the method when the button is clicked
                    method.Invoke(monoBehaviour, null);
                }
            }
        }
    }
}