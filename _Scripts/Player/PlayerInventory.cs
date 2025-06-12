using com.game.itemsystem;
using com.game.player.itemsystemextensions;
using com.game.player.statsystemextensions;
using com.game.statsystem;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace com.game.player
{
    public class PlayerInventory : MonoBehaviour, IInventory<PlayerItemProfile>, IInventory<ItemObject<PlayerItemProfile>>
    {
        //[SerializeField] private List<PlayerItemProfile> m_itemBank = new();

        [SerializeField] private List<ItemObject<PlayerItemProfile>> m_items = new();

        [SerializeField]
        private Dictionary<ItemObject<PlayerItemProfile>, List<ModifierObject<PlayerStatType>>>
            m_itemModifierEntries = new();

        public bool Full => m_items.Count >= Constants.Gameplay.MAX_ITEMS_ON_PLAYER;

        //string m_labelText = "No items selected.\n\nSelect an item to display its description here.";
        //ItemObject m_selectedItem;
        PlayerStats m_stats;

        private void Start()
        {
            m_stats = Player.Instance.Hub.Stats;
            //m_itemBank = ItemManager.GetItemsOfType<PlayerItemProfile>().ToList();
        }

        public void ForceUpdate()
        {
            m_items.ForEach(item => item.Update());
        }

        public bool Add(PlayerItemProfile itemProfile)
        {
            if (Full)
                return false;

            if (itemProfile == null)
                return false;

            ItemObject<PlayerItemProfile> itemToAdd = new(itemProfile);

            return Add(itemToAdd);
        }

        public void Remove(PlayerItemProfile itemProfile)
        {
            ItemObject<PlayerItemProfile> itemToRemove = 
                m_items.FirstOrDefault(item => item.Profile.Equals(itemProfile));

            Remove(itemToRemove);
        }

        public bool Add(ItemObject<PlayerItemProfile> target)
        {
            ApplyItemModifiers(target, target.Profile);
            m_items.Add(target);

            return true;
        }

        public void Remove(ItemObject<PlayerItemProfile> target)
        {
            if (target == null)
                return;

            if (!m_items.Contains(target)) 
                return;

            m_items.Remove(target);
            RevertItemModifiers(target);
            target.Dispose();

            m_items.Remove(target);
        }

        public bool Combine(int bottomIndex, int topIndex)
        {
            if (m_items.Count <= bottomIndex)
                return false;

            if (m_items.Count <= topIndex)
                return false;

            return Combine(m_items[bottomIndex], m_items[topIndex]);
        }

        public bool Combine(ItemObject<PlayerItemProfile> bottom, ItemObject<PlayerItemProfile> top)
        {
            if (bottom == null)
                return false;

            if (top == null)
                return false;

            if (!m_items.Contains(bottom))
                return false;

            if (!m_items.Contains(top))
                return false;

            ItemObject<PlayerItemProfile>.Combine(bottom, top);
            Remove(top);

            return true;
        }

        void ApplyItemModifiers(ItemObject<PlayerItemProfile> targetItem, PlayerItemProfile profile)
        {
            List<ModifierObject<PlayerStatType>> modifiers = new();

            profile.StatModifications.ForEach(mod =>
            {
                modifiers.Add(m_stats.Manipulator.ModifyWith(mod));
            });

            profile.StatCaps.ForEach(cap =>
            {
                modifiers.Add(m_stats.Manipulator.CapWith(cap));
            });

            profile.StatOverrides.ForEach(ovr =>
            {
                m_stats.Manipulator.OverrideWith(ovr);
            });

            m_itemModifierEntries.Add(targetItem, modifiers);
            ItemActionDispatcher.DispatchAll(targetItem);
        }

        void RevertItemModifiers(ItemObject<PlayerItemProfile> targetItem)
        {
            List<ModifierObject<PlayerStatType>> modifiers = m_itemModifierEntries[targetItem];

            modifiers.ForEach(mod =>
            {
                m_stats.Manipulator.Demodify(mod);
            });

            // simply can not revert overrides, so...
        }

        const float k_totalGUIWidth = 340f;

        public void OnTestGUI()
        {
            //GUILayout.BeginHorizontal(GUILayout.Width(k_totalGUIWidth));

            //GUILayout.BeginVertical("box");

            //GUILayout.Label("Shop");

            //m_itemBank.ForEach(itemProfile =>
            //{
            //    GUILayout.BeginHorizontal();

            //    if (GUILayout.Button($"Buy {itemProfile.DisplayName}"))
            //        Add(ItemObject.Create(itemProfile));

            //    if (GUILayout.Button("i", GUILayout.Width(20f)))
            //        m_labelText = ItemSystemHelpers.Text.GenerateDescription(itemProfile, true);

            //    GUILayout.EndHorizontal();
            //});

            //GUILayout.EndVertical();

            //GUILayout.BeginVertical("box");

            //GUILayout.Label("Inventory");

            //List<ItemObject> itemsMarkedForRemoval = new();

            //m_items.ForEach(itemObject =>
            //{
            //    GUILayout.BeginHorizontal();

            //    if (GUILayout.Button($"Sell {itemObject.Profile.DisplayName}"))
            //        itemsMarkedForRemoval.Add(itemObject);

            //    if (GUILayout.Button("i"))
            //        m_labelText = ItemSystemHelpers.Text.GenerateDescription(itemObject, true);

            //    GUILayout.EndHorizontal();
            //});

            //itemsMarkedForRemoval.ForEach(item => Remove(item));

            //GUILayout.EndVertical();

            //GUILayout.EndHorizontal();

            //GUILayout.Label(m_labelText);
        }
    }
}
