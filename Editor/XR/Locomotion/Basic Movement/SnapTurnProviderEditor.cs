//using Chroma.UnityTools;
using Chroma.Utility.Editor;
using UnityEditor;
using UnityEngine;

namespace Chroma.XR.Locomotion
{
    [CustomEditor(typeof(SnapTurnProvider))]
    public class SnapTurnProviderEditor : Editor
    {
        SerializedProperty turnAmount, debounceTime, turnLR, turnAround;
        SerializedProperty blinkLR, blink180, fadeInTime, fadeOutTime;
        SerializedProperty blinkLRInBlock, blinkLROutBlock, blink180InBlock, blink180OutBlock;
        SerializedProperty locomotionSystem, fade;
        SerializedProperty leftHandInput, rightHandInput, inputLeft, inputRight;

        bool showBlink = false;
        bool showInput = false;
        bool showReferences = false;

        const int INDENT_WIDTH = 15;
        const int MIN_FIELD_WIDTH = 155;


        private void OnEnable()
        {
            turnAmount = serializedObject.FindProperty("m_TurnAmount");
            debounceTime = serializedObject.FindProperty("m_DebounceTime");
            turnLR = serializedObject.FindProperty("m_EnableTurnLeftRight");
            turnAround = serializedObject.FindProperty("m_EnableTurnAround");
            blinkLR = serializedObject.FindProperty("_UseBlinkLeftRight");
            blink180 = serializedObject.FindProperty("_UseBlinkTurnAround");
            blinkLRInBlock = serializedObject.FindProperty("_LeftRightFadeInBlockMovement");
            blinkLROutBlock = serializedObject.FindProperty("_LeftRightFadeOutBlockMovement");
            blink180InBlock = serializedObject.FindProperty("_TurnAroundFadeInBlockMovement");
            blink180OutBlock = serializedObject.FindProperty("_TurnAroundFadeOutBlockMovement");
            fadeInTime = serializedObject.FindProperty("_FadeInTime");
            fadeOutTime = serializedObject.FindProperty("_FadeOutTime");

            locomotionSystem = serializedObject.FindProperty("m_System");
            fade = serializedObject.FindProperty("_ScreenFade");

            leftHandInput = serializedObject.FindProperty("_LeftHandInput");
            rightHandInput = serializedObject.FindProperty("_RightHandInput");
            inputLeft = serializedObject.FindProperty("m_LeftHandSnapTurnAction");
            inputRight = serializedObject.FindProperty("m_RightHandSnapTurnAction");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            EditorFunctions.DrawScriptField(serializedObject);

            EditorFunctions.DrawMultiplePropertyFields(new[] { turnLR, turnAround, turnAmount, debounceTime });

            GUIStyle foldoutStyle = new GUIStyle(EditorStyles.foldout);
            foldoutStyle.fontStyle = FontStyle.Bold;

            GUIStyle rightLabelStyle = new GUIStyle(EditorStyles.label);
            rightLabelStyle.alignment = TextAnchor.MiddleRight;

            EditorGUILayout.Space();
            showBlink = EditorGUILayout.Foldout(showBlink, new GUIContent("Blink Settings"), true, foldoutStyle);
            if (showBlink)
            {
                EditorGUI.indentLevel++;

                EditorFunctions.DrawMultiplePropertyFields(new[] { blinkLR, blink180, fadeInTime, fadeOutTime },
                    new[] { new GUIContent("Blink On Left & Right Turn", blinkLR.tooltip), new GUIContent("Blink On Turn Around", blink180.tooltip),
                    new GUIContent("Fade In Time", fadeInTime.tooltip), new GUIContent("Fade Out Time", fadeOutTime.tooltip) });

                EditorGUILayout.Space();
                EditorGUILayout.LabelField("Block Movement On Fade", EditorStyles.boldLabel);

                Rect inputRect = EditorGUI.IndentedRect(EditorGUILayout.GetControlRect());
                inputRect.x -= INDENT_WIDTH;
                inputRect = EditorGUI.PrefixLabel(inputRect, new GUIContent("Left & Right Turn"));
                var labels = new[] { new GUIContent("In"), new GUIContent("Out") };
                var properties = new[] { blinkLRInBlock, blinkLROutBlock };
                EditorFunctions.DrawMultiplePropertyFieldsInLine(inputRect, labels, properties);

                inputRect = EditorGUI.IndentedRect(EditorGUILayout.GetControlRect());
                inputRect.x -= INDENT_WIDTH;
                inputRect = EditorGUI.PrefixLabel(inputRect, new GUIContent("Turn Around"));
                properties = new[] { blink180InBlock, blink180OutBlock };
                EditorFunctions.DrawMultiplePropertyFieldsInLine(inputRect, labels, properties);

                EditorGUI.indentLevel--;
            }

            EditorGUILayout.Space();
            showReferences = EditorGUILayout.Foldout(showReferences, new GUIContent("References"), true, foldoutStyle);
            if (showReferences)
            {
                EditorGUI.indentLevel++;
                EditorFunctions.DrawMultiplePropertyFields(new[] { locomotionSystem, fade });
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

            serializedObject.ApplyModifiedProperties();
        }
    }
}
