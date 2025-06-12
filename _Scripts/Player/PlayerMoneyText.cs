using com.absence.attributes;
using TMPro;
using UnityEngine;

namespace com.game.player
{
    public class PlayerMoneyText : MonoBehaviour
    {
        [SerializeField, Required] private TMP_Text m_text;
        [SerializeField] private bool m_displayPaused = false;
        [SerializeField] private bool m_displayUnpaused = true;

        private void Start()
        {
            Game.OnPause += OnPause;
            Game.OnPause += OnResume;
        }

        private void OnPause()
        {
            m_text.gameObject.SetActive(m_displayPaused);
        }

        private void OnResume()
        {
            m_text.gameObject.SetActive(m_displayUnpaused);
        }

        private void Update()
        {
            m_text.text = $"{Player.Instance.Hub.Money.Money}$";
        }
    }
}
