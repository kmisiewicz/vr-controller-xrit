//using Chroma.UnityTools;
using Chroma.Utility.Editor;
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

        const int INDENT_WIDTH = 15;
        const int MIN_FIELD_WIDTH = 155;


        private void OnEnable()
        {
            turnSpeed = serializedObject.FindProperty("m_TurnSpeed");
            useVignette = serializedObject.FindProperty("_UseVignette");

            locomotionSystem = serializedObject.FindProperty("m_System");
            vignette = serializedObject.FindProperty("_Vignette");

            leftHandInput = serializedObject.FindProperty("_LeftHandInput");
            rightHandInput = serializedObject.FindProperty("_RightHandInput");
            inputLeft = serializedObject.FindProperty("m_LeftHandTurnAction");
            inputRight = serializedObject.FindProperty("m_RightHandTurnAction");

            startTurnEvent = serializedObject.FindProperty("StartedTurning");
            stopTurnEvent = serializedObject.FindProperty("FinishedTurning");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            EditorFunctions.DrawScriptField(serializedObject);

            EditorFunctions.DrawMultiplePropertyFields(new[] { turnSpeed, useVignette });

            GUIStyle foldoutStyle = new GUIStyle(EditorStyles.foldout);
            foldoutStyle.fontStyle = FontStyle.Bold;

            EditorGUILayout.Space();
            showReferences = EditorGUILayout.Foldout(showReferences, new GUIContent("References"), true, foldoutStyle);
            if (showReferences)
            {
                EditorGUI.indentLevel++;
                EditorFunctions.DrawMultiplePropertyFields(new[] { locomotionSystem, vignette });
                EditorGUI.indentLevel--;
            }

            EditorGUILayout.Space();
            showInput = EditorGUILayout.Foldout(showInput, new GUIContent("Input"), true, foldoutStyle);
            if (showInput)
            {
                EditorGUI.indentLevel++;

                Rect inputRect = EditorGUI.IndentedRect(EditorGUILayout.GetControlRect());
                inputRect.x -= INDENT_WIDTH;
                inputRect = EditorGUI.PrefixLabel(inputRect, new GUIContent("Enable Input"));
                var labels = inputRect.width >= MIN_FIELD_WIDTH ? new[] { new GUIContent("Left Hand"), new GUIContent("Right Hand") } : 
                    new[] { new GUIContent("L"), new GUIContent("R") };
                var properties = new[] { leftHandInput, rightHandInput };
                EditorFunctions.DrawMultiplePropertyFieldsInLine(inputRect, labels, properties);

                EditorFunctions.DrawMultiplePropertyFields(new[] { inputLeft, inputRight });

                EditorGUI.indentLevel--;
            }

            EditorGUILayout.Space();
            showEvents = EditorGUILayout.Foldout(showEvents, new GUIContent("Events"), true, foldoutStyle);
            if (showEvents)
            {
                EditorGUI.indentLevel++;
                EditorFunctions.DrawMultiplePropertyFields(new[] { startTurnEvent, stopTurnEvent });
                EditorGUI.indentLevel--;
            }

            serializedObject.ApplyModifiedProperties();
        }
    }
}
