using com.game.statsystem.presetobjects;
using UnityEditor;
using UnityEngine;

namespace com.game.statsystem.editor
{
    [CustomPropertyDrawer(typeof(StatCap), true)]
    public class StatCapPropertyDrawer : PropertyDrawer
    {
        private const float TOGGLE_WIDTH = 20f;
        private const float HORIZONTAL_SPACING = 4f;

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            bool isArrayElement = property.propertyPath.Contains("Array");
            int height = isArrayElement ? 3 : 4;

            return StatManipulatorEditorHelpers.CalculateHeight(isArrayElement, height);
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            SerializedProperty capLowProp = property.FindPropertyRelative("CapLow");
            SerializedProperty capHighProp = property.FindPropertyRelative("CapHigh");
            SerializedProperty minValueProp = property.FindPropertyRelative("MinValue");
            SerializedProperty maxValueProp = property.FindPropertyRelative("MaxValue");

            StatCap cap = property.boxedValue as StatCap;

            float minValue = minValueProp.floatValue;
            float maxValue = maxValueProp.floatValue;
            bool capLow = capLowProp.boolValue;
            bool capHigh = capHighProp.boolValue;

            GUIContent actualLabel = EditorGUI.BeginProperty(position, label, property);

            GUIContent minFieldLabel = new GUIContent()
            {
                text = "Min",
                tooltip = minValueProp.tooltip,
            };

            GUIContent maxFieldLabel = new GUIContent()
            {
                text = "Max",
                tooltip = maxValueProp.tooltip,
            };

            GUIContent capLowLabel = new GUIContent()
            {
                text = "",
                tooltip = capLowProp.tooltip,
            };

            GUIContent capHighLabel = new GUIContent()
            {
                text = "",
                tooltip = capHighProp.tooltip,
            };

            Rect actualPosition = StatManipulatorEditorHelpers.BeginManipulator(position, property, $"Player Stat Cap ({actualLabel})"
                , cap.GetEnumType(), out int statTypeIndex);

            actualPosition.height = EditorGUIUtility.singleLineHeight;

            float defaultX = actualPosition.x;
            float width = actualPosition.width - HORIZONTAL_SPACING;

            float toggleWidth = TOGGLE_WIDTH;
            float fieldWidth = width - toggleWidth;

            actualPosition.width = toggleWidth;

            capLow = EditorGUI.ToggleLeft(actualPosition, capLowLabel, capLow);
            actualPosition.x += toggleWidth + HORIZONTAL_SPACING;

            actualPosition.width = fieldWidth;

            if (!capLow) GUI.enabled = false;
            minValue = EditorGUI.FloatField(actualPosition, minFieldLabel, minValue);
            if (!capLow) GUI.enabled = true;

            actualPosition.y += EditorGUIUtility.singleLineHeight;
            actualPosition.y += EditorGUIUtility.standardVerticalSpacing;
            actualPosition.x -= toggleWidth + HORIZONTAL_SPACING;

            actualPosition.width = toggleWidth;

            capHigh = EditorGUI.ToggleLeft(actualPosition, capHighLabel, capHigh);
            actualPosition.x += toggleWidth + HORIZONTAL_SPACING;

            actualPosition.width = fieldWidth;

            if (!capHigh) GUI.enabled = false;
            maxValue = EditorGUI.FloatField(actualPosition, maxFieldLabel, maxValue);
            if (!capHigh) GUI.enabled = true;

            if (capHigh && capLow && minValue > maxValue) minValue = maxValue;

            if (StatManipulatorEditorHelpers.EndManipulator(property))
            {
                UnityEngine.Object target = property.serializedObject.targetObject;

                Undo.RecordObject(target, "Player Stat Cap Object (Editor)");

                StatManipulatorEditorHelpers.ApplyManipulatorChanges(property, statTypeIndex);
                capLowProp.boolValue = capLow;
                capHighProp.boolValue = capHigh;
                minValueProp.floatValue = minValue;
                maxValueProp.floatValue = maxValue;

                property.serializedObject.ApplyModifiedProperties();
                EditorUtility.SetDirty(target);
            }

            EditorGUI.EndProperty();
        }
    }
}
