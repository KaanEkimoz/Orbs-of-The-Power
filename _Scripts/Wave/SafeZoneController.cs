using com.absence.attributes;
using com.absence.attributes.experimental;
using com.game.camera;
using com.game.generics.entities;
using com.game.player;
using System;
using UnityEngine;

namespace com.game.altarsystem
{
    public class SafeZoneController : MonoBehaviour
    {
        [SerializeField, Required] private Door m_walls;
        [SerializeField, InlineEditor] private CameraShakeProfile m_cameraShakeProfile;
        [SerializeField, Readonly] bool m_playerIsInSafeZone;

        public bool PlayerIsInside => m_playerIsInSafeZone;

        public event Action<bool> OnContactWithPlayer;

        public void ActivateWalls()
        {
            if (!m_walls.Close())
                return;

            if (m_cameraShakeProfile != null)
                Player.Instance.Hub.Camera.Shake(m_walls.AnimationDuration, m_cameraShakeProfile.Amplitude, m_cameraShakeProfile.Frequency, true);
        }

        public void DeactivateWalls()
        {
            if (!m_walls.Open())
                return;

            if (m_cameraShakeProfile != null)
                Player.Instance.Hub.Camera.Shake(m_walls.AnimationDuration, m_cameraShakeProfile.Amplitude, m_cameraShakeProfile.Frequency, true);
        }

        private void OnTriggerEnter(Collider other)
        {
            m_playerIsInSafeZone = true;
            OnContactWithPlayer?.Invoke(true);
        }

        private void OnTriggerExit(Collider other)
        {
            m_playerIsInSafeZone = false;
            OnContactWithPlayer?.Invoke(false);
        }
    }
}
