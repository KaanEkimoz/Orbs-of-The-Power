using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

namespace com.game.player
{
    public class PlayerAttackCursor : MonoBehaviour
    {
        [SerializeField] private LayerMask m_mask;
        [SerializeField] private Image m_image;
        [SerializeField] private RectTransform m_canvas;
        [SerializeField] private PlayerAttackIndicator m_indicator;
        [SerializeField] private Transform m_playerFirepoint;

        Camera m_camera;

        private void Start()
        {
            m_camera = Camera.main;
            UpdateAlignment();
        }

        private void Update()
        {
            if (Game.Paused)
            {
                m_image.color = Color.white;
                return;
            }

            UpdatePosition();
            UpdateColor();
        }

        void UpdatePosition()
        {
            Vector3 worldPosition = m_indicator.transform.position;
            worldPosition.y += m_playerFirepoint.position.y;

            Vector3 viewportPosition = m_camera.WorldToViewportPoint(worldPosition);
            Vector2 canvasPosition = new Vector2(
            ((viewportPosition.x * m_canvas.sizeDelta.x) - (m_canvas.sizeDelta.x * 0.5f)),
            ((viewportPosition.y * m_canvas.sizeDelta.y) - (m_canvas.sizeDelta.y * 0.5f)));

            m_image.rectTransform.anchoredPosition = canvasPosition;
        }

        void UpdateColor()
        {
            if (Game.Paused)
            {
                m_image.color = Color.white;
                return;
            }

            var ray = m_camera.ScreenPointToRay(Mouse.current.position.ReadValue());

            if (Physics.Raycast(ray, out var hitInfo, Mathf.Infinity, m_mask))
            {
                m_image.color = Color.red;
            }

            else
            {
                m_image.color = Color.white;
            }
        }

        void UpdateAlignment()
        {
            transform.forward = Camera.main.transform.forward;
        }
    }
}
