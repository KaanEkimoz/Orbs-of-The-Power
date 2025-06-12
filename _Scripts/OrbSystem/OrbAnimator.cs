using System;
using System.Collections.Generic;
using System.Linq;
using com.absence.attributes;
using com.absence.attributes.experimental;
using com.game.player;
using DG.Tweening;
using DG.Tweening.Core.Easing;
using UnityEngine;
using UnityEngine.Splines;

namespace com.game.orbsystem
{
    [RequireComponent(typeof(SimpleOrb))]
    public class OrbAnimator : MonoBehaviour
    {
        public enum OnEllipseState
        {
            Idle_Sway,
            AllOrbCall_Rotate,
        }

        [SerializeField, Readonly] private SimpleOrb m_target;
        [SerializeField, Readonly] private OnEllipseState m_state = OnEllipseState.Idle_Sway;
        [SerializeField, InlineEditor] private OrbAnimationData m_data;

        public OnEllipseState CurrentState
        {
            get
            {
                return m_state;
            }

            set
            {
                m_state = value;
            }
        }

        float m_swayCoefficient = 0f;
        float m_unstickYShift = 0f;
        float m_throwTimer;
        float m_throwTilt;
        Vector3 m_initialEulers;
        Vector3 m_initialPositionBeforeThrow;
        Vector3 m_lastSwayDirection = Vector3.up;
        PlayerOrbController m_controller;
        Tween m_ellipseScaleTween;
        Tween m_swayCoefficientTween;
        Tween m_unstickYShiftTween;
        Tween m_throwAnimationTween;
        bool m_inThrowAnimation = false;
        SplineContainer m_throwSpline;
        Vector3 m_lastFirePoint;

        private void Awake()
        {
            float delay = UnityEngine.Random.Range(m_data.randomSwayDelay.x, m_data.randomSwayDelay.y);

            if (m_data.swayEnabled)
            {
                m_swayCoefficientTween = DOVirtual.Float(0f, 1f, 1f / m_data.swaySpeed, f => m_swayCoefficient = f)
                .SetDelay(delay)
                .SetLoops(-1, LoopType.Yoyo)
                .SetEase(m_data.swayEase);
            }

            m_unstickYShiftTween = DOVirtual.Float(-0.5f, 0.5f, 1f / m_data.unstickSpeed, f => m_unstickYShift = f)
                .SetDelay(delay)
                .SetLoops(-1, LoopType.Yoyo)
                .SetEase(m_data.unstickEase);
        }

        private void Start()
        {
            m_controller = Player.Instance.Hub.OrbContainer.Controller;
            m_initialEulers = transform.eulerAngles;

            m_target.OnStateChanged += OnOrbStateChanged;
            m_controller.OnAllOrbsCalledWithReturn += OnAllOrbsCalled;
            m_controller.OnAllOrbsReturn += OnAllOrbsReturn;

            if (m_data.swayEnabled)
                m_target.SubscribeToTargetPositionPostProcessing(OrbTargetPositionPostProcess);
        }

        private void Update()
        {
            if (m_inThrowAnimation)
                AnimateThrow();
        }

        private void AnimateThrow()
        {
            Vector3 localSplinePosition = m_throwSpline.EvaluatePosition(m_throwTimer);

            //transform.position = m_throwSpline.transform.position + localSplinePosition;
            transform.position = localSplinePosition;
        }

        private Vector3 OrbTargetPositionPostProcess(Vector3 input, OrbState state)
        {
            switch (state)
            {
                case OrbState.OnEllipse:
                    return GetSwayPosition(input);
                case OrbState.Sticked:
                    return GetUnstickPosition(input);
                case OrbState.Throwing:
                    return RunThrowAnimation(input);
                default:
                    return input;
            }
        }

