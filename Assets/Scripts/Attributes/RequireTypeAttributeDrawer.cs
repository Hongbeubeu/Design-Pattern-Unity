#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(RequireTypeAttribute))]
public class RequireTypeDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        var requireTypeAttribute = (RequireTypeAttribute)attribute;

        EditorGUI.BeginProperty(position, label, property);

        if (property.propertyType == SerializedPropertyType.ObjectReference)
        {
            // Hiển thị ObjectField
            property.objectReferenceValue = EditorGUI.ObjectField(position, label, property.objectReferenceValue,
                typeof(GameObject), true);

            if (property.objectReferenceValue != null)
            {
                GameObject gameObject = property.objectReferenceValue as GameObject;
                if (gameObject != null)
                {
                    // Kiểm tra xem GameObject có component nào thuộc kiểu yêu cầu không
                    var hasRequiredComponent = false;
                    foreach (var component in gameObject.GetComponents<Component>())
                    {
                        if (requireTypeAttribute.RequiredType.IsInstanceOfType(component))
                        {
                            hasRequiredComponent = true;
                            break;
                        }
                    }

                    if (!hasRequiredComponent)
                    {
                        property.objectReferenceValue = null; // Xóa đối tượng không phù hợp
                        Debug.LogWarning(
                            $"{gameObject.name} does not have a component of type {requireTypeAttribute.RequiredType}");
                    }
                }
            }
        }
        else
        {
            EditorGUI.LabelField(position, label.text, "Use RequireType with Object fields.");
        }

        EditorGUI.EndProperty();
    }
}
#endif