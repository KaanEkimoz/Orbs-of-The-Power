using com.game.itemsystem;
using com.game.itemsystem.scriptables;
using com.game.orbsystem.itemsystemextensions;
using com.game.orbsystem.statsystemextensions;
using com.game.statsystem;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace com.game.orbsystem
{
    [System.Serializable]
    public class OrbInventory : IInventory<OrbItemProfile>, IInventory<ItemObject<OrbItemProfile>>
    {
        public ItemObject<OrbItemProfile> CurrentItem => m_currentItem;
        public OrbItemProfile CastedProfile => m_itemProfile;

        ItemObject<OrbItemProfile> m_currentItem;
        OrbStats m_stats;

        List<ModifierObject<OrbStatType>> m_modifiers;
        OrbItemProfile m_itemProfile;

        public OrbInventory(OrbStats stats)
        {
            m_stats = stats;
            m_modifiers = new();
            m_currentItem = null;
        }

        public void Update()
        {
            m_currentItem?.Update();
        }

        public bool Add(OrbItemProfile profile)
        {
            if (profile == null)
                return false;

            if (m_currentItem == null)
            {
                m_currentItem = new(profile);
                m_itemProfile = profile;
                ApplyStatModifiers(m_currentItem);
                return true;
            }

            if (!ItemRecipeManager.Exists(m_currentItem.Profile, profile, out ItemRecipeProfile recipeProfile)) 
                return false;

            if (ItemManager.GetItem(recipeProfile.ResultGuid) is not OrbItemProfile resultProfile)
                return false;

            RemoveCurrentElement();

            m_currentItem = new(resultProfile);
            m_itemProfile = resultProfile;
            ApplyStatModifiers(m_currentItem);
            return true;
        }

        public bool Add(ItemObject<OrbItemProfile> target)
        {
            if (target == null)
                return false;

            if (m_currentItem == null)
            {
                m_currentItem = target;
                m_itemProfile = target.Profile;
                ApplyStatModifiers(m_currentItem);
                return true;
            }

            if (!ItemRecipeManager.Exists(m_currentItem.Profile, target.Profile, out ItemRecipeProfile recipeProfile))
                return false;

            if (ItemManager.GetItem(recipeProfile.ResultGuid) is not OrbItemProfile resultProfile)
                return false;

            RemoveCurrentElement();

            m_currentItem = new(resultProfile);
            m_itemProfile = resultProfile;
            ApplyStatModifiers(m_currentItem);
            return true;
        }

        /// <summary>
        /// Use <see cref="RemoveCurrentElement"/> instead.
        /// </summary>
        /// <param name="target"></param>
        [Obsolete]
        public void Remove(ItemObject<OrbItemProfile> target)
        {
            
        }

        /// <summary>
        /// Use <see cref="RemoveCurrentElement"/> instead.
        /// </summary>
        [Obsolete]
        public void Remove(OrbItemProfile target)
        {
            RemoveCurrentElement();
        }

        public void RemoveCurrentElement()
        {
            if (m_currentItem == null)
                return;

            RevertCurrentModifiers();
            m_currentItem = null;
            m_itemProfile = null;
        }

        private void ApplyStatModifiers(ItemObject<OrbItemProfile> targetItem)
        {
            OrbItemProfile profile = targetItem.Profile;

            profile.StatModifications.ForEach(mod =>
            {
                m_modifiers.Add(m_stats.Manipulator.ModifyWith(mod));
            });

            profile.StatCaps.ForEach(cap =>
            {
                m_modifiers.Add(m_stats.Manipulator.CapWith(cap));
            });

            profile.StatOverrides.ForEach(ovr =>
            {
                m_stats.Manipulator.OverrideWith(ovr);
            });

            ItemActionDispatcher.DispatchAll(targetItem);
        }

        void RevertCurrentModifiers()
        {
            m_modifiers.ForEach(mod =>
            {
                m_stats.Manipulator.Demodify(mod);
            });

            m_modifiers.Clear();

            // simply can not revert overrides, so...
        }

        public Sprite GetIcon()
        {
            if (m_currentItem == null) 
                return null;

            return m_currentItem.Profile.Icon;
        }

        public string GenerateDescription()
        {
            if (m_currentItem == null) 
                return "No elements.";

            StringBuilder sb = new();
            sb.Append(m_currentItem.Profile.DisplayName);
            sb.Append("\n\n");
            sb.Append(ItemSystemHelpers.Text.GenerateDescription(m_currentItem, false));
            return sb.ToString();
        }
    }
}
