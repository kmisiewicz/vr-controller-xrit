using System;
using UnityEngine;

namespace Chroma.Utility.Attributes
{
    [AttributeUsage(AttributeTargets.Field)]
    public class OnValueChangedAttribute : PropertyAttribute
    {
        public string FunctionName { get; private set; }

        public OnValueChangedAttribute(string functionName) => this.FunctionName = functionName;
    }
}
