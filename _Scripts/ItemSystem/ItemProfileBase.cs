using com.absence.attributes;
using com.absence.attributes.experimental;
using com.absence.utilities.experimental.databases;
using com.game.itemsystem.gamedependent;
using System.Collections.Generic;
using UnityEngine;

using com.game.scriptableeventsystem;
using com.game.subconditionsystem;

namespace com.game.itemsystem.scriptables
{
    /// <summary>
    /// The abstract scriptable object used for holding built-in data of an item.
    /// </summary>
    public abstract class ItemProfileBase : ScriptableObject, IDatabaseMember<string>
    {
        [Header1("Item Profile")]

        [Readonly]
        public string Guid = System.Guid.NewGuid().ToString();
        public string CustomItemId = string.Empty;
        public ItemRarity Rarity = ItemRarity.Common;
        public int Level = 0;

        [Space, Header3("Information")]

        [SpriteField, Tooltip("Icon of this item that will be displayed in the UI.")]
        public Sprite Icon;

        [Tooltip("Name of this item that will be displayed in the UI.")] 
        public string DisplayName;

        [Multiline, Tooltip("Description of this item. It <b>MUST NOT</b> contain the stat modification log of the item.")] 
        public string Description;

        [Space, Header3("Specifics")]
        [SerializeField, Readonly, Tooltip("Unless rarity is set to <color=yellow>'Legendary'</color>, <color=white>'Second'</color> specifics will be <b>ignored</b>.")]
        private ItemRarity m_hoverForDetails;
        [Readonly, InlineEditor(newButtonId = 2392, delButtonId = 2393)] public SubconditionProfileBase FirstSpecificCondition;
        [Readonly, InlineEditor(newButtonId = 2394, delButtonId = 2395)] public ScriptableEventProfileBase FirstSpecificEvent;
        [Readonly, InlineEditor(newButtonId = 2396, delButtonId = 2397)] public SubconditionProfileBase SecondSpecificCondition;
        [Readonly, InlineEditor(newButtonId = 2398, delButtonId = 2399)] public ScriptableEventProfileBase SecondSpecificEvent;

        [Space, Header3("Actions and Behaviour (Legacy)")]

        public List<ItemCustomAction> CustomActions = new();
        [InlineEditor] public ItemBehaviour Behaviour;

        [Button("Generate Description")]
        protected void PrintFullDescription()
        {
            Debug.Log(ItemSystemHelpers.Text.GenerateDescription(this, true));
        }

        public virtual string TypeName => null;
        public abstract string GenerateFurtherDescription(ItemObject context, bool richText);

        public string GetDatabaseKey()
        {
            return Guid;
        }

        private void OnValidate()
        {
            m_hoverForDetails = Rarity;
        }
    }
}
