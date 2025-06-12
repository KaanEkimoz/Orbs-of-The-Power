using com.game.scriptableeventsystem;
using System;
using System.Text;
using UnityEngine;

namespace com.game.scriptables.events
{
    [CreateAssetMenu(menuName = "Game/Scriptable Event System/Sequential Event (Generic)",
        fileName = "New Sequential Event",
        order = int.MinValue)]
    public class SequentialScriptableEventProfile : MultiScriptableEventProfileBase
    {
        public override Action<ScriptableEventActionContext, object[]> GenerateAction(ScriptableEventObject instance)
        {
            return (ctx, args) =>
            {
                for (int i = 0; i < instance.Children.Count; i++)
                {
                    var evt = instance.Children[i];
                    if (ctx == ScriptableEventActionContext.Invoke) evt.Invoke();
                    else if (ctx == ScriptableEventActionContext.StartDurableEvent) evt.StartDurableEvent();
                    else if (ctx == ScriptableEventActionContext.StopDurableEvent) evt.StopDurableEvent();
                }
            };
        }

        public override string GenerateDescription(bool richText = false, ScriptableEventObject instance = null)
        {
            StringBuilder sb = new();

            if (instance == null)
            {
                for (int i = 0; i < m_subEvents.Count; i++)
                {
                    var evt = m_subEvents[i];

                    sb.Append(evt.GenerateDescription(richText, null));

                    if (i == m_subEvents.Count - 1) sb.Append(".");
                    else sb.Append(", ");
                }

                return sb.ToString();
            }

            for (int i = 0; i < instance.Children.Count; i++)
            {
                var evt = instance.Children[i];

                sb.Append(evt.GenerateDescription(richText));

                if (i == m_subEvents.Count - 1) sb.Append(".");
                else sb.Append(", ");
            }

            return sb.ToString();
        }

        public override void OnInstantiation(ScriptableEventObject instance)
        {
            foreach (var evt in m_subEvents) 
            {
                instance.Children.Add(new ScriptableEventObject(evt));
            }
        }

        public override void OnRuntimeEventSubscription(ScriptableEventObject instance)
        {
        }

        public override void OnUpdate(GameRuntimeEvent evt, ScriptableEventObject instance)
        {
        }
    }
}
