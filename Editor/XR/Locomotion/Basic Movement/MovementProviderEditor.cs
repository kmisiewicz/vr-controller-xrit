//using Chroma.UnityTools;
using UnityEditor;
using UnityEngine;

namespace Chroma.XR.Locomotion
{
    [CustomEditor(typeof(MovementProvider))]
    public class MovementProviderEditor : Editor
    {
        SerializedProperty speed, maxVelocityChange, strafe, useVignette, forwardSource;//, requireGrounded;
        SerializedProperty locomotionSystem, vignette, forwardSources, body;//, groundChecker;
        SerializedProperty leftHandInput, rightHandInput, inputLeft, inputRight;
        SerializedProperty startMoveEvent, stopMoveEvent;

        bool showInput = false;
        bool showEvents = false;
        bool showReferences = false;


        private void OnEnable()
        {
            speed = serializedObject.FindProperty("moveSpeed");
            maxVelocityChange = serializedObject.FindProperty("maxVelocityChange");
            strafe = serializedObject.FindProperty("enableStrafe");
            useVignette = serializedObject.FindProperty("useVignette");
            forwardSource = serializedObject.FindProperty("forwardSource");
            //requireGrounded = serializedObject.FindProperty("requireGrounded");

            locomotionSystem = serializedObject.FindProperty("m_System");
            vignette = serializedObject.FindProperty("vignette");
            forwardSources = serializedObject.FindProperty("forwardSources");
            body = serializedObject.FindProperty("body");
            //groundChecker = serializedObject.FindProperty("groundChecker");

            leftHandInput = serializedObject.FindProperty("leftHandInput");
            rightHandInput = serializedObject.FindProperty("rightHandInput");
            inputLeft = serializedObject.FindProperty("leftHandMoveAction");
            inputRight = serializedObject.FindProperty("rightHandMoveAction");

            startMoveEvent = serializedObject.FindProperty("startedMoving");
            stopMoveEvent = serializedObject.FindProperty("finishedMoving");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            //EditorExtensions.DrawChromaBar();

            EditorGUILayout.PropertyField(speed, new GUIContent("Move Speed"), true);
            EditorGUILayout.PropertyField(maxVelocityChange, new GUIContent("Max Velocity Change"), true);
            EditorGUILayout.PropertyField(forwardSource, new GUIContent("Forward Source"), true);
            EditorGUILayout.PropertyField(strafe, new GUIContent("Enable Strafe"), true);
            EditorGUILayout.PropertyField(useVignette, new GUIContent("Use Vignette"), true);
            //EditorGUILayout.PropertyField(requireGrounded, new GUIContent("Require Grounded"), true);

            GUIStyle foldoutStyle = EditorStyles.foldout;
            foldoutStyle.fontStyle = FontStyle.Bold;

            EditorGUILayout.Space();
            showReferences = EditorGUILayout.Foldout(showReferences, new GUIContent("References"), true, foldoutStyle);
            if (showReferences)
            {
                EditorGUI.indentLevel++;

                EditorGUILayout.PropertyField(locomotionSystem, new GUIContent("System"), true);
                EditorGUILayout.PropertyField(body, new GUIContent("Body"), true);
                //EditorGUILayout.PropertyField(groundChecker, new GUIContent("Ground Checker"), true);
                EditorGUILayout.PropertyField(vignette, new GUIContent("Vignette"), true);
                foldoutStyle.fontStyle = FontStyle.Normal;
                EditorGUILayout.PropertyField(forwardSources, new GUIContent("Forward Sources"), true);
                foldoutStyle.fontStyle = FontStyle.Bold;

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

                EditorGUILayout.PropertyField(inputLeft, new GUIContent("Left Hand Move Action"), true);
                EditorGUILayout.PropertyField(inputRight, new GUIContent("Right Hand Move Action"), true);

                EditorGUI.indentLevel--;
            }

            EditorGUILayout.Space();
            showEvents = EditorGUILayout.Foldout(showEvents, new GUIContent("Events"), true, foldoutStyle);
            if (showEvents)
            {
                EditorGUI.indentLevel++;

                EditorGUILayout.PropertyField(startMoveEvent, new GUIContent("Started Moving"), true);
                EditorGUILayout.PropertyField(stopMoveEvent, new GUIContent("Finished Moving"), true);

                EditorGUI.indentLevel--;
            }

            serializedObject.ApplyModifiedProperties();
        }
    }
}
