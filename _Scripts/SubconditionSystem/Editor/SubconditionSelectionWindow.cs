using System.Collections.Generic;
using System;
using UnityEditor;
using UnityEngine;
using System.Reflection;
using com.absence.utilities;

namespace com.game.subconditionsystem.editor
{
    public class SubconditionSelectionWindow : EditorWindow
    {
        static int s_selectedTypeIndex = 0;
        static GUIContent[] s_contents;

        UnityEngine.Object m_root;

        public event Action<SubconditionProfileBase> OnApplySelectionOneShot = null;

        public static void Initiate(Action<SubconditionProfileBase> onApplySelectionOneShot, UnityEngine.Object root = null)
        {
            SubconditionTypeCache.Refresh(false);
            GenerateGridContent();
            ClampSelectionIndex();

            SubconditionSelectionWindow window = (SubconditionSelectionWindow)EditorWindow.GetWindow(typeof(SubconditionSelectionWindow));
            window.m_root = root;
            window.OnApplySelectionOneShot += onApplySelectionOneShot;
            window.titleContent = new GUIContent("Select Subcondition Event Type");
            window.Show();
        }

        private void OnGUI()
        {
            bool noTypes = SubconditionTypeCache.NoTypesFound;

            if (noTypes)
            {
                GUILayout.Label("There are no types derived from 'SubconditionProfileBase' in this project.");

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
            List<Type> foundTypes = SubconditionTypeCache.FoundTypes;

            SubconditionProfileBase cnd = SubconditionEditorHelpers.CreateSubconditionProfile(foundTypes[s_selectedTypeIndex], m_root);
            OnApplySelectionOneShot?.Invoke(cnd);
            OnApplySelectionOneShot = null;
        }

        static void ClampSelectionIndex()
        {
            int typeCount = SubconditionTypeCache.FoundTypeCount;

            if (s_selectedTypeIndex < 0) s_selectedTypeIndex = 0;
            else if (s_selectedTypeIndex >= typeCount) s_selectedTypeIndex = 0;
        }

        static void GenerateGridContent()
        {
            List<Type> foundTypes = SubconditionTypeCache.FoundTypes;

            int typeCount = foundTypes.Count;

            s_contents = new GUIContent[typeCount];
            for (int i = 0; i < typeCount; i++)
            {
                Type type = foundTypes[i];

                string tooltip = "No tooltip set for this sub-type.";
                PropertyInfo property = type.GetProperty(nameof(SubconditionProfileBase.DesignerTooltip));

                if (property != null)
                {
                    object val = property.GetValue(null);

                    if (val != null)
                        tooltip = val.ToString();
                }

                s_contents[i] = new GUIContent()
                {
                    text = Helpers.SplitCamelCase(type.Name, " ").Replace("Subcondition Profile", "").Trim(),
                    tooltip = tooltip,
                    image = null,
                };
            }
        }
    }
}
