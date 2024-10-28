using System;
using UnityEditor;
using UnityEngine;

namespace SimpleParticleSystem
{
    [CustomPropertyDrawer(typeof(ShapeEmission))]
    public class ShapeEmissionDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);

            // Căn lề nội bộ
            var lineHeight = EditorGUIUtility.singleLineHeight;
            var padding = EditorGUIUtility.standardVerticalSpacing;
            var fieldRect = new Rect(position.x, position.y, position.width, lineHeight);

            // Thuộc tính "shapeType"
            var shapeTypeProp = property.FindPropertyRelative("shapeEmissionType");
            EditorGUI.PropertyField(fieldRect, shapeTypeProp);
            fieldRect.y += lineHeight + padding;

            // Thuộc tính "position"
            var positionProp = property.FindPropertyRelative("position");
            EditorGUI.PropertyField(fieldRect, positionProp, new GUIContent("Position"));
            fieldRect.y += lineHeight + padding;

            // Hiển thị các thuộc tính tùy thuộc vào shapeType
            var shapeType = (ShapeEmissionType)shapeTypeProp.enumValueIndex;
            switch (shapeType)
            {
                case ShapeEmissionType.Circle:
                    SerializedProperty radiusPropCircle = property.FindPropertyRelative("radius");
                    EditorGUI.PropertyField(fieldRect, radiusPropCircle, new GUIContent("Radius"));
                    fieldRect.y += lineHeight + padding;
                    break;

                case ShapeEmissionType.Sphere:
                    SerializedProperty radiusPropSphere = property.FindPropertyRelative("radius");
                    EditorGUI.PropertyField(fieldRect, radiusPropSphere, new GUIContent("Radius"));
                    fieldRect.y += lineHeight + padding;
                    break;

                case ShapeEmissionType.Box:
                    SerializedProperty sizeProp = property.FindPropertyRelative("size");
                    EditorGUI.PropertyField(fieldRect, sizeProp, new GUIContent("Size"));
                    fieldRect.y += lineHeight + padding;
                    break;
                case ShapeEmissionType.Point:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            EditorGUI.EndProperty();
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            // Chiều cao của thuộc tính thay đổi tùy theo shapeType
            var lineHeight = EditorGUIUtility.singleLineHeight;
            var padding = EditorGUIUtility.standardVerticalSpacing;

            var shapeTypeProp = property.FindPropertyRelative("shapeEmissionType");
            var shapeType = (ShapeEmissionType)shapeTypeProp.enumValueIndex;

            var height = lineHeight * 2 + padding; // shapeType + position

            // Thêm chiều cao tùy theo loại shapeType
            switch (shapeType)
            {
                case ShapeEmissionType.Circle:
                case ShapeEmissionType.Sphere:
                    height += lineHeight + padding; // radius
                    break;
                case ShapeEmissionType.Box:
                    height += lineHeight + padding; // size
                    break;
                case ShapeEmissionType.Point:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            return height;
        }
    }
}