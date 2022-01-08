//using Chroma.UnityTools;
using Chroma.Utility.Editor;
using UnityEditor;
using UnityEngine;

namespace Chroma.XR.Locomotion
{
    [CustomEditor(typeof(TeleportationProvider))]
    public class TeleportationProviderEditor : Editor
    {
        SerializedProperty useBlink, fadeInTime, fadeOutTime, fadeInBlock, fadeOutBlock, leftHandInput, rightHandInput;
        SerializedProperty locomotionSystem, fade, body, leftRay, rightRay;

        bool showReferences = false;

        const int INDENT_WIDTH = 15;
        const int MIN_FIELD_WIDTH = 155;


        private void OnEnable()
        {
            leftHandInput = serializedObject.FindProperty("_LeftHandInput");
            rightHandInput = serializedObject.FindProperty("_RightHandInput");
            useBlink = serializedObject.FindProperty("_UseBlink");
            fadeInTime = serializedObject.FindProperty("_FadeInTime");
            fadeOutTime = serializedObject.FindProperty("_FadeOutTime");
            fadeInBlock = serializedObject.FindProperty("_FadeInBlockMovement");
            fadeOutBlock = serializedObject.FindProperty("_FadeOutBlockMovement");

            locomotionSystem = serializedObject.FindProperty("m_System");
            fade = serializedObject.FindProperty("_ScreenFade");
            body = serializedObject.FindProperty("_BodyRoot");
            leftRay = serializedObject.FindProperty("_LeftRayToggler");
            rightRay = serializedObject.FindProperty("_RightRayToggler");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            EditorFunctions.DrawScriptField(serializedObject);

            GUIStyle foldoutStyle = new GUIStyle(EditorStyles.foldout);
            foldoutStyle.fontStyle = FontStyle.Bold;

            Rect inputRect = EditorGUILayout.GetControlRect();
            inputRect = EditorGUI.PrefixLabel(inputRect, new GUIContent("Enable Input"));
            var labels = inputRect.width >= MIN_FIELD_WIDTH ? new[] { new GUIContent("Left Hand"), new GUIContent("Right Hand") } : 
                new[] { new GUIContent("L"), new GUIContent("R") };
            var properties = new[] { leftHandInput, rightHandInput };
            EditorFunctions.DrawMultiplePropertyFieldsInLine(inputRect, labels, properties);

            EditorGUILayout.PropertyField(useBlink, new GUIContent("Use Blink"));

            inputRect = EditorGUILayout.GetControlRect();
            inputRect = EditorGUI.PrefixLabel(inputRect, new GUIContent("Fade Time"));
            labels = new[] { new GUIContent("In"), new GUIContent("Out") };
            properties = new[] { fadeInTime, fadeOutTime };
            EditorFunctions.DrawMultiplePropertyFieldsInLine(inputRect, labels, properties);

            inputRect = EditorGUILayout.GetControlRect();
            inputRect = EditorGUI.PrefixLabel(inputRect, new GUIContent("Block Movement on Fade"));
            properties = new[] { fadeInBlock, fadeOutBlock };
            EditorFunctions.DrawMultiplePropertyFieldsInLine(inputRect, labels, properties);

            EditorGUILayout.Space();
            showReferences = EditorGUILayout.Foldout(showReferences, new GUIContent("References"), true, foldoutStyle);
            if (showReferences)
            {
                EditorGUI.indentLevel++;
                EditorFunctions.DrawMultiplePropertyFields(new[] { locomotionSystem, body, fade, leftRay, rightRay });
                EditorGUI.indentLevel--;
            }

            serializedObject.ApplyModifiedProperties();
        }
    }
}
