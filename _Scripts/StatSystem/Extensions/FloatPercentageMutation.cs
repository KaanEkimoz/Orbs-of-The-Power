using com.absence.variablesystem;
using com.absence.variablesystem.mutations;
using System;

namespace com.game.statsystem.extensions
{
    [System.Serializable]
    public class FloatPercentageMutation : Mutation<float>
    {
        public FloatPercentageMutation() : base()
        {
        }

        public FloatPercentageMutation(float mutationValue) : base(mutationValue)
        {
        }

        public FloatPercentageMutation(float mutationValue, AffectionMethod affectionMethod) : base(mutationValue, affectionMethod)
        {
        }

        protected override int m_order => 0;

        public override float Apply(float initialValue)
        {
            float realPercentage = Value / 100f;
            float absValue = Math.Abs(initialValue);

            return initialValue + (realPercentage * absValue);
        }
    }
}
