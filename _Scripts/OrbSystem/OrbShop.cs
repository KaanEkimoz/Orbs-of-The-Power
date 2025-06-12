using com.game.itemsystem.scriptables;
using com.game.itemsystem;
using com.game.shopsystem;
using System.Collections.Generic;
using System.Linq;
using System;
using UnityEngine;
using com.game.orbsystem.itemsystemextensions;

namespace com.game.orbsystem
{
    public class OrbShop : MonoBehaviour, IShop<OrbItemProfile>
    {
        public List<OrbItemProfile> ItemsAvailable
        {
            get
            {
                return m_itemsAvailable;
            }

            set
            {
                m_itemsAvailable = value;
            }
        }

        public List<OrbItemProfile> ItemsOnStand
        {
            get
            {
                return m_itemsOnStand;
            }

            set
            {
                m_itemsOnStand = value;
            }
        }

        List<ItemProfileBase> IShop.ItemsAvailable { get { return null; } set { } }
        List<ItemProfileBase> IShop.ItemsOnStand { get { return null; } set { } }

        public event Action<IShop<OrbItemProfile>> OnReroll = null;

        List<OrbItemProfile> m_itemsAvailable;
        List<OrbItemProfile> m_itemsOnStand;

        event Action<IShop> IShop.OnReroll
        {
            add
            {
                OnReroll += value;
            }

            remove
            {
                OnReroll -= value;
            }
        }

        private void Awake()
        {
            Reinitialize();
        }

        public void Reinitialize()
        {
            m_itemsAvailable = ItemManager.GetItemsOfType<OrbItemProfile>();
        }

        public void Reroll()
        {
            Reroll(Constants.Shopping.ORB_SHOP_CAPACITY);
        }

        public void Reroll(int count)
        {
            OrbItemProfile[] itemsFound = new OrbItemProfile[count];
            for (int i = 0; i < count; i++)
            {
                itemsFound[i] = m_itemsAvailable[UnityEngine.Random.Range(0, m_itemsAvailable.Count)];
            }

            m_itemsOnStand = itemsFound.ToList();
            OnReroll?.Invoke(this);
        }
    }
}
