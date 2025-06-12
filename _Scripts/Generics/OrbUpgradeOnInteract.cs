using com.absence.attributes;
using com.game.interactionsystem;
using com.game.orbsystem;
using com.game.orbsystem.itemsystemextensions;
using com.game.ui;
using UnityEngine;

namespace com.game.generics
{
    public class OrbUpgradeOnInteract : InteractableBase
    {
        [SerializeField, Readonly] private bool m_interactable = true;
        [SerializeField, Required] private OrbShop m_shop;

        bool m_initialized;

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

        public override bool OnInteract(IInteractor interactor)
        {
            if (!m_initialized)
                m_shop.Reroll();

            m_initialized = true;

            OrbShopUI ui = SceneManager.Instance.EnterOrbUpgradeScreen(m_shop, false);
            ui.OnItemBoughtOneShot += OnUpgradeBought;
            ui.PassButton.OnClick += OnPassButtonClicked;
            return true;
        }

        private void OnPassButtonClicked()
        {
            Game.Resume();

            Dispose();
        }

        private void OnUpgradeBought(OrbItemProfile profile)
        {
            SceneManager.Instance.OrbContainerUI.ConfirmButton.OnClick += OnPassButtonClicked;
        }
    }
}
