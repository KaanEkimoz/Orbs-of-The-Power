using UnityEngine;

namespace com.game.player
{
    public class PlayerMoneyLogic : MonoBehaviour
    {
        [SerializeField] private int m_money;
        public int Money { get { return m_money; } set { m_money = value; } }

        public bool CanAfford(int amount)
        {
            return m_money >= amount;
        }

        public void Spend(int amount)
        {
            if (amount < 0)
            {
                Gain(-amount);
                return;
            }

            int newMoney = m_money - amount;
            m_money = Mathf.Max(0, newMoney);
        }

        public void Gain(int amount)
        {
            if (amount < 0)
            {
                Spend(-amount);
                return;
            }

            m_money += amount;
        }
    }
}
