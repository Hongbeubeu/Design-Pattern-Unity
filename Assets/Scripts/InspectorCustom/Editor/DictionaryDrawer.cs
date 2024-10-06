using System.Collections.Generic;
using System.Globalization;
using UnityEditor;
using UnityEngine;

namespace InspectorCustom
{
    public readonly struct BorderItem
    {
        public float Left { get; }

        public float Right { get; }

        public float Top { get; }

        public float Bottom { get; }

        public BorderItem(float left, float right, float top, float bottom)
        {
            Left = left;
            Right = right;
            Top = top;
            Bottom = bottom;
        }
    }

    [CustomPropertyDrawer(typeof(SerializableDictionary<,>), true)]
    public class DictionaryDrawer : PropertyDrawer
    {
        private const float KEY_VALUE_SPACING = 4;
        private const float ITEM_SPACING = 4;
        private const float TITLE_WIDTH = 50;
        private const float BUTTON_WIDTH = 30;
        private const float DRAG_HANDLE_WIDTH = 20;
        private const float DELETE_BUTTON_WIDTH = 25;
        private const float ARRAY_SIZE_WIDTH = 50;
        private const float MOVE_BUTTON_WIDTH = 30;

        private static readonly BorderItem Border = new(left: 4, right: 4, top: 4, bottom: 4);

        private static readonly Color ErrorColor = new(1, 0, 0, 0.1f);
        private static readonly Color EvenColor = new(0.4f, 0.4f, 0.4f, 0.5f);
        private static readonly Color MiddleColor = new(0.6f, 0.6f, 0.6f, 0.6f);
        private static readonly Color OddColor = new(0.3f, 0.3f, 0.3f, 0.5f);
        private static readonly Color ItemColor = new(0.2f, 0.2f, 0.2f, 0.5f);
        private static readonly Color SelectedColor = new(0.1f, 0.3f, 0.5f, 0.5f);

        private static readonly float LineHeight = EditorGUIUtility.singleLineHeight;

        private static GUIStyle _boldLabelStyle;
        private static GUIStyle _boldLabelStyleMiddle;
        private static GUIStyle _boldLabelStyleYellow;
        private static GUIStyle _boldLabelStyleRed;
        private static GUIStyle _buttonStyle;
        private static GUIStyle _buttonStyleRed;
        private static GUIStyle _buttonStyle10;
        private static GUIStyle _buttonStyle12;

        private readonly Dictionary<object, List<int>> _keyDuplicates = new();
        private static readonly NullObject NullObjectValue = new();

        private bool _isInitialized;
        private int _currentSelectedIndex = -1;
        private float _currentWidth;
        private int _currentPageNumber = 1;
        private int _itemsPerPage = 10;

        private class NullObject
        {
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            _currentWidth = position.width;

            // Draw property field
            using (new EditorGUIProperty(position, property, label))
            {
                var keys = property.FindPropertyRelative("keys");
                var values = property.FindPropertyRelative("values");

                // Check if keys property exists
                if (keys == null)
                {
                    EditorGUI.LabelField(position, "Keys property not found", _boldLabelStyleRed);
                    EditorGUI.EndProperty();
                    return;
                }

                // Check for duplicates
                CheckForDuplicates(keys);
                if (!_isInitialized)
                {
                    InitializeStyle();
                    RemoveDuplicates(keys, values);
                }


                var foldoutPropertyRect = new Rect(position.x, position.y, _currentWidth - ARRAY_SIZE_WIDTH, LineHeight);
                label = property.isExpanded ? new GUIContent($"▼ {label.text}") : new GUIContent($"► {label.text}");
                EditorGUI.DrawRect(foldoutPropertyRect, foldoutPropertyRect.Contains(Event.current.mousePosition) ? EvenColor : OddColor);

                var textColor = Color.white;
                if (property.isExpanded)
                {
                    textColor = Color.green;
                }

                EditorGUI.LabelField(foldoutPropertyRect, label, new GUIStyle(GUI.skin.label)
                                                                 {
                                                                     fontStyle = FontStyle.Bold,
                                                                     normal = { textColor = textColor }
                                                                 });
                if (Event.current.type == EventType.MouseDown && foldoutPropertyRect.Contains(Event.current.mousePosition))
                {
                    property.isExpanded = !property.isExpanded;
                    Event.current.Use();
                }

                keys.arraySize = EditorGUI.IntField(new Rect(position.x + _currentWidth - ARRAY_SIZE_WIDTH, position.y, ARRAY_SIZE_WIDTH, LineHeight), keys.arraySize);
                values.arraySize = keys.arraySize;

                if (property.isExpanded)
                {
                    DrawProperty(position, property, keys, values);
                }
                else
                {
                    _currentSelectedIndex = -1;
                }
            }
        }


