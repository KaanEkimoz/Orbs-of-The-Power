using com.absence.attributes;
using com.game.interactionsystem;
using System;
using System.Text;
using TMPro;
using UnityEngine;

namespace com.game.generics
{
    public class InteractableCanvasHelper : MonoBehaviour
    {
        [SerializeField] private bool m_hideIfNotInteractable = false;
        [SerializeField, Required] private Canvas m_canvas;
        [SerializeField, Required] private TMP_Text m_interactionText;

        IInteractable m_interactable;
        IInteractor m_player;
        bool m_picked;
        bool m_seen;

        private void Awake()
        {
            SetCanvasVisibility(false);

            m_interactable = GetComponent<IInteractable>();
            m_interactable.OnPicked += OnPicked;
            m_interactable.OnSeen += OnSeen;
        }

        private void Update()
        {
            m_canvas.gameObject.SetActive((!m_interactable.Hidden) && m_seen && m_picked);

            StringBuilder sb = new();

            if (m_picked)
            {
                sb.Append(m_player.GenerateInteractorMessage(m_interactable));
                sb.Append("\n\n");
            }

            sb.Append(m_interactable.RichInteractionMessage);

            m_interactionText.text = sb.ToString().Trim();
        }

        private void OnSeen(IInteractor interactor, bool seen)
        {
            m_seen = seen;
        }

        private void OnPicked(IInteractor interactor, bool picked)
        {
            if (!interactor.IsPlayer)
                return;

            m_player = interactor;
            m_picked = picked;
            if (m_hideIfNotInteractable) SetCanvasVisibility(picked && m_interactable.Interactable);
            else SetCanvasVisibility(picked);
        }

        private void SetCanvasVisibility(bool visibility)
        {
            m_canvas.gameObject.SetActive(visibility);
        }
    }
}
