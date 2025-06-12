using com.absence.editor.gui;
using com.absence.utilities;
using com.game.statsystem.internals;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEditor;
using UnityEngine;

namespace com.game.statsystem.editor
{
    [CustomEditor(typeof(DefaultStats), true)]
    public class DefaultStatsCustomEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            bool error = false;
            IDefaultStats defaultStats = target as IDefaultStats;

            serializedObject.Update();
            SerializedProperty initProp = serializedObject.FindProperty("m_initialized");
            bool initalized = initProp.boolValue;

            if (!initalized)
            {
                EditorGUILayout.HelpBox("This DefaultStats object is not initialized yet. " +
                    "Hit Refresh to initialize it.", 
                    MessageType.Warning);

                DrawRefreshButton();
                return;
            }

            SerializedProperty statListProp = serializedObject.FindProperty("m_defaultValues");
            Type enumType = defaultStats.GetEnumType();
            int statCount = statListProp.arraySize;
            List<string> enumNames = Enum.GetNames(enumType).ToList();

            absentGUILayout.Header1($"Default Stats for {defaultStats.GetTitle()}");

            if (statCount <= 0f)
            {
                EditorGUILayout.HelpBox($"The enum type of this object ({enumType.Name}) is empty.", 
                    MessageType.Info);

                DrawRefreshButton();
                return;
            }

            if (statCount != enumNames.Count) error = true;

            for (int i = 0; i < statCount; i++)
            {
                SerializedProperty prop = statListProp.GetArrayElementAtIndex(i);
                SerializedProperty enumProp = prop.FindPropertyRelative("TargetStat");
                SerializedProperty valueProp = prop.FindPropertyRelative("Value");

                string rawLabel = Enum.GetName(enumType, enumProp.boxedValue);
                string label;

                if (rawLabel == null)
                {
                    error = true;
                    label = "#null#";
                } 

                else if (!rawLabel.ToLower().Equals(enumNames[i].ToLower()))
                {
                    error = true;

                    StringBuilder sb = new(Helpers.SplitCamelCase(rawLabel, " "));
                    sb.Append("<b> (?)</b>");

                    label = sb.ToString();
                }

                else
                {
                    label = Helpers.SplitCamelCase(rawLabel, " ");
                }

                float value = valueProp.floatValue;

                float newValue = EditorGUILayout.FloatField(label, value);

                if (newValue != value)
                {
                    Undo.RecordObject(target, "Default Stats Tweak (Editor)");

                    valueProp.floatValue = newValue;
                    serializedObject.ApplyModifiedProperties();

                    EditorUtility.SetDirty(target);
                }
            }

            if (!error) EditorGUILayout.Space(20);
            else EditorGUILayout.HelpBox("There are some mismatches. Please refresh.", MessageType.Error);

            DrawRefreshButton();

            return;

            void DrawRefreshButton()
            {
                if (GUILayout.Button("Refresh"))
                {
                    Undo.RecordObject(target, "Default Stats Reinitialize (Editor)");

                    defaultStats.Reinitialize();

                    EditorUtility.SetDirty(target);
                }
            }
        }
    }
}
