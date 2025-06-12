using com.absence.attributes;
using com.game.itemsystem.gamedependent;

namespace com.game.itemsystem
{
    /// <summary>
    /// The reference type used for holding the data of a custom action held by an item.
    /// </summary>
    [System.Serializable]
    public class ItemCustomAction
    {
        public ItemActionType ActionType = ItemActionType.None;

        [ShowIf(nameof(ActionType), ItemActionType.SpawnItemBehaviour)]
        public ItemBehaviour ItemBehaviour;
    }
}
