using com.game.player;
using com.game.subconditionsystem;
using System;
using System.Text;
using UnityEngine;

namespace com.game.scriptables.subconditions
{
    [CreateAssetMenu(menuName = "Game/Subcondition System/OrbState Subcondition (Player)",
    fileName = "New Player OrbState Subcondition",
    order = int.MinValue)]
    public class PlayerOrbStateSubconditionProfile : SubconditionProfileBase
    {
        public static new string DesignerTooltip => "Checks if the orb held by the player is at a specific state (Throwing, Returning, On Ellipse or Sticked).";

        [SerializeField] private OrbState m_targetState;

        public override Func<object[], bool> GenerateConditionFormula(SubconditionObject instance)
        {
            return (args) =>
            {
                bool result = Player.Instance.Hub.OrbContainer.Controller.OrbHeld.currentState == m_targetState;

                if (Invert)
                    return !result;

                return result;
            };
        }

        public override string GenerateDescription(bool richText = false, SubconditionObject instance = null)
        {
            StringBuilder sb = new();
            sb.Append("the orb held by Player is ");
            sb.Append(Enum.GetName(typeof(OrbState), m_targetState).ToLower());

            return sb.ToString();
        }
    }
}
