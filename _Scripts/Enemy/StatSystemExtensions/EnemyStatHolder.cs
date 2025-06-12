using com.game.statsystem;

namespace com.game.enemysystem.statsystemextensions
{
    public sealed class EnemyStatHolder : StatHolder<EnemyStatType>
    {
        public EnemyStatHolder(DefaultStats<EnemyStatType> defaultValues) : base(defaultValues)
        {
        }
    }
}