        private void DrawProperty(Rect position, SerializedProperty property, SerializedProperty keys, SerializedProperty values)
        {
            // Draw bound box
            var helpBoxRect = new Rect(position.x, position.y + LineHeight, position.width, GetPropertyHeight(property, GUIContent.none) - LineHeight + ITEM_SPACING);
            EditorGUI.HelpBox(helpBoxRect, "", MessageType.None);

            using (new DisposableAction(() => { position.y += ITEM_SPACING; }, () => { position.y -= ITEM_SPACING; }))
            {
                var (from, to) = GetPageRange(_currentPageNumber, _itemsPerPage, keys.arraySize);
                var arraySize = to - from;
                // Notifying user if dictionary is empty
                if (keys.arraySize == 0)
                {
                    EditorGUI.indentLevel++;
                    EditorGUI.LabelField(new Rect(position.x, position.y + LineHeight, position.width, LineHeight), "Dictionary is empty", _boldLabelStyleYellow);
                    EditorGUI.indentLevel--;
                }
                else
                {
                    // Draw key-value pairs
                    for (var i = from; i < to; i++)
                    {
                        var currentIndexInPage = i - from;
                        var currentKeyProperty = keys.GetArrayElementAtIndex(i);
                        var currentValueProperty = values.GetArrayElementAtIndex(i);

                        var keyRectHeight = EditorGUI.GetPropertyHeight(currentKeyProperty, GUIContent.none);
                        var valueRectHeight = EditorGUI.GetPropertyHeight(currentValueProperty, GUIContent.none);

                        // Background
                        using (new DisposableAction(() =>
                        {
                            position.x += Border.Left;
                            _currentWidth -= Border.Left;
                            _currentWidth -= Border.Right;
                        }, () =>
                        {
                            position.x -= Border.Left;
                            _currentWidth += Border.Left;
                            _currentWidth += Border.Right;
                        }))
                        {
                            // Draw background color for each key-value pair
                            var boundColorRect = new Rect(position.x, position.y + (2 * currentIndexInPage + 1) * LineHeight, _currentWidth, keyRectHeight + valueRectHeight + KEY_VALUE_SPACING + Border.Bottom);

                            if (Event.current.type == EventType.MouseDown && boundColorRect.Contains(Event.current.mousePosition))
                            {
                                if (_currentSelectedIndex != i)
                                {
                                    _currentSelectedIndex = i;
                                    Event.current.Use();
                                }
                            }

                            if (_currentSelectedIndex == i)
                            {
                                EditorGUI.DrawRect(boundColorRect, SelectedColor);
                            }
                            else
                            {
                                if (!IsValidKeyAt(i))
                                    EditorGUI.DrawRect(boundColorRect, ErrorColor);
                                else
                                    EditorGUI.DrawRect(boundColorRect, i % 2 == 0 ? EvenColor : OddColor);
                            }

                            // Indicator label
                            var indicatorRect = new Rect(position.x + Border.Left, position.y + (2 * currentIndexInPage + 1) * LineHeight + Border.Top, DRAG_HANDLE_WIDTH, LineHeight);
                            EditorGUI.LabelField(indicatorRect, "═", _boldLabelStyle);

                            var moveUpButtonRect = new Rect(position.x + DRAG_HANDLE_WIDTH + 2, position.y + (2 * currentIndexInPage + 1) * LineHeight + Border.Top, MOVE_BUTTON_WIDTH, LineHeight);
                            var moveDownButtonRect = new Rect(position.x + DRAG_HANDLE_WIDTH + 2, position.y + (2 * currentIndexInPage + 2) * LineHeight + Border.Top, MOVE_BUTTON_WIDTH, LineHeight);

                            // Move up button
                            using (new DisposableDisabledGUI(i == 0))
                            {
                                EditorGUI.DrawRect(moveUpButtonRect, EvenColor);
                                EditorGUI.LabelField(moveUpButtonRect, "▲", _buttonStyle10);

                                if (GUI.Button(moveUpButtonRect, "▲", _buttonStyle10))
                                {
                                    keys.MoveArrayElement(i, i - 1);
                                    values.MoveArrayElement(i, i - 1);
                                    _currentSelectedIndex = i - 1;
                                }
                            }

                            // Move down button
                            using (new DisposableDisabledGUI(i == keys.arraySize - 1))
                            {
                                EditorGUI.DrawRect(moveDownButtonRect, EvenColor);
                                EditorGUI.LabelField(moveDownButtonRect, "▼", _buttonStyle10);

                                if (GUI.Button(moveDownButtonRect, "▼", _buttonStyle10))
                                {
                                    keys.MoveArrayElement(i, i + 1);
                                    values.MoveArrayElement(i, i + 1);
                                    _currentSelectedIndex = i + 1;
                                }
                            }

                            var deleteButtonRect = new Rect(position.x + _currentWidth - DELETE_BUTTON_WIDTH - Border.Right, position.y + (2 * currentIndexInPage + 1) * LineHeight + Border.Top, DELETE_BUTTON_WIDTH, LineHeight);

                            // Key field
                            var keyTitleRect = new Rect(position.x + DRAG_HANDLE_WIDTH + 2 + MOVE_BUTTON_WIDTH, position.y + (2 * currentIndexInPage + 1) * LineHeight + Border.Top, TITLE_WIDTH, LineHeight);
                            var keyRect = new Rect(position.x + DRAG_HANDLE_WIDTH + MOVE_BUTTON_WIDTH + TITLE_WIDTH, position.y + (2 * currentIndexInPage + 1) * LineHeight + Border.Top, _currentWidth - DRAG_HANDLE_WIDTH - MOVE_BUTTON_WIDTH - TITLE_WIDTH - DELETE_BUTTON_WIDTH - 2 * Border.Right, LineHeight);

                            var keyBoundRect = keyRect;
                            keyBoundRect.height = keyRectHeight;

                            EditorGUI.DrawRect(keyBoundRect, ItemColor);
                            EditorGUI.LabelField(keyTitleRect, "Key");
                            DrawProperty(keyRect, currentKeyProperty);

                            position.y += keyRectHeight - LineHeight + KEY_VALUE_SPACING;

                            // Value field
                            var valueTitleRect = new Rect(position.x + DRAG_HANDLE_WIDTH + 2 + MOVE_BUTTON_WIDTH, position.y + (2 * currentIndexInPage + 2) * LineHeight, TITLE_WIDTH, LineHeight);
                            var valueRect = new Rect(position.x + DRAG_HANDLE_WIDTH + MOVE_BUTTON_WIDTH + TITLE_WIDTH, position.y + (2 * currentIndexInPage + 2) * LineHeight, _currentWidth - DRAG_HANDLE_WIDTH - MOVE_BUTTON_WIDTH - TITLE_WIDTH - DELETE_BUTTON_WIDTH - 2 * Border.Right, LineHeight);

                            var valueBoundRect = valueRect;
                            valueBoundRect.height = valueRectHeight;

                            EditorGUI.DrawRect(valueBoundRect, ItemColor);
                            EditorGUI.LabelField(valueTitleRect, "Value");
                            DrawProperty(valueRect, currentValueProperty);

                            // Context menu copy/paste for key
                            if (Event.current.type == EventType.ContextClick && keyTitleRect.Contains(Event.current.mousePosition))
                            {
                                var menu = new GenericMenu();
                                var currentIndex = i;
                                menu.AddItem(new GUIContent("Copy Key"), false, () => CopyProperty(keys.GetArrayElementAtIndex(currentIndex)));
                                menu.AddItem(new GUIContent("Paste Key"), false, () => PasteProperty(keys.GetArrayElementAtIndex(currentIndex)));
                                menu.ShowAsContext();
                                Event.current.Use(); // Mark event as used
                            }

                            position.y += valueRectHeight - LineHeight + ITEM_SPACING;

                            // Delete button
                            if (GUI.Button(deleteButtonRect, "☓", _buttonStyleRed))
                            {
                                keys.DeleteArrayElementAtIndex(i);
                                values.DeleteArrayElementAtIndex(i);
                            }

                            DoContextMenu(keys, values, valueTitleRect, i, boundColorRect);
                        }
                    }
                }

                // Pagination
                if (keys.arraySize > 0)
                {
                    var paginationRect = new Rect(position.x + Border.Left, position.y + (2 * arraySize + 1) * LineHeight, _currentWidth - 2 * BUTTON_WIDTH - Border.Left - 2 * Border.Right, LineHeight);
                    EditorGUI.DrawRect(paginationRect, ItemColor);

                    EditorGUI.LabelField(paginationRect, $"Page {_currentPageNumber} of {Mathf.Ceil(keys.arraySize / (float)_itemsPerPage)}", _boldLabelStyleMiddle);
                    _itemsPerPage = EditorGUI.IntField(new Rect(position.x + Border.Left + 4 * BUTTON_WIDTH, position.y + (2 * arraySize + 1) * LineHeight, BUTTON_WIDTH, LineHeight), _itemsPerPage);
                    _itemsPerPage = _itemsPerPage == 0 ? 1 : _itemsPerPage; // Prevent division by zero
                }

                if (keys.arraySize > _itemsPerPage)
                {
                    // Previous button
                    using (new DisposableDisabledGUI(_currentPageNumber == 1))
                    {
                        var previousButtonRect = new Rect(position.x + 6 * BUTTON_WIDTH, position.y + (2 * arraySize + 1) * LineHeight, BUTTON_WIDTH, LineHeight);
                        EditorGUI.DrawRect(previousButtonRect, OddColor);


                        EditorGUI.LabelField(previousButtonRect, "◀", _buttonStyle10);
                        if (GUI.Button(previousButtonRect, "◀", _buttonStyle10))
                        {
                            _currentPageNumber = Mathf.Max(1, _currentPageNumber - 1);
                        }
                    }

                    // Next button
                    using (new DisposableDisabledGUI(_currentPageNumber == Mathf.CeilToInt(keys.arraySize / (float)_itemsPerPage)))
                    {
                        var nextButtonRect = new Rect(position.x + 7 * BUTTON_WIDTH + ITEM_SPACING, position.y + (2 * arraySize + 1) * LineHeight, BUTTON_WIDTH, LineHeight);
                        EditorGUI.DrawRect(nextButtonRect, OddColor);
                        EditorGUI.LabelField(nextButtonRect, "▶", _buttonStyle12);

                        if (GUI.Button(nextButtonRect, "▶", _buttonStyle12))
                        {
                            _currentPageNumber = Mathf.Min(Mathf.CeilToInt(keys.arraySize / (float)_itemsPerPage), _currentPageNumber + 1);
                        }
                    }
                }

                // Add button
                var addButtonRect = new Rect(position.x + _currentWidth - 2 * BUTTON_WIDTH - Border.Right, position.y + (2 * arraySize + 1) * LineHeight, BUTTON_WIDTH, LineHeight);
                EditorGUI.DrawRect(addButtonRect, ItemColor);
                EditorGUI.LabelField(addButtonRect, "＋", _buttonStyle);

                if (Event.current.type == EventType.MouseDown && addButtonRect.Contains(Event.current.mousePosition))
                {
                    keys.arraySize++;
                    values.arraySize++;
                    Event.current.Use();
                }

                // Remove button
                using (new DisposableDisabledGUI(arraySize == 0))
                {
                    var removeButtonRect = new Rect(position.x + _currentWidth - BUTTON_WIDTH - Border.Right, position.y + (2 * arraySize + 1) * LineHeight, BUTTON_WIDTH, LineHeight);
                    EditorGUI.DrawRect(removeButtonRect, ItemColor);
                    EditorGUI.LabelField(removeButtonRect, "－", _buttonStyle);

                    if (Event.current.type == EventType.MouseDown && removeButtonRect.Contains(Event.current.mousePosition))
                    {
                        if (_currentSelectedIndex >= 0)
                        {
                            keys.DeleteArrayElementAtIndex(_currentSelectedIndex);
                            values.DeleteArrayElementAtIndex(_currentSelectedIndex);
                            _currentSelectedIndex = -1;
                        }
                        else
                        {
                            keys.arraySize--;
                            values.arraySize--;
                        }

                        Event.current.Use();
                    }
                }
            }
        }

