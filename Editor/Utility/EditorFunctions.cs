using UnityEditor;
using UnityEngine;

namespace Chroma.Utility.Editor
{
    public static class EditorFunctions
    {
        const float SUB_LABEL_SPACING = 5;

        /// <summary>
        /// Draws multiple property fields in one line (row) in a given <paramref name="position"/>.
        /// </summary>
        public static void DrawMultiplePropertyFieldsInLine(Rect position, GUIContent[] subLabels, SerializedProperty[] properties)
        {
            // backup gui settings
            var indent = EditorGUI.indentLevel;
            var labelWidth = EditorGUIUtility.labelWidth;

            // draw properties
            var propsCount = properties.Length;
            var width = (position.width - (propsCount - 1) * SUB_LABEL_SPACING) / propsCount;
            var contentPos = new Rect(position.x, position.y, width, position.height);
            EditorGUI.indentLevel = 0;
            for (var i = 0; i < propsCount; i++)
            {
                EditorGUIUtility.labelWidth = EditorStyles.label.CalcSize(subLabels[i]).x + 2;
                EditorGUI.PropertyField(contentPos, properties[i], subLabels[i]);
                contentPos.x += width + SUB_LABEL_SPACING;
            }

            // restore gui settings
            EditorGUIUtility.labelWidth = labelWidth;
            EditorGUI.indentLevel = indent;
        }

        /// <summary>
        /// Draws multiple property fields from <paramref name="properties"/>.
        /// </summary>
        public static void DrawMultiplePropertyFields(SerializedProperty[] properties)
        {
            foreach (var property in properties)
                EditorGUILayout.PropertyField(property);
        }

        public static void DrawMultiplePropertyFields(SerializedProperty[] properties, GUIContent[] labels)
        {
            if (properties.Length != labels.Length)
                Debug.LogError($"There are {properties.Length} and {labels.Length} to draw.");
            for (int i = 0; i < properties.Length; i++)
                EditorGUILayout.PropertyField(properties[i], labels[i], true);
        }

        public static void DrawUILine(Color color, int thickness = 2, int padding = 10)
        {
            Rect r = EditorGUILayout.GetControlRect(GUILayout.Height(padding + thickness));
            r.height = thickness;
            r.y += padding / 2;
            r.x -= 2;
            r.width += 6;
            EditorGUI.DrawRect(r, color);
        }

        public static void DrawChromaBar()
        {
            Rect r = EditorGUILayout.GetControlRect(GUILayout.Height(5));
            r.height = 2;
            r.x -= 2;
            float d = r.width = (r.width + 6) / 3;
            EditorGUI.DrawRect(r, new Color(0.8f, 0f, 0f));
            r.x += d;
            EditorGUI.DrawRect(r, new Color(0.8f, 0.8f, 0f));
            r.x += d;
            EditorGUI.DrawRect(r, new Color(0f, 0f, 0.8f));
        }

        public static void DrawScriptField(SerializedObject so, bool active = false)
        {
            bool guiState = GUI.enabled;
            GUI.enabled = active;
            var script = so.FindProperty("m_Script");
            EditorGUILayout.PropertyField(script, true, new GUILayoutOption[0]);
            GUI.enabled = guiState;
        }
    }
}