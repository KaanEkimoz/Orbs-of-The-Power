using com.absence.attributes;
using com.absence.attributes.experimental;
using com.absence.utilities.experimental.databases;
using UnityEngine;

namespace com.game.abilitysystem
{
    [CreateAssetMenu(fileName = "New Ability", menuName = "Game/Ability System/Ability Profile", order = int.MinValue)]
    public class AbilityProfile : ScriptableObject, IDatabaseMember<string>
    {
        [Header1("Ability Profile")]

        [Readonly]
        public string Guid = System.Guid.NewGuid().ToString();

        [Space, Header3("Information")]

        [SpriteField, Tooltip("Icon of this ability that will be displayed in the UI.")]
        public Sprite Icon;

        [Tooltip("Name of this ability that will be displayed in the UI.")]
        public string DisplayName;

        [Multiline, Tooltip("Description of this ability. It <b>MUST NOT</b> contain the stat modification log of the ability.")]
        public string Description;

        [Space, Header3("Settings")]
        public AbilityUseType UseType;
        [ShowIf(nameof(UseType), AbilityUseType.Toggle)] public bool IsCancellable = true;
        public bool IsStackable = false;

        [Space, Header3("Behaviour")]
        [ShowIf(nameof(IsStackable)), Min(2)] public int DefaultMaxStack = 2;
        public float DefaultDuration;
        public float DefaultCooldown;
        [InlineEditor] public AbilityBehaviour BehaviourPrefab;

        public string GetDatabaseKey()
        {
            return Guid.ToString();
        }
    }
}
