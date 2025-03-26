using System;
using UnityEngine;

namespace hcore.Attributes
{
    [AttributeUsage(AttributeTargets.All, AllowMultiple = false, Inherited = true)]
    public class RequireTypeAttribute : PropertyAttribute
    {
        public Type RequiredType { get; private set; }

        public RequireTypeAttribute(Type requiredType)
        {
            RequiredType = requiredType;
        }
    }
}