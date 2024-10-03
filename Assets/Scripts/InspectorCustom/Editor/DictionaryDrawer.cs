using UnityEditor;
using UnityEngine;

namespace InspectorCustom
{
    [CustomPropertyDrawer(typeof(SerializableDictionary<,>), true)]
    public class DictionaryDrawer : PropertyDrawer
    {
        private const float ITEM_SPACING = 10;
        private const float TITLE_WIDTH = 50;
        private const float BUTTON_WIDTH = 30;

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);
            var keys = property.FindPropertyRelative("keys");
            var values = property.FindPropertyRelative("values");

            var entryHeight = EditorGUIUtility.singleLineHeight;
            property.isExpanded = EditorGUI.Foldout(new Rect(position.x, position.y, position.width, entryHeight), property.isExpanded, label.text);
            if (property.isExpanded)
            {
                // var helpBoxRect = new Rect(position.x + 20, position.y + entryHeight, position.width, keys.arraySize * 2 * entryHeight + (keys.arraySize + 1) * ITEM_SPACING);
                // EditorGUI.HelpBox(helpBoxRect, "", MessageType.None);

                for (var i = 0; i < keys.arraySize; i++)
                {
                    if (i == 0)
                        position.y += ITEM_SPACING;
                    var keyTitleRect = new Rect(position.x + 2, position.y + (2 * i + 1) * entryHeight, TITLE_WIDTH, entryHeight);
                    var keyRect = new Rect(position.x + TITLE_WIDTH, position.y + (2 * i + 1) * entryHeight, position.width - TITLE_WIDTH, entryHeight);

                    EditorGUI.LabelField(keyTitleRect, "Key");
                    var offset = DrawProperty(keyRect, keys.GetArrayElementAtIndex(i));
                    position.y += offset - EditorGUIUtility.singleLineHeight;

                    var valueTitleRect = new Rect(position.x + 2, position.y + (2 * i + 2) * entryHeight, TITLE_WIDTH, entryHeight);
                    var valueRect = new Rect(position.x + TITLE_WIDTH, position.y + (2 * i + 2) * entryHeight, position.width - TITLE_WIDTH, entryHeight);

                    EditorGUI.LabelField(valueTitleRect, "Value");
                    offset = DrawProperty(valueRect, values.GetArrayElementAtIndex(i));
                    position.y += offset - EditorGUIUtility.singleLineHeight;
                    position.y += ITEM_SPACING;
                }

                if (GUI.Button(new Rect(position.x + position.width - BUTTON_WIDTH, position.y + (2 * keys.arraySize + 1) * entryHeight, BUTTON_WIDTH, entryHeight), "+"))
                {
                    keys.arraySize++;
                    values.arraySize++;
                }

                if (GUI.Button(new Rect(position.x + position.width - 2 * BUTTON_WIDTH, position.y + (2 * keys.arraySize + 1) * entryHeight, BUTTON_WIDTH, entryHeight), "-"))
                {
                    keys.arraySize--;
                    values.arraySize--;
                }
            }

            EditorGUI.EndProperty();
        }

        private float DrawProperty(Rect position, SerializedProperty property)
        {
            if (property.hasVisibleChildren)
            {
                if (property.type.Contains("SerializableDictionary"))
                {
                    EditorGUI.PropertyField(position, property, new GUIContent($"{property.type}<{property.FindPropertyRelative("keys")?.arrayElementType},{property.FindPropertyRelative("values")?.arrayElementType}>"), true);
                }
                else
                {
                    EditorGUI.PropertyField(position, property, new GUIContent($"{property.type}"), true);
                }
            }
            else
            {
                EditorGUI.PropertyField(position, property, GUIContent.none);
            }

            return GetPropertyHeight(property, GUIContent.none);
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            var height = EditorGUIUtility.singleLineHeight;
            if (!property.isExpanded) return height;

            if (property.type.Contains("SerializableDictionary"))
            {
                var keys = property.FindPropertyRelative("keys");
                var arraySize = keys?.arraySize ?? 0;
                height = (2 * arraySize + 2) * EditorGUIUtility.singleLineHeight + (arraySize + 1) * ITEM_SPACING;

                if (keys is { hasVisibleChildren: true })
                {
                    for (var i = 0; i < keys.arraySize; i++)
                    {
                        height += EditorGUI.GetPropertyHeight(keys.GetArrayElementAtIndex(i), GUIContent.none) - EditorGUIUtility.singleLineHeight;
                    }
                }

                var values = property.FindPropertyRelative("values");
                if (values is { hasVisibleChildren: true })
                {
                    for (var i = 0; i < values.arraySize; i++)
                    {
                        height += EditorGUI.GetPropertyHeight(values.GetArrayElementAtIndex(i), GUIContent.none) - EditorGUIUtility.singleLineHeight;
                    }
                }
            }
            else
            {
                height = EditorGUI.GetPropertyHeight(property, GUIContent.none, true);
            }

            return height;
        }
    }
}