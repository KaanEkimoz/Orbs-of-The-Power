using com.game.itemsystem;
using com.game.player;
using com.game.player.statsystemextensions;
using com.game.statsystem;
using com.game.testing;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace com.game.generics.itembehaviours
{
    public class EnemiesKilledInWaveItemBehaviour : ItemBehaviour
    {
        [SerializeField, Min(1)] private int m_amountOfEnemies = 1;
        [SerializeField] private PlayerStatModification m_modification = new();

        int m_kills;
        int m_steps;
        List<ModifierObject<PlayerStatType>> m_modifiers = new();

        IStatManipulator<PlayerStatType> m_manipulator;

        public override string GenerateActionDescription(bool richText)
        {
            float value = m_modification.Value;

            string sign = value > 0 ? "+" : "";
            string mode = m_modification.ModificationType ==
                statsystem.presetobjects.StatModificationType.Percentage ? "%" : "";

            StringBuilder sb = new();

            if (richText) sb.Append("<b>");

            sb.Append(sign);
            sb.Append(value);
            sb.Append(mode);

            if (richText) sb.Append("</b>");

            sb.Append(" ");

            sb.Append(StatSystemHelpers.Text.GetDisplayName(m_modification.TargetStatType, richText));

            sb.Append(" for every ");

            if (m_amountOfEnemies != 1)
            {
                if (richText) sb.Append("<b>");

                sb.Append(m_amountOfEnemies);

                if (richText) sb.Append("</b>");
            }

            if (m_amountOfEnemies == 1) sb.Append("enemy killed in a wave.");
            else sb.Append(" enemies killed in a wave.");

            return sb.ToString();
        }

        public override string GenerateDataDescription(bool richText)
        {
            StringBuilder sb = new();
            sb.Append(" stats gained: ");
            sb.Append(m_steps * m_modification.Value);

            if (!richText) return sb.ToString();
            else return com.game.utilities.Helpers.Text.Italic(sb.ToString());
        }

        public override void OnSpawn()
        {
            TestEventChannel.OnEnemyKilled += OnEnemyKilled;

            m_kills = 0;
            m_manipulator = Player.Instance.Hub.Stats.Manipulator;
        }

        private void OnEnemyKilled()
        {
            m_kills++;
            if (m_kills >= m_amountOfEnemies) StepUp();
        }

        void StepUp()
        {
            m_kills = 0;
            m_steps++;

            m_modifiers.Add(m_manipulator.ModifyWith(m_modification));
        }

        public override void OnDespawn()
        {
            TestEventChannel.OnEnemyKilled -= OnEnemyKilled;

            m_modifiers.ForEach(modifier =>
            {
                m_manipulator.Demodify(modifier);
            });
        }
    }
}
