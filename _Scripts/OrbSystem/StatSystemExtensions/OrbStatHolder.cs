using com.game.statsystem;

namespace com.game.orbsystem.statsystemextensions
{
    /// <summary>
    /// Another example use case of <see cref="StatHolder{T}"/>.
    /// </summary>
    [System.Serializable]
    public sealed class OrbStatHolder : StatHolder<OrbStatType>
    {
        public OrbStatHolder(DefaultStats<OrbStatType> defaultValues) : base(defaultValues)
        {
        }
    }
}
