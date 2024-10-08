#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

public class FSMEditorWindow : EditorWindow
{
    [MenuItem("Window/Custom FSM Editor")]
    public static void ShowWindow()
    {
        var wnd = GetWindow<FSMEditorWindow>();
        wnd.titleContent = new GUIContent("FSM Editor");
    }

    public void CreateGUI()
    {
        var root = rootVisualElement;

        // Vùng làm việc chính (canvas)
        var canvas = new VisualElement
                     {
                         style =
                         {
                             flexGrow = 1,
                             backgroundColor = new Color(0.15f, 0.15f, 0.15f),
                             position = Position.Relative
                         }
                     };

        // Tạo state node mẫu
        var stateNode = CreateStateNode("State 1");
        canvas.Add(stateNode);

        root.Add(canvas);
    }

    private VisualElement CreateStateNode(string stateName)
    {
        // Node chính cho state
        var node = new VisualElement
                   {
                       style =
                       {
                           width = 100,
                           height = 50,
                           backgroundColor = Color.gray,
                           borderBottomWidth = 2,
                           borderLeftWidth = 2,
                           borderRightWidth = 2,
                           borderTopWidth = 2,
                           borderBottomColor = Color.black,
                           borderLeftColor = Color.black,
                           borderRightColor = Color.black,
                           borderTopColor = Color.black,
                           position = Position.Absolute,
                           left = 100,
                           top = 100
                       }
                   };

        Label stateLabel = new Label(stateName);
        node.Add(stateLabel);

        var offset = Vector2.zero;

        // Đăng ký sự kiện kéo thả cho state
        node.RegisterCallback<MouseDownEvent>(evt =>
        {
            var nodePosition = new Vector2(node.style.left.value.value, node.style.top.value.value);
            offset = evt.mousePosition - nodePosition;
            node.CaptureMouse();
        });

        node.RegisterCallback<MouseMoveEvent>(evt =>
        {
            if (node.HasMouseCapture())
            {
                var newPos = evt.mousePosition - offset;
                node.style.left = newPos.x;
                node.style.top = newPos.y;
            }
        });

        node.RegisterCallback<MouseUpEvent>(evt => { node.ReleaseMouse(); });

        return node;
    }
}

#endif