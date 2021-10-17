using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace Chroma.Utility.Attributes
{
    [CustomPropertyDrawer(typeof(OnValueChangedAttribute))]
    public class OnValueChangedDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginChangeCheck();

            EditorGUI.PropertyField(position, property, label, true);

            if (EditorGUI.EndChangeCheck())
            {
                Object obj = property.serializedObject.targetObject;
                OnValueChangedAttribute a = (OnValueChangedAttribute)attribute;
                MethodInfo method = obj.GetType().GetMethod(a.FunctionName, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
                property.serializedObject.ApplyModifiedProperties();

                if (method != null)
                    method.Invoke(obj, null);
                else
                    Debug.LogWarning($"Tried to call method '{a.FunctionName}' that doesn't exist.");
            }
        }
    }
}