        private static void DoContextMenu(SerializedProperty keys, SerializedProperty values, Rect valueTitleRect, int index, Rect boundColorRect)
        {
            // Context menu copy/paste for value
            if (Event.current.type == EventType.ContextClick && valueTitleRect.Contains(Event.current.mousePosition))
            {
                var menu = new GenericMenu();
                var currentIndex = index;
                menu.AddItem(new GUIContent("Copy Value"), false, () => CopyProperty(values.GetArrayElementAtIndex(currentIndex)));
                menu.AddItem(new GUIContent("Paste Value"), false, () => PasteProperty(values.GetArrayElementAtIndex(currentIndex)));
                menu.ShowAsContext();
                Event.current.Use(); // Mark event as used
            }

            // Context menu copy/paste for key-value
            if (Event.current.type == EventType.ContextClick && boundColorRect.Contains(Event.current.mousePosition))
            {
                var menu = new GenericMenu();
                var currentIndex = index;
                menu.AddItem(new GUIContent("Copy Key-Value"), false, () =>
                {
                    CopyProperty(keys.GetArrayElementAtIndex(currentIndex));
                    CopyProperty(values.GetArrayElementAtIndex(currentIndex));
                });
                if (CopiedProperties.Count > 0)
                {
                    menu.AddItem(new GUIContent("Paste Key-Value"), false, () =>
                    {
                        PasteProperty(keys.GetArrayElementAtIndex(currentIndex), 1);
                        PasteProperty(values.GetArrayElementAtIndex(currentIndex));
                    });
                }
                else
                {
                    menu.AddDisabledItem(new GUIContent("Paste Key-Value"));
                }

                menu.ShowAsContext();
                Event.current.Use(); // Mark event as used
            }
        }

