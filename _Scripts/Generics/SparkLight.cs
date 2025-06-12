using com.absence.attributes;
using com.absence.timersystem;
using DG.Tweening;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace com.game.generics
{
    public class SparkLight : MonoBehaviour, ISpark
    {
        [SerializeField] private bool m_startOnAwake;
        [SerializeField] private bool m_destroyOnEnd;
        [SerializeField, Required] private List<Light> m_lights;
        [SerializeField] private Ease m_ease;
        [SerializeField, MinMaxSlider(0f, 1f)] private Vector2 m_durationRange;
        [SerializeField, MinMaxSlider(0f, 5f)] private Vector2 m_cooldownRange;
        [SerializeField] private int m_loops;
        [SerializeField, MinMaxSlider(0f, 10f)] private Vector2 m_intensityRange;

        public bool InCooldown => m_inCooldown;

        public event Action OnEnd;

        bool m_inCooldown;
        Sequence m_sequence;
        ITimer m_cooldownTimer;

        private void Awake()
        {
            ForceStop();
            if (m_startOnAwake) Spark();
        }

        public void Spark()
        {
            if (m_sequence != null)
                return;

            if (InCooldown)
                return;

            m_sequence = DOTween.Sequence();
            foreach (Light light in m_lights)
            {
                light.intensity = m_intensityRange.y;
                Tween tween = light.DOIntensity(m_intensityRange.x, UnityEngine.Random.Range(m_durationRange.x, m_durationRange.y));
                m_sequence.Insert(0f, tween);
            }

            m_sequence
                .SetEase(m_ease)
                .SetLoops(m_loops, LoopType.Yoyo)
                .OnComplete(OnSequenceComplete)
                .OnKill(OnSequenceKill);
        }

        public void ForceStop()
        {
            if (m_sequence != null)
                m_sequence.Kill();

            if (InCooldown)
                return;

            ResetAllLights();
        }

        void OnSequenceKill()
        {
            ResetAllLights();
            m_sequence = null;

            OnEnd?.Invoke();
        }

        void OnSequenceComplete()
        {
            ResetAllLights();
            m_sequence = null;

            if (m_destroyOnEnd)
            {
                Destroy(gameObject);
                return;
            }

            m_inCooldown = true;
            m_cooldownTimer = Timer.Create(UnityEngine.Random.Range(m_cooldownRange.x, m_cooldownRange.y))
                .OnComplete(OnTimerComplete);
            m_cooldownTimer.Restart();

            OnEnd?.Invoke();
        }

        void ResetAllLights()
        {
            foreach (Light light in m_lights)
            {
                ResetLight(light);
            }
        }

        void ResetLight(Light target)
        {
            target.intensity = m_intensityRange.x;
        }

        private void OnTimerComplete(TimerCompletionContext context)
        {
            m_cooldownTimer = null;
            m_inCooldown = false;
        }

        private void OnDestroy()
        {
            m_sequence?.Kill();
            m_cooldownTimer?.Fail();
        }
    }
}
