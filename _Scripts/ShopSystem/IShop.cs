using com.game.itemsystem.scriptables;
using System;
using System.Collections.Generic;

namespace com.game.shopsystem
{
    public interface IShop
    {
        public List<ItemProfileBase> ItemsAvailable { get; set; }
        public List<ItemProfileBase> ItemsOnStand { get; set; }
        public event Action<IShop> OnReroll;
        public void Reinitialize();
        public void Reroll();
        public void Reroll(int count);
    }

    public interface IShop<T> : IShop where T : ItemProfileBase
    {
        public new List<T> ItemsAvailable { get; set; }
        public new List<T> ItemsOnStand { get; set; }
        public new event Action<IShop<T>> OnReroll;
    }
}
