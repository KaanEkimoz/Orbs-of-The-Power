using com.game.abilitysystem.ui;
using com.game.player;
using UnityEngine;

namespace com.game.ui
{
    public class AbilityUIUpdater : MonoBehaviour
    {
        [SerializeField] private AbilityDisplayGP m_ability1Display;
        [SerializeField] private AbilityDisplayGP m_ability2Display;
        [SerializeField] private AbilityDisplayGP m_ability3Display;
        [SerializeField] private AbilityDisplayGP m_ability4Display;

        PlayerAbilitySet m_abilitySet;

        private void Start()
        {
            m_abilitySet = Player.Instance.Hub.Abilities;
            m_abilitySet.OnSetChanged += (_) => Refresh();

            Refresh();
        }

        public void Refresh()
        {
            if (m_abilitySet == null)
                return;

            m_ability1Display.Initialize(m_abilitySet.Ability1);
            m_ability2Display.Initialize(m_abilitySet.Ability2);
            m_ability3Display.Initialize(m_abilitySet.Ability3);
            m_ability4Display.Initialize(m_abilitySet.Ability4);
        }
    }
}
