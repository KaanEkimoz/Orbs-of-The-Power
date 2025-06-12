using com.absence.attributes;
using com.absence.attributes.experimental;
using com.game.camera;
using UnityEngine;

namespace com.game.orbsystem
{
    [CreateAssetMenu(menuName = "Game/Orb System/Orb Combat Effect Data", fileName = "New Combat Effect Data", order = int.MinValue)]
    public class OrbCombatEffectData : ScriptableObject
    {
        [Header1("Orb Combat Effect Data")]

        [Space, Header2("Slow")]
        [Range(0f, 100f)] public float throwSlowAmount = 100f;
        [Range(0f, 5f)]   public float throwSlowDuration = 1.5f;
        [Range(0f, 100f)] public float returnSlowAmount = 100f;
        [Range(0f, 5f)]   public float returnSlowDuration = 1.5f;

        [Space, Header2("Knockback")]
        public bool throwKnockback = true;
        [ShowIf(nameof(throwKnockback))] public bool penetrationKnockback = false;
        [ShowIf(nameof(throwKnockback))] public float throwKnockbackStrength = 1f;
        public bool returnKnockback = true;
        [ShowIf(nameof(returnKnockback))] public float returnKnockbackStrength = 0.1f;

        [Space, Header2("Camera Shake")]
        [InlineEditor] public CameraShakeProfile hitEnemyShakeProfile;
        [InlineEditor] public CameraShakeProfile callFromEnemyShakeProfile;
    }
}
