using com.absence.variablesystem;
using com.absence.variablesystem.mutations;

namespace com.game.statsystem.extensions
{
    public class FloatCapMutation : Mutation<float>
    {
        public bool CapLow;
        public bool CapHigh;

        public float MinValue;
        public float MaxValue;

        protected override int m_order => 0;

        public FloatCapMutation() : base(0f, AffectionMethod.Overall)
        {
        }

        protected FloatCapMutation(AffectionMethod affectionMethod) : base(0f, affectionMethod)
        {
        }

        public override float Apply(float initialValue)
        {
            if (CapLow && initialValue < MinValue)
            {
                return MinValue;
            }

            if (CapHigh && initialValue > MaxValue) 
            { 
                return MaxValue;
            }

            return initialValue;
        }

        public override void OnAdd(Variable<float> variable)
        {
            base.OnAdd(variable);

            if (!StatSystemSettings.CAPS_WIPE_OUT_MOD_HISTORY) return;

            if (AffectionMethod != AffectionMethod.Overall) return;

            FloatCapMutation mut = CreateSecondary();
            variable.Mutate(mut);
        }

        public override void OnRemove(Variable<float> variable)
        {
            base.OnAdd(variable);

            if (!StatSystemSettings.CAPS_WIPE_OUT_MOD_HISTORY) return;

            if (AffectionMethod != AffectionMethod.Overall) return;

            FloatCapMutation mut = CreateSecondary();
            variable.Mutate(mut);
        }

        FloatCapMutation CreateSecondary()
        {
            return new FloatCapMutation(AffectionMethod.InOrder)
            {
                CapLow = CapLow,
                CapHigh = CapHigh,
                MinValue = MinValue,
                MaxValue = MaxValue
            };
        }
    }
}
