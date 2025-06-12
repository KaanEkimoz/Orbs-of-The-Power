using UnityEngine;
using UnityEngine.UI;

namespace com.game.ui
{
    public class ParanoiaBarSegment : MonoBehaviour
    {
        [SerializeField] private Image m_foreground;

        public void SetFillAmount(float amount)
        {
            if (amount < 0f) amount = 0f;
            else if (amount > 1f) amount = 1f;

            m_foreground.fillAmount = amount;
        }
    }
}
