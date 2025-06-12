using com.absence.attributes;
using com.game.player;
using com.game.subconditionsystem;
using System;
using System.Text;
using UnityEngine;

namespace com.game.scriptables.subconditions
{
    public class PlayerParanoiaLevelSubconditionProfile : SubconditionProfileBase
    {
        public static new string DesignerTooltip => "Checks if the paranoia of player is above or below than a specific value.";

        public enum LevelType
        {
            Percentage,
            Segment,
        }

        public enum ComparisonMode
        {
            Above,
            Below,
        }

        [SerializeField] private LevelType m_levelType = LevelType.Segment;
        [SerializeField] private ComparisonMode m_comparisonMode = ComparisonMode.Above;

        [SerializeField, ShowIf(nameof(m_levelType), LevelType.Segment), Range(0, Constants.Paranoia.PARANOIA_SEGMENT_COUNT - 1)]
        private int m_referenceSegmentIndex = 0;

        [SerializeField, ShowIf(nameof(m_levelType), LevelType.Percentage), Range(0, 100)]
        private int m_referencePercentage = 0;

        [SerializeField] private bool m_acceptEquals = false;

        public override Func<object[], bool> GenerateConditionFormula(SubconditionObject instance)
        {
            return (args) =>
            {
                PlayerParanoiaLogic paranoia = Player.Instance.Hub.Paranoia;
                bool result = false;

                if (m_levelType == LevelType.Percentage)
                {
                    switch (m_comparisonMode)
                    {
                        case ComparisonMode.Above:
                            if (m_acceptEquals) result = m_referencePercentage >= paranoia.TotalPercentage;
                            else result = m_referencePercentage > paranoia.TotalPercentage;
                            break;
                        case ComparisonMode.Below:
                            if (m_acceptEquals) result = m_referencePercentage <= paranoia.TotalPercentage;
                            else result = m_referencePercentage < paranoia.TotalPercentage;
                            break;
                        default:
                            break;
                    }
                }

                else if (m_levelType == LevelType.Segment)
                {
                    switch (m_comparisonMode)
                    {
                        case ComparisonMode.Above:
                            if (m_acceptEquals) result = m_referenceSegmentIndex >= paranoia.SegmentIndex;
                            else result = m_referenceSegmentIndex > paranoia.SegmentIndex;
                            break;
                        case ComparisonMode.Below:
                            if (m_acceptEquals) result = m_referenceSegmentIndex <= paranoia.SegmentIndex;
                            else result = m_referenceSegmentIndex < paranoia.SegmentIndex;
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
            if (m_levelType == LevelType.Segment) targetValue = m_referenceSegmentIndex;
            else if (m_levelType == LevelType.Percentage) targetValue = m_referencePercentage;

            sb.Append("paranoia");

            if (m_levelType == LevelType.Segment)
            {
                if (m_acceptEquals) sb.Append(" is in segment ");
                else sb.Append(" is in a segment ");
            }

            else if (m_levelType == LevelType.Percentage)
            {
                sb.Append(" is ");
            }

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
