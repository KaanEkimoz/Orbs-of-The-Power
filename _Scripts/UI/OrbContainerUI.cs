using com.absence.attributes.experimental;
using com.absence.utilities;
using com.game.itemsystem;
using com.game.itemsystem.scriptables;
using com.game.orbsystem;
using com.game.orbsystem.itemsystemextensions;
using com.game.player;
using System;
using System.Collections.Generic;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace com.game.ui
{
    public class OrbContainerUI : MonoBehaviour
    {
        [Header("Utilities")]
        [SerializeField] private GameObject m_panel;
        [SerializeField] private GameObject m_cachePanel;
        [SerializeField] private RectTransform m_pivot;
        [SerializeField] private RectTransform m_cacheStand;
        [SerializeField] private GameObject m_descriptionPanel;
        [SerializeField] private TMP_Text m_cacheDescriptionText;
        [SerializeField] private TMP_Text m_descriptionText;
        [SerializeField] private Button m_backButton;
        [SerializeField] private Button m_resetButton;
        [SerializeField] private Button m_confirmButton;
        [SerializeField, InlineEditor] private OrbDisplay m_displayPrefab;
        [SerializeField, InlineEditor] private ItemDisplay m_compactDisplayPrefab;

        [Space, Header("Settings")]

        [SerializeField] private float m_diameter;
        [SerializeField] private bool m_constantDescription;

        public ButtonHandle ResetButton => m_resetButtonHandle;
        public ButtonHandle BackButton => m_backButtonHandle;
        public ButtonHandle ConfirmButton => m_confirmButtonHandle;

        public PlayerOrbContainer Container => m_container;

        PlayerOrbContainer m_container;
        Dictionary<SimpleOrb, OrbDisplay> m_displays = new();
        List<ItemDisplay> m_upgradeDisplays = new();
        ItemDisplay m_hoveredUpgradeDisplay;
        ItemDisplay m_selectedUpgradeDisplay;
        SimpleOrb m_hoveredOrb;
        bool m_undoable;
        bool m_temporary;

        ButtonHandle m_resetButtonHandle;
        ButtonHandle m_backButtonHandle;
        ButtonHandle m_confirmButtonHandle;

        private void Start()
        {
            m_container = Player.Instance.Hub.OrbContainer;
            if (!m_constantDescription) InitializeDescriptionPanel(null);

            CacheButtons();

            m_container.OnInitialize((_) => HardRedraw());
            Hide(true);
        }

        private void Update()
        {
            if (!m_panel.activeInHierarchy)
                return;

            if (!m_temporary)
                return;

            if (Player.Instance.Hub.InputHandler.UIBackButtonPressed)
                Hide(false);
        }

        void CacheButtons()
        {
            m_backButtonHandle = new ButtonHandle(m_backButton);
            m_resetButtonHandle = new ButtonHandle(m_resetButton);
            m_confirmButtonHandle = new ButtonHandle(m_confirmButton);

            BackButton.Visibility = () => m_temporary;
            ResetButton.Visibility = HasCacheOrUndo;
            ConfirmButton.Visibility = HasCacheOrUndo;
            ResetButton.Interactability = HasUndo;
        }

        void ResetButtons()
        {
            BackButton.ClearClickCallbacks();
            ConfirmButton.ClearClickCallbacks();
            ResetButton.ClearClickCallbacks();

            ResetButton.OnClick += ResetChanges;
            ConfirmButton.OnClick += ConfirmChanges;
        }

        public void RefreshButtons()
        {
            BackButton.Refresh();
            ConfirmButton.Refresh();
            ResetButton.Refresh();
        }

        bool HasCacheOrUndo()
        {
            return HasCache() || HasUndo();
        }

        bool HasCache()
        {
            return m_container.UpgradeCache != null && m_container.UpgradeCache.Count > 0;
        }

        bool HasUndo()
        {
            return m_undoable;
        }

        public void SetUpgradeCache(IEnumerable<OrbItemProfile> enumerable)
        {
            m_container.SetUpgradeCache(enumerable);
        }

        public void SetVisibility(bool visibility, bool temporarily = false)
        {
            m_temporary = temporarily;
            m_panel.SetActive(visibility);
            ResetButtons();

            if (visibility)
                HardRedraw();
        }

        public void Show(bool refresh = false, bool temporarily = false)
        {
            m_temporary = temporarily;
            if (refresh) SoftRefresh();
            SetVisibility(true, temporarily);
        }

        public void Hide(bool clear = false, bool temporarily = false)
        {
            SetVisibility(false, temporarily);
            if (clear) Clear();
        }

        public void SoftRedraw()
        {
            foreach (KeyValuePair<SimpleOrb, OrbInventory> kvp in m_container.OrbInventoryEntries)
            {
                SimpleOrb orb = kvp.Key;
                OrbInventory inventory = kvp.Value;

                m_displays[orb].Initialize(orb, inventory);
            }

            InitializeDescriptionPanel(m_hoveredOrb);
            RefreshButtons();
        }

        public void HardRedraw()
        {
            Clear();
            DrawOrbs();
            DrawUpgradeCache();
            RefreshButtons();
        }

        void DrawOrbs()
        {
            int count = m_container.OrbInventoryEntries.Count;
            float stepAngle = 360f / count;
            int index = 0;
            foreach (KeyValuePair<SimpleOrb, OrbInventory> kvp in m_container.OrbInventoryEntries)
            {
                SimpleOrb orb = kvp.Key;
                OrbInventory inventory = kvp.Value;

                float totalAngle = index * stepAngle;
                float cos = Mathf.Cos(totalAngle * Mathf.Deg2Rad);
                float sin = Mathf.Sin(totalAngle * Mathf.Deg2Rad);
                Vector2 direction = new Vector2(sin, cos);
                Vector2 position = direction * m_diameter;

                OrbDisplay display = GameObject.Instantiate(m_displayPrefab);
                display.transform.SetParent(m_pivot, false);
                display.transform.localPosition = position;

                display.Initialize(orb, inventory);

                display.onPointerEnter += InitializeDescriptionPanel;
                display.onPointerClick += OnOrbDisplayClick;

                if (!m_constantDescription)
                    display.onPointerExit += (_) => InitializeDescriptionPanel(null);

                m_displays.Add(orb, display);

                index++;
            }
        }

        private void OnOrbDisplayClick(SimpleOrb orb)
        {
            if (m_selectedUpgradeDisplay == null)
                return;

            bool success = m_container.ApplyUpgrade(m_selectedUpgradeDisplay.Profile as OrbItemProfile, orb);

            if (!success)
                return;

            m_displays[orb].Refresh();

            m_undoable = true;

            UnselectCurrentUpgrade(true);

            RefreshButtons();
            InitializeDescriptionPanel(orb);
        }

        void UnselectCurrentUpgrade(bool discard)
        {
            if (m_selectedUpgradeDisplay == null)
                return;

            m_selectedUpgradeDisplay.UnlockOutline();
            if (discard) m_selectedUpgradeDisplay.Interactable = false;

            m_selectedUpgradeDisplay = null;
        }

        void ResetChanges()
        {
            m_undoable = false;
            m_container.UndoAll();

            RefreshButtons();
            if (m_hoveredOrb != null) InitializeDescriptionPanel(m_hoveredOrb);

            UnselectCurrentUpgrade(false);

            FetchOrbIcons();
            RefreshUpgradeDescription();

            foreach (ItemDisplay display in m_upgradeDisplays)
            {
                display.Interactable = true;
            }
        }

        void DrawUpgradeCache()
        {
            if (m_container.UpgradeCache == null || m_container.UpgradeCache.Count == 0)
            {
                m_cachePanel.SetActive(false);
                return;
            }

            m_cachePanel.SetActive(true);

            foreach (OrbItemProfile upgrade in m_container.UpgradeCache) 
            {
                ItemDisplay display = Instantiate(m_compactDisplayPrefab, m_cacheStand);
                display.Initialize(upgrade);
                display.onPointerEnter += OnHoverUpgrade;
                display.onPointerExit += (_) => OnHoverUpgrade(null);
                display.onPointerClick += OnSelectUpgrade;

                m_upgradeDisplays.Add(display);
            }
        }

        private void FetchOrbIcons()
        {
            foreach (OrbDisplay display in m_displays.Values)
            {
                display.Refresh();
            }
        }

        private void OnSelectUpgrade(ItemDisplay display)
        {
            UnselectCurrentUpgrade(false);

            if (m_selectedUpgradeDisplay != null && m_selectedUpgradeDisplay.Equals(display))
                m_selectedUpgradeDisplay = null;
            else
                m_selectedUpgradeDisplay = display;

            if (m_selectedUpgradeDisplay != null)
            {
                m_selectedUpgradeDisplay.LockOutline();
                m_selectedUpgradeDisplay.CanvasGroup.alpha = 0.5f;
            }

            RefreshUpgradeDescription();
        }

        private void OnHoverUpgrade(ItemDisplay display)
        {
            m_hoveredUpgradeDisplay = display;

            RefreshUpgradeDescription();
        }

        private void RefreshUpgradeDescription()
        {
            if (m_hoveredUpgradeDisplay == null)
                m_cacheDescriptionText.text = 
                    m_selectedUpgradeDisplay != null ?
                    GenerateFullDescription(m_selectedUpgradeDisplay.Profile) :
                    string.Empty;
            else
                m_cacheDescriptionText.text = GenerateFullDescription(m_hoveredUpgradeDisplay.Profile);
        }

        private string GenerateFullDescription(ItemProfileBase profile)
        {
            StringBuilder sb = new();
            sb.Append(profile.DisplayName);
            sb.Append("\n\n");
            sb.Append(ItemSystemHelpers.Text.GenerateDescription(profile, false));
            return sb.ToString();
        }

        void ConfirmChanges()
        {
            m_undoable = false;
            m_container.IrreversiblyApplyUndoCache();
        }

        void InitializeDescriptionPanel(SimpleOrb orb)
        {
            if (m_hoveredOrb != null && m_displays.ContainsKey(m_hoveredOrb))
                m_displays[m_hoveredOrb].SetOutlineVisibility(false);

            if (orb == null)
            {
                m_hoveredOrb = null;
                m_descriptionPanel.SetActive(false);
                return;
            }

            m_hoveredOrb = orb;
            m_displays[orb].SetOutlineVisibility(true);
            m_descriptionPanel.SetActive(true);
            m_descriptionText.text = m_container.OrbInventoryEntries[orb].GenerateDescription();
        }

        private void SetupButton(Button target, Action action)
        {
            if (action == null)
            {
                target.onClick.RemoveAllListeners();
                target.gameObject.SetActive(false);
                return;
            }

            target.gameObject.SetActive(true);
            target.onClick.AddListener(() => action.Invoke());
        }

        public void SoftRefresh()
        {
            m_container.Refresh();
            HardRedraw();
        }

        public void Clear()
        {
            m_selectedUpgradeDisplay = null;
            m_hoveredUpgradeDisplay = null;

            m_cacheStand.DestroyChildren();
            m_pivot.DestroyChildren();
            m_displays.Clear();
            m_upgradeDisplays.Clear();
        }
    }
}
