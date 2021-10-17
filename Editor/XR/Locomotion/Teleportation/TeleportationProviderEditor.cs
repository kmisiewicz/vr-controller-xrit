//using Chroma.UnityTools;
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


        private void OnEnable()
        {
            leftHandInput = serializedObject.FindProperty("leftHandInput");
            rightHandInput = serializedObject.FindProperty("rightHandInput");
            useBlink = serializedObject.FindProperty("useBlink");
            fadeInTime = serializedObject.FindProperty("fadeInTime");
            fadeOutTime = serializedObject.FindProperty("fadeOutTime");
            fadeInBlock = serializedObject.FindProperty("fadeInBlockMovement");
            fadeOutBlock = serializedObject.FindProperty("fadeOutBlockMovement");

            locomotionSystem = serializedObject.FindProperty("m_System");
            fade = serializedObject.FindProperty("screenFade");
            body = serializedObject.FindProperty("bodyRoot");
            leftRay = serializedObject.FindProperty("leftRayToggler");
            rightRay = serializedObject.FindProperty("rightRayToggler");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            //EditorExtensions.DrawChromaBar();

            GUIStyle foldoutStyle = new GUIStyle(EditorStyles.foldout);
            foldoutStyle.fontStyle = FontStyle.Bold;
                        
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Enable Input", GUILayout.Width(EditorGUIUtility.labelWidth));
            EditorGUILayout.LabelField("Left Hand", GUILayout.Width(80));
            EditorGUILayout.PropertyField(leftHandInput, GUIContent.none, GUILayout.Width(50));
            EditorGUILayout.LabelField("Right Hand", GUILayout.Width(80));
            EditorGUILayout.PropertyField(rightHandInput, GUIContent.none, GUILayout.Width(50));
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.PropertyField(useBlink, new GUIContent("Use Blink"));

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Fade Time", GUILayout.Width(EditorGUIUtility.labelWidth));
            EditorGUILayout.LabelField("In", GUILayout.Width(15));
            EditorGUILayout.PropertyField(fadeInTime, GUIContent.none, GUILayout.Width(70));
            GUILayout.Space(30);
            EditorGUILayout.LabelField("Out", GUILayout.Width(25));
            EditorGUILayout.PropertyField(fadeOutTime, GUIContent.none, GUILayout.Width(70));
            EditorGUILayout.EndHorizontal();
            
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Block Movement on Fade", GUILayout.Width(EditorGUIUtility.labelWidth));
            EditorGUILayout.LabelField("In", GUILayout.Width(15));
            EditorGUILayout.PropertyField(fadeInBlock, GUIContent.none, GUILayout.Width(70));
            GUILayout.Space(30);
            EditorGUILayout.LabelField("Out", GUILayout.Width(25));
            EditorGUILayout.PropertyField(fadeOutBlock, GUIContent.none, GUILayout.Width(70));
            EditorGUILayout.EndHorizontal();

            //EditorGUILayout.Space(-EditorGUIUtility.standardVerticalSpacing);
            EditorGUILayout.Space();
            showReferences = EditorGUILayout.Foldout(showReferences, new GUIContent("References"), true, foldoutStyle);
            if (showReferences)
            {
                EditorGUI.indentLevel++;

                EditorGUILayout.PropertyField(locomotionSystem, new GUIContent("System"), true);
                EditorGUILayout.PropertyField(body, new GUIContent("Body"), true);
                EditorGUILayout.PropertyField(fade, new GUIContent("Screen Fade"), true);
                EditorGUILayout.PropertyField(leftRay, new GUIContent("Left Ray Toggler"), true);
                EditorGUILayout.PropertyField(rightRay, new GUIContent("Right Ray Toggler"), true);

                EditorGUI.indentLevel--;
            }

            serializedObject.ApplyModifiedProperties();
        }
    }
}
