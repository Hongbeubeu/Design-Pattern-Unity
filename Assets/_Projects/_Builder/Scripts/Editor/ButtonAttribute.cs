using System;
using UnityEngine;

[AttributeUsage(AttributeTargets.Method, Inherited = true, AllowMultiple = false)]
public class ButtonAttribute : PropertyAttribute
{
    public string ButtonName { get; }

    public ButtonAttribute(string buttonName = null)
    {
        ButtonName = buttonName;
    }
}