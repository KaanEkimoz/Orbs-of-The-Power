using com.absence.attributes;
using UnityEngine;

namespace com.game.player
{
    [System.Serializable]
    public class PlayerCharacterCustomAction
    {
        public PlayerCharacterActionType ActionType = PlayerCharacterActionType.None;

        [HideIf(nameof(ActionType), PlayerCharacterActionType.None)]
        public string Description;

        [ShowIf(nameof(ActionType), PlayerCharacterActionType.SpawnCharacterBehaviour)]
        public GameObject CharacterBehaviour;
    }
}
