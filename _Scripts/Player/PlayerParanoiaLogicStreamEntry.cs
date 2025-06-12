using System.Collections.Generic;
using UnityEngine;

namespace com.game.player
{
    [System.Serializable]
    public class PlayerParanoiaLogicStreamEntry
    {
        public enum EntryMode
        {
            Raw,
            Curve,
            PerSegment,
        }

        public InterfaceReference<IParanoiaTarget, MonoBehaviour> Target;
        public EntryMode Mode;
        public float OverallMultiplier;
        public float OverallShift;
        public AnimationCurve Curve;
        public List<float> PerSegmentList;

        public float GetValue(PlayerParanoiaLogic paranoia)
        {
            if (paranoia == null)
            {
                Debug.LogError("An error occurred while calculating paranoia affection amount. Returning NaN.");
                return float.NaN;
            }

            switch (Mode)
            {
                case EntryMode.Raw:
                    return paranoia.TotalPercentage01;
                case EntryMode.Curve:
                    return (Curve.Evaluate(paranoia.TotalPercentage01) * OverallMultiplier) + OverallShift;
                case EntryMode.PerSegment:
                    return (PerSegmentList[paranoia.SegmentIndex] * OverallMultiplier) + OverallShift;
                default:
                    Debug.LogError("An error occurred while calculating paranoia affection amount. Returning NaN.");
                    return float.NaN;
            }
        }
    }
}
