using UnityEditor;
using UnityEngine;

namespace com.game.utilities
{
    [CustomPropertyDrawer(typeof(SkinnedField), true)]
    public class SkinnedFieldPropertyDrawer : PropertyDrawer
    {
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            SerializedProperty skinValueProp = property.FindPropertyRelative("SkinValue");

            return EditorGUI.GetPropertyHeight(skinValueProp, label, true);
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            SerializedProperty allowSceneObjectsProp = property.FindPropertyRelative("m_allowSceneObjects");
            SerializedProperty skinValueProp = property.FindPropertyRelative("m_skinValue");

            SkinnedField boxed = property.boxedValue as SkinnedField;

            EditorGUI.BeginProperty(position, label, property);

            skinValueProp.objectReferenceValue = 
                EditorGUI.ObjectField(position, label, skinValueProp.objectReferenceValue, 
                boxed.GetSkinType(), allowSceneObjectsProp.boolValue);

            EditorGUI.EndProperty();
        }
    }
}
