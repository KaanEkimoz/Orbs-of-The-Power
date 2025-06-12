using com.game.player.scriptables;
using com.game.statsystem;
using System.Collections.Generic;

namespace com.game.player.statsystemextensions
{
    /// <summary>
    /// An example use case of <see cref="StatHolder{T}"/>.
    /// </summary>
    [System.Serializable]
    public sealed class PlayerStatHolder : StatHolder<PlayerStatType>
    {
        public PlayerStatHolder(DefaultStats<PlayerStatType> defaultValues) : base(defaultValues)
        {
        }

        public void ApplyCharacterProfile(PlayerCharacterProfile profile)
        {
            List<PlayerStatOverride> overrides = profile.Overrides;
            List<PlayerStatModification> modifications = profile.Modifications;
            List<PlayerStatCap> caps = profile.Caps;

            overrides.ForEach(ovr =>
            {
                OverrideWith(ovr);
            });

            modifications.ForEach(mod =>
            {
                ModifyWith(mod);
            });

            caps.ForEach(cap =>
            {
                CapWith(cap);
            });
        }
    }
}
