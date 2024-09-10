using System;
using UnityEngine;

[AttributeUsage(AttributeTargets.All, AllowMultiple = false, Inherited = true)]
public class RequireTypeAttribute : PropertyAttribute
{
    public Type RequiredType { get; private set; }

    public RequireTypeAttribute(Type requiredType)
    {
        RequiredType = requiredType;
    }
}