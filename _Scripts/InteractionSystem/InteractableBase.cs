using com.absence.attributes;
using System.Text;
using System;
using UnityEngine;
using com.game.miscs;

namespace com.game.interactionsystem
{
    public abstract class InteractableBase : MonoBehaviour, IInteractable
    {
        [SerializeField] private GameObject m_container;
        [SerializeField] private bool m_destroyOnInteraction = false;
        [SerializeField, Readonly] private bool m_hidden = false;

        public virtual string RichInteractionMessage
        {
            get
            {
                if (m_interactionMessagePipeline != null)
                {
                    StringBuilder sb = new StringBuilder();
                    m_interactionMessagePipeline.Invoke(sb);

                    return sb.ToString().Trim();
                }

                return null;
            }
        }
        public abstract bool Interactable { get; set; }
        public virtual bool Hidden
        {
            get
            {
                return m_hidden; 
            }

            set
            {
                m_hidden = value;
            }
        }
        public bool Disposed { get; protected set; }

        public event Action<IInteractable, IInteractor, bool> OnInteraction;
        public event Action<IInteractor, bool> OnSeen;
        public event Action<IInteractor, bool> OnPicked;
        public event Action<IInteractable> OnDispose;

        Action<StringBuilder> m_interactionMessagePipeline = null;
        Action<StringBuilder> m_interactionCallbackPopupPipeline = null;

        public bool Interact(IInteractor interactor)
        {
            PopupManager popupManager = PopupManager.Instance;

            if (!Interactable)
            {
                CastInteractionEvents(interactor, false);
                SpawnPopup(interactor, false, popupManager);
                return false;
            }

            if (!OnInteract(interactor))
            {
                CastInteractionEvents(interactor, false);
                SpawnPopup(interactor, false, popupManager);
                return false;
            }

            if (m_destroyOnInteraction)
                Dispose();

            CastInteractionEvents(interactor, true);
            SpawnPopup(interactor, true, popupManager);
            return true;
        }

        public abstract bool OnInteract(IInteractor interactor);

        protected void SpawnPopup(IInteractor interactor, bool success, PopupManager manager = null)
        {
            string popupText = GenerateInteractionPopupText(interactor, success);

            if (popupText == null)
                return;

            if (manager != null)
                manager.CreateDefault(popupText, transform.position);
        }

        public virtual void CastInteractionEvents(IInteractor interactor, bool result)
        {
            OnInteraction?.Invoke(this, interactor, result);
        }

        public virtual void CommitPicked(IInteractor sender, bool picked)
        {
            OnPicked?.Invoke(sender, picked);
        }

        public virtual void CommitSeen(IInteractor sender, bool seen)
        {
            OnSeen?.Invoke(sender, seen);
        }

        public virtual string GenerateInteractionPopupText(IInteractor sender, bool success)
        {
            if (m_interactionCallbackPopupPipeline != null)
            {
                StringBuilder sb = new StringBuilder();
                m_interactionCallbackPopupPipeline.Invoke(sb);

                return sb.ToString().Trim();
            }

            return null;
        }

        public virtual void Dispose()
        {
            Disposed = true;
            OnDispose?.Invoke(this);

            if (m_container != null)
                Destroy(m_container);
            else
                Destroy(gameObject);
        }

        public void EnpipeInteractionMessage(Action<StringBuilder> builder)
        {
            m_interactionMessagePipeline += builder;
        }

        public void EnpipeInteractionCallbackPopup(Action<StringBuilder> builder)
        {
            m_interactionCallbackPopupPipeline += builder;
            m_interactionCallbackPopupPipeline += (sb) => sb.Append("\n\n");
        }

        [Button("Generate Description")]
        protected void PrintDescription()
        {
            Debug.Log(RichInteractionMessage);
        }
    }
}
