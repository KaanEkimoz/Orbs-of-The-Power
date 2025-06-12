using com.game.player;
using System;
using UnityEngine;
using UnityEngine.UI;

namespace com.game.ui
{
    public class OrbDisplayGP : MonoBehaviour
    {
        public enum DisplayState
        {
            Ready,
            Thrown,
            Recalling,
        }

        [SerializeField] private Image m_image;
        [SerializeField] private GameObject m_selectionIndicator;
        [SerializeField] private GameObject m_cooldownPanel;
        [SerializeField] private Image m_cooldownFillImage;
        [SerializeField] private GameObject m_throwingIndicator;
        [SerializeField] private GameObject m_returningIndicator;
        [SerializeField] private GameObject m_stickedIndicator;

        public Image Image => m_image;

        Sprite m_initialSprite;
        bool m_isRotating = false;
        SimpleOrb m_orb;
        PlayerOrbContainer m_container;

        private void Start()
        {
            SetDisplay(OrbState.OnEllipse);
        }

        public void Initialize(SimpleOrb orb, PlayerOrbContainer container)
        {
            if (orb == null)
                throw new System.Exception("You can't initialize a gameplay orb display with a null orb reference.");

            m_initialSprite = m_image.sprite;

            if (m_orb != null && m_orb != orb)
            {
                m_orb.OnStateChanged -= OnOrbStateChanged;
            }

            m_orb = orb;
            m_orb.OnStateChanged += OnOrbStateChanged;
            m_container = container;

            SetSelected(false);
            OnOrbStateChanged(m_orb.currentState);
        }

        public void Refresh()
        {
            if (m_orb == null)
                return;

            Sprite iconFound = m_container.OrbInventoryEntries[m_orb].GetIcon();
            m_image.sprite = iconFound != null ? iconFound : m_initialSprite;
        }

        private void OnOrbStateChanged(OrbState state)
        {
            SetDisplay(state);
        }

        private void Update()
        {
            if (m_isRotating) 
                transform.up = Vector2.up;

            if (m_orb == null)
                return;

            bool inCooldown = m_orb.InCooldown;

            m_cooldownPanel.SetActive(inCooldown);

            if (!inCooldown)
                return;

            m_cooldownFillImage.fillAmount = m_orb.CooldownTimer / m_orb.MaxCooldown;
        }

        public void SetRotating(bool rotating)
        {
            m_isRotating = rotating;
        }

        public void SetSelected(bool value)
        {
            if (m_selectionIndicator != null) 
                m_selectionIndicator.SetActive(value);
        }

        private void OnDestroy()
        {
            if (m_orb != null)
                m_orb.OnStateChanged -= OnOrbStateChanged;
        }

        void SetDisplay(OrbState state)
        {
            if (m_throwingIndicator != null) m_throwingIndicator.SetActive(false);
            if (m_returningIndicator != null) m_returningIndicator.SetActive(false);
            if (m_stickedIndicator != null) m_stickedIndicator.SetActive(false);

            switch (state)
            {
                case OrbState.Throwing:
                    if (m_throwingIndicator != null) m_throwingIndicator.SetActive(true);
                    break;
                case OrbState.Sticked:
                    if (m_stickedIndicator != null) m_stickedIndicator.SetActive(true);
                    break;
                case OrbState.Returning:
                    if (m_returningIndicator != null) m_returningIndicator.SetActive(true);
                    break;
                case OrbState.OnEllipse:
                default:
                    break;
            }
        }
    }
}
