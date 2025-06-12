using com.absence.utilities.experimental.databases;
using com.game.itemsystem;
using com.game.itemsystem.scriptables;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AddressableAssets; // DO NOT ERASE.

namespace com.game
{
    public class ItemRecipeManager
    {
        public const bool ENABLED = false;
        public const string ITEM_PROFILE_TAG = "recipe-profile";

#if !(DEVELOPMENT_BUILD || UNITY_EDITOR)
        public const Addressables.MergeMode ItemProfileTagMergeModeRelease = Addressables.MergeMode.Intersection;
        public static readonly IList<object> ItemProfileTagsRelease = new List<object>()
        {
            ITEM_PROFILE_TAG,
            Constants.AssetManagement.RELEASE_BUILD_ASSET_LABEL,
        };
#endif

        static IDatabaseInstance<string, ItemRecipeProfile> s_instance;
        public static IDatabaseInstance<string, ItemRecipeProfile> Instance => s_instance;

        static List<ItemRecipeProfile> s_recipes;

#pragma warning disable CS0162 // Unreachable code detected
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterAssembliesLoaded)]
        static void Initialize()
        {
            if (!ENABLED)
                return;

            if (s_instance != null)
                s_instance.Dispose();

#if DEVELOPMENT_BUILD || UNITY_EDITOR
            s_instance = new AddressablesDatabaseInstance<string, ItemRecipeProfile>
                (ITEM_PROFILE_TAG, false);
#else
            s_instance = new AddressablesDatabaseInstance<string, ItemRecipeProfile>
            (ItemProfileTagsRelease, ItemProfileTagMergeModeRelease, false);
#endif

            s_recipes = new();
            s_instance.Refresh();

            foreach (ItemRecipeProfile recipe in s_instance)
            {
                s_recipes.Add(recipe);
            }

            //s_instance.Dispose();
            s_instance = null;
        }
#pragma warning restore CS0162 // Unreachable code detected

        public static ItemRecipeProfile GetRecipe(string guid)
        {
            if (!ENABLED)
                return null;

            return s_instance[guid];
        }

        public static bool Exists(ItemProfileBase item1, ItemProfileBase item2, out ItemRecipeProfile result)
        {
            result = null;
            if (!ENABLED)
                return false;

            result = GetRecipe(item1, item2);
            return result != null;
        }

        public static ItemRecipeProfile GetRecipe(ItemProfileBase item1, ItemProfileBase item2)
        {
            if (!ENABLED)
                return null;

            string guid1 = item1.Guid;
            string guid2 = item2.Guid;

            IEnumerable<ItemRecipeProfile> search = s_recipes.Where(recipe =>
            {
                bool match = recipe.LHSGuid.Equals(guid1) && recipe.RHSGuid.Equals(guid2);
                bool inversedMatch = recipe.LHSGuid.Equals(guid2) && recipe.RHSGuid.Equals(guid1);

                return match || inversedMatch;
            });
            List<ItemRecipeProfile> result = search != null ? search.ToList() : new List<ItemRecipeProfile>();

            if (result.Count == 0) return null;
            if (result.Count > 1)
                Debug.LogError("There are multiple recipes with the same formula. This is not supported right now. Returning the first one found.");

            return result[0];
        }

        public static T GetRecipe<T>(string guid) where T : ItemRecipeProfile
        {
            if (!ENABLED)
                return null;

            return s_instance[guid] as T;
        }
    }
}
