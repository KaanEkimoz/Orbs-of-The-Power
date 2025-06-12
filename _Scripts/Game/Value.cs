using com.absence.attributes;
using com.absence.variablesystem.builtin;
using com.game.player;
using System.Text;
using UnityEngine;

namespace com.game
{
    [System.Serializable]
    public class Value
    {
        public enum ValueType
        {
            Health,
            Paranoia,
            Money,
            Experience,
            Seconds,
        }

        [SerializeField] private ValueType m_type;

        [SerializeField, MinMaxSlider(-Constants.Values.EDGE_VALUE_AMOUNT, Constants.Values.EDGE_VALUE_AMOUNT)] 
        private Vector2Int m_range;

        [SerializeField] private int m_value;
        [SerializeField] private bool m_random;

        public IntegerVariable Variable { get; private set; } = new(0);
        public bool IsRandom => m_random;
        public bool IsZero
        {
            get
            {
                if (!IsRandom)
                    return RawValue == 0;

                return RawRange == Vector2Int.zero; // !!!
            }
        }

        public Vector2Int RawRange => m_range;
        public int RawValue => m_value;

        public int GetValue()
        {
            if (m_random)
                return GetRealValue(Random.Range(m_range.x, m_range.y));

            return GetRealValue(m_value);
        }

        public int GetClampedValue(int minValue = -Constants.Values.EDGE_VALUE_AMOUNT, int maxValue = Constants.Values.EDGE_VALUE_AMOUNT)
        {
            return Mathf.Clamp(GetValue(), minValue, maxValue);
        }

        public bool LowerThan(int target, bool acceptEqual = false, bool defaultsTo = true)
        {
            if (IsRandom)
                return defaultsTo;

            return acceptEqual ?
                GetValue() <= target : GetValue() < target;
        }

        public bool HigherThan(int target, bool acceptEqual = false, bool defaultsTo = true)
        {
            if (IsRandom)
                return defaultsTo;

            return acceptEqual ?
                GetValue() >= target : GetValue() > target;
        }

        //public bool LessThanPlayer(bool acceptEqual = false)
        //{
        //    return LowerThan(GetAccordingPlayerValue(), acceptEqual, true);
        //}

        //public bool GreaterThanPlayer(bool acceptEqual = false)
        //{
        //    return HigherThan(GetAccordingPlayerValue(), acceptEqual, true);
        //}

        //int GetAccordingPlayerValue()
        //{
        //    int result = 0;
        //    switch (m_type)
        //    {
        //        case ValueType.Health:
        //            result = Mathf.FloorToInt(Player.Instance.Hub.Combatant.Health);
        //            break;
        //        case ValueType.Paranoia:
        //            result = Mathf.FloorToInt(Player.Instance.Hub.Paranoia.TotalPercentage);
        //            break;
        //        case ValueType.Money:
        //            result = Player.Instance.Hub.Money.Money;
        //            break;
        //        case ValueType.Experience:
        //            result = Player.Instance.Hub.Leveling.CurrentExperience;
        //            break;
        //        case ValueType.Seconds:
        //            result = int.MinValue;
        //            break;
        //        default:
        //            break;
        //    }

        //    return result;
        //}

        int GetRealValue(int underlyingValue)
        {
            Variable.UnderlyingValue = underlyingValue;
            return Variable.Value;
        }

        public override string ToString()
        {
            StringBuilder sb = new();

            if (m_random)
                sb.Append("??");
            else
                sb.Append(GetValue());

            switch (m_type)
            {
                case ValueType.Health:
                    sb.Append(" HP");
                    break;
                case ValueType.Paranoia:
                    sb.Append(" Paranoia");
                    break;
                case ValueType.Money:
                    sb.Append("$");
                    break;
                case ValueType.Experience:
                    sb.Append(" Exp");
                    break;
                case ValueType.Seconds:
                    sb.Append(" seconds");
                    break;
                default:
                    break;
            }

            return sb.ToString();
        }
    }
}