        private Vector3 RunThrowAnimation(Vector3 firePoint)
        {
            m_lastFirePoint = firePoint;
            if (!m_data.throwEnabled)
            {
                m_target.transform.position = firePoint;
                m_target.CommitThrowAnimationEnd();
                m_throwSpline = null;
                return firePoint;
            }

            m_throwSpline = m_target.ThrowSpline;
            m_initialPositionBeforeThrow = transform.position;

            m_throwTilt = UnityEngine.Random.Range(m_data.throwTiltRange.x, m_data.throwTiltRange.y);
            m_controller.SplineTiltPivot.localRotation = Quaternion.Euler(0f , 0f, m_throwTilt);

            UnityEngine.Splines.Spline spline = m_throwSpline.Spline;
            var knotArray = spline.ToArray();
            //int knotIndex = knotArray.Length - 1;

            BezierKnot firstKnot = knotArray[0];
            //BezierKnot knot = knotArray[knotIndex];

            //knot.Position = m_lastFirePoint - m_throwSpline.transform.position; // ???
            firstKnot.Position = m_throwSpline.transform.InverseTransformPoint(transform.position);
            //knot.Position = m_throwSpline.transform.InverseTransformPoint((m_lastFirePoint)); // ???

            spline.SetKnot(0, firstKnot);
            //spline.SetKnot(knotIndex, knot);

            m_throwAnimationTween?.Kill();

            m_throwTimer = 0f;
            m_throwAnimationTween = DOVirtual.Float(0f, 1f, m_data.throwDuration, OnThrowAnimationTweenUpdates)
                .SetEase(m_data.throwAnimationEase)
                .OnComplete(OnThrowAnimationTweenEnds)
                .OnKill(OnThrowAnimationTweenKills);

            m_inThrowAnimation = true;
            return firePoint;
        }

        private void OnThrowAnimationTweenUpdates(float value)
        {
            m_throwTimer = value;
        }

        private void OnThrowAnimationTweenEnds()
        {
            m_throwAnimationTween = null;
            m_throwTimer = 0f;

            m_inThrowAnimation = false;

            Vector3 position = transform.position;
            position.y = m_controller.FirePointGlobal.y;

            m_controller.SplineTiltPivot.localRotation = Quaternion.Euler(0f, 0f, 0f);

            transform.position = position;
            m_throwSpline = null;
            m_target.CommitThrowAnimationEnd();
        }

        private void OnThrowAnimationTweenKills()
        {
            m_throwAnimationTween = null;
        }

        private void OnAllOrbsReturn()
        {
            m_state = OnEllipseState.Idle_Sway;
            //ScaleEllipse(1f);
        }

        private void OnAllOrbsCalled(IEnumerable<SimpleOrb> orbsCalled)
        {
            m_state = OnEllipseState.AllOrbCall_Rotate;
            //ScaleEllipse(m_ellipseScaleMultiplier);
        }

        //void ScaleEllipse(float targetValue)
        //{
        //    if (m_ellipseScaleTween != null)
        //        m_ellipseScaleTween.Kill();

        //    m_ellipseScaleTween =
        //        DOVirtual.Float(m_controller.EllipseSizeMultiplier, targetValue, m_ellipseScaleDuration,
        //        f => m_controller.EllipseSizeMultiplier = f)
        //        .SetEase(m_ellipseScaleEase);
        //}

        private void OnOrbStateChanged(OrbState state)
        {
            //switch (state)
            //{
            //    case OrbState.OnEllipse:
            //        break;
            //    case OrbState.Sticked:
            //        break;
            //    case OrbState.Throwing:
            //        break;
            //    case OrbState.Returning:
            //        break;
            //    default:
            //        break;
            //}
        }

        private Vector3 GetUnstickPosition(Vector3 input)
        {
            if (m_target.StickedTransform != null)
                return input;

            input.y += m_unstickYShift * m_data.unstickMagnitude;
            return input;
        }

        Vector3 GetSwayPosition(Vector3 input)
        {
            if (m_state != OnEllipseState.Idle_Sway)
                return input;

            return input + (m_swayCoefficient * m_data.swayMagnitude * (transform.position - m_controller.EllipseCenterGlobal));
        }

        public Vector3 GetRotatedVector(Vector3 originalEuler, float angle, Vector3 axis)
        {
            return (Quaternion.AngleAxis(angle, axis) * Quaternion.Euler(originalEuler)).eulerAngles;
        }

        private void Reset()
        {
            m_target = GetComponent<SimpleOrb>();
        }
    }
}
