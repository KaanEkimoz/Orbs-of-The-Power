using com.absence.utilities;
using System.Text;
using UnityEngine;

namespace com.game.miscs
{
    public class PopupManager : Singleton<PopupManager>
    {
        [SerializeField] private PopupBehaviour m_defaultPopupPrefab;
        [SerializeField] private PopupBehaviour m_damagePopupPrefab;
        [SerializeField] private PopupBehaviour m_experiencePopupPrefab;
        [SerializeField] private PopupBehaviour m_moneyPopupPrefab;

        public PopupBehaviour CreateDefault(string text, Vector3 position)
        {
            PopupBehaviour popup = Create(m_defaultPopupPrefab, position, Vector3.zero);
            popup.SetText(text);

            return popup;
        }

        public PopupBehaviour CreateDamagePopup(float damage, Vector3 hitPoint, bool displayPercentages = false)
        {
            PopupBehaviour popup = Create(m_damagePopupPrefab, hitPoint, Vector3.zero);
            popup.AutoStartFadeOut = true;
            popup.MoveUpDuringFadeOut = true;
            popup.DestroyAfterFadeOut = true;

            string mask = displayPercentages ? "" : "0";
            popup.SetText(damage.ToString(mask));

            return popup;
        }

        public PopupBehaviour CreateExperiencePopup(int amount, Vector3 position)
        {
            PopupBehaviour popup = Create(m_experiencePopupPrefab, position, Vector3.zero);
            popup.AutoStartFadeOut = true;
            popup.MoveUpDuringFadeOut = true;
            popup.DestroyAfterFadeOut = true;

            StringBuilder sb = new("+");
            sb.Append(amount);

            popup.SetText(sb.ToString());

            return popup;
        }

        public PopupBehaviour CreateMoneyPopup(int amount, Vector3 position)
        {
            PopupBehaviour popup = Create(m_moneyPopupPrefab, position, Vector3.zero);
            popup.AutoStartFadeOut = true;
            popup.MoveUpDuringFadeOut = true;
            popup.DestroyAfterFadeOut = true;

            StringBuilder sb = new("+");
            sb.Append(amount);
            sb.Append("$");

            popup.SetText(sb.ToString());

            return popup;
        }

        T Create<T>(T prefab, Vector3 position, Vector3 eulerAngles) where T : PopupBehaviour
        {
            return Instantiate(prefab, position, Quaternion.Euler(eulerAngles));
        }
    }
}
