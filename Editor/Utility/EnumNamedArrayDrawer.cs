using UnityEditor;
using UnityEngine;

namespace Chroma.Utility
{
    // https://stackoverflow.com/questions/55583071/see-enumerated-indices-of-array-in-unity-inspector
    // https://stackoverflow.com/questions/24892935/custom-property-drawers-for-generic-classes-c-sharp-unity

    [CustomPropertyDrawer(typeof(EnumNamedArray), true)]
    public class EnumNamedArrayDrawer : PropertyDrawer
    {
        bool showElements = true;

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);

            var values = property.FindPropertyRelative("Values");
            var names = property.FindPropertyRelative("Names");

            showElements = EditorGUI.Foldout(new Rect(position.x, position.y, position.width, EditorGUIUtility.singleLineHeight), showElements, label.text + $" ({values.arraySize})", true);
            if (showElements)
            {
                EditorGUI.indentLevel++;

                for (var i = 0; i < values.arraySize; i++)
                {
                    var name = names.GetArrayElementAtIndex(i);
                    var value = values.GetArrayElementAtIndex(i);

                    position.y += EditorGUIUtility.singleLineHeight;

                    var indentedRect = EditorGUI.IndentedRect(position);

                    EditorGUI.LabelField(new Rect(position.x, position.y, EditorGUIUtility.labelWidth, EditorGUIUtility.singleLineHeight), name.stringValue);
                    EditorGUI.PropertyField(new Rect(position.x + EditorGUIUtility.labelWidth - indentedRect.x / 2, position.y,
                        EditorGUIUtility.currentViewWidth - EditorGUIUtility.labelWidth - indentedRect.x / 4, EditorGUIUtility.singleLineHeight), value, GUIContent.none, true);
                }

                EditorGUI.indentLevel--;
            }

            EditorGUI.EndProperty();
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            var values = property.FindPropertyRelative("Values");

            return showElements ? (values.arraySize + 1) * EditorGUIUtility.singleLineHeight : EditorGUIUtility.singleLineHeight;
        }
    }
}
