using System;
using System.Collections.Generic;
using UnityEngine;

namespace com.game.itemsystem.gamedependent
{
    /// <summary>
    /// The static class responsible for correlating item actions with their corresponding functions.
    /// </summary>
    public static class ItemActionCorrelator
    {
        public static Dictionary<ItemActionType, Action<ItemObject, ItemCustomAction>> Data = new()
        {
            { ItemActionType.None, NoAction },
            { ItemActionType.SpawnItemBehaviour, SpawnItemBehaviour },
        };

        public static Action<ItemObject, ItemCustomAction> GetAction(ItemActionType actionType)
        {
            return Data[actionType];
        }

        private static void NoAction(ItemObject context, ItemCustomAction action)
        {
            Debug.Log("No actions.");
        }

        private static void SpawnItemBehaviour(ItemObject context, ItemCustomAction action)
        {
            ItemBehaviour bhv = GameObject.Instantiate(action.ItemBehaviour);
            bhv.Initialize(context);
        }
    }
}
