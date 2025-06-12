namespace com.game.statsystem.internals
{
    [System.Serializable]
    public abstract class StatDefaultValue
    {
    }

    [System.Serializable]
    public class StatDefaultValue<T> : StatDefaultValue where T : System.Enum
    {
        public T TargetStat;
        public float Value;

        public StatDefaultValue(T targetStat, float value)
        {
            TargetStat = targetStat;
            Value = value;
        }
    }
}
