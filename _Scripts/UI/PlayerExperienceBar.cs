using com.game.player;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace com.game.ui
{
    public class PlayerExperienceBar : MonoBehaviour
    {
        [SerializeField] private Image m_fillImage;
        [SerializeField] private TMP_Text m_ratioText;

        PlayerLevelingLogic m_leveling;

        private void Start()
        {
            m_leveling = Player.Instance.Hub.Leveling;
        }

        private void Update()
        {
            int experience = m_leveling.CurrentExperience;
            int targetExperience = m_leveling.ExperienceNeededForNextLevel;
            float ratio = (float)experience / (float)targetExperience;

            m_fillImage.fillAmount = ratio;
            m_ratioText.text = $"{experience}/{targetExperience}";
        }
    }
}
