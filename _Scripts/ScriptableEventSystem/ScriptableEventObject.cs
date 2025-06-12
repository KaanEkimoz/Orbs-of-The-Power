using System;
using System.Collections.Generic;

namespace com.game.scriptableeventsystem
{
    public class ScriptableEventObject : IDisposable
    {
        ScriptableEventProfileBase m_profile;

        Action<ScriptableEventActionContext, object[]> m_action;

        object[] m_args;

        public List<ScriptableEventObject> Children;

        public ScriptableEventProfileBase Profile => m_profile;
        public bool Bypass { get; set; }
        public object[] Arguments { get { return m_args; } set { m_args = value; } }

        public event Action<ScriptableEventObject> OnInvoke;
        public event Action<bool> OnDurableEventStateChanged;
        public event Action<ScriptableEventObject> OnDispose;

        bool m_durableEventInProgress = false;

        public ScriptableEventObject(ScriptableEventProfileBase profile)
        {
            Children = new();
            Bypass = false;
            m_profile = profile;

            profile.OnInstantiation(this);

            RegenerateEventAction();
            SubscribeToEvents();
        }

        protected virtual void SubscribeToEvents()
        {
            m_profile.OnRuntimeEventSubscription(this);
        }

        public void RegenerateEventAction()
        {
            Children?.ForEach(child => child.RegenerateEventAction());
            m_action = m_profile.GenerateAction(this);
        }

        public virtual void Update(params object[] args)
        {
            Children?.ForEach(child => child.Update(args));
            m_profile.OnUpdate(Game.Event, this);
        }

        public void Invoke()
        {
            Invoke(m_args);
        }

        public virtual void Invoke(params object[] args)
        {
            if (Bypass)
                return;

            m_action?.Invoke(ScriptableEventActionContext.Invoke, m_args);
        }

        public void StartDurableEvent()
        {
            if (m_durableEventInProgress)
                return;

            m_durableEventInProgress = true;
            OnDurableEventStateChanged?.Invoke(true);

            m_action?.Invoke(ScriptableEventActionContext.StartDurableEvent, m_args);
        }

        public void StopDurableEvent()
        {
            if (!m_durableEventInProgress) 
                return;

            m_durableEventInProgress = false;
            OnDurableEventStateChanged?.Invoke(false);

            m_action?.Invoke(ScriptableEventActionContext.StopDurableEvent, m_args);
        }

        public virtual string GenerateDescription(bool richText = false)
        {
            return m_profile.GenerateDescription(richText, this);
        }

        public virtual void Dispose()
        {
            Children?.ForEach(child => child.Dispose());
            Children = null;
            OnDispose?.Invoke(this);
        }
    }
}
