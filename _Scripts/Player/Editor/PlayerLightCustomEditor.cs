using com.absence.attributes.editor;
using com.absence.editor.gui;
using UnityEditor;
using UnityEngine;

namespace com.game.player.editor
{
    [CustomEditor(typeof(PlayerLight))]
    public class PlayerLightCustomEditor : Editor
    {
        Editor m_defaultEditor;
        int m_simulationLightStat;
        int m_simulationOrbCount;
        bool m_isSimulating;

        private void OnEnable()
        {
            Editor.CreateCachedEditor(target, typeof(absentEditorExtension), ref m_defaultEditor);
        }

        private void OnDisable()
        {
            Editor.DestroyImmediate(m_defaultEditor);
            m_defaultEditor = null;
        }

        public override void OnInspectorGUI()
        {
            m_defaultEditor.OnInspectorGUI();

            //PlayerLight playerLight = target as PlayerLight;

            //EditorGUILayout.BeginVertical("box");

            //EditorGUILayout.LabelField("Simulation", absentEditorStyles.Header3Style);

            //m_simulationLightStat = EditorGUILayout.IntField("Simulated Light Stat", m_simulationLightStat);
            //m_simulationOrbCount = EditorGUILayout.IntField("Simulated Orb Count", m_simulationOrbCount);

            //Color defaultColor = GUI.color;

            //if (m_isSimulating) GUI.color = Color.green;
            //if (GUILayout.Button("Simulate"))
            //{
            //    m_isSimulating = !m_isSimulating;
            //}

            //GUI.color = defaultColor;

            //EditorGUILayout.EndVertical();
        }

        private void OnSceneGUI()
        {
            if (!Application.isPlaying)
                return;

            PlayerLight playerLight = target as PlayerLight;

            bool hit = playerLight.HasGround;
            RaycastHit groundData = playerLight.GroundData;

            const float kAlpha = 1f;

            Color fullSeenColor = Color.red;
            fullSeenColor.a = kAlpha;

            Color halfSeenColor = Color.yellow;
            halfSeenColor.a = kAlpha;

            Handles.color = fullSeenColor;
            if (hit) Handles.DrawWireDisc(groundData.point, Vector2.up, playerLight.FullVisionRadius);
            Handles.color = halfSeenColor;
            if (hit) Handles.DrawWireDisc(groundData.point, Vector2.up, playerLight.HalfVisionRadius);
        }
    }
}
