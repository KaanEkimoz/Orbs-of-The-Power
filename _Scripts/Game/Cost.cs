using com.game.player;
using com.game.player.statsystemextensions;
using com.game.statsystem;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace com.game
{
    [System.Serializable]
    public class Cost
    {
        public Value Money;
        public Value Experience;
        public Value Health;
        public Value StatDuration;

        public List<PlayerStatModification> StatModifications = new();
        public List<PlayerStatCap> StatCaps = new();
        public List<PlayerStatOverride> StatOverrides = new();

        public bool CanAfford(Player target)
        {
            bool canAffordMoney = Money.LowerThan(target.Hub.Money.Money, true, true);
            bool canAffordExperience = Experience.LowerThan(target.Hub.Leveling.CurrentExperience, true, true);
            bool canAffordHealth = Health.LowerThan(Mathf.FloorToInt(target.Hub.Combatant.Health), false, true);

            return canAffordMoney && canAffordExperience && canAffordHealth;
        }

        public void Perform(Player target)
        {
            target.Hub.Money.Spend(Money.GetValue());
            target.Hub.Leveling.LoseExperience(Experience.GetValue());
            target.Hub.Combatant.TakeDamage(Health.GetClampedValue(maxValue: Mathf.FloorToInt(target.Hub.Combatant.Health) - 1));

            float statDuration = StatDuration.GetValue();

            foreach (var mod in StatModifications)
            {
                target.Hub.Stats.Manipulator.ModifyWith(mod);
            }

            foreach (var mod in StatCaps)
            {
                target.Hub.Stats.Manipulator.CapWith(mod);
            }

            foreach (var mod in StatOverrides)
            {
                target.Hub.Stats.Manipulator.OverrideWith(mod);
            }
        }

        public string GenerateDescription(bool richText = false)
        {
            Player target = Player.Instance;

            bool canAffordMoney = Money.LowerThan(target.Hub.Money.Money, true, true);
            bool canAffordExperience = Experience.LowerThan(target.Hub.Leveling.CurrentExperience, true, true);
            bool canAffordHealth = Health.LowerThan(Mathf.FloorToInt(target.Hub.Combatant.Health), false, true);

            int moneyCost = Money.GetValue();
            int experienceCost = Experience.GetValue();
            int healthCost = Health.GetValue();
            int statCostDuration = StatDuration.GetValue();

            bool noMoneyCost = moneyCost == 0;
            bool noExperienceCost = experienceCost == 0;
            bool noHealthCost = healthCost == 0;
            bool noStatCost = statCostDuration <= 0;

            if (noMoneyCost && noExperienceCost && noStatCost && noHealthCost)
                return "No cost.";

            StringBuilder sb = new("Cost: \n");

            if (!noHealthCost)
            {
                if (noExperienceCost && noMoneyCost && noStatCost)
                    return $"Costs {GetRichValueText(Health, canAffordHealth)}";

                sb.Append($"\t{GetRichValueText(Health, canAffordHealth)}\n");
            }

            if (!noMoneyCost)
            {
                if (noExperienceCost && noHealthCost && noStatCost)
                    return $"Costs {GetRichValueText(Money, canAffordMoney)}.";

                sb.Append($"\t{GetRichValueText(Money, canAffordMoney)}\n");
            }

            if (!noExperienceCost)
            {
                if (noMoneyCost && noHealthCost && noStatCost)
                    return $"Costs {GetRichValueText(Experience, canAffordExperience)}.";

                sb.Append($"\t{GetRichValueText(Experience, canAffordExperience)}\n");
            }

            if (!noStatCost)
            {
                foreach (var mod in StatModifications)
                {
                    sb.Append("\t");
                    sb.Append(StatSystemHelpers.Text.GenerateDescription(mod, richText));
                    sb.Append("\n");
                }

                foreach (var mod in StatCaps)
                {
                    sb.Append("\t");
                    sb.Append(StatSystemHelpers.Text.GenerateDescription(mod, richText));
                    sb.Append("\n");
                }

                foreach (var mod in StatOverrides)
                {
                    sb.Append("\t");
                    sb.Append(StatSystemHelpers.Text.GenerateDescription(mod, richText));
                    sb.Append("\n");
                }

                if (statCostDuration > 0f)
                    sb.Append($"for {StatDuration}.");
            }

            return sb.ToString();
        }

        string GetRichValueText(Value target, bool lessThanPlayer)
        {
            StringBuilder sb = new();

            if (lessThanPlayer)
                sb.Append("<color=yellow>");
            else
                sb.Append("<color=red>");

            sb.Append(target.ToString());

            sb.Append("</color>");

            return sb.ToString();
        }
    }
}
