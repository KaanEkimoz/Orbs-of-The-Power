using com.game.itemsystem;
using com.game.itemsystem.scriptables;
using com.game.player.itemsystemextensions;
using com.game.shopsystem;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace com.game.player
{
    public class PlayerShop : MonoBehaviour, IShop<PlayerItemProfile>
    {
        public List<PlayerItemProfile> ItemsAvailable
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

        public List<PlayerItemProfile> ItemsOnStand
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

        public event Action<IShop<PlayerItemProfile>> OnReroll = null;

        List<PlayerItemProfile> m_itemsAvailable;
        List<PlayerItemProfile> m_itemsOnStand;

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
            m_itemsAvailable = ItemManager.GetItemsOfType<PlayerItemProfile>();
        }

        public void Reroll()
        {
            Reroll(Constants.Shopping.PLAYER_SHOP_CAPACITY);
        }

        public void Reroll(int count)
        {
            PlayerItemProfile[] itemsFound = new PlayerItemProfile[count];
            for (int i = 0; i < count; i++)
            {
                itemsFound[i] = m_itemsAvailable[UnityEngine.Random.Range(0, m_itemsAvailable.Count)];
            }

            m_itemsOnStand = itemsFound.ToList();
            OnReroll?.Invoke(this);
        }
    }
}
