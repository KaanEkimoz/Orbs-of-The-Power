using com.absence.attributes;
using UnityEngine;

namespace com.game.orbsystem
{
    [CreateAssetMenu(menuName = "Game/Orb System/Orb Combat Data", fileName = "New Orb Combat Data", order = int.MinValue)]
    public class OrbCombatData : ScriptableObject
    {
        public bool useCooldown = true;
        [EnableIf(nameof(useCooldown)), Min(0f)] public float baseCooldown;
        [Min(0f)] public float cooldownStepdownOnReturnHit;
        [Range(0f, 1f)] public float cooldownDamageMultiplier = 1f;
    }
}
