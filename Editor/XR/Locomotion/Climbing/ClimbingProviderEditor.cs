using Chroma.Utility.Editor;
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

        const int INDENT_WIDTH = 15;
        const int MIN_FIELD_WIDTH = 155;


        private void OnEnable()
        {
            pullForce = serializedObject.FindProperty("_PullForce");
            pullThreshold = serializedObject.FindProperty("_PullThreshold");
            gravityDampFactor = serializedObject.FindProperty("_GravityDampFactor");
            gravityDampTime = serializedObject.FindProperty("_GravityDampTime");
            ledgeClimbLayerMask = serializedObject.FindProperty("_LedgeLayerMask");
            ledgeClimbTime = serializedObject.FindProperty("_ClimbOnLedgeDuration");
            ledgeClimbMinHeightAbove = serializedObject.FindProperty("_MinHeightOverLedge");

            locomotionSystem = serializedObject.FindProperty("m_System");
            locomotionSystemExtender = serializedObject.FindProperty("_LocomotionSystemExtender");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            EditorFunctions.DrawScriptField(serializedObject);

            GUIStyle foldoutStyle = new GUIStyle(EditorStyles.foldout);
            foldoutStyle.fontStyle = FontStyle.Bold;

            Rect inputRect = EditorGUILayout.GetControlRect();
            inputRect = EditorGUI.PrefixLabel(inputRect, new GUIContent("Pull"));
            var labels = inputRect.width >= MIN_FIELD_WIDTH ? new[] { new GUIContent("Force", pullForce.tooltip), new GUIContent("Threshold", pullThreshold.tooltip) } :
                new[] { new GUIContent("Force", pullForce.tooltip), new GUIContent("Thr", pullThreshold.tooltip)};
            var properties = new[] { pullForce, pullThreshold };
            EditorFunctions.DrawMultiplePropertyFieldsInLine(inputRect, labels, properties);

            inputRect = EditorGUILayout.GetControlRect();
            inputRect = EditorGUI.PrefixLabel(inputRect, new GUIContent("Gravity Damp"));
            labels = inputRect.width >= MIN_FIELD_WIDTH ? new[] { new GUIContent("Factor", gravityDampFactor.tooltip), new GUIContent("Time", gravityDampTime.tooltip) } :
                new[] { new GUIContent("Factor", gravityDampFactor.tooltip), new GUIContent("T", gravityDampTime.tooltip) };
            properties = new[] { gravityDampFactor, gravityDampTime };
            EditorFunctions.DrawMultiplePropertyFieldsInLine(inputRect, labels, properties);

            inputRect = EditorGUILayout.GetControlRect();
            inputRect = EditorGUI.PrefixLabel(inputRect, new GUIContent("Ledge Climb"));
            labels = inputRect.width >= MIN_FIELD_WIDTH ? new[] { new GUIContent("Min Height", ledgeClimbMinHeightAbove.tooltip), new GUIContent("Time", ledgeClimbTime.tooltip) } :
                new[] { new GUIContent("MinH", ledgeClimbMinHeightAbove.tooltip), new GUIContent("T", ledgeClimbTime.tooltip) };
            properties = new[] { ledgeClimbMinHeightAbove, ledgeClimbTime };
            EditorFunctions.DrawMultiplePropertyFieldsInLine(inputRect, labels, properties);

            EditorGUILayout.PropertyField(ledgeClimbLayerMask);

            EditorGUILayout.Space();
            showReferences = EditorGUILayout.Foldout(showReferences, new GUIContent("References"), true, foldoutStyle);
            if (showReferences)
            {
                EditorGUI.indentLevel++;
                EditorFunctions.DrawMultiplePropertyFields(new[] { locomotionSystem, locomotionSystemExtender });
                EditorGUI.indentLevel--;
            }

            serializedObject.ApplyModifiedProperties();
        }
    }
}