        private static void DrawProperty(Rect position, SerializedProperty property)
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
        }

        private void InitializeStyle()
        {
            _isInitialized = true;
            _boldLabelStyle = new GUIStyle(GUI.skin.label)
                              {
                                  fontStyle = FontStyle.Bold
                              };
            _boldLabelStyleMiddle = new GUIStyle(GUI.skin.label)
                                    {
                                        fontStyle = FontStyle.Bold,
                                        fontSize = 15,
                                        normal =
                                        {
                                            textColor = MiddleColor
                                        }
                                    };
            _boldLabelStyle = new GUIStyle(GUI.skin.label)
                              {
                                  fontStyle = FontStyle.Bold,
                                  fontSize = 15,
                                  normal =
                                  {
                                      textColor = MiddleColor
                                  }
                              };
            _boldLabelStyleYellow = new GUIStyle(GUI.skin.label)
                                    {
                                        fontSize = 15,
                                        fontStyle = FontStyle.Bold,
                                        normal =
                                        {
                                            textColor = Color.yellow
                                        }
                                    };
            _boldLabelStyleRed = new GUIStyle(GUI.skin.label)
                                 {
                                     fontSize = 15,
                                     fontStyle = FontStyle.Bold,
                                     normal =
                                     {
                                         textColor = Color.red
                                     }
                                 };

            _buttonStyle = new GUIStyle(GUI.skin.label)
                           {
                               fontSize = 20,
                               alignment = TextAnchor.MiddleCenter
                           };
            _buttonStyle10 = new GUIStyle(GUI.skin.button)
                             {
                                 fontSize = 10
                             };
            _buttonStyle12 = new GUIStyle(GUI.skin.button)
                             {
                                 fontSize = 12
                             };

            _buttonStyleRed = new GUIStyle(GUI.skin.button)
                              {
                                  fontSize = 15,
                                  fontStyle = FontStyle.Bold,
                                  normal =
                                  {
                                      textColor = Color.red
                                  }
                              };
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            var height = LineHeight;
            if (!property.isExpanded) return height;

            if (property.type.Contains("SerializableDictionary"))
            {
                var keys = property.FindPropertyRelative("keys");
                var (from, to) = GetPageRange(_currentPageNumber, _itemsPerPage, keys.arraySize);
                var arraySize = to - from;
                height = (2 * arraySize + 2) * LineHeight + (arraySize + 1) * ITEM_SPACING + arraySize * KEY_VALUE_SPACING;

                if (keys is { hasVisibleChildren: true })
                {
                    for (var i = from; i < to; i++)
                    {
                        height += EditorGUI.GetPropertyHeight(keys.GetArrayElementAtIndex(i), GUIContent.none) - LineHeight;
                    }
                }

                var values = property.FindPropertyRelative("values");
                if (values is { hasVisibleChildren: true })
                {
                    for (var i = from; i < to; i++)
                    {
                        height += EditorGUI.GetPropertyHeight(values.GetArrayElementAtIndex(i), GUIContent.none) - LineHeight;
                    }
                }
            }
            else
            {
                height = EditorGUI.GetPropertyHeight(property, GUIContent.none, true);
            }

            return height;
        }

