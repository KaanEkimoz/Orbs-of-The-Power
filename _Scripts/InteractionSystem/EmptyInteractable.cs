using System;
using System.Text;
using UnityEngine;
using UnityEngine.Events;

namespace com.game.interactionsystem
{
    public class EmptyInteractable : InteractableBase
    {
        [SerializeField] private bool m_interactable = true;
        [SerializeField] private string m_richInteractionMessage = string.Empty;
        [SerializeField] private string m_richInteractionCallbackPopup = string.Empty;

        [Space]

        [SerializeField] private UnityEvent<IInteractor, bool> m_onInteract;
        [SerializeField] private UnityEvent<IInteractor, bool> m_onSeen;
        [SerializeField] private UnityEvent<IInteractor, bool> m_onPicked;

        public override string RichInteractionMessage
        {
            get
            {
                string bs = base.RichInteractionMessage;

                if (bs != null)
                    return bs;

                return m_richInteractionMessage;
            }
        }
        public override bool Interactable
        {
            get
            {
                return m_interactable;
            }

            set
            {
                m_interactable = value;
            }
        }

        Action<StringBuilder> m_interactionMessagePipeline = null;
        Action<StringBuilder> m_interactionCallbackPopupPipeline = null;

        public override bool OnInteract(IInteractor interactor)
        {
            return true;
        }

        public override void CastInteractionEvents(IInteractor interactor, bool result)
        {
            base.CastInteractionEvents(interactor, result);
            m_onInteract?.Invoke(interactor, result);
        }

        public override void CommitSeen(IInteractor sender, bool seen)
        {
            base.CommitSeen(sender, seen);
            m_onSeen?.Invoke(sender, seen);
        }

        public override void CommitPicked(IInteractor sender, bool picked)
        {
            base.CommitPicked(sender, picked);
            m_onPicked?.Invoke(sender, picked);
        }

        public override string GenerateInteractionPopupText(IInteractor sender, bool success)
        {
            string bs = base.GenerateInteractionPopupText(sender, success);

            if (bs != null)
                return bs;

            return m_richInteractionCallbackPopup;
        }
    }
}
