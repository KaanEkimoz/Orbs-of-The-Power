using com.absence.utilities.experimental.databases;
using com.game.itemsystem.scriptables;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets; // DO NOT ERASE.

namespace com.game.itemsystem
{
    public static class ItemManager
    {
        public const bool ENABLED = true;
        public const string ITEM_PROFILE_TAG = "item-profile";

#if !(DEVELOPMENT_BUILD || UNITY_EDITOR)
        public const Addressables.MergeMode ItemProfileTagMergeModeRelease = Addressables.MergeMode.Intersection;
        public static readonly IList<object> ItemProfileTagsRelease = new List<object>()
        {
            ITEM_PROFILE_TAG,
            Constants.AssetManagement.RELEASE_BUILD_ASSET_LABEL,
        };
#endif

        static IDatabaseInstance<string, ItemProfileBase> s_instance;
        public static IDatabaseInstance<string, ItemProfileBase> Instance => s_instance;

#pragma warning disable CS0162 // Unreachable code detected
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterAssembliesLoaded)]
        static void Initalize()
        {
            if (!ENABLED)
                return;

            if (s_instance != null)
                s_instance.Dispose();

#if DEVELOPMENT_BUILD || UNITY_EDITOR
            s_instance = new AddressablesDatabaseInstance<string, ItemProfileBase>
                (ITEM_PROFILE_TAG, false);
#else
            s_instance = new AddressablesDatabaseInstance<string, ItemProfileBase>
            (ItemProfileTagsRelease, ItemProfileTagMergeModeRelease, false);
#endif

            s_instance.Refresh();
        }
#pragma warning restore CS0162 // Unreachable code detected

        public static List<T> GetItemsOfType<T>() where T : ItemProfileBase
        {
            List<T> result = new List<T>();
            foreach (ItemProfileBase item in s_instance)
            {
                if (item is T itemCasted) result.Add(itemCasted);
            }

            return result;
        }

        public static IEnumerable<T> GetItemsOfTypeOneByOne<T>() where T : ItemProfileBase
        {
            foreach (ItemProfileBase item in s_instance)
            {
                if (item is T itemCasted) yield return itemCasted;
            }
        }

        public static ItemProfileBase GetItem(string guid)
        {
            return s_instance[guid];
        }

        public static T GetItem<T>(string guid) where T : ItemProfileBase
        {
            return s_instance[guid] as T;
        }

        public static T GetItemByCustomId<T>(string customId) where T : ItemProfileBase
        {
            T result = null;
            foreach (ItemProfileBase item in s_instance)
            {
                if (item is not T itemCasted)
                    continue;

                if (itemCasted.CustomItemId != customId)
                    continue;

                result = itemCasted;
            }

            return result;
        }
    }
}
