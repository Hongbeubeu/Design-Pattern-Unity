using System;
using UnityEditor;
using UnityEngine;

#if UNITY_EDITOR
namespace hcore.SimpleParticleSystem
{
    [CustomPropertyDrawer(typeof(RandomNumberInt))]
    [CustomPropertyDrawer(typeof(RandomNumberFloat))]
    [CustomPropertyDrawer(typeof(RandomNumberDouble))]
    public class RandomNumberDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);

            // Hiển thị nhãn của thuộc tính
            position = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), label);

            // Lấy các thuộc tính
            var typeProperty = property.FindPropertyRelative("type");
            var valueProperty = property.FindPropertyRelative("constantValue");
            var minProperty = property.FindPropertyRelative("min");
            var maxProperty = property.FindPropertyRelative("max");
            var curveProperty = property.FindPropertyRelative("curve");

            // Xác định chiều rộng cho các trường
            var typeRect = new Rect(position.x, position.y, position.width / 3, position.height);
            var valueRect = new Rect(position.x + position.width / 3, position.y, position.width / 3 * 2, position.height);

            // Hiển thị trường type
            EditorGUI.PropertyField(typeRect, typeProperty, GUIContent.none);

            // Hiển thị các trường còn lại dựa trên giá trị của type
            var type = (RandomType)typeProperty.enumValueIndex;
            switch (type)
            {
                case RandomType.Constant:
                    EditorGUI.PropertyField(valueRect, valueProperty, new GUIContent(""));
                    break;

                case RandomType.Range:
                    // Chia nhỏ khung cho min và max
                    var minRect = new Rect(position.x + position.width / 3, position.y, position.width / 3, position.height);
                    var maxRect = new Rect(position.x + position.width * 2 / 3, position.y, position.width / 3, position.height);

                    EditorGUI.PropertyField(minRect, minProperty, new GUIContent(""));
                    EditorGUI.PropertyField(maxRect, maxProperty, new GUIContent(""));
                    break;

                case RandomType.Curve:
                    EditorGUI.PropertyField(valueRect, curveProperty, new GUIContent(""));
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            EditorGUI.EndProperty();
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return EditorGUIUtility.singleLineHeight;
        }
    }
}
#endif