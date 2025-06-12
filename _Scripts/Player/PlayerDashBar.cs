using com.absence.attributes;
using UnityEngine;
using UnityEngine.UI;

namespace com.game.player
{
    public class PlayerDashBar : MonoBehaviour
    {
        [SerializeField, Required] private CanvasGroup m_panel;
        [SerializeField, Required] private Image m_fillImage;

        ThirdPersonController m_movementScript;

        private void Start()
        {
            m_movementScript = Player.Instance.Hub.ThirdPersonController;
        }

        private void Update()
        {
            m_panel.gameObject.SetActive(m_movementScript.DashCount <= 0 && m_movementScript.DashCooldownTimer > 0f);

            m_fillImage.fillAmount = m_movementScript.DashCooldownTimer / m_movementScript.DashCooldown;
        }
    }
}
