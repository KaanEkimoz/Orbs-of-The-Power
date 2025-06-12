using com.game.statsystem.presetobjects;
using UnityEditor;
using UnityEngine;

namespace com.game.statsystem.editor
{
    [CustomPropertyDrawer(typeof(StatOverride), true)]
    public class StatOverridePropertyDrawer : PropertyDrawer
    {
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            bool isArrayElement = property.propertyPath.Contains("Array");
            int height = isArrayElement ? 2 : 3;

            return StatManipulatorEditorHelpers.CalculateHeight(isArrayElement, height);
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            SerializedProperty valueProp = property.FindPropertyRelative("NewValue");

            StatOverride ovr = property.boxedValue as StatOverride;

            float value = valueProp.floatValue;

            GUIContent actualLabel = EditorGUI.BeginProperty(position, label, property);

            Rect actualPosition = StatManipulatorEditorHelpers.BeginManipulator(position, property, $"Player Stat Override ({actualLabel})"
                , ovr.GetEnumType(), out int statTypeIndex);

            actualPosition.height = EditorGUIUtility.singleLineHeight;

            GUIContent valueFieldLabel = new GUIContent()
            {
                text = "New Value",
                tooltip = valueProp.tooltip,
            };

            value = EditorGUI.FloatField(actualPosition, valueFieldLabel, value);
            actualPosition.y += EditorGUIUtility.singleLineHeight;

            if (StatManipulatorEditorHelpers.EndManipulator(property))
            {
                UnityEngine.Object target = property.serializedObject.targetObject;

                Undo.RecordObject(target, "Player Stat Override Object (Editor)");

                StatManipulatorEditorHelpers.ApplyManipulatorChanges(property, statTypeIndex);
                valueProp.floatValue = value;

                property.serializedObject.ApplyModifiedProperties();
                EditorUtility.SetDirty(target);
            }

            EditorGUI.EndProperty();
        }
    }
}
