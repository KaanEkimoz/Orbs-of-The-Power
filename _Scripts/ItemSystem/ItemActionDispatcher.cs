using com.game.itemsystem.gamedependent;

namespace com.game.itemsystem
{
    /// <summary>
    /// The static class responsible for dispatching custom item actions.
    /// </summary>
    public static class ItemActionDispatcher
    {
        public static void DispatchAll(ItemObject targetItem)
        {
            targetItem.Profile.CustomActions.ForEach(act =>
            {
                DispatchOne(targetItem, act);
            });
        }

        public static void DispatchOne(ItemObject context, ItemCustomAction action)
        {
            var act = ItemActionCorrelator.GetAction(action.ActionType);
            act?.Invoke(context, action);
        }
    }
}
