using System;
using UnityEditor;
using UnityEngine;

namespace com.game.utilities.editor
{
    [CustomPropertyDrawer(typeof(Value))]
    public class ValuePropertyDrawer : PropertyDrawer
    {
        const float k_enumFieldCoefficient = 0.3f;
        const float k_realFieldCoefficient = 0.6f;
        const float k_randomToggleCoefficient = 0.1f;

        const float k_hardcodedFieldErrorLeftShift = 0f;
        const float k_hardcodedEnumErrorLeftExpand = 0f;

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            float height = EditorGUIUtility.singleLineHeight;
            float spacing = EditorGUIUtility.standardVerticalSpacing;

            return height + spacing;
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            SerializedProperty enumProp = property.FindPropertyRelative("m_type");
            SerializedProperty rangeProp = property.FindPropertyRelative("m_range");
            SerializedProperty valueProp = property.FindPropertyRelative("m_value");
            SerializedProperty randomProp = property.FindPropertyRelative("m_random");

            int memberCount = 3;

            float height = EditorGUIUtility.singleLineHeight;
            float spacing = EditorGUIUtility.standardVerticalSpacing;

            bool isRandom = randomProp.boolValue;

            GUIContent actualLabel = EditorGUI.BeginProperty(position, label, property);

            Rect realPosition = EditorGUI.PrefixLabel(position, actualLabel);

            realPosition.height = height;

            Rect[] rects = absence.utilities.Helpers.SliceRectHorizontally(realPosition, memberCount,
                spacing, 0f,
                k_enumFieldCoefficient, k_realFieldCoefficient, k_randomToggleCoefficient);

            GUIContent empty = new("");
            GUIContent toggleContent = new(empty)
            {
                tooltip = "Random"
            };

            Rect enumRect = rects[0];
            Rect fieldRect = rects[1];
            Rect toggleRect = rects[2];

            enumRect.x -= k_hardcodedEnumErrorLeftExpand;
            enumRect.width += k_hardcodedEnumErrorLeftExpand;

            fieldRect.x -= k_hardcodedFieldErrorLeftShift;
            fieldRect.width += k_hardcodedFieldErrorLeftShift;

            enumProp.enumValueIndex = 
                (int)(Value.ValueType)EditorGUI.EnumPopup(enumRect, (Enum)(Value.ValueType)enumProp.enumValueIndex);

            if (isRandom) EditorGUI.PropertyField(fieldRect, rangeProp, empty, true);
            else EditorGUI.PropertyField(fieldRect, valueProp, empty, true);

            randomProp.boolValue = EditorGUI.ToggleLeft(toggleRect, toggleContent, randomProp.boolValue);

            EditorGUI.EndProperty();
        }
    }
}

