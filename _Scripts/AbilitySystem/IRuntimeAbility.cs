using com.game.abilitysystem.gamedependent;
using System;

namespace com.game.abilitysystem
{
    public interface IRuntimeAbility
    {
        string Guid { get; }
        //RuntimeAbilityState State { get; }

        float Duration { get; }
        float Cooldown { get; }

        float DurationLeft { get; }
        float CooldownLeft { get; }

        int MaxStack { get; }
        int CurrentStack { get; }

        bool InUse { get; }
        bool InCooldown { get; }
        bool ReadyToUse { get; }

        //event Action<bool> OnUseAction;

        bool CanUse(AbilityUseContext context);
    }
}
