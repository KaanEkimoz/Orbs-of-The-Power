using com.absence.utilities.experimental.databases;
using System.Collections.Generic; // DO NOT ERASE.
using UnityEngine;
using UnityEngine.AddressableAssets; // DO NOT ERASE.

namespace com.game.abilitysystem
{
    public static class AbilityManager
    {
        public const string ABILITY_PROFILE_TAG = "ability-profile";
        static IDatabaseInstance<string, AbilityProfile> s_database;

#if !(DEVELOPMENT_BUILD || UNITY_EDITOR)
        public const Addressables.MergeMode AbilityProfileTagMergeModeRelease = Addressables.MergeMode.Intersection;
        public static readonly IList<object> AbilityProfileTagsRelease = new List<object>
        {
            ABILITY_PROFILE_TAG,
            Constants.AssetManagement.RELEASE_BUILD_ASSET_LABEL,
        };
#endif

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterAssembliesLoaded)]
        static void Initialize()
        {
#if DEVELOPMENT_BUILD || UNITY_EDITOR
            s_database = new AddressablesDatabaseInstance<string, AbilityProfile>
                (ABILITY_PROFILE_TAG, true);
#else
            s_database = new AddressablesDatabaseInstance<string, AbilityProfile>
            (AbilityProfileTagsRelease, AbilityProfileTagMergeModeRelease, true);       
#endif
            Refresh();
        }

        public static void Refresh()
        {
            s_database.Refresh();
        }

        public static AbilityProfile Get(string guid)
        {
            return s_database[guid];
        }

        public static bool TryGet(string guid, out AbilityProfile output)
        {
            bool result = s_database.TryGet(guid, out output);
            return result;
        }
    }
}
