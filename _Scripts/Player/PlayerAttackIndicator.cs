using UnityEngine;
using UnityEngine.InputSystem;

namespace com.game.player
{
    public class PlayerAttackIndicator : MonoBehaviour
    {
        [SerializeField] private LayerMask m_detectionMask;

        void Update()
        {
            if (Game.Paused)
                return;

            UpdatePosition();
        }

        void UpdatePosition()
        {
            var ray = Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue());

            if (!Physics.Raycast(ray, out var hitInfo, Mathf.Infinity, m_detectionMask))
                return;

            transform.position = hitInfo.point;
        }
    }
}
