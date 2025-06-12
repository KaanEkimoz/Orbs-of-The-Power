using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace com.game.abilitysystem.ui
{
    public class AbilityDisplayGP : MonoBehaviour
    {
        public const bool COOLDOWN_FILL = false;

        [SerializeField] private bool m_cooldownFill;
        [SerializeField] private CanvasGroup m_graphic;
        [SerializeField] private GameObject m_cooldownPanel;
        [SerializeField] private Image m_semiCooldownFillImage;
        [SerializeField] private Image m_cooldownFillImage;
        [SerializeField] private TMP_Text m_nameText;
        [SerializeField] private TMP_Text m_useCountText;
        [SerializeField] private TMP_Text m_cooldownText;

        IRuntimeAbility m_ability;
        bool m_readyToUse;
        bool m_stackable;
        float m_totalCooldown;
        float m_totalDuration;
        int m_totalUseCount;

        static string CreateCooldownString(float seconds)
        {
            if (seconds < 60f)
                return seconds.ToString("0");

            string mins = (seconds / 60f).ToString("0");
            string secs = (seconds % 60f).ToString("0");

            StringBuilder sb = new(mins);
            sb.Append(".");
            sb.Append(secs);

            return sb.ToString();
        }

        private void Start()
        {
            Reinitialize();
        }

        public void Initialize(IRuntimeAbility ability)
        {
            m_ability = ability;
            Reinitialize();
        }

        void Reinitialize()
        {
            if (m_ability == null)
            {
                m_graphic.alpha = 0f;
                enabled = false;
                return;
            }

            m_stackable = m_ability.MaxStack > 1;
            if (m_useCountText != null && !m_stackable)
                m_useCountText.gameObject.SetActive(false);

            m_graphic.alpha = 1f;
            enabled = true;

            m_totalCooldown = m_ability.Cooldown;
            m_totalDuration = m_ability.Duration;
            m_totalUseCount = m_ability.MaxStack;
        }

        private void Update()
        {
            if (m_ability == null) 
                return;

            float timerValue = m_ability.CooldownLeft;

            if (m_nameText != null)
                m_nameText.text = "ABILITY_NAME";

            if (m_useCountText != null && !m_stackable)
                m_useCountText.text = m_ability.CurrentStack.ToString();

            if (!m_readyToUse)
            {
                UpdateCooldown();
                return;
            }

            if (m_semiCooldownFillImage) m_semiCooldownFillImage.fillAmount = timerValue / m_totalCooldown;

            if (m_ability.CurrentStack == 0 && timerValue > 0f)
            {
                SetReady(false);
            }
        }

        void UpdateCooldown()
        {
            float timerValue = m_ability.CooldownLeft;

            if (timerValue <= 0f)
            {
                SetReady(true);
                return;
            }

            if (COOLDOWN_FILL && m_cooldownFill && m_cooldownFillImage != null) 
                m_cooldownFillImage.fillAmount = timerValue / m_totalCooldown;

            if (m_cooldownText != null)
                m_cooldownText.text = CreateCooldownString(timerValue);
        }

        public void SetReady(bool ready)
        {
            m_readyToUse = ready;
            m_cooldownPanel.SetActive(!ready);
            m_semiCooldownFillImage.gameObject.SetActive(ready);
            if (m_stackable) m_useCountText?.gameObject.SetActive(ready);
        }
    }
}
