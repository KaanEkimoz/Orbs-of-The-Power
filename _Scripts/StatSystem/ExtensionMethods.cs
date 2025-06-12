using System;
using UnityEngine;

namespace com.game.statsystem
{
    public static class ExtensionMethods
    {
        //public static float GetStat<T>(this IStats<T> stats, T targetStat) where T : Enum
        //{
        //    if (!Application.isPlaying)
        //        return stats.m_defaultStats.GetDefaultValue(targetStat);

        //    float rawStatValue = stats..GetStat(targetStat);

        //    if (stats.Pipeline == null)
        //    {
        //        Debug.LogWarning("Orb stat pipeline is null.");
        //        return rawStatValue;
        //    }

        //    return stats.Pipeline.Process(targetStat, rawStatValue);
        //}
    }
}