        #region Pagination

        private static (int from, int to) GetPageRange(int currentPage, int itemsPerPage, int totalItems)
        {
            var from = (currentPage - 1) * itemsPerPage;
            var to = Mathf.Min(currentPage * itemsPerPage, totalItems);
            return (from, to);
        }

        private static int CountItemsAtCurrentPage(int currentPage, int itemsPerPage, int totalItems)
        {
            var (from, to) = GetPageRange(currentPage, itemsPerPage, totalItems);
            return to - from;
        }

        #endregion

        #region Validation

        private void CheckForDuplicates(SerializedProperty keys)
        {
            if (keys == null) return;
            _keyDuplicates.Clear();
            for (var i = 0; i < keys.arraySize; i++)
            {
                var key = keys.GetArrayElementAtIndex(i);
                object keyValue = key.propertyType switch
                                  {
                                      SerializedPropertyType.Integer => key.intValue,
                                      SerializedPropertyType.String => key.stringValue,
                                      SerializedPropertyType.Enum => key.enumValueIndex,
                                      SerializedPropertyType.ObjectReference => key.objectReferenceValue,
                                      SerializedPropertyType.Boolean => key.boolValue,
                                      SerializedPropertyType.Float => key.floatValue,
                                      SerializedPropertyType.Vector2 => key.vector2Value,
                                      SerializedPropertyType.Vector3 => key.vector3Value,
                                      SerializedPropertyType.Vector4 => key.vector4Value,
                                      SerializedPropertyType.Character => key.intValue,
                                      SerializedPropertyType.Color => key.colorValue,
                                      SerializedPropertyType.Quaternion => key.quaternionValue,
                                      SerializedPropertyType.Rect => key.rectValue,
                                      SerializedPropertyType.Bounds => key.boundsValue,
                                      SerializedPropertyType.Vector2Int => key.vector2IntValue,
                                      SerializedPropertyType.Vector3Int => key.vector3IntValue,
                                      SerializedPropertyType.RectInt => key.rectIntValue,
                                      SerializedPropertyType.BoundsInt => key.boundsIntValue,
                                      _ => NullObjectValue
                                  };

                if (keyValue == null)
                    keyValue = NullObjectValue;

                if (!_keyDuplicates.TryGetValue(keyValue, out var duplicate))
                {
                    _keyDuplicates.Add(keyValue, new List<int> { i });
                }
                else
                {
                    duplicate.Add(i);
                }
            }
        }

