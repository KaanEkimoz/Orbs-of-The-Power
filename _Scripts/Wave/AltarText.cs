using com.absence.attributes;
using com.absence.timersystem;
using TMPro;
using UnityEngine;

namespace com.game.altarsystem
{
    public class AltarText : MonoBehaviour
    {
        [SerializeField, Required] private GameObject m_panel;
        [SerializeField, Required] private TMP_Text m_informationText;
        [SerializeField, Required] private TMP_Text m_durationText;
        //[SerializeField, Required] private CanvasGroup m_canvasGroup;
        [SerializeField] private string m_infoDuringForcedWave;
        [SerializeField] private string m_infoDuringForcedWavePaused;
        [SerializeField] private string m_infoDuringRestOfWave;
        [SerializeField] private string m_infoDuringSafeZone;
        [SerializeField] private string m_safeText;
        [SerializeField] private string m_nonSafeText;
        [SerializeField] private string m_challengeActiveNonSafeText;

        private void Update()
        {
            ITimer timer = SceneManager.Instance.ForcedWaveTimer;

            if (timer != null)
            {
                float time = timer.CurrentTime;
                bool paused = timer.IsPaused;
                string format = time < 10f ? "0.00" : "0";

                if (paused) m_informationText.text = m_infoDuringForcedWavePaused;
                else m_informationText.text = m_infoDuringForcedWave;

                if (paused) m_durationText.text = $"<color=grey>{time.ToString(format)}s</color>";
                else m_durationText.text = $"{time.ToString(format)}s";

                return;
            }

            if (GameManager.Instance.State == GameState.InWave)
            {
                m_informationText.text = m_infoDuringRestOfWave;

                if (SceneManager.Instance.AnyChallengeActive) m_durationText.text = m_challengeActiveNonSafeText;
                else m_durationText.text = m_nonSafeText;

                return;
            }

            if (GameManager.Instance.State == GameState.BetweenWaves)
            {
                m_informationText.text = m_infoDuringSafeZone;
                m_durationText.text = m_safeText;

                return;
            }

            m_informationText.text = string.Empty;
            m_durationText.text = string.Empty;
        }
    }
}

