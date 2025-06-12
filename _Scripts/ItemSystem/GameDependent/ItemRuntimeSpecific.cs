using com.game.scriptableeventsystem;
using com.game.subconditionsystem;
using System;
using System.Text;

namespace com.game.itemsystem.gamedependent
{
    public class ItemRuntimeSpecific : IDisposable
    {
        SubconditionObject m_subcondition;
        ScriptableEventObject m_event;

        public SubconditionObject Subcondition => m_subcondition;
        public ScriptableEventObject Event => m_event;

        public ItemRuntimeSpecific(SubconditionObject subcondition, ScriptableEventObject @event) 
        {
            if (subcondition == null)
                throw new Exception("Subcondition of a Specific can not be null!");

            if (@event == null)
                throw new Exception("Event of a Specific can not be null!");

            m_subcondition = subcondition;
            m_event = @event;

            m_subcondition.OnIgnite += (_) => m_event.Invoke();
            m_subcondition.OnResultChanged += (r) =>
            {
                if (r)
                    m_event.StartDurableEvent();
                else
                    m_event.StopDurableEvent();
            };
        }

        public static ItemRuntimeSpecific Copy(ItemRuntimeSpecific original)
        {
            return new ItemRuntimeSpecific(new SubconditionObject(original.m_subcondition.Profile),
                new ScriptableEventObject(original.m_event.Profile));
        }

        public void Update()
        {
            m_subcondition.Update();
            m_event.Update();

            if (m_subcondition.GetResult())
                m_event.Invoke();
        }

        public string GenerateDescription(bool richText = false)
        {
            StringBuilder sb = new();

            sb.Append("when ");
            sb.Append(m_subcondition.GenerateDescription(richText));
            sb.Append("; ");
            sb.Append(m_event.GenerateDescription(richText));
            sb.Append("\n");

            return sb.ToString();
        }

        public void Dispose()
        {
            m_subcondition.Dispose();
            m_event.Dispose();
        }
    }
}
