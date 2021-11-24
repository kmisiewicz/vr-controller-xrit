using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Chroma.XR.Locomotion
{
    [CustomEditor(typeof(ClimbingProvider))]
    public class NewBehaviourScript : Editor
    {
        SerializedProperty pullForce, pullThreshold, gravityDampFactor, gravityDampTime;
        SerializedProperty ledgeClimbLayerMask, ledgeClimbTime, ledgeClimbMinHeightAbove;

        SerializedProperty locomotionSystem, locomotionSystemExtender;

        bool showReferences = false;


        private void OnEnable()
        {
            pullForce = serializedObject.FindProperty("pullForce");
            pullThreshold = serializedObject.FindProperty("pullThreshold");
            gravityDampFactor = serializedObject.FindProperty("gravityDampFactor");
            gravityDampTime = serializedObject.FindProperty("gravityDampTime");
            ledgeClimbLayerMask = serializedObject.FindProperty("ledgeLayerMask");
            ledgeClimbTime = serializedObject.FindProperty("climbOnLedgeDuration");
            ledgeClimbMinHeightAbove = serializedObject.FindProperty("minHeightOverLedge");

            locomotionSystem = serializedObject.FindProperty("m_System");
            locomotionSystemExtender = serializedObject.FindProperty("locomotionSystemExtender");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            GUIStyle foldoutStyle = new GUIStyle(EditorStyles.foldout);
            foldoutStyle.fontStyle = FontStyle.Bold;

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Pull", GUILayout.Width(EditorGUIUtility.labelWidth));
            EditorGUILayout.LabelField(new GUIContent("Force", pullForce.tooltip), GUILayout.Width(65));
            EditorGUILayout.PropertyField(pullForce, GUIContent.none, GUILayout.Width(65));
            GUILayout.Space(10);
            EditorGUILayout.LabelField(new GUIContent("Threshold", pullThreshold.tooltip), GUILayout.Width(65));
            EditorGUILayout.PropertyField(pullThreshold, GUIContent.none, GUILayout.Width(65));
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Gravity Damp", GUILayout.Width(EditorGUIUtility.labelWidth));
            EditorGUILayout.LabelField(new GUIContent("Factor", gravityDampFactor.tooltip), GUILayout.Width(65));
            EditorGUILayout.PropertyField(gravityDampFactor, GUIContent.none, GUILayout.Width(65));
            GUILayout.Space(10);
            EditorGUILayout.LabelField(new GUIContent("Time", gravityDampTime.tooltip), GUILayout.Width(65));
            EditorGUILayout.PropertyField(gravityDampTime, GUIContent.none, GUILayout.Width(65));
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Ledge Climb", GUILayout.Width(EditorGUIUtility.labelWidth));
            EditorGUILayout.LabelField(new GUIContent("Min Height", ledgeClimbMinHeightAbove.tooltip), GUILayout.Width(65));
            EditorGUILayout.PropertyField(ledgeClimbMinHeightAbove, GUIContent.none, GUILayout.Width(65));
            GUILayout.Space(10);
            EditorGUILayout.LabelField(new GUIContent("Time", ledgeClimbTime.tooltip), GUILayout.Width(65));
            EditorGUILayout.PropertyField(ledgeClimbTime, GUIContent.none, GUILayout.Width(65));
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.PropertyField(ledgeClimbLayerMask);

            EditorGUILayout.Space();
            showReferences = EditorGUILayout.Foldout(showReferences, new GUIContent("References"), true, foldoutStyle);
            if (showReferences)
            {
                EditorGUI.indentLevel++;

                EditorGUILayout.PropertyField(locomotionSystem, new GUIContent("System"), true);
                EditorGUILayout.PropertyField(locomotionSystemExtender);

                EditorGUI.indentLevel--;
            }

            serializedObject.ApplyModifiedProperties();
        }
    }
}
