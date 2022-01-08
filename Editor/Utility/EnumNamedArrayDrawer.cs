using UnityEditor;
using UnityEngine;

namespace Chroma.Utility
{
    // https://stackoverflow.com/questions/55583071/see-enumerated-indices-of-array-in-unity-inspector
    // https://stackoverflow.com/questions/24892935/custom-property-drawers-for-generic-classes-c-sharp-unity

    [CustomPropertyDrawer(typeof(EnumNamedArray), true)]
    public class EnumNamedArrayDrawer : PropertyDrawer
    {
        bool showElements = false;

        const int INDENT_WIDTH = 15;

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);

            var values = property.FindPropertyRelative("Values");
            var names = property.FindPropertyRelative("Names");

            showElements = EditorGUI.Foldout(new Rect(position.x, position.y, position.width, EditorGUIUtility.singleLineHeight),
                showElements, new GUIContent(label.text + $" ({values.arraySize})", property.tooltip), true);
            if (showElements)
            {
                EditorGUI.indentLevel++;

                for (var i = 0; i < values.arraySize; i++)
                {
                    var name = names.GetArrayElementAtIndex(i);
                    var value = values.GetArrayElementAtIndex(i);

                    position.y += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
                    var indentedRect = EditorGUI.IndentedRect(position);
                    indentedRect.x -= INDENT_WIDTH * EditorGUI.indentLevel;

                    indentedRect = EditorGUI.PrefixLabel(indentedRect, new GUIContent(SplitCamelCase(name.stringValue)));
                    indentedRect.x -= INDENT_WIDTH * EditorGUI.indentLevel;
                    indentedRect.width += INDENT_WIDTH * (EditorGUI.indentLevel * 2);
                    EditorGUI.PropertyField(indentedRect, value, GUIContent.none, true);
                }

                EditorGUI.indentLevel--;
            }

            EditorGUI.EndProperty();
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            var values = property.FindPropertyRelative("Values");

            return showElements ? (values.arraySize + 1) * (EditorGUIUtility.singleLineHeight 
                + EditorGUIUtility.standardVerticalSpacing) : EditorGUIUtility.singleLineHeight;
        }

        private string SplitCamelCase(string input)
        {
            return System.Text.RegularExpressions.Regex.Replace(input, "([A-Z])", " $1", System.Text.RegularExpressions.RegexOptions.Compiled).Trim();
        }
    }
}
