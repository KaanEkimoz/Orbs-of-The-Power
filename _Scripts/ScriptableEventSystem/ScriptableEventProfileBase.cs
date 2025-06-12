using com.absence.attributes;
using System;
using UnityEngine;

namespace com.game.scriptableeventsystem
{
    public abstract class ScriptableEventProfileBase : ScriptableObject
    {
        public static string DesignerTooltip => null;

        [SerializeField, Header1("Scriptable Event"), Readonly] private string m_guid = System.Guid.NewGuid().ToString();
        [HideInInspector] public bool IsSubAsset = false;

        public string Guid => m_guid;

        public abstract Action<ScriptableEventActionContext, object[]> GenerateAction(ScriptableEventObject instance);
        public abstract string GenerateDescription(bool richText = false, ScriptableEventObject instance = null);
        public abstract void OnInstantiation(ScriptableEventObject instance);
        public abstract void OnRuntimeEventSubscription(ScriptableEventObject instance);
        public abstract void OnUpdate(GameRuntimeEvent evt, ScriptableEventObject instance);

        [Button("Generate Description")]
        protected void PrintFullDescription()
        {
            Debug.Log(GenerateDescription(true, null));
        }
    }
}