        private bool IsValidKeyAt(int index)
        {
            var isDuplicate = IsDuplicateKeyAt(index);
            var isNull = IsNullKeyAt(index);
            return !isDuplicate && !isNull;
        }

        private bool IsDuplicateKeyAt(int index)
        {
            foreach (var kvp in _keyDuplicates)
            {
                if (kvp.Value.Contains(index))
                {
                    return kvp.Value.Count > 1;
                }
            }

            return false;
        }

        private bool IsNullKeyAt(int index)
        {
            foreach (var kvp in _keyDuplicates)
            {
                if (kvp.Value.Contains(index))
                {
                    return kvp.Key == NullObjectValue;
                }
            }

            return false;
        }

        private void RemoveDuplicates(SerializedProperty keys, SerializedProperty values)
        {
            List<int> invalidIndexes = new();
            foreach (var kvp in _keyDuplicates)
            {
                if (kvp.Value.Count > 1)
                {
                    for (var i = 1; i < kvp.Value.Count; i++)
                    {
                        if (invalidIndexes.Contains(kvp.Value[i])) continue;
                        invalidIndexes.Add(kvp.Value[i]);
                    }
                }

                if (kvp.Key == NullObjectValue)
                {
                    foreach (var index in kvp.Value)
                    {
                        if (invalidIndexes.Contains(index)) continue;
                        invalidIndexes.Add(index);
                    }
                }
            }

            invalidIndexes.Sort((a, b) => b.CompareTo(a));
            foreach (var duplicateIndex in invalidIndexes)
            {
                keys.DeleteArrayElementAtIndex(duplicateIndex);
                values.DeleteArrayElementAtIndex(duplicateIndex);
            }
        }

        #endregion

        #region CopyPaste

        private static readonly List<string> CopiedProperties = new(); // Store multiple copied properties

