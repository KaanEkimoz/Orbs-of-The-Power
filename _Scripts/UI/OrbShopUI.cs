using com.absence.attributes.experimental;
using com.absence.utilities;
using com.game.orbsystem.itemsystemextensions;
using com.game.player;
using com.game.shopsystem;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace com.game.ui
{
    public class OrbShopUI : MonoBehaviour
    {
        public const int REROLL_COST = 30;

        [SerializeField] private GameObject m_panel;
        [SerializeField] private RectTransform m_stand;
        [SerializeField] private TMP_Text m_title;
        [SerializeField] private TMP_Text m_moneyText;
        [SerializeField] private Button m_inventoryButton;
        [SerializeField] private Button m_rerollButton;
        [SerializeField] private Button m_passButton;
        [SerializeField, InlineEditor] private ItemDisplay m_itemDisplayPrefab;

        public event Action<OrbItemProfile> OnItemBought;
        public event Action<OrbItemProfile> OnItemBoughtOneShot;
        public TMP_Text Title => m_title;

        IShop<OrbItemProfile> m_shop;
        PlayerMoneyLogic m_money;
        int m_passIncome;

        ButtonHandle m_rerollButtonHandle;
        ButtonHandle m_inventoryButtonHandle;
        ButtonHandle m_passButtonHandle;

        public ButtonHandle RerollButton => m_rerollButtonHandle;
        public ButtonHandle InventoryButton => m_inventoryButtonHandle;
        public ButtonHandle PassButton => m_passButtonHandle;

        bool m_anythingHappened;

        private void Start()
        {
            m_money = Player.Instance.Hub.Money;

            CacheButtons();

            m_passIncome = Mathf.FloorToInt(REROLL_COST * 1.5f);

            Hide(true);
        }

        private void Update()
        {
            if (!m_panel.activeInHierarchy)
                return;

            if (Player.Instance.Hub.InputHandler.UIBackButtonPressed)
            {
                Hide(false);
                Game.Resume();
            }
        }

        private void CacheButtons()
        {
            m_rerollButtonHandle = new ButtonHandle(m_rerollButton);
            m_passButtonHandle = new ButtonHandle(m_passButton);
            m_inventoryButtonHandle = new ButtonHandle(m_inventoryButton);

            RerollButton.Interactability = () => m_money.CanAfford(REROLL_COST);
        }

        private void ClearButtonActions()
        {
            RerollButton.ClearClickCallbacks();
            PassButton.ClearClickCallbacks();
            InventoryButton.ClearClickCallbacks();

            RerollButton.OnClick += OnRerollButton;
            PassButton.OnClick += OnPassButton;
        }

        private void RefreshButtons()
        {
            RerollButton.Refresh();
            PassButton.Refresh();
            InventoryButton.Refresh();
        }

        private void OnRerollButton()
        {
            m_money.Spend(REROLL_COST);
            SoftReroll();
        }

        private void OnPassButton()
        {
            m_money.Gain(m_passIncome);
        }

        public void SetVisibility(IShop<OrbItemProfile> shop, bool visibility)
        {
            if (m_shop != null)
                m_shop.OnReroll -= OnShopReroll;

            m_shop = shop;

            if (m_shop != null)
                m_shop.OnReroll += OnShopReroll;

            GenerateDisplay(m_shop);
            m_panel.SetActive(visibility);
            ClearButtonActions();

            // !!!!!!

            if (!visibility)
                SceneManager.Instance.ExitOrbInventory();
        }

        public void Show(IShop<OrbItemProfile> shop, bool reroll = false)
        {
            if (m_shop != null)
                m_shop.OnReroll -= OnShopReroll;

            m_shop = shop;
            if (reroll) SoftReroll();
            SetVisibility(shop, true);
        }

        public void Hide(bool clear = false)
        {
            SetVisibility(m_shop, false);
            if (clear) Clear();
        }

        void OnShopReroll(IShop<OrbItemProfile> shop)
        {
            GenerateDisplay(shop);
        }

        public void SoftReroll()
        {
            m_shop.Reroll();
        }

        void GenerateDisplay(IShop<OrbItemProfile> shop)
        {
            Clear();

            if (shop == null)
                return;

            foreach (OrbItemProfile item in shop.ItemsOnStand)
            {
                ItemDisplay display = Instantiate(m_itemDisplayPrefab, m_stand);
                display.Initialize(item);
                display.SetButtonText("Get");
                display.onButtonClick += (_) =>
                {
                    InvokeOnGet(item);
                    RefreshButtons();
                    RefreshTexts();
                };
            }

            RefreshButtons();
            RefreshTexts();

            bool canAfford = m_money.CanAfford(REROLL_COST);
            string greenLabel = "green";
            string colorLabel = canAfford ? "white" : "red";

            RerollButton.Text = $"Reroll <color={colorLabel}>{REROLL_COST}$</color>";
            PassButton.Text = $"Pass <color={greenLabel}>+{m_passIncome}$</color>";
        }

        void RefreshTexts()
        {
            m_moneyText.text = $"Balance: {m_money.Money.ToString()}$";
        }

        private void InvokeOnGet(OrbItemProfile itemInContext)
        {
            OnItemBought?.Invoke(itemInContext);
            OnItemBoughtOneShot?.Invoke(itemInContext);
            OnItemBoughtOneShot = null;
        }

        public void Clear()
        {
            m_stand.DestroyChildren();
        }
    }
}
