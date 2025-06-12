using com.absence.attributes;
using com.absence.timersystem;
using com.game.interactionsystem;
using com.game.utilities;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

namespace com.game.generics
{
    public class ChallengeOnInteract : InteractableBase
    {
        public static float CurrentChallengeDurationLeft { get; protected set; } = float.NaN;

        [SerializeField] private bool m_manualToken = false;
        [SerializeField, Readonly] private bool m_interactable = true;
        [SerializeField, EnableIf(nameof(m_manualToken))] private string m_challengeToken;

        [SerializeField, Min(0f)] private float m_baseChallengeDuration;

        [SerializeField] private TMP_Text m_durationText;
        [SerializeField] private List<SpawnerBase> m_spawners;

        bool m_challengeActive;
        ITimer m_challengeTimer;

        public override bool Interactable
        {
            get
            {
                return m_interactable && (!SceneManager.Instance.AnyChallengeActive);
            }

            set
            {
                m_interactable = value;
            }
        }

        public override string RichInteractionMessage
        {
            get
            {
                return "Starts a challenge.";
            }
        }

        private void Awake()
        {
            m_spawners = m_spawners.Where(spawner => !spawner.ControlledBySceneManager).ToList();
            m_challengeActive = false;

            m_challengeActive = false;
            SetSpawners(false);
            ClearSpawners();

            if (m_durationText != null)
                m_durationText.text = string.Empty;
        }

        public override bool OnInteract(IInteractor interactor)
        {
            if (!interactor.IsPlayer)
                return false;

            bool success = SceneManager.Instance.StartChallenge(m_challengeToken, OnChallengeEnds);

            if (!success)
                return false;

            CommitStartChallenge();

            return true;
        }

        private void OnChallengeEnds(bool success)
        {
            CommitEndChallenge();

            if (success)
                Dispose();
        }

        void SetSpawners(bool active)
        {
            m_spawners.ForEach(spawner => spawner.IsActive = active);
        }

        void ClearSpawners()
        {
            m_spawners.ForEach(spawner => spawner.Clear());
        }

        void CommitStartChallenge()
        {
            if (m_challengeActive)
                return;

            m_challengeActive = true;

            m_challengeTimer = Timer.Create(m_baseChallengeDuration)
                .OnTick(OnChallengeTimerTick)
                .OnComplete(OnChallengeTimerComplete);
        }

        private void OnChallengeTimerTick()
        {
            float time = m_challengeTimer.CurrentTime;

            CurrentChallengeDurationLeft = time;

            if (m_durationText != null)
            {
                string format = time < 10f ? "0.00" : "0";
                m_durationText.text = $"Challenge ends in:\n{m_challengeTimer.CurrentTime.ToString(format)}";
            }
        }

        private void OnChallengeTimerComplete(TimerCompletionContext context)
        {
            m_challengeTimer = null;

            SceneManager.Instance.EndChallenge(m_challengeToken, context.Succeeded);

            if (m_durationText != null)
                m_durationText.text = string.Empty;
        }

        void CommitEndChallenge()
        {
            if (!m_challengeActive)
                return;

            m_challengeActive = false;
            m_challengeTimer?.Fail();

            CurrentChallengeDurationLeft = float.NaN;
        }

        private void Reset()
        {
            m_challengeToken = System.Guid.NewGuid().ToString();
        }
    }
}
