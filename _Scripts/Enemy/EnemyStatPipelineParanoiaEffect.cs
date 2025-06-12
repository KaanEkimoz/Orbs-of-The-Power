using com.game.enemysystem.statsystemextensions;
using com.game.player;
using com.game.statsystem;
using System.Collections.Generic;
using UnityEngine;

namespace com.game.enemysystem
{
    public class EnemyStatPipelineParanoiaEffect : StatPipelineComponentBase<EnemyStatType>
    {
        public enum EffectMode
        {
            Curve,
            PerSegment,
        }

        [SerializeField] private EffectMode m_mode = EffectMode.Curve;

        [SerializeField]
        private AnimationCurve m_curve;

        [SerializeField]
        private List<float> m_perSegmentList;

        [SerializeField] private float m_overallShift;
        [SerializeField] private float m_overallMultiplier;

        protected override void Initialize_Internal()
        {
        }

        protected override float Process_Internal(EnemyStatType statType, float statCoefficient, float rawValue)
        {
            PlayerParanoiaLogic paranoia = Player.Instance.Hub.Paranoia;

            if (paranoia == null)
                return rawValue;

            switch (m_mode)
            {
                case EffectMode.Curve:
                    return (m_curve.Evaluate(paranoia.TotalPercentage01) * statCoefficient * m_overallMultiplier * rawValue) + m_overallShift;
                case EffectMode.PerSegment:
                    return (m_perSegmentList[paranoia.SegmentIndex] * statCoefficient * m_overallMultiplier * rawValue) + m_overallShift;
                default:
                    Debug.LogError("An error occurred while calculating paranoia affection amount. Returning NaN.");
                    return float.NaN;
            }
        }
    }
}
