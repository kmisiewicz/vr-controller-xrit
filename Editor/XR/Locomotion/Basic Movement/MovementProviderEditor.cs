using Chroma.Utility.Editor;
using UnityEditor;
using UnityEngine;

namespace Chroma.XR.Locomotion
{
    [CustomEditor(typeof(MovementProvider))]
    public class MovementProviderEditor : Editor
    {
        SerializedProperty speed, maxVelocityChange, strafe, useVignette, forwardSource, inAirControl, slopeControl;
        SerializedProperty locomotionSystem, vignette, forwardSources, body, gravityProvider;
        SerializedProperty leftHandInput, rightHandInput, inputLeft, inputRight;
        SerializedProperty startMoveEvent, stopMoveEvent;

        bool showInput = false;
        bool showEvents = false;
        bool showReferences = false;

        const int INDENT_WIDTH = 15;
        const int MIN_FIELD_WIDTH = 155;
        

        private void OnEnable()
        {
            speed = serializedObject.FindProperty("_MoveSpeed");
            maxVelocityChange = serializedObject.FindProperty("_MaxVelocityChange");
            strafe = serializedObject.FindProperty("_EnableStrafe");
            useVignette = serializedObject.FindProperty("_UseVignette");
            forwardSource = serializedObject.FindProperty("_ForwardSource");
            inAirControl = serializedObject.FindProperty("_InAirControlMultiplier");

            locomotionSystem = serializedObject.FindProperty("m_System");
            vignette = serializedObject.FindProperty("_Vignette");
            forwardSources = serializedObject.FindProperty("_ForwardSources");
            body = serializedObject.FindProperty("_Body");
            gravityProvider = serializedObject.FindProperty("_GravityProvider");

            leftHandInput = serializedObject.FindProperty("_LeftHandInput");
            rightHandInput = serializedObject.FindProperty("_RightHandInput");
            inputLeft = serializedObject.FindProperty("_LeftHandMoveAction");
            inputRight = serializedObject.FindProperty("_RightHandMoveAction");

            startMoveEvent = serializedObject.FindProperty("StartedMoving");
            stopMoveEvent = serializedObject.FindProperty("FinishedMoving");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            EditorFunctions.DrawScriptField(serializedObject);

            GUIStyle foldoutStyle = new GUIStyle(EditorStyles.foldout);
            foldoutStyle.fontStyle = FontStyle.Bold;

            EditorFunctions.DrawMultiplePropertyFields(new[] { speed, maxVelocityChange, forwardSource, strafe, useVignette, inAirControl });

            EditorGUILayout.Space();
            showReferences = EditorGUILayout.Foldout(showReferences, new GUIContent("References"), true, foldoutStyle);
            if (showReferences)
            {
                EditorGUI.indentLevel++;
                EditorFunctions.DrawMultiplePropertyFields(new[] { locomotionSystem, body, vignette, forwardSources, gravityProvider });
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
                var labels = inputRect.width >= MIN_FIELD_WIDTH ? new[] { new GUIContent("Left Hand"), new GUIContent("Right Hand") } : new[] { new GUIContent("L"), new GUIContent("R") };
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
                EditorFunctions.DrawMultiplePropertyFields(new[] { startMoveEvent, stopMoveEvent });
                EditorGUI.indentLevel--;
            }

            serializedObject.ApplyModifiedProperties();
        }

        
    }
}
