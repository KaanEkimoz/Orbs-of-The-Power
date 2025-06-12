using com.absence.attributes;
using DG.Tweening;
using UnityEngine;

namespace com.game.orbsystem
{
    [CreateAssetMenu(menuName = "Game/Orb System/Orb Animation Data", fileName = "New Orb Animation Data", order = int.MinValue)]
    public class OrbAnimationData : ScriptableObject
    {
        [Header("Sway Animation (Idle)")]
        [SerializeField] public bool swayEnabled;
        [SerializeField] public Ease swayEase = Ease.Linear;
        [SerializeField] public float swayMagnitude;
        [SerializeField, Min(0.001f)] public float swaySpeed;
        [SerializeField, MinMaxSlider(0f, 1f)] public Vector2 randomSwayDelay;

        [Header("Move Ping Pong Animation (Unstick)")]
        [SerializeField] public Ease unstickEase = Ease.Linear;
        [SerializeField] public float unstickMagnitude;
        [SerializeField, Min(0.001f)] public float unstickSpeed;

        [Header("Throw Animation (Throw)")]
        [SerializeField] public Ease throwAnimationEase;
        [SerializeField] public bool throwEnabled = true;
        [SerializeField] public float throwPivotFactor;
        [SerializeField, MinMaxSlider(-90f, 90f)] public Vector2 throwTiltRange;
        [SerializeField, Min(0.001f)] public float throwDuration;
    }
}
