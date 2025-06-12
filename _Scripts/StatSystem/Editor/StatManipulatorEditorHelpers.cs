using com.absence.editor.gui;
using System;
using UnityEditor;
using UnityEngine;

namespace com.game.statsystem.editor
{
    public static class StatManipulatorEditorHelpers
    {
        public const float HORIZONTAL_PADDING = 16f;
        public const float VERTICAL_PADDING = 12f;
        public const float VERTICAL_MARGIN_FOR_ARRAY_ELEMENT = 2f;
        public const float VERTICAL_SPACING = 8f;
        public const float VERTICAL_MARGIN = 2f;

        public static float CalculateHeight(bool isArrayElement, int lineCount)
        {
            float lines = (EditorGUIUtility.singleLineHeight * lineCount) +
                (EditorGUIUtility.standardVerticalSpacing * lineCount);

            if (!isArrayElement)
                return lines + VERTICAL_MARGIN + VERTICAL_SPACING + VERTICAL_PADDING + absence.editor.internals.Constants.LINE_HEIGHT +
                absence.editor.internals.Constants.LINE_OFFSET +
                absence.editor.internals.Constants.LINE_MARGINS;
            else
                return lines + VERTICAL_MARGIN_FOR_ARRAY_ELEMENT;
        }

        public static Rect BeginManipulator(Rect position, SerializedProperty property, string label, Type enumType, out int statTypeIndex)
        {
            bool isArrayElement = property.propertyPath.Contains("Array");
            float originalY = position.y;

            if (!isArrayElement)
            {
                position.y += VERTICAL_MARGIN / 2;
                position.height -= VERTICAL_MARGIN;

                GUIStyle boxStyle = new GUIStyle("window");
                boxStyle.richText = true;

                GUIContent labelContent = new GUIContent()
                {
                    text = label,
                    tooltip = property.tooltip,
                };

                GUI.Box(position, labelContent, boxStyle);
                position.y += EditorGUIUtility.singleLineHeight;

                position.width -= HORIZONTAL_PADDING;
                position.x += HORIZONTAL_PADDING / 2;
                position.y += VERTICAL_PADDING / 2;
            }

            else
            {
                position.y += VERTICAL_MARGIN_FOR_ARRAY_ELEMENT / 2;
            }

            float originalHeight = position.height;

            SerializedProperty statTypeProp = property.FindPropertyRelative("TargetStatType");

            GUIContent statTypeLabel = new GUIContent()
            {
                text = "Target Stat",
                tooltip = statTypeProp.tooltip,
            };

            EditorGUI.BeginChangeCheck();

            position.height = EditorGUIUtility.singleLineHeight;
            if (!isArrayElement) EditorGUI.PropertyField(position, statTypeProp, true);
            else EditorGUI.PropertyField(position, statTypeProp, statTypeLabel, true);

            statTypeIndex = statTypeProp.enumValueIndex;

            position.y += EditorGUIUtility.singleLineHeight;
            position.y += EditorGUIUtility.standardVerticalSpacing;

            if (!isArrayElement) 
            { 
                absentGUI.DrawLine(position); 
                position.y += VERTICAL_SPACING;
            }

            position.height = originalHeight - (position.y - originalY);
            return position;
        }

        public static void ApplyManipulatorChanges(SerializedProperty property, int targetStat)
        {
            SerializedProperty statTypeProp = property.FindPropertyRelative("TargetStatType");
            statTypeProp.enumValueIndex = targetStat;
        }

        public static bool EndManipulator(SerializedProperty property)
        {
            bool isArrayElement = property.propertyPath.Contains("Array");

            return EditorGUI.EndChangeCheck();
        }
    }
}
