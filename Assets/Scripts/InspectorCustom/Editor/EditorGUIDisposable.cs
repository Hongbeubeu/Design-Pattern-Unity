using System;
using UnityEditor;
using UnityEngine;

namespace hcore.InspectorCustom
{
    public class DisposableAction : IDisposable
    {
        private readonly Action _restoreAction;

        public DisposableAction(Action modifyAction, Action restoreAction)
        {
            _restoreAction = restoreAction;
            modifyAction?.Invoke();
        }

        public void Dispose()
        {
            _restoreAction?.Invoke();
        }
    }

    public class EditorGUIProperty : IDisposable
    {
        public EditorGUIProperty(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);
        }

        public void Dispose()
        {
            EditorGUI.EndProperty();
        }
    }

    public class DisposableDisabledGUI : IDisposable
    {
        public DisposableDisabledGUI(bool disable)
        {
            EditorGUI.BeginDisabledGroup(disable);
        }

        public void Dispose()
        {
            EditorGUI.EndDisabledGroup();
        }
    }
}