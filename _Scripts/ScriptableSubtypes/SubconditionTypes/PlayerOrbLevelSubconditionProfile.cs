using com.absence.attributes;
using com.game.orbsystem;
using com.game.player;
using com.game.subconditionsystem;
using System;
using System.Text;
using UnityEngine;

namespace com.game.scriptables.subconditions
{
    public class PlayerOrbLevelSubconditionProfile : SubconditionProfileBase
    {
        public static new string DesignerTooltip => "Checks if level of the element on the orb held by the player is greater, less or equal to a specific value. Defaulting to a predefined value.";

        public enum ComparisonMode
        {
            Less,
            Equals,
            Greater,
        }

        [SerializeField] private ComparisonMode m_comparisonMode = ComparisonMode.Greater;
        [SerializeField, Min(0)] private int m_referenceLevel;

        [SerializeField, DisableIf(nameof(m_comparisonMode), ComparisonMode.Equals)] 
        private bool m_acceptEquals = true;

        [SerializeField] private bool m_defaultValueIfNoElements = false;

        public override Func<object[], bool> GenerateConditionFormula(SubconditionObject instance)
        {
            return (args) =>
            {
                SimpleOrb orb = Player.Instance.Hub.OrbContainer.Controller.OrbHeld;
                OrbInventory inventory = Player.Instance.Hub.OrbContainer.OrbInventoryEntries[orb];

                if (inventory.CurrentItem == null)
                    return m_defaultValueIfNoElements;

                int level = inventory.CurrentItem.Profile.Level;
                bool result;

                switch (m_comparisonMode)
                {
                    case ComparisonMode.Less:
                        if (m_acceptEquals) result = level <= m_referenceLevel;
                        else result = level < m_referenceLevel;
                        break;
                    case ComparisonMode.Equals:
                        result = level == m_referenceLevel;
                        break;
                    case ComparisonMode.Greater:
                        if (m_acceptEquals) result = level >= m_referenceLevel;
                        else result = level > m_referenceLevel;
                        break;
                    default:
                        result = false;
                        break;
                }

                if (Invert)
                    result = !result;

                return result;
            };
        }

        public override string GenerateDescription(bool richText = false, SubconditionObject instance = null)
        {
            StringBuilder sb = new();

            sb.Append("the orb held has an element of level ");

            if (m_comparisonMode == ComparisonMode.Equals)
                sb.Append($"{m_referenceLevel}");
            else if (m_acceptEquals)
                sb.Append($"{m_referenceLevel} or ");

            if (m_comparisonMode == ComparisonMode.Greater)
                sb.Append("greater");
            else if (m_comparisonMode == ComparisonMode.Less)
                sb.Append("less");

            if (m_comparisonMode != ComparisonMode.Equals && (!m_acceptEquals))
                sb.Append($" than {m_referenceLevel}");

            return sb.ToString();
        }
    }
}
