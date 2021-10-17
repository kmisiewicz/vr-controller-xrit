//using Chroma.UnityTools;
using UnityEditor;
using UnityEngine;

namespace Chroma.XR.Locomotion
{
    [CustomEditor(typeof(ContinuousTurnProvider))]
    public class ContinuousTurnProviderEditor : Editor
    {
        SerializedProperty turnSpeed, useVignette;
        SerializedProperty locomotionSystem, vignette;
        SerializedProperty leftHandInput, rightHandInput, inputLeft, inputRight;
        SerializedProperty startTurnEvent, stopTurnEvent;

        bool showReferences = false;
        bool showInput = false;
        bool showEvents = false;


        private void OnEnable()
        {
            turnSpeed = serializedObject.FindProperty("m_TurnSpeed");
            useVignette = serializedObject.FindProperty("useVignette");

            locomotionSystem = serializedObject.FindProperty("m_System");
            vignette = serializedObject.FindProperty("vignette");

            leftHandInput = serializedObject.FindProperty("leftHandInput");
            rightHandInput = serializedObject.FindProperty("rightHandInput");
            inputLeft = serializedObject.FindProperty("m_LeftHandTurnAction");
            inputRight = serializedObject.FindProperty("m_RightHandTurnAction");

            startTurnEvent = serializedObject.FindProperty("startedTurning");
            stopTurnEvent = serializedObject.FindProperty("finishedTurning");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            //EditorExtensions.DrawChromaBar();

            EditorGUILayout.PropertyField(turnSpeed, new GUIContent("Turn Speed"), true);
            EditorGUILayout.PropertyField(useVignette, new GUIContent("Use Vignette"), true);

            GUIStyle foldoutStyle = new GUIStyle(EditorStyles.foldout);
            foldoutStyle.fontStyle = FontStyle.Bold;

            EditorGUILayout.Space();
            showReferences = EditorGUILayout.Foldout(showReferences, new GUIContent("References"), true, foldoutStyle);
            if (showReferences)
            {
                EditorGUI.indentLevel++;

                EditorGUILayout.PropertyField(locomotionSystem, new GUIContent("System"), true);
                EditorGUILayout.PropertyField(vignette, new GUIContent("Vignette"), true);

                EditorGUI.indentLevel--;
            }

            EditorGUILayout.Space();
            showInput = EditorGUILayout.Foldout(showInput, new GUIContent("Input"), true, foldoutStyle);
            if (showInput)
            {
                EditorGUI.indentLevel++;

                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("Enable Input", GUILayout.Width(EditorGUIUtility.labelWidth));
                EditorGUILayout.LabelField("Left Hand", GUILayout.Width(80));
                EditorGUILayout.PropertyField(leftHandInput, GUIContent.none, GUILayout.Width(50));
                EditorGUILayout.LabelField("Right Hand", GUILayout.Width(80));
                EditorGUILayout.PropertyField(rightHandInput, GUIContent.none, GUILayout.Width(50));
                EditorGUILayout.EndHorizontal();

                EditorGUILayout.PropertyField(inputLeft, new GUIContent("Left Hand Turn Action"), true);
                EditorGUILayout.PropertyField(inputRight, new GUIContent("Right Hand Turn Action"), true);

                EditorGUI.indentLevel--;
            }

            EditorGUILayout.Space();
            showEvents = EditorGUILayout.Foldout(showEvents, new GUIContent("Events"), true, foldoutStyle);
            if (showEvents)
            {
                EditorGUI.indentLevel++;

                EditorGUILayout.PropertyField(startTurnEvent, new GUIContent("Started Turning"), true);
                EditorGUILayout.PropertyField(stopTurnEvent, new GUIContent("Finished Turning"), true);

                EditorGUI.indentLevel--;
            }

            serializedObject.ApplyModifiedProperties();
        }
    }
}
