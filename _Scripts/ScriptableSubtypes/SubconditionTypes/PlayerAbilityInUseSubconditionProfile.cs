using com.game.abilitysystem;
using com.game.player;
using com.game.subconditionsystem;
using System;
using System.Text;
using UnityEngine;

namespace com.game.scriptables.subconditions
{
    public class PlayerAbilityInUseSubconditionProfile : SubconditionProfileBase
    {
        public static new string DesignerTooltip => "Checks if a specific ability is in use by the player.";

#if UNITY_EDITOR
        [SerializeField] private AbilityProfile m_abilityReference;
#endif

        [SerializeField, HideInInspector] private string m_targetAbilityGuid;
        [SerializeField, HideInInspector] private string m_targetAbilityDisplayName;

        public override Func<object[], bool> GenerateConditionFormula(SubconditionObject instance)
        {
            return (args) =>
            {
                bool result = Player.Instance.Hub.Abilities.InUse(m_targetAbilityGuid);

                if (Invert)
                    result = !result;

                return result;
            };
        }

        public override string GenerateDescription(bool richText = false, SubconditionObject instance = null)
        {
            StringBuilder sb = new();

            sb.Append(m_targetAbilityDisplayName);
            sb.Append(" in use");

            return sb.ToString();
        }

        private void OnValidate()
        {
#if UNITY_EDITOR
            m_targetAbilityGuid = m_abilityReference != null ? m_abilityReference.Guid : null;
            m_targetAbilityDisplayName = m_abilityReference != null ? m_abilityReference.DisplayName : null;
#endif
        }
    }
}
