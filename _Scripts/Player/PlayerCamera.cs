using System;
using System.Collections;
using System.Collections.Generic;
using com.absence.timersystem;
using com.game.camera;
using Unity.Cinemachine;
using UnityEngine;

namespace com.game.player
{
    public class PlayerCamera : MonoBehaviour
    {
        public class ShakeProps
        {
            public float Duration;
            public float AmplitudeGain;
            public float FrequencyGain;

            public ShakeProps(float duration, float amplitudeGain, float frequencyGain)
            {
                Duration = duration;
                AmplitudeGain = amplitudeGain;
                FrequencyGain = frequencyGain;
            }
        }

        [SerializeField] private Camera m_camera;
        [SerializeField] private CinemachineBasicMultiChannelPerlin m_noise;

        ShakeProps m_currentShake;
        Queue<ShakeProps> m_shakesInQueue = new();

        float m_timeInQueue;
        ITimer m_shakeTimer;

        private void Awake()
        {
            ClearShake();
        }

        public void Shake(CameraShakeProfile profile, bool enqueueIfBusy = false) 
        {
            Shake(profile.Duration, profile.Amplitude, profile.Frequency);
        }

        public void Shake(float duration, float amplitudeGain, float frequencyGain, bool enqueueIfBusy = false)
        {
            ShakeProps shake = new(duration, amplitudeGain, frequencyGain);

            if (m_currentShake == null)
                StartShake(shake);

            if (!enqueueIfBusy)
                return;

            if (duration > (m_timeInQueue + m_shakeTimer.CurrentTime))
            {
                shake.Duration -= m_shakeTimer.CurrentTime;
                m_timeInQueue += shake.Duration;
                m_shakesInQueue.Enqueue(shake);
            }
        }

        public void ClearShakeQueue()
        {
            m_shakesInQueue.Clear();
        }

        void StartShake(ShakeProps props)
        {
            m_currentShake = props;
            m_timeInQueue = Mathf.Max(0f, m_timeInQueue - props.Duration);

            m_noise.AmplitudeGain = props.AmplitudeGain;
            m_noise.FrequencyGain = props.FrequencyGain;

            m_shakeTimer?.Fail();
            m_shakeTimer = Timer.Create(props.Duration)
                .OnPauseResume(OnShakeTimerPauseResume)
                .OnComplete(OnShakeTimerComplete);
        }

        void ClearShake()
        {
            m_noise.AmplitudeGain = 0f;
            m_noise.FrequencyGain = 0f;
        }

        private void OnShakeTimerPauseResume(bool paused)
        {
            if (paused)
            {
                ClearShake();
            }

            else
            {
                m_noise.AmplitudeGain = m_currentShake.AmplitudeGain;
                m_noise.FrequencyGain = m_currentShake.FrequencyGain;
            }
        }

        private void OnShakeTimerComplete(TimerCompletionContext context)
        {
            m_shakeTimer = null;
            m_currentShake = null;

            ClearShake();

            if (m_shakesInQueue.TryDequeue(out ShakeProps shake))
                StartShake(shake);
        }
    }
}
