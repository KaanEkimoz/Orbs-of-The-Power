using UnityEditor;
using UnityEngine;

namespace com.game.utilities.editor
{
    [CustomPropertyDrawer(typeof(SkinnedFieldAttribute))]
    public class SkinnedFieldAttributePropertyDrawer : PropertyDrawer
    {
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            float height = EditorGUIUtility.singleLineHeight;
            float spacing = EditorGUIUtility.standardVerticalSpacing;

            return height + spacing;
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            SerializedProperty keyPropNameProp = property.FindPropertyRelative("m_keyPropName");
            SerializedProperty allowSceneObjectsProp = property.FindPropertyRelative("m_keyPropName");
            SerializedProperty skinValueProp = property.FindPropertyRelative("m_skinValue");
            SerializedProperty realValueProp = property.FindPropertyRelative("m_realValue");

            SkinnedFieldAttribute attr = attribute as SkinnedFieldAttribute;

            keyPropNameProp.stringValue = attr.keyPropertyName;
            allowSceneObjectsProp.boolValue = attr.allowSceneObjects;

            UnityEngine.Object skinValue = skinValueProp.objectReferenceValue;
            bool isNull = skinValue == null;

            SerializedProperty searchResultProp = null;

            if (!isNull) 
                searchResultProp = new SerializedObject(skinValue).FindProperty(keyPropNameProp.stringValue);

            // filled by GPT :^).
            switch (realValueProp.propertyType)
            {
                case SerializedPropertyType.Integer:
                    realValueProp.intValue = isNull ? 0 : searchResultProp?.intValue ?? 0;
                    break;
                case SerializedPropertyType.Boolean:
                    realValueProp.boolValue = isNull ? false : searchResultProp?.boolValue ?? false;
                    break;
                case SerializedPropertyType.Float:
                    realValueProp.floatValue = isNull ? 0f : searchResultProp?.floatValue ?? 0f;
                    break;
                case SerializedPropertyType.String:
                    realValueProp.stringValue = isNull ? string.Empty : searchResultProp?.stringValue ?? string.Empty;
                    break;
                case SerializedPropertyType.Color:
                    realValueProp.colorValue = isNull ? Color.black : searchResultProp?.colorValue ?? Color.black;
                    break;
                case SerializedPropertyType.ObjectReference:
                    realValueProp.objectReferenceValue = isNull ? null : searchResultProp?.objectReferenceValue;
                    break;
                case SerializedPropertyType.LayerMask:
                    realValueProp.intValue = isNull ? 0 : searchResultProp?.intValue ?? 0;
                    break;
                case SerializedPropertyType.Enum:
                    realValueProp.enumValueIndex = isNull ? 0 : searchResultProp?.enumValueIndex ?? 0;
                    break;
                case SerializedPropertyType.Vector2:
                    realValueProp.vector2Value = isNull ? Vector2.zero : searchResultProp?.vector2Value ?? Vector2.zero;
                    break;
                case SerializedPropertyType.Vector3:
                    realValueProp.vector3Value = isNull ? Vector3.zero : searchResultProp?.vector3Value ?? Vector3.zero;
                    break;
                case SerializedPropertyType.Vector4:
                    realValueProp.vector4Value = isNull ? Vector4.zero : searchResultProp?.vector4Value ?? Vector4.zero;
                    break;
                case SerializedPropertyType.Rect:
                    realValueProp.rectValue = isNull ? new Rect() : searchResultProp?.rectValue ?? new Rect();
                    break;
                case SerializedPropertyType.ArraySize:
                    realValueProp.intValue = isNull ? 0 : searchResultProp?.intValue ?? 0;
                    break;
                case SerializedPropertyType.Character:
                    realValueProp.intValue = isNull ? 0 : searchResultProp?.intValue ?? 0;
                    break;
                case SerializedPropertyType.AnimationCurve:
                    realValueProp.animationCurveValue = isNull ? new AnimationCurve() : searchResultProp?.animationCurveValue ?? new AnimationCurve();
                    break;
                case SerializedPropertyType.Bounds:
                    realValueProp.boundsValue = isNull ? new Bounds() : searchResultProp?.boundsValue ?? new Bounds();
                    break;
                case SerializedPropertyType.Quaternion:
                    realValueProp.quaternionValue = isNull ? Quaternion.identity : searchResultProp?.quaternionValue ?? Quaternion.identity;
                    break;
                case SerializedPropertyType.ExposedReference:
                    realValueProp.exposedReferenceValue = isNull ? null : searchResultProp?.exposedReferenceValue;
                    break;
                case SerializedPropertyType.FixedBufferSize:
                    realValueProp.intValue = isNull ? 0 : searchResultProp?.intValue ?? 0;
                    break;
                case SerializedPropertyType.Vector2Int:
                    realValueProp.vector2IntValue = isNull ? Vector2Int.zero : searchResultProp?.vector2IntValue ?? Vector2Int.zero;
                    break;
                case SerializedPropertyType.Vector3Int:
                    realValueProp.vector3IntValue = isNull ? Vector3Int.zero : searchResultProp?.vector3IntValue ?? Vector3Int.zero;
                    break;
                case SerializedPropertyType.RectInt:
                    realValueProp.rectIntValue = isNull ? new RectInt() : searchResultProp?.rectIntValue ?? new RectInt();
                    break;
                case SerializedPropertyType.BoundsInt:
                    realValueProp.boundsIntValue = isNull ? new BoundsInt() : searchResultProp?.boundsIntValue ?? new BoundsInt();
                    break;
                case SerializedPropertyType.ManagedReference:
                    realValueProp.managedReferenceValue = isNull ? null : searchResultProp?.managedReferenceValue;
                    break;
                case SerializedPropertyType.Hash128:
                    realValueProp.hash128Value = isNull ? new Hash128() : searchResultProp?.hash128Value ?? new Hash128();
                    break;
                case SerializedPropertyType.RenderingLayerMask:
                    realValueProp.uintValue = isNull ? 0u : searchResultProp?.uintValue ?? 0u;
                    break;
                case SerializedPropertyType.Gradient:
                case SerializedPropertyType.Generic:
                default:
                    Debug.LogError("Skinned field of non-supported type!!!");
                    break;
            }
        }
    }
}
