using com.absence.attributes;
using com.game.itemsystem;
using com.game.itemsystem.scriptables;
using com.game.player.statsystemextensions;
using com.game.statsystem;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace com.game.player.itemsystemextensions
{
    [CreateAssetMenu(fileName = "New Player Item", menuName = "Game/Item System/ Player Item Profile", order = int.MinValue)]
    public class PlayerItemProfile : ItemProfileBase
    {
        [Space, Header3("More Information")]
        [Min(0)] public int Price = 0;

        [Space, Header3("Effects on Player Stats (Generics)")]

        [SerializeField] private List<PlayerStatOverride> m_playerStatOverrides;
        [SerializeField] private List<PlayerStatModification> m_playerStatModifications;
        [SerializeField] private List<PlayerStatCap> m_playerStatCaps;

        public List<PlayerStatOverride> StatOverrides => m_playerStatOverrides;
        public List<PlayerStatModification> StatModifications => m_playerStatModifications;
        public List<PlayerStatCap> StatCaps => m_playerStatCaps;

        public override string TypeName => "Item";

        public override string GenerateFurtherDescription(ItemObject context, bool richText)
        {
            StringBuilder sb = new();

            m_playerStatOverrides.ForEach(ovr => 
            {
                sb.Append(StatSystemHelpers.Text.GenerateDescription(ovr, richText));
                sb.Append("\n");
            });

            m_playerStatModifications.ForEach(mod =>
            {
                sb.Append(StatSystemHelpers.Text.GenerateDescription(mod, richText));
                sb.Append("\n");
            });

            m_playerStatCaps.ForEach(cap =>
            {
                sb.Append(StatSystemHelpers.Text.GenerateDescription(cap, richText));
                sb.Append("\n");
            });

            return sb.ToString();
        }
    }
}
