using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;

namespace com.game.subconditionsystem.editor
{
    public static class SubconditionTypeCache
    {
        public const bool DEBUG_MODE = false;

        static List<Type> s_foundTypes;

        public static int FoundTypeCount => s_foundTypes.Count;
        public static List<Type> FoundTypes => s_foundTypes;

        public static bool NoTypesFound => s_foundTypes == null || s_foundTypes.Count == 0;

        static SubconditionTypeCache()
        {
            Refresh(DEBUG_MODE);
        }

        /// <summary>
        /// Use to refresh the type cache.
        /// </summary>
        /// <param name="debugMode">If true, result will get displayed in the console window.</param>
        public static void Refresh(bool debugMode)
        {
            s_foundTypes = TypeCache.GetTypesDerivedFrom<SubconditionProfileBase>().
                Where(type => !type.IsAbstract).ToList();

            if (debugMode) EditorJobsHelper.PrintTypeCache();
        }
    }
}
