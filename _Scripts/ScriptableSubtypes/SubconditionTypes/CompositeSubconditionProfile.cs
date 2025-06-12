using com.absence.attributes;
using com.absence.attributes.experimental;
using com.game.subconditionsystem;
using System;
using System.Text;
using UnityEngine;

namespace com.game.scriptables.subconditions
{
    [CreateAssetMenu(menuName = "Game/Subcondition System/Composite Subcondition (Generic)", 
        fileName = "New Composite Subcondition",
        order = int.MinValue)]
    public class CompositeSubconditionProfile : SubconditionProfileBase
    {
        public static new string DesignerTooltip => "Combines 2 subconditions like a logic gate (eg. AND, OR).";

        public enum CompositeType
        {
            And,
            Or,
        }

        static string GetCompositeTypeString(CompositeType compositeType)
        {
            return Enum.GetName(typeof(CompositeType), compositeType).ToLower();
        }

        [SerializeField] private CompositeType m_compositeType = CompositeType.And;

        [SerializeField, DisableIf(nameof(IsSubAsset)), InlineEditor(newButtonId = 2303, delButtonId = 2304)]
        private SubconditionProfileBase m_subcondition1;

        [SerializeField, DisableIf(nameof(IsSubAsset)), InlineEditor(newButtonId = 2305, delButtonId = 2306)]
        private SubconditionProfileBase m_subcondition2;

        public override Func<object[], bool> GenerateConditionFormula(SubconditionObject instance)
        {
            return (args) =>
            {
                bool result1 = instance.Children[0].GetResult();
                bool result2 = instance.Children[1].GetResult();

                bool result = m_compositeType switch
                {
                    CompositeType.And => result1 && result2,
                    CompositeType.Or => result1 || result2,
                    //CompositeType.Xor => (result1 || result2) && (!(result1 && result2)),
                    _ => throw new NotImplementedException()
                };

                if (Invert)
                    return !result;

                return result;
            };
        }

        public override string GenerateDescription(bool richText = false, SubconditionObject instance = null)
        {
            StringBuilder sb = new();
            if (instance == null)
            {
                sb.Append(m_subcondition1.GenerateDescription(richText, null));

                if (richText) sb.Append("<b>");
                sb.Append($" {GetCompositeTypeString(m_compositeType)} ");
                if (richText) sb.Append("</b>");

                sb.Append(m_subcondition2.GenerateDescription(richText, null));

                return sb.ToString();
            }

            sb.Append(m_subcondition1.GenerateDescription(richText, instance.Children[0]));

            if (richText) sb.Append("<b>");
            sb.Append($" {GetCompositeTypeString(m_compositeType)} ");
            if (richText) sb.Append("</b>");

            sb.Append(m_subcondition2.GenerateDescription(richText, instance.Children[1]));

            return sb.ToString();
        }

        public override void OnInstantiation(SubconditionObject instance)
        {
            base.OnInstantiation(instance);

            instance.Children.Add(new SubconditionObject(m_subcondition1));
            instance.Children.Add(new SubconditionObject(m_subcondition2));
        }
    }
}
