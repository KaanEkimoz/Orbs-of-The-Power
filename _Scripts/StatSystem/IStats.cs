namespace com.game.statsystem
{
    public interface IStats<T> where T : System.Enum
    {
        /// <summary>
        /// Property to use while adding modifiers to stats.
        /// </summary>
        IStatManipulator<T> Manipulator { get; }
        /// <summary>
        /// Pipeline responsible for applying external effects to stats.
        /// </summary>
        public StatPipeline<T> Pipeline { get; }
        /// <summary>
        /// Use to get a stat.
        /// </summary>
        /// <param name="targetStat">Target stat.</param>
        /// <returns>Returns the current value of target stat.</returns>
        float GetStat(T targetStat);
    }
}
