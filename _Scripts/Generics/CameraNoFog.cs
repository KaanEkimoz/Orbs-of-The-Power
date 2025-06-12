using com.absence.attributes;
using System;
using UnityEngine;
using UnityEngine.Rendering;

namespace com.game.generics
{
    [RequireComponent(typeof(Camera))]
    public class CameraNoFog : MonoBehaviour
    {
        [SerializeField, Readonly] private Camera m_camera;

        bool m_acted = false;
        bool m_fog = false;

        private void OnEnable()
        {
            RenderPipelineManager.beginCameraRendering += OnBeginRendering;
            RenderPipelineManager.endCameraRendering += OnEndRendering;
        }

        private void OnDisable()
        {
            RenderPipelineManager.beginCameraRendering -= OnBeginRendering;
            RenderPipelineManager.endCameraRendering -= OnEndRendering;
        }

        private void OnEndRendering(ScriptableRenderContext context, Camera camera)
        {
            if (camera != m_camera)
                return;

            if (!m_acted)
                return;

            RenderSettings.fog = m_fog;
            m_acted = false;
        }

        private void OnBeginRendering(ScriptableRenderContext context, Camera camera)
        {
            if (camera != m_camera)
                return;

            m_fog = RenderSettings.fog;
            RenderSettings.fog = false;
            m_acted = true;
        }

        private void Reset()
        {
            m_camera = GetComponent<Camera>();
        }
    }
}