        private static void CopyProperty(SerializedProperty property)
        {
            var copiedProperty = string.Empty;
            switch (property.propertyType)
            {
                case SerializedPropertyType.Integer:
                    copiedProperty = property.intValue.ToString();
                    break;

                case SerializedPropertyType.Float:
                    copiedProperty = property.floatValue.ToString(CultureInfo.InvariantCulture);
                    break;

                case SerializedPropertyType.String:
                    copiedProperty = property.stringValue;
                    break;

                case SerializedPropertyType.Boolean:
                    copiedProperty = property.boolValue.ToString();
                    break;

                case SerializedPropertyType.Enum:
                    copiedProperty = property.enumValueIndex.ToString();
                    break;

                case SerializedPropertyType.Color:
                    var color = property.colorValue;
                    copiedProperty = $"#{ColorUtility.ToHtmlStringRGBA(color)}";
                    break;

                case SerializedPropertyType.ObjectReference:
                    if (property.objectReferenceValue != null)
                    {
                        copiedProperty = $"ObjectReference: {property.objectReferenceInstanceIDValue}";
                    }
                    else
                    {
                        Debug.LogWarning("No object reference to copy.");
                    }

                    break;

                case SerializedPropertyType.Vector2:
                    copiedProperty = $"{property.vector2Value.x},{property.vector2Value.y}";
                    break;

                case SerializedPropertyType.Vector3:
                    copiedProperty = $"{property.vector3Value.x},{property.vector3Value.y},{property.vector3Value.z}";
                    break;

                case SerializedPropertyType.Vector4:
                    copiedProperty = $"{property.vector4Value.x},{property.vector4Value.y},{property.vector4Value.z},{property.vector4Value.w}";
                    break;

                case SerializedPropertyType.Rect:
                    var rect = property.rectValue;
                    copiedProperty = $"{rect.x},{rect.y},{rect.width},{rect.height}";
                    break;

                case SerializedPropertyType.Bounds:
                    var bounds = property.boundsValue;
                    copiedProperty = $"{bounds.center.x},{bounds.center.y},{bounds.center.z},{bounds.size.x},{bounds.size.y},{bounds.size.z}";
                    break;

                case SerializedPropertyType.AnimationCurve:
                    var curve = property.animationCurveValue;
                    copiedProperty = JsonUtility.ToJson(curve);
                    break;

                case SerializedPropertyType.Character:
                    var character = (char)property.intValue;
                    copiedProperty = character.ToString();
                    break;

                case SerializedPropertyType.Generic:
                    var genericValue = property.boxedValue;
                    copiedProperty = JsonUtility.ToJson(genericValue);
                    break;
                case SerializedPropertyType.LayerMask:
                case SerializedPropertyType.ArraySize:
                case SerializedPropertyType.Gradient:
                case SerializedPropertyType.Quaternion:
                case SerializedPropertyType.ExposedReference:
                case SerializedPropertyType.FixedBufferSize:
                case SerializedPropertyType.Vector2Int:
                case SerializedPropertyType.Vector3Int:
                case SerializedPropertyType.RectInt:
                case SerializedPropertyType.BoundsInt:
                case SerializedPropertyType.ManagedReference:
                case SerializedPropertyType.Hash128:
                default:
                    Debug.LogWarning("Unsupported property type for copying.");
                    break;
            }

            CopiedProperties.Insert(0, copiedProperty);
        }

