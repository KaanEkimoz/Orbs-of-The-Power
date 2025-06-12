using com.game.statsystem.presetobjects;

namespace com.game.player.statsystemextensions
{
    [System.Serializable]
    public sealed class PlayerStatModification : StatModification<PlayerStatType>
    {
    }

    [System.Serializable]
    public sealed class PlayerStatOverride : StatOverride<PlayerStatType>
    {
    }

    [System.Serializable]
    public sealed class PlayerStatCap : StatCap<PlayerStatType>
    {
    }
}
