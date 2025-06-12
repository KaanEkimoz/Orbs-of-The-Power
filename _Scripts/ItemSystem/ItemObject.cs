using com.game.itemsystem.scriptables;
using System;
using System.Collections.Generic;

using com.game.scriptableeventsystem;
using com.game.subconditionsystem;
using com.game.itemsystem.gamedependent;
using com.game.player.itemsystemextensions;

namespace com.game.itemsystem
{
    [System.Serializable]
    public abstract class ItemObject : IDisposable
    {
        public static ItemObject<T> Create<T>(T profile) where T : ItemProfileBase
        {
            return new ItemObject<T>(profile);
        }

        public static ItemObject<T> Create<T>(string guid) where T : ItemProfileBase
        {
            return new ItemObject<T>(guid);
        }

        public ItemProfileBase Profile { get; }

        public List<ItemRuntimeSpecific> Specifics;

        public List<ItemBehaviour> Behaviours { get; protected set; }
        public Dictionary<string, object> CustomData = new();

        public event Action OnDispose = null;

        public ItemObject(ItemProfileBase profile)
        {
            Specifics = new();
            Profile = profile;

            if (profile.FirstSpecificCondition != null && profile.FirstSpecificEvent != null)
            {
                Specifics.Add(new ItemRuntimeSpecific(new SubconditionObject(profile.FirstSpecificCondition), 
                    new ScriptableEventObject(profile.FirstSpecificEvent)));
            }

            if (profile.Rarity == gamedependent.ItemRarity.Legendary)
            {
                if (profile.SecondSpecificCondition != null && profile.SecondSpecificEvent != null)
                {
                    Specifics.Add(new ItemRuntimeSpecific(new SubconditionObject(profile.SecondSpecificCondition),
                        new ScriptableEventObject(profile.SecondSpecificEvent)));
                }
            }

            Behaviours = new();
        }

        public ItemObject(string guid)
        {
            Profile = ItemManager.GetItem(guid);
            Behaviours = new();
        }

        protected void InvokeOnDispose()
        {
            OnDispose?.Invoke();
        }
        public abstract void Dispose();
        public abstract void Update();
    }

    /// <summary>
    /// The reference type used for holding runtime data of an item instance.
    /// </summary>
    [System.Serializable]
    public class ItemObject<T> : ItemObject where T : ItemProfileBase
    {
        public static ItemObject<T> Combine(ItemObject<T> bottom, ItemObject<T> top)
        {
            foreach (ItemRuntimeSpecific specific in top.Specifics)
            {
                bottom.Specifics.Add(ItemRuntimeSpecific.Copy(specific));
            }

            top.Dispose();

            return bottom;
        }

        public new T Profile { get; private set; }

        public ItemObject(T profile) : base(profile)
        {
            if (profile == null)
                throw new Exception("Provided item profile for ItemObject is null.");

            Profile = profile;
            CustomData = new();
        }

        public ItemObject(string guid) : base(guid)
        {
            T profile = ItemManager.GetItem<T>(guid);

            if (profile == null)
                throw new Exception("Provided item profile for ItemObject is null.");

            Profile = profile;
            CustomData = new();
        }

        public override void Dispose()
        {
            Profile = null;
            CustomData = null;

            Specifics.ForEach(specific => specific.Dispose());

            InvokeOnDispose();
        }

        public override void Update()
        {
            Specifics.ForEach(specific => specific.Update());
        }
    }
}
