using com.game.miscs;
using System.Text;
using UnityEngine;

namespace com.game
{
    [System.Serializable]
    public class Drop
    {
        public enum SpawnMode
        {
            Single,
            Individual,
            Random
        }

        public SpawnMode Spawns = SpawnMode.Single;
        public Value Money;
        public Value Experience;

        int m_calculatedMoneyAmount;
        int m_calculatedExperienceAmount;
        bool m_calculated = false;

        public void DiscardCalculations()
        {
            m_calculated = false;
        }

        public void CalculateAmounts()
        {
            m_calculated = true;

            m_calculatedMoneyAmount = Money.GetValue();
            m_calculatedExperienceAmount = Experience.GetValue();
        }

        public void Perform(Transform sender)
        {
            if (DropManager.Instance == null)
            {
                Debug.LogError("There is no DropManager in the scene!");
                return;
            }

            int money = GetValue(Money, m_calculatedMoneyAmount);
            int exp = GetValue(Experience, m_calculatedExperienceAmount);

            switch (Spawns)
            {
                case SpawnMode.Single:
                    DropManager.Instance.SpawnMoneyDrop(money, sender.position);
                    DropManager.Instance.SpawnExperienceDrop(exp, sender.position);

                    break;
                case SpawnMode.Individual:
                    DropManager.Instance.SpawnIndividualMoneyDrops(money, sender.position);
                    DropManager.Instance.SpawnIndividualExperienceDrops(exp, sender.position);

                    break;
                case SpawnMode.Random:
                    DropManager.Instance.SpawnRandomlySeperatedMoneyDrops(money, sender.position);
                    DropManager.Instance.SpawnRandomlySeperatedExperienceDrops(exp, sender.position);
                    break;
                default:
                    break;
            }
        }

        public string GenerateDescription(bool richText = false)
        {
            bool noMoneyDrop = Money.IsZero;
            bool noExperienceDrop = Experience.IsZero;

            if (noMoneyDrop && noExperienceDrop)
                return "Drops nothing.";

            StringBuilder sb = new StringBuilder("Drops:\n");

            if (!noMoneyDrop)
            {
                if (noExperienceDrop)
                    return $"Drops {GetRichValueText(Money)}.";

                sb.Append($"\t{GetRichValueText(Money)}");
                sb.Append("\n");
            }

            if (!noExperienceDrop)
            {
                if (noMoneyDrop)
                    return $"Drops {GetRichValueText(Experience)}.";

                sb.Append($"\t{GetRichValueText(Experience)}");
            }

            return sb.ToString();
        }

        string GetRichValueText(Value target)
        {
            StringBuilder sb = new();

            sb.Append("<color=green>");
            sb.Append(target.ToString());
            sb.Append("</color>");

            return sb.ToString();
        }

        int GetValue(Value target, int defaultsTo)
        {
            return (target.IsRandom) ?
                (m_calculated ? defaultsTo : target.GetValue()) : target.GetValue();
        }
    }
}
