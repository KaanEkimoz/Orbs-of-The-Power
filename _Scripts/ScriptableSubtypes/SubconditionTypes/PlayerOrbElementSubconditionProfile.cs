using com.game.player;
using com.game.subconditionsystem;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace com.game.scriptables.subconditions
{
    public class PlayerOrbElementSubconditionProfile : SubconditionProfileBase
    {
        public static new string DesignerTooltip => "Checks if the orb held by the player has any element (or what elements it has).";

        [Flags]
        public enum DefinedElementTypes
        {
            None = 0,
            Ice = (1 << 0),
            Fire = (1 << 1),
            Electric = (1 << 2),
            Soul = (1 << 3),
            LifeSteal = (1 << 4),
        }

        public const int AnyElement = -1;

        static Dictionary<Type, DefinedElementTypes> s_typeDefinitionDict = new()
        {
            { typeof(SimpleOrb), DefinedElementTypes.None },
            { typeof(IceOrb), DefinedElementTypes.Ice },
            { typeof(FireOrb), DefinedElementTypes.Fire },
            { typeof(ElectricOrb), DefinedElementTypes.Electric },
            { typeof(SoulOrb), DefinedElementTypes.Soul },
            { typeof(LifestealOrb), DefinedElementTypes.LifeSteal },
        };

        static DefinedElementTypes GetDefinedType(Type type)
        {
            if (s_typeDefinitionDict.TryGetValue(type, out DefinedElementTypes val))
                return val;

            return DefinedElementTypes.None;
        }

        [SerializeField] private DefinedElementTypes m_includedElements = DefinedElementTypes.None;

        public override Func<object[], bool> GenerateConditionFormula(SubconditionObject instance)
        {
            return (args) =>
            {
                Type orbType = Player.Instance.Hub.OrbContainer.Controller.OrbHeld.GetType();

                DefinedElementTypes type = GetDefinedType(orbType);

                bool result = m_includedElements.HasFlag(type);

                // ???
                if (m_includedElements == DefinedElementTypes.None && orbType == typeof(SimpleOrb))
                    result = true;

                if (Invert)
                    result = !result;

                return result;
            };
        }

        public override string GenerateDescription(bool richText = false, SubconditionObject instance = null)
        {
            StringBuilder sb = new();

            sb.Append("the orb held ");

            if (m_includedElements == (DefinedElementTypes)AnyElement) sb.Append("is any element");
            else if (m_includedElements == DefinedElementTypes.None) sb.Append("has no element");
            else
            {
                sb.Append("is ");

                string text = m_includedElements.ToString();
                text = text.Replace(", ", " or ");
                sb.Append(text);
            }

            sb.Append(" ");

            return sb.ToString();
        }
    }
}
