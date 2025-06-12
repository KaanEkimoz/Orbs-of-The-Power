using com.absence.editor.gui;
using com.game.itemsystem.scriptables;
using System;
using UnityEditor;
using UnityEngine;

namespace com.game.itemsystem.editor
{
    [CustomEditor(typeof(ItemRecipeProfile), true, isFallback = false)]
    public class ItemRecipeProfileEditor : Editor
    {
        private const int k_plusIconFontSize = 40;
        private const float k_horizontalMargins = 60f;
        private const float k_spriteIconWidthDiff = 18f;
        private const float k_spriteIconMargins = 1f;
        private const float k_itemIconHeight = 120f;

        private void OnEnable()
        {
            ItemDatabase.Refresh();
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            //SerializedProperty typeDependentProp = serializedObject.FindProperty("m_allowDifferentSubtypes");            
            SerializedProperty guid1Prop = serializedObject.FindProperty("m_guid1");            
            SerializedProperty guid2Prop = serializedObject.FindProperty("m_guid2");
            SerializedProperty resultGuidProp = serializedObject.FindProperty("m_resultGuid");

            //bool typeDependent = typeDependentProp.boolValue;
            string guid1 = guid1Prop.stringValue;
            string guid2 = guid2Prop.stringValue;
            string resultGuid = resultGuidProp.stringValue;

            Type baseType = typeof(ItemProfileBase);
            ItemProfileBase profile1 = null;
            ItemProfileBase profile2 = null;
            ItemProfileBase resultProfile = null;

            if (ItemDatabase.TryGetItem(guid1, out ItemProfileBase result1))
                profile1 = result1;

            if (ItemDatabase.TryGetItem(guid2, out ItemProfileBase result2))
                profile2 = result2;

            if (ItemDatabase.TryGetItem(resultGuid, out ItemProfileBase result))
                resultProfile = result;

            //Type type1 = typeDependent ? (profile2 != null ? (profile2.GetType()) : (baseType)) : (baseType);
            //Type type2 = typeDependent ? (profile1 != null ? (profile1.GetType()) : (baseType)) : (baseType);
            //Type resultType = typeDependent ? (type1.Equals(type2) ? (type1) : (baseType)) : (baseType);

            Type type1 = baseType;
            Type type2 = baseType;
            Type resultType = baseType;

            GUIStyle iconStyle = new GUIStyle(EditorStyles.label)
            {
                fontStyle = FontStyle.Bold,
                fontSize = k_plusIconFontSize,
                richText = true,
            };

            GUIStyle nameStyle = new GUIStyle(EditorStyles.label)
            {
                alignment = TextAnchor.MiddleCenter,
            };

            GUIStyle typeStyle = new GUIStyle(EditorStyles.centeredGreyMiniLabel)
            {
            };

            float height = EditorGUIUtility.singleLineHeight;
            float spacing = EditorGUIUtility.standardVerticalSpacing;

            absentGUILayout.Header1("Item Recipe Profile");

            EditorGUI.BeginChangeCheck();

            //typeDependent = EditorGUILayout.Toggle("Allow Different Item Sub-types", typeDependent);

            EditorGUILayout.BeginHorizontal(GUILayout.Height(k_itemIconHeight + (height * 2) + (spacing * 3)), GUILayout.ExpandWidth(true));

            EditorGUILayout.Space(k_horizontalMargins / 2);

            DrawProfileField(ref profile1, type1);
            DrawIcon("+");
            DrawProfileField(ref profile2, type2);
            DrawIcon("=");
            DrawProfileField(ref resultProfile, resultType);

            EditorGUILayout.Space(k_horizontalMargins / 2);

            EditorGUILayout.EndHorizontal();

            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObject(target, "Item Recipe Profile (Editor)");

                //typeDependentProp.boolValue = typeDependent;
                guid1Prop.stringValue = profile1 != null ? profile1.Guid : string.Empty;
                guid2Prop.stringValue = profile2 != null ? profile2.Guid : string.Empty;
                resultGuidProp.stringValue = resultProfile != null ? resultProfile.Guid : string.Empty;

                serializedObject.ApplyModifiedProperties();
                EditorUtility.SetDirty(target);
            }

            //if (!typeDependent)
            //    return;

            return;

            void DrawIcon(string icon)
            {
                GUIContent iconContent = new GUIContent(icon);
                float iconWidth = iconStyle.CalcSize(iconContent).x;

                GUI.enabled = false;

                GUILayout.FlexibleSpace();

                EditorGUILayout.BeginVertical();

                EditorGUILayout.LabelField(iconContent, iconStyle, GUILayout.Width(iconWidth), GUILayout.ExpandHeight(true));
                EditorGUILayout.Space(height);
                EditorGUILayout.Space(height);

                EditorGUILayout.EndVertical();

                GUILayout.FlexibleSpace();

                GUI.enabled = true;
            }

            void DrawProfileField(ref ItemProfileBase profile, Type type)
            {
                EditorGUILayout.BeginVertical(GUILayout.Width(k_itemIconHeight), GUILayout.ExpandHeight(true));

                GUILayoutOption[] iconOptions = new GUILayoutOption[]
                {
                    GUILayout.Width(k_itemIconHeight),
                    GUILayout.Height(k_itemIconHeight),
                };

                GUILayoutOption[] labelOptions = new GUILayoutOption[]
                {
                    GUILayout.Width(k_itemIconHeight),
                    GUILayout.Height(height),
                };

                profile = (ItemProfileBase)EditorGUILayout.ObjectField(new GUIContent(""), profile, type, false, iconOptions);
                DrawPreviewSprite(ref profile);

                string profileName = profile != null ? profile.DisplayName : "No items selected.";
                string profileType = profile != null ? profile.GetType().Name : string.Empty;

                EditorGUILayout.LabelField(profileName, nameStyle, labelOptions);
                EditorGUILayout.LabelField(profileType, typeStyle, labelOptions);

                EditorGUILayout.EndVertical();
            }

            void DrawPreviewSprite(ref ItemProfileBase profile)
            {
                if (profile == null)
                    return;

                if (profile.Icon == null)
                    return;

                Rect rect = GUILayoutUtility.GetLastRect();
                rect.width -= k_spriteIconWidthDiff;
                rect.x += k_spriteIconMargins;
                rect.y += k_spriteIconMargins;
                rect.width -= k_spriteIconMargins *  2;
                rect.height -= k_spriteIconMargins * 2;
                EditorGUI.DrawTextureTransparent(rect, profile.Icon.texture, ScaleMode.ScaleAndCrop);
            }
        }
    }
}
