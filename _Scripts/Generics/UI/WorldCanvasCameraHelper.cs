using com.absence.attributes;
using UnityEngine;

namespace com.game.generics
{
    public class WorldCanvasCameraHelper : MonoBehaviour
    {
        [SerializeField, Readonly]
        private Canvas m_canvas;

        [SerializeField] private bool m_rotateInUpdate;

        Camera m_camera;

        private void Start()
        {
            m_camera = Camera.main;
            m_canvas.worldCamera = m_camera;
            transform.forward = m_camera.transform.forward;
        }

        private void Update()
        {
            if (m_rotateInUpdate)
                transform.forward = m_camera.transform.forward;
        }

        private void Reset()
        {
            m_canvas = GetComponent<Canvas>();
        }
    }
}
