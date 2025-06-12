using com.absence.utilities;
using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace com.game.scriptableeventsystem.editor
{
    public class ScriptableEventSelectionWindow : EditorWindow
    {
        static int s_selectedTypeIndex = 0;
        static GUIContent[] s_contents;

        UnityEngine.Object m_root;

        public event Action<ScriptableEventProfileBase> OnApplySelectionOneShot = null;

        public static void Initiate(Action<ScriptableEventProfileBase> onApplySelectionOneShot, UnityEngine.Object root = null)
        { 
            ScriptableEventTypeCache.Refresh(false);
            GenerateGridContent();
            ClampSelectionIndex();

            ScriptableEventSelectionWindow window = (ScriptableEventSelectionWindow)EditorWindow.GetWindow(typeof(ScriptableEventSelectionWindow));
            window.m_root = root;
            window.OnApplySelectionOneShot += onApplySelectionOneShot;
            window.titleContent = new GUIContent("Select Scriptable Event Type");
            window.Show();
        }

        private void OnGUI()
        {
            bool noTypes = ScriptableEventTypeCache.NoTypesFound;

            if (noTypes)
            {
                GUILayout.Label("There are no types derived from 'ScriptableEventProfileBase' in this project.");

                if (GUILayout.Button("Close"))
                {
                    OnApplySelectionOneShot = null;
                    Close();
                }

                return;
            }

            s_selectedTypeIndex = GUILayout.SelectionGrid(s_selectedTypeIndex, s_contents, 1);

            if (Event.current != null)
            {
                if (Event.current.isKey)
                {
                    switch (Event.current.keyCode)
                    {
                        case KeyCode.KeypadEnter:
                        case KeyCode.Return:
                            ApplySelection();
                            Close();
                            break;

                        case KeyCode.Escape:
                            OnApplySelectionOneShot = null;
                            Close();
                            break;

                        default:
                            break;
                    }
                }

                else if (Event.current.button == 0 && Event.current.clickCount == 2)
                {
                    ApplySelection();
                    Close();
                }
            }

            GUILayout.Space(20);

            if (GUILayout.Button("Create"))
            {
                ApplySelection();
                Close();
            }

            if (GUILayout.Button("Cancel"))
            {
                OnApplySelectionOneShot = null;
                Close();
            }
        }

        void ApplySelection()
        {
            List<Type> foundTypes = ScriptableEventTypeCache.FoundTypes;

            ScriptableEventProfileBase evt = ScriptableEventsEditorHelpers.CreateScriptableEvent(foundTypes[s_selectedTypeIndex], m_root);
            OnApplySelectionOneShot?.Invoke(evt);
            OnApplySelectionOneShot = null;
        }

        static void ClampSelectionIndex()
        {
            int typeCount = ScriptableEventTypeCache.FoundTypeCount;

            if (s_selectedTypeIndex < 0) s_selectedTypeIndex = 0;
            else if (s_selectedTypeIndex >= typeCount) s_selectedTypeIndex = 0;
        }

        static void GenerateGridContent()
        {
            List<Type> foundTypes = ScriptableEventTypeCache.FoundTypes;

            int typeCount = foundTypes.Count;

            s_contents = new GUIContent[typeCount];
            for (int i = 0; i < typeCount; i++)
            {
                Type type = foundTypes[i];

                string tooltip = "No tooltip set for this sub-type.";
                PropertyInfo property = type.GetProperty(nameof(ScriptableEventProfileBase.DesignerTooltip));

                if (property != null)
                {
                    object val = property.GetValue(null);

                    if (val != null)
                        tooltip = val.ToString();
                }

                s_contents[i] = new GUIContent()
                {
                    text = Helpers.SplitCamelCase(type.Name, " ").Replace("Scriptable Event Profile", "").Trim(),
                    tooltip = tooltip,
                    image = null,
                };
            }
        }
    }
}
