using com.game.subconditionsystem;
using System;
using System.Text;
using UnityEngine;

namespace com.game.scriptables.subconditions
{
    public class PlayerEnemyKillCountSubconditionProfile : SubconditionProfileBase
    {
        public enum KillContext
        {
            InWave,
            InRun,
        }

        [SerializeField] private KillContext m_context = KillContext.InWave;
        [SerializeField, Min(1)] private int m_referenceCount = 1;
        [SerializeField] private bool m_acceptEquals = false;

        public override Func<object[], bool> GenerateConditionFormula(SubconditionObject instance)
        {
            return (args) =>
            {
                bool result = false;

                int comparisonTarget = 0;

                if (m_context == KillContext.InWave)
                    comparisonTarget = SceneManager.Instance.EnemiesKilledByPlayerThisWave;
                else if (m_context == KillContext.InRun)
                    comparisonTarget = SceneManager.Instance.EnemiesKilledByPlayerThisRun;

                if (m_acceptEquals) result = comparisonTarget >= m_referenceCount;
                else result = comparisonTarget > m_referenceCount;

                if (Invert)
                    result = !result;

                return result;
            };
        }

        public override string GenerateDescription(bool richText = false, SubconditionObject instance = null)
        {
            StringBuilder sb = new();

            if (!m_acceptEquals)
                sb.Append("more than ");

            sb.Append(m_referenceCount);
            sb.Append(" ");

            if (m_acceptEquals)
                sb.Append("or more ");

            sb.Append("enemies killed");

            if (m_context == KillContext.InWave)
                sb.Append(" in this wave");
            else if (m_context == KillContext.InRun)
                sb.Append(" in this run");

            return sb.ToString();
        }
    }
}