        private static void PasteProperty(SerializedProperty property, int index = 0)
        {
            if (CopiedProperties.Count == 0)
            {
                Debug.LogWarning("No copied properties to paste.");
                return;
            }

            if (index >= CopiedProperties.Count)
            {
                Debug.LogWarning("No copied properties to paste.");
                return;
            }

            var clipboardContent = CopiedProperties[index];

            switch (property.propertyType)
            {
                case SerializedPropertyType.Integer:
                    if (int.TryParse(clipboardContent, out var intValue))
                    {
                        property.intValue = intValue;
                    }

                    break;

                case SerializedPropertyType.Float:
                    if (float.TryParse(clipboardContent, out var floatValue))
                    {
                        property.floatValue = floatValue;
                    }

                    break;

                case SerializedPropertyType.String:
                    property.stringValue = clipboardContent;
                    break;

                case SerializedPropertyType.Boolean:
                    if (bool.TryParse(clipboardContent, out var boolValue))
                    {
                        property.boolValue = boolValue;
                    }

                    break;

                case SerializedPropertyType.Enum:
                    if (int.TryParse(clipboardContent, out var enumIndex))
                    {
                        property.enumValueIndex = enumIndex;
                    }

                    break;

                case SerializedPropertyType.Color:
                    if (ColorUtility.TryParseHtmlString(clipboardContent, out var color))
                    {
                        property.colorValue = color;
                    }

                    break;

                case SerializedPropertyType.ObjectReference:
                    if (clipboardContent.StartsWith("ObjectReference: ") &&
                        int.TryParse(clipboardContent.Substring("ObjectReference: ".Length), out var instanceID))
                    {
                        var obj = EditorUtility.InstanceIDToObject(instanceID);
                        if (obj != null)
                        {
                            property.objectReferenceValue = obj;
                        }
                    }

                    break;

                case SerializedPropertyType.Vector2:
                    var vector2Values = clipboardContent.Split(',');
                    if (vector2Values.Length == 2 &&
                        float.TryParse(vector2Values[0], out var vec2X) &&
                        float.TryParse(vector2Values[1], out var vec2Y))
                    {
                        property.vector2Value = new Vector2(vec2X, vec2Y);
                    }

                    break;

                case SerializedPropertyType.Vector3:
                    var vector3Values = clipboardContent.Split(',');
                    if (vector3Values.Length == 3 &&
                        float.TryParse(vector3Values[0], out var vec3X) &&
                        float.TryParse(vector3Values[1], out var vec3Y) &&
                        float.TryParse(vector3Values[2], out var vec3Z))
                    {
                        property.vector3Value = new Vector3(vec3X, vec3Y, vec3Z);
                    }

                    break;

                case SerializedPropertyType.Vector4:
                    var vector4Values = clipboardContent.Split(',');
                    if (vector4Values.Length == 4 &&
                        float.TryParse(vector4Values[0], out var vec4X) &&
                        float.TryParse(vector4Values[1], out var vec4Y) &&
                        float.TryParse(vector4Values[2], out var vec4Z) &&
                        float.TryParse(vector4Values[3], out var vec4W))
                    {
                        property.vector4Value = new Vector4(vec4X, vec4Y, vec4Z, vec4W);
                    }

                    break;

                case SerializedPropertyType.Rect:
                    var rectValues = clipboardContent.Split(',');
                    if (rectValues.Length == 4 &&
                        float.TryParse(rectValues[0], out var rectX) &&
                        float.TryParse(rectValues[1], out var rectY) &&
                        float.TryParse(rectValues[2], out var rectW) &&
                        float.TryParse(rectValues[3], out var rectH))
                    {
                        property.rectValue = new Rect(rectX, rectY, rectW, rectH);
                    }

                    break;

                case SerializedPropertyType.Bounds:
                    var boundsValues = clipboardContent.Split(',');
                    if (boundsValues.Length == 6 &&
                        float.TryParse(boundsValues[0], out var centerX) &&
                        float.TryParse(boundsValues[1], out var centerY) &&
                        float.TryParse(boundsValues[2], out var centerZ) &&
                        float.TryParse(boundsValues[3], out var sizeX) &&
                        float.TryParse(boundsValues[4], out var sizeY) &&
                        float.TryParse(boundsValues[5], out var sizeZ))
                    {
                        property.boundsValue = new Bounds(new Vector3(centerX, centerY, centerZ), new Vector3(sizeX, sizeY, sizeZ));
                    }

                    break;

                case SerializedPropertyType.AnimationCurve:
                    try
                    {
                        var curve = JsonUtility.FromJson<AnimationCurve>(clipboardContent);
                        property.animationCurveValue = curve;
                    }
                    catch
                    {
                        Debug.LogWarning("Failed to parse animation curve from clipboard.");
                    }

                    break;
                case SerializedPropertyType.Character:
                    if (char.TryParse(clipboardContent, out var character))
                    {
                        property.intValue = character;
                    }

                    break;

                case SerializedPropertyType.Generic:
                    property.boxedValue = JsonUtility.FromJson(clipboardContent, property.boxedValue.GetType());
                    break;

                case SerializedPropertyType.LayerMask:
                case SerializedPropertyType.ArraySize:
                case SerializedPropertyType.Gradient:
                case SerializedPropertyType.Quaternion:
                case SerializedPropertyType.ExposedReference:
                case SerializedPropertyType.FixedBufferSize:
                case SerializedPropertyType.Vector2Int:
                case SerializedPropertyType.Vector3Int:
                case SerializedPropertyType.RectInt:
                case SerializedPropertyType.BoundsInt:
                case SerializedPropertyType.ManagedReference:
                case SerializedPropertyType.Hash128:
                default:
                    Debug.LogWarning("Unsupported property type for pasting.");
                    break;
            }

            property.serializedObject.ApplyModifiedProperties();
        }

        #endregion
    }
}