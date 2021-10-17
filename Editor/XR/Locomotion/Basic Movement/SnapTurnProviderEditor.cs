//using Chroma.UnityTools;
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


        private void OnEnable()
        {
            turnAmount = serializedObject.FindProperty("m_TurnAmount");
            debounceTime = serializedObject.FindProperty("m_DebounceTime");
            turnLR = serializedObject.FindProperty("m_EnableTurnLeftRight");
            turnAround = serializedObject.FindProperty("m_EnableTurnAround");
            blinkLR = serializedObject.FindProperty("useBlinkLeftRight");
            blink180 = serializedObject.FindProperty("useBlinkTurnAround");
            blinkLRInBlock = serializedObject.FindProperty("leftRightFadeInBlockMovement");
            blinkLROutBlock = serializedObject.FindProperty("leftRightFadeOutBlockMovement");
            blink180InBlock = serializedObject.FindProperty("turnAroundFadeInBlockMovement");
            blink180OutBlock = serializedObject.FindProperty("turnAroundFadeOutBlockMovement");
            fadeInTime = serializedObject.FindProperty("fadeInTime");
            fadeOutTime = serializedObject.FindProperty("fadeOutTime");

            locomotionSystem = serializedObject.FindProperty("m_System");
            fade = serializedObject.FindProperty("screenFade");

            leftHandInput = serializedObject.FindProperty("leftHandInput");
            rightHandInput = serializedObject.FindProperty("rightHandInput");
            inputLeft = serializedObject.FindProperty("m_LeftHandSnapTurnAction");
            inputRight = serializedObject.FindProperty("m_RightHandSnapTurnAction");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            //EditorExtensions.DrawChromaBar();

            EditorGUILayout.PropertyField(turnLR, new GUIContent("Enable Left & Right Turn"));
            EditorGUILayout.PropertyField(turnAround, new GUIContent("Enable Turn Around"));
            EditorGUILayout.PropertyField(turnAmount, new GUIContent("Turn Amount"));
            EditorGUILayout.PropertyField(debounceTime, new GUIContent("Debounce Time"));

            GUIStyle foldoutStyle = new GUIStyle(EditorStyles.foldout);
            foldoutStyle.fontStyle = FontStyle.Bold;

            GUIStyle rightLabelStyle = new GUIStyle(EditorStyles.label);
            rightLabelStyle.alignment = TextAnchor.MiddleRight;

            EditorGUILayout.Space();
            showBlink = EditorGUILayout.Foldout(showBlink, new GUIContent("Blink Settings"), true, foldoutStyle);
            if (showBlink)
            {
                EditorGUI.indentLevel++;

                EditorGUILayout.PropertyField(blinkLR, new GUIContent("Blink On Left & Right Turn"));
                EditorGUILayout.PropertyField(blink180, new GUIContent("Blink On Turn Around"));
                EditorGUILayout.PropertyField(fadeInTime, new GUIContent("Fade In Time"));
                EditorGUILayout.PropertyField(fadeOutTime, new GUIContent("Fade Out Time"));

                EditorGUILayout.Space();
                EditorGUILayout.LabelField("Block Movement On Fade", EditorStyles.boldLabel);

                Rect r = EditorGUILayout.GetControlRect(GUILayout.Height(EditorGUIUtility.singleLineHeight * 2));

                float col = r.width /= 3;
                r.width = col;
                r.height = EditorGUIUtility.singleLineHeight;
                EditorGUI.LabelField(r, "Left & Right Turn");
                r.y += EditorGUIUtility.singleLineHeight;
                EditorGUI.LabelField(r, "Turn Around");

                r.x += col;
                r.y -= EditorGUIUtility.singleLineHeight;
                r.width /= 2;
                EditorGUI.LabelField(r, "In", rightLabelStyle);
                r.x += r.width;
                EditorGUI.PropertyField(r, blinkLRInBlock, GUIContent.none);
                r.x -= r.width;
                r.y += EditorGUIUtility.singleLineHeight;
                EditorGUI.LabelField(r, "In", rightLabelStyle);
                r.x += r.width;
                EditorGUI.PropertyField(r, blink180InBlock, GUIContent.none);

                r.x += r.width;
                r.y -= EditorGUIUtility.singleLineHeight;
                EditorGUI.LabelField(r, "Out", rightLabelStyle);
                r.x += r.width;
                EditorGUI.PropertyField(r, blinkLROutBlock, GUIContent.none);
                r.x -= r.width;
                r.y += EditorGUIUtility.singleLineHeight;
                EditorGUI.LabelField(r, "Out", rightLabelStyle);
                r.x += r.width;
                EditorGUI.PropertyField(r, blink180OutBlock, GUIContent.none);

                EditorGUI.indentLevel--;
            }

            EditorGUILayout.Space();
            showReferences = EditorGUILayout.Foldout(showReferences, new GUIContent("References"), true, foldoutStyle);
            if (showReferences)
            {
                EditorGUI.indentLevel++;

                EditorGUILayout.PropertyField(locomotionSystem, new GUIContent("System"), true);
                EditorGUILayout.PropertyField(fade, new GUIContent("Screen Fade"), true);

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

                EditorGUILayout.PropertyField(inputLeft, new GUIContent("Left Hand Snap Turn Action"), true);
                EditorGUILayout.PropertyField(inputRight, new GUIContent("Right Hand Snap Turn Action"), true);

                EditorGUI.indentLevel--;
            }

            serializedObject.ApplyModifiedProperties();
        }
    }
}
