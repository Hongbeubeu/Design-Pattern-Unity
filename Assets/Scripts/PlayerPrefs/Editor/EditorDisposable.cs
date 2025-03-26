#if UNITY_EDITOR
using System;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace hcore.PlayerPrefsManager.Editor
{
    public class UndoAndDirty : IDisposable
    {
        private readonly Object _target;

        public UndoAndDirty(Object target, string undoText = null)
        {
            _target = target;
            Undo.RegisterCompleteObjectUndo(_target, undoText ?? _target.name);
        }

        public void Dispose()
        {
            EditorUtility.SetDirty(_target);
        }
    }

    public class DisabledGUI : IDisposable
    {
        private readonly bool _enabled;

        public DisabledGUI(bool disable)
        {
            _enabled = GUI.enabled;
            GUI.enabled = !disable;
        }

        public void Dispose()
        {
            GUI.enabled = _enabled;
        }
    }

    public class VerticalHelpBox : IDisposable
    {
        private readonly bool _centered;

        public VerticalHelpBox(bool centered = false, params GUILayoutOption[] options)
        {
            _centered = centered;
            EditorGUILayout.BeginVertical(EditorStyles.helpBox, options);
            if (centered) GUILayout.FlexibleSpace();
        }

        public void Dispose()
        {
            if (_centered) GUILayout.FlexibleSpace();
            EditorGUILayout.EndVertical();
        }
    }

    public class HorizontalHelpBox : IDisposable
    {
        private readonly bool _centered;

        public HorizontalHelpBox(bool centered = false, params GUILayoutOption[] options)
        {
            _centered = centered;
            EditorGUILayout.BeginHorizontal(EditorStyles.helpBox, options);
            if (centered) GUILayout.FlexibleSpace();
        }

        public void Dispose()
        {
            if (_centered) GUILayout.FlexibleSpace();
            EditorGUILayout.EndHorizontal();
        }
    }

    public class HorizontalLayout : IDisposable
    {
        private readonly bool _centered;

        public HorizontalLayout(bool centered = false, params GUILayoutOption[] options)
        {
            _centered = centered;
            EditorGUILayout.BeginHorizontal(options);
            if (centered) GUILayout.FlexibleSpace();
        }

        public void Dispose()
        {
            if (_centered) GUILayout.FlexibleSpace();
            EditorGUILayout.EndHorizontal();
        }
    }

    public class VerticalLayout : IDisposable
    {
        private readonly bool _centered;

        public VerticalLayout(bool centered = false, params GUILayoutOption[] options)
        {
            _centered = centered;
            EditorGUILayout.BeginVertical(options);
            if (centered) GUILayout.FlexibleSpace();
        }

        public void Dispose()
        {
            if (_centered) GUILayout.FlexibleSpace();
            EditorGUILayout.EndVertical();
        }
    }

    public class BeginScrollView : IDisposable
    {
        public BeginScrollView(ref Vector2 pos, params GUILayoutOption[] options)
        {
            pos = EditorGUILayout.BeginScrollView(pos, options);
        }

        public void Dispose()
        {
            EditorGUILayout.EndScrollView();
        }
    }

    public class Indent : IDisposable
    {
        private readonly int _indent;

        public Indent(int indent = 1)
        {
            _indent = indent;
            var currentLevel = EditorGUI.indentLevel;
            EditorGUI.indentLevel = currentLevel + _indent;
        }

        public void Dispose()
        {
            var currentLevel = EditorGUI.indentLevel;
            EditorGUI.indentLevel = currentLevel - _indent;
        }
    }

    public class BackgroundColorScope : IDisposable
    {
        private readonly Color _color;

        public BackgroundColorScope(Color color)
        {
            _color = GUI.backgroundColor;
            GUI.backgroundColor = color;
        }

        public void Dispose()
        {
            GUI.backgroundColor = _color;
        }
    }
}
#endif