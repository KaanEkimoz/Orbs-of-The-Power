using com.absence.utilities;
using com.game.itemsystem;
using com.game.itemsystem.scriptables;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace com.game.ui
{
    public class ItemDisplay : MonoBehaviour, IDisplay<ItemObject>, IDisplay<ItemProfileBase>, 
        IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
    {
        [SerializeField] private Image m_iconImage;
        [SerializeField] private TMP_Text m_nameText;
        [SerializeField] private TMP_Text m_typeText;
        [SerializeField] private GameObject m_descriptionPanel;
        [SerializeField] private TMP_Text m_descriptionText;
        [SerializeField] private GameObject m_outline;
        [SerializeField] private CanvasGroup m_canvasGroup;
        [SerializeField] private Button m_buyButton;
        [HideInInspector, SerializeField] private TMP_Text m_buyButtonText;

        public event Action<ItemDisplay> onPointerEnter;
        public event Action<ItemDisplay> onPointerExit;
        public event Action<ItemDisplay> onPointerClick;

        public CanvasGroup CanvasGroup => m_canvasGroup;

        public Func<ItemDisplay, bool> buttonInteractabilityProvider;
        public event Action<ItemDisplay> onButtonClick;

        ItemObject m_object;
        ItemProfileBase m_profile;
        float m_nonInteractableAlpha = 0.3f;

        public ItemObject Target => m_object;
        public ItemProfileBase Profile => m_profile;
        public bool Interactable
        {
            get
            {
                return m_interactable;
            }

            set
            {
                m_interactable = value;
                Refresh();
            }
        }

        bool m_lockOutlineState;
        bool m_hover;
        bool m_interactable = true;

        private void Start()
        {
            if (m_buyButton != null) 
                m_buyButton.onClick.AddListener(OnButtonClick);

            OnPointerExit(null);
        }

        private void OnButtonClick()
        {
            onButtonClick?.Invoke(this);
        }

        public void Initialize(ItemObject instance)
        {
            m_object = instance;
            m_profile = instance.Profile;
            Redraw();
        }

        public void Initialize(ItemProfileBase profile)
        {
            m_object = null;
            m_profile = profile;
            Redraw();
        }

        public void SetNonInteractableAlpha(float newValue)
        {
            m_nonInteractableAlpha = newValue;
        }

        public void Refresh()
        {
            Redraw();

            if (m_buyButton != null)
            {
                if (buttonInteractabilityProvider != null)
                    m_buyButton.interactable = buttonInteractabilityProvider.Invoke(this);
                else
                    m_buyButton.interactable = true;
            }

            if (!m_interactable)
            {
                m_canvasGroup.alpha = m_nonInteractableAlpha;
                m_canvasGroup.interactable = false;
                UnlockOutline();
                SetOutlineVisibility(false);
                LockOutline();
            }

            else
            {
                m_canvasGroup.alpha = 1f;
                m_canvasGroup.interactable = true;
                UnlockOutline();
            }
        }

        public void SetButtonText(string text, bool richText = false)
        {
            if (m_buyButton == null)
                return;

            m_buyButtonText.richText = richText;
            m_buyButtonText.text = text;
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            m_hover = true;

            if (!m_interactable)
                return;

            if (m_descriptionPanel != null)
                m_descriptionPanel.SetActive(true);

            SetOutlineVisibility(true);

            onPointerEnter?.Invoke(this);
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            m_hover = false;

            if (!m_interactable)
                return;

            if (m_descriptionPanel != null)
                m_descriptionPanel.SetActive(false);

            SetOutlineVisibility(false);

            onPointerExit?.Invoke(this);
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            if (!m_interactable)
                return;

            onPointerClick?.Invoke(this);
        }

        public void LockOutline() => m_lockOutlineState = true;
        public void UnlockOutline()
        {
            m_lockOutlineState = false;

            SetOutlineVisibility(m_hover);
        }

        public void SetOutlineVisibility(bool visibility)
        {
            if (m_lockOutlineState)
                return;

            if (m_outline != null)
                m_outline.SetActive(visibility);
        }

        void Redraw()
        {
            if (m_profile == null)
                return;

            if (m_profile.Icon != null) m_iconImage.sprite = m_profile.Icon;
            if (m_nameText != null) m_nameText.text = m_profile.DisplayName;
            if (m_typeText != null) m_typeText.text = m_profile.TypeName ?? Helpers.SplitCamelCase(m_profile.GetType().Name, " ");

            if (m_descriptionText == null)
                return;

            if (m_object != null) m_descriptionText.text = ItemSystemHelpers.Text.GenerateDescription(m_object, false);
            else m_descriptionText.text = ItemSystemHelpers.Text.GenerateDescription(m_profile, false);
        }

        private void OnValidate()
        {
            if (m_buyButton == null) m_buyButtonText = null;
            else m_buyButtonText = m_buyButton.GetComponentInChildren<TMP_Text>();
        }
    }
}
