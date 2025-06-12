using com.game.statsystem;
using UnityEngine;

namespace com.game.enemysystem.statsystemextensions
{
    [CreateAssetMenu(fileName = "New EnemyDefaultStats", menuName = "Game/Stat System/Enemy Default Stats", order = int.MinValue)]
    public class EnemyDefaultStats : DefaultStats<EnemyStatType>
    {
        public override string GetTitle() => "an Enemy";
    }
}
