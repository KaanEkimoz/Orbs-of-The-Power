using com.absence.attributes;
using com.game.player;
using UnityEngine;

namespace com.game.generics
{
    public class AltarPositionIndicator : PositionIndicatorBase
    {
        Vector3 m_altarPosition;
        Transform m_playerTransform;

        protected override void Start()
        {
            m_altarPosition = GetAltarPosition();
            m_playerTransform = Player.Instance.transform;

            base.Start();
        }

        Vector3 GetAltarPosition()
        {
            return SceneManager.Instance.AltarTransform.position;
        }

        protected override Vector3 GetOrigin()
        {
            return m_playerTransform.position;
        }

        protected override Vector3 GetTarget()
        {
            return m_altarPosition;
        }
    }
}
