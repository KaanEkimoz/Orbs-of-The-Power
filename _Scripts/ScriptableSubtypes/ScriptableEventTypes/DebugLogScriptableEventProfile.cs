using com.game.scriptableeventsystem;
using System;
using UnityEngine;

namespace com.game.scriptables.events
{
    [CreateAssetMenu(menuName = "Game/Scriptable Event System/Debug Log Event (Debugging)",
    fileName = "New DebugLog Event",
    order = int.MinValue)]
    public class DebugLogScriptableEventProfile : ScriptableEventProfileBase
    {
        [Space, SerializeField] private string m_message;

        public override Action<ScriptableEventActionContext, object[]> GenerateAction(ScriptableEventObject instance)
        {
            return (ctx, args) =>
            {
                if (ctx == ScriptableEventActionContext.Invoke) Debug.Log(m_message);
                else if (ctx == ScriptableEventActionContext.StartDurableEvent) Debug.Log("Start Debugging");
                else if (ctx == ScriptableEventActionContext.StopDurableEvent) Debug.Log("Stop Debugging");
            };
        }

        public override string GenerateDescription(bool richText = false, ScriptableEventObject instance = null)
        {
            return "prints message on editor console";
        }

        public override void OnInstantiation(ScriptableEventObject instance)
        {
        }

        public override void OnRuntimeEventSubscription(ScriptableEventObject instance)
        {
        }

        public override void OnUpdate(GameRuntimeEvent evt, ScriptableEventObject instance)
        {
        }
    }
}
