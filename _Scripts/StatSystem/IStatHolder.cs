using System;

namespace com.game.statsystem
{
    public interface IStatHolder<T> where T : Enum
    {
        /// <summary>
        /// Use to get the current value of a stat.
        /// </summary>
        /// <param name="targetStat">Target stat.</param>
        /// <returns>Returns the current value.</returns>
        float GetStat(T targetStat);
        /// <summary>
        /// Use to try 'n get the current value of a stat.
        /// </summary>
        /// <param name="targetStat">Target stat.</param>
        /// <param name="value">Gives 0f if the function returns false, gives the current value of the target stat otherwise.</param>
        /// <returns>Returns false if something went wrong, true otherwise.</returns>
        bool TryGetStat(T targetStat, out float value);
    }
}
