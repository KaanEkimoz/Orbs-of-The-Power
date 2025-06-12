using com.absence.attributes;
using com.absence.attributes.experimental;
using com.game.itemsystem;
using com.game.itemsystem.scriptables;
using com.game.orbsystem.statsystemextensions;
using com.game.statsystem;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace com.game.orbsystem.itemsystemextensions
{
    [CreateAssetMenu(fileName = "New Orb Item", menuName = "Game/Item System/ Orb Item Profile", order = int.MinValue)]
    public class OrbItemProfile : ItemProfileBase
    {
        [SerializeField, InlineEditor] private SimpleOrb m_prefab;

        [Space, Header3("Effects on Orb Stats (Generics)")]

        [SerializeField] private List<OrbStatOverride> m_orbStatOverrides;
        [SerializeField] private List<OrbStatModification> m_orbStatModifications;
        [SerializeField] private List<OrbStatCap> m_orbStatCaps;

        public SimpleOrb Prefab => m_prefab;

        public List<OrbStatOverride> StatOverrides => m_orbStatOverrides;
        public List<OrbStatModification> StatModifications => m_orbStatModifications;
        public List<OrbStatCap> StatCaps => m_orbStatCaps;

        public override string TypeName => "Elemental";

        public override string GenerateFurtherDescription(ItemObject context, bool richText)
        {
            StringBuilder sb = new();

            m_orbStatOverrides.ForEach(ovr =>
            {
                sb.Append(StatSystemHelpers.Text.GenerateDescription(ovr, richText));
                sb.Append("\n");
            });

            m_orbStatModifications.ForEach(mod =>
            {
                sb.Append(StatSystemHelpers.Text.GenerateDescription(mod, richText));
                sb.Append("\n");
            });

            m_orbStatCaps.ForEach(cap =>
            {
                sb.Append(StatSystemHelpers.Text.GenerateDescription(cap, richText));
                sb.Append("\n");
            });

            return sb.ToString();
        }
    }
}
