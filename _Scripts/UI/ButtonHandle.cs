using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace com.game.ui
{
    [System.Serializable]
    public class ButtonHandle
    {
        public static readonly string BUTTON = "Button";
        public static readonly Func<bool> TRUE = () => true;

        [SerializeField] private Button m_target;
        [HideInInspector, SerializeField] private TMP_Text m_text;

        Func<bool> m_visibility;
        Func<bool> m_interactability;

        public event Action OnClick;
        public Func<bool> Visibility
        {
            get
            {
                return m_visibility;
            }

            set
            {
                if (value != null)
                    m_visibility = value;
                else
                    m_visibility = TRUE;

                Refresh();
            }
        }
        public Func<bool> Interactability
        {
            get
            {
                return m_interactability;
            }

            set
            {
                if (value != null)
                    m_interactability = value;
                else 
                    m_interactability = TRUE;

                Refresh();
            }
        }
        public string Text
        {
            get
            {
                return m_text.text;
            }

            set
            {
                if (value != null)
                    m_text.text = value;
                else
                    m_text.text = BUTTON;
            }
        }

        public ButtonHandle(Button target)
        {
            m_target = target;
            m_text = target.GetComponentInChildren<TMP_Text>();

            target.onClick.RemoveAllListeners();
            target.onClick.AddListener(InvokeOnClick);

            OnClick = null;
            m_interactability = TRUE;
            m_visibility = TRUE;

            Refresh();
        }

        public void Refresh()
        {
            m_target.interactable = m_interactability.Invoke();
            m_target.gameObject.SetActive(m_visibility.Invoke());
        }

        public void ClearClickCallbacks()
        {
            OnClick = null;
        }

        void InvokeOnClick()
        {
            OnClick?.Invoke();
        }
    }
}
