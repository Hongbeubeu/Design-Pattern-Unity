using System;
using UnityEngine;

[AttributeUsage(AttributeTargets.Method, Inherited = true, AllowMultiple = false)]
public class ButtonAttribute : PropertyAttribute
{
    public string ButtonName { get; }
    public ButtonHeight ButtonHeight { get; }

    public ButtonAttribute(string buttonName = null, ButtonHeight height = ButtonHeight.Small)
    {
        ButtonName = buttonName;
        ButtonHeight = height;
    }
}

public enum ButtonHeight
{
    Small,
    Medium,
    Tall,
    Huge
}