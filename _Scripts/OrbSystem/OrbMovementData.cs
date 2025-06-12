using UnityEngine;

namespace com.game.orbsystem
{
    [CreateAssetMenu(menuName = "Game/Orb System/Orb Movement Data", fileName = "New Orb Movement Data", order = int.MinValue)]
    public class OrbMovementData : ScriptableObject
    {
        public float movementSpeed = 5f;
        public float throwSpeedMultiplier = 1f;
        public float recallSpeedMultiplier = 1f;
        public float destickDuration = 1f;
        public float destickStrength = 1f;
        public AnimationCurve destickCurve = AnimationCurve.Linear(0f, 0f, 1f, 1f);
        public float recallSpeedTimeTravelledBase = 1.2f;
        public float recallSpeedCoefficientOverTimeTravelled = 1f;
        public float recallSpeedMaxCoefficientOverTimeTravelled = 5f;
        //public float recallSecondaryInterestDistanceDeadZone = 100f;
        //public float recallSecondaryInterestMagnitude = 0.1f;
        public float onEllipseSpeedMutliplier = 1f;
        public float onEllipseDistanceFactor = 1f;
        public float maxDistance = 20f;
    }
}
