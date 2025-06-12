using com.game.abilitysystem;
using com.game.itemsystem;
using com.game.itemsystem.scriptables;
using com.game.ui.gamedependent.datatypes;

namespace com.game.ui.gamedependent.internals
{
    public static class ExtensionMethods
    {
        public static ItemUIData GenerateUIData(this ItemObject itemObject)
        {
            return new ItemUIData()
            {
                Icon = itemObject.Profile.Icon,
                DisplayName = itemObject.Profile.DisplayName,
                Description = ItemSystemHelpers.Text.GenerateDescription(itemObject, false),
            };
        }

        public static ItemUIData GenerateUIData(this ItemProfileBase itemSO)
        {
            return new ItemUIData()
            {
                Icon = itemSO.Icon,
                DisplayName = itemSO.DisplayName,
                Description = ItemSystemHelpers.Text.GenerateDescription(itemSO, false),
            };
        }

        public static AbilityUIData GenerateUIData(this AbilityProfile abilitySO)
        {
            return new AbilityUIData()
            {
                Icon = abilitySO.Icon,
                DisplayName = abilitySO.DisplayName,
                Description = abilitySO.Description,
            };
        }

        public static AbilityUIData GenerateUIData(this IRuntimeAbility runtimeAbility)
        {
            return new AbilityUIData()
            {
                //Icon = runtimeAbility.Icon,
                //DisplayName = runtimeAbility.DisplayName,
                //Description = runtimeAbility.Description,
            };
        }
    }
}
