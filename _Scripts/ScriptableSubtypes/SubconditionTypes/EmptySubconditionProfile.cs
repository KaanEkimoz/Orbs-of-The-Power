using com.game.subconditionsystem;
using System;

namespace com.game.scriptables.subconditions
{
    public class EmptySubconditionProfile : SubconditionProfileBase
    {
        public static new string DesignerTooltip => "Returns true.";

        public override bool DisplayKeyword => false;

        public override Func<object[], bool> GenerateConditionFormula(SubconditionObject instance)
        {
            return (args) =>
            {
                return true;
            };
        }

        public override string GenerateDescription(bool richText = false, SubconditionObject instance = null)
        {
            return string.Empty;
        }
    }
}
