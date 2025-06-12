using com.game.interactionsystem;
using com.game.player;
using System.Text;
using UnityEngine;

namespace com.game.generics
{
    public class DropOnInteract : InteractableBase
    {
        [SerializeField] private bool m_interactable = true;
        [SerializeField] private Cost m_cost;
        [Space, SerializeField] private bool m_precalculateDropAmounts = true;
        [SerializeField] private Drop m_drop;

        public override string RichInteractionMessage
        {
            get
            {
                StringBuilder sb = new();

                sb.Append(m_cost.GenerateDescription(true));
                sb.Append("\n");
                sb.Append(m_drop.GenerateDescription(true));

                return sb.ToString();
            }
        }

        public override bool Interactable 
        { 
            get
            {
                return m_interactable && m_cost.CanAfford(Player.Instance);
            }

            set
            {
                m_interactable = value;
            }
        }

        private void Awake()
        {
            m_drop.DiscardCalculations();

            if (m_precalculateDropAmounts)
                m_drop.CalculateAmounts();
        }

        public override bool OnInteract(IInteractor interactor)
        {
            m_cost.Perform(Player.Instance);
            m_drop.Perform(transform);

            return true;
        }

        public override string GenerateInteractionPopupText(IInteractor sender, bool success)
        {
            if (success)
                return null;

            return "<color=red>Couldn't afford.</color>";
        }
    }
}
