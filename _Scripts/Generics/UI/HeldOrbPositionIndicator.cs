using com.absence.attributes;
using com.game.player;
using UnityEngine;
using UnityEngine.UI;

namespace com.game.generics
{
    public class HeldOrbPositionIndicator : PositionIndicatorBase
    {
        [SerializeField, Required] private Image m_orbIconImage;

        Transform m_playerTransform;
        PlayerOrbContainer m_orbContainer;
        PlayerOrbController m_orbController;
        Sprite m_initialIcon;

        protected override void Start()
        {
            m_initialIcon = m_orbIconImage.sprite;

            m_playerTransform = Player.Instance.transform;
            m_orbContainer = Player.Instance.Hub.OrbContainer;
            m_orbController = m_orbContainer.Controller;

            base.Start();
        }

        public override void Refresh()
        {
            base.Refresh();

            Sprite icon = m_orbContainer.OrbInventoryEntries[m_orbController.OrbHeld].GetIcon();

            if (icon == null)
                icon = m_initialIcon;

            m_orbIconImage.sprite = icon;
        }

        protected override Vector3 GetOrigin()
        {
            return m_playerTransform.position;
        }

        protected override Vector3 GetTarget()
        {
            return m_orbController.OrbHeld.transform.position;
        }
    }
}
