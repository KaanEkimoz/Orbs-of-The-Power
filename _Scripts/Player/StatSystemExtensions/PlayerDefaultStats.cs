using com.game.statsystem;
using UnityEngine;

namespace com.game.player.statsystemextensions
{
    [CreateAssetMenu(fileName = "New PlayerDefaultStats", menuName = "Game/Stat System/Player Default Stats", order = int.MinValue)]
    public class PlayerDefaultStats : DefaultStats<PlayerStatType>
    {
        public override string GetTitle() => "Player";
    }
}
