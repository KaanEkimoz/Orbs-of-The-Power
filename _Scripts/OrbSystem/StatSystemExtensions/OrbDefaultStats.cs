using com.game.statsystem;
using UnityEngine;

namespace com.game.orbsystem.statsystemextensions
{
    [CreateAssetMenu(fileName = "new OrbDefaultStats", menuName = "Game/Stat System/Orb Default Stats", order = int.MinValue)]
    public class OrbDefaultStats : DefaultStats<OrbStatType>
    {
        public override string GetTitle() => "an Orb";
    }
}
