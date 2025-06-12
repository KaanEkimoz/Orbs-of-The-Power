using com.game.orbsystem.statsystemextensions;
using com.game.player.statsystemextensions;
using UnityEngine;

namespace com.game.ui.gamedependent.datatypes
{
    [System.Serializable]
    public abstract class StatUIData
    {
        public Sprite Icon;
        public string DisplayName;
        [Multiline] public string Description;
    }

    [System.Serializable]
    public abstract class StatUIData<T> : StatUIData where T : System.Enum
    {
        public T TargetStat;
    }

    [System.Serializable]
    public class PlayerStatUIData : StatUIData<PlayerStatType>
    {
    }

    [System.Serializable]
    public class OrbStatUIData : StatUIData<OrbStatType>
    {
    }
}
