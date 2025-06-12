using com.absence.attributes;
using System;
using UnityEngine;

namespace com.game.subconditionsystem
{
    public abstract class SubconditionProfileBase : ScriptableObject
    {
        public enum KeywordSelection
        {
            When,
            While,
            If, 
            For,
        }

        public static string DesignerTooltip => null;
        // public static new string DesignerTooltip => 

        [SerializeField, Header1("Subcondition"), Readonly] private string m_guid = System.Guid.NewGuid().ToString();

        [SerializeField, ShowIf(nameof(m_displayKeyword))] private KeywordSelection m_keyword = KeywordSelection.When;
        //[SerializeField] private bool m_invert = false;
        [SerializeField] private bool m_bypassStateChangeBallbacks = false;

        [HideInInspector] public bool IsSubAsset = false;

        [SerializeField, HideInInspector] private bool m_displayKeyword;

        public string Guid => m_guid;
        public string Keyword => Enum.GetName(typeof(KeywordSelection), m_keyword);

        public bool Invert => false;
        public bool BypassStateChangeCallbacks => m_bypassStateChangeBallbacks;

        public virtual bool DisplayKeyword => true;

        public abstract Func<object[], bool> GenerateConditionFormula(SubconditionObject instance);
        public abstract string GenerateDescription(bool richText = false, SubconditionObject instance = null);

        public virtual void OnInstantiation(SubconditionObject instance)
        {
        }

        public virtual void OnRuntimeEventSubscription(SubconditionObject instance)
        {
        }

        public virtual void OnUpdate(GameRuntimeEvent evt, SubconditionObject instance)
        {
        }

        public virtual void OnValidate()
        {
            m_displayKeyword = DisplayKeyword;
        }

        [Button("Generate Description")]
        protected void PrintFullDescription()
        {
            Debug.Log(GenerateDescription(true, null));
        }
    }
}
