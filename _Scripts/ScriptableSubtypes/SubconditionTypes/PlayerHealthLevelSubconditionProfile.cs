using com.absence.attributes;
using com.game.subconditionsystem;
using System.Text;
using System;
using UnityEngine;
using com.game.player;

namespace com.game.scriptables.subconditions
{
    public class PlayerHealthLevelSubconditionProfile : SubconditionProfileBase
    {
        public static new string DesignerTooltip => "Checks if player's health is above or below than a specific value.";

        public enum LevelType
        {
            Percentage,
            Value,
        }

        public enum ComparisonMode
        {
            Above,
            Below,
        }

        [SerializeField] private LevelType m_levelType = LevelType.Percentage;
        [SerializeField] private ComparisonMode m_comparisonMode = ComparisonMode.Above;

        [SerializeField, ShowIf(nameof(m_levelType), LevelType.Percentage), Range(0, 100)]
        private int m_referencePercentage = 0;

        [SerializeField, ShowIf(nameof(m_levelType), LevelType.Value), Min(0)]
        private int m_referenceValue = 0;

        [SerializeField] private bool m_acceptEquals = false;

        public override Func<object[], bool> GenerateConditionFormula(SubconditionObject instance)
        {
            return (args) =>
            {
                PlayerCombatant combatant = Player.Instance.Hub.Combatant;
                bool result = false;

                if (m_levelType == LevelType.Percentage)
                {
                    switch (m_comparisonMode)
                    {
                        case ComparisonMode.Above:
                            if (m_acceptEquals) result = m_referencePercentage >= combatant.Health / combatant.MaxHealth;
                            else result = m_referencePercentage > combatant.Health / combatant.MaxHealth;
                            break;
                        case ComparisonMode.Below:
                            if (m_acceptEquals) result = m_referencePercentage <= combatant.Health / combatant.MaxHealth;
                            else result = m_referencePercentage < combatant.Health / combatant.MaxHealth;
                            break;
                        default:
                            break;
                    }
                }

                else if (m_levelType == LevelType.Value)
                {
                    switch (m_comparisonMode)
                    {
                        case ComparisonMode.Above:
                            if (m_acceptEquals) result = m_referenceValue >= combatant.Health;
                            else result = m_referenceValue > combatant.Health;
                            break;
                        case ComparisonMode.Below:
                            if (m_acceptEquals) result = m_referenceValue <= combatant.Health;
                            else result = m_referenceValue < combatant.Health;
                            break;
                        default:
                            break;
                    }
                }

                if (Invert)
                    result = !result;

                return result;
            };
        }

        public override string GenerateDescription(bool richText = false, SubconditionObject instance = null)
        {
            StringBuilder sb = new();

            string suffix = m_levelType == LevelType.Percentage ? "%" : "";

            int targetValue = 0;
            if (m_levelType == LevelType.Value) targetValue = m_referenceValue;
            else if (m_levelType == LevelType.Percentage) targetValue = m_referencePercentage;

            sb.Append("your HP is ");

            if (m_acceptEquals)
                sb.Append($"{targetValue}{suffix} or ");

            if (m_comparisonMode == ComparisonMode.Above)
                sb.Append("above");
            else if (m_comparisonMode == ComparisonMode.Below)
                sb.Append("below");

            if (!m_acceptEquals)
                sb.Append($" {targetValue}{suffix}");

            return sb.ToString();
        }
    }
}
