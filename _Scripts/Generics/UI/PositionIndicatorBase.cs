using com.absence.attributes;
using UnityEngine;

namespace com.game.generics
{
    public abstract class PositionIndicatorBase : MonoBehaviour
    {
        public static Matrix4x4 CameraMatrix =
        new(new Vector4(Mathf.Sqrt(2) / 2, Mathf.Sqrt(2) / 2),
        new Vector4(-Mathf.Sqrt(2) / 2, Mathf.Sqrt(2) / 2),
        Vector4.zero,
        Vector4.zero);

        public enum DisplayMode
        {
            [InspectorName("UI/Arrow")] UI_Arrow,
        }

        [SerializeField] private DisplayMode m_displayMode = DisplayMode.UI_Arrow;
        [SerializeField, Required] private Canvas m_canvas;
        [SerializeField] private GameObject m_genericDisplayerTarget;
        [SerializeField, Min(0)] private float m_minDeadZone;
        [SerializeField, Min(0)] private float m_maxDeadZone;

        [SerializeField, Required, ShowIf(nameof(m_displayMode), DisplayMode.UI_Arrow)]
        private RectTransform m_arrowTransform;

        [SerializeField, ShowIf(nameof(m_displayMode), DisplayMode.UI_Arrow)]
        private float m_arrowRange;

        [SerializeField, ShowIf(nameof(m_displayMode), DisplayMode.UI_Arrow)]
        private Vector2 m_horizontalBounds;

        [SerializeField, ShowIf(nameof(m_displayMode), DisplayMode.UI_Arrow)]
        private Vector2 m_verticalBounds;

        RectTransform m_canvasTransform;
        Transform m_playerTransform;
        Vector3 m_altarPosition;
        float m_canvasRatio;

        protected virtual void Start()
        {
            m_canvasTransform = m_canvas.GetComponent<RectTransform>();
            m_canvasRatio = GetRatioOfCanvas(m_canvasTransform);
        }

        private void Update()
        {
            Refresh();
        }

        public virtual void Refresh()
        {
            Vector3 directionXZ = GetTarget() - GetOrigin();
            directionXZ.y = 0f;

            float magnitude = directionXZ.magnitude;
            bool exceedsDeadzone = magnitude >= (m_minDeadZone * m_minDeadZone);
            m_genericDisplayerTarget.SetActive(exceedsDeadzone);

            directionXZ.Normalize();

            switch (m_displayMode)
            {
                case DisplayMode.UI_Arrow:
                    RefreshUIArrow(directionXZ, magnitude);
                    break;
                default:
                    break;
            }
        }

        void RefreshUIArrow(Vector3 directionXZ, float magnitude)
        {
            float multiplier = Mathf.Min(1f, (magnitude - (m_minDeadZone * m_minDeadZone)) / (m_maxDeadZone * m_maxDeadZone));

            Vector2 normalizedUIPosition = CameraMatrix.MultiplyVector(new Vector2(directionXZ.x, directionXZ.z));
            Vector2 uiPosition = m_canvasRatio * normalizedUIPosition;
            Vector2 scaledUIPosition = m_arrowRange * multiplier * uiPosition;

            scaledUIPosition.x = Mathf.Clamp(scaledUIPosition.x, m_horizontalBounds.x, m_horizontalBounds.y);
            scaledUIPosition.y = Mathf.Clamp(scaledUIPosition.y, m_verticalBounds.x, m_verticalBounds.y);

            m_arrowTransform.up = uiPosition;
            m_arrowTransform.anchoredPosition = scaledUIPosition;
        }

        float GetRatioOfCanvas(RectTransform canvasTransform)
        {
            float width = canvasTransform.rect.width;
            float height = canvasTransform.rect.height;

            float ratio = width / height;
            return ratio;
        }

        protected abstract Vector3 GetOrigin();
        protected abstract Vector3 GetTarget();
    }
}
