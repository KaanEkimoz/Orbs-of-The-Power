using com.absence.attributes;
using DG.Tweening;
using TMPro;
using UnityEngine;

namespace com.game.miscs
{
    public class PopupBehaviour : MonoBehaviour
    {
        public const float FADE_OUT_ALPHA = 0f;

        [Header("Utilities")]

        [SerializeField] private GameObject m_container;
        [SerializeField] private CanvasGroup m_group;
        [SerializeField] private TMP_Text m_text;

        [Space, Header("Fade Out Effect Settings")]

        [SerializeField] private Ease m_fadeOutEase;
        [SerializeField] private float m_fadeOutDuration;
        [SerializeField] private float m_fadeOutDelay;

        [Space(5), SerializeField] 
        private bool m_moveUpDuringFadeOut;

        [SerializeField, ShowIf(nameof(m_moveUpDuringFadeOut))]
        private float m_moveUpMagnitude;

        [SerializeField, ShowIf(nameof(m_moveUpDuringFadeOut))]
        private RectTransform m_moveUpTarget;

        [Space, Header("Other Settings")]

        [SerializeField]
        private bool m_autoStartFadeOut = true;

        [SerializeField]
        private bool m_destroyAfterFadeOut = true;

        public bool MoveUpDuringFadeOut
        {
            get
            {
                return m_moveUpDuringFadeOut;
            }

            set
            {
                m_moveUpDuringFadeOut = value;
            }
        }
        public bool AutoStartFadeOut
        {
            get
            {
                return m_autoStartFadeOut;
            }

            set
            {
                m_autoStartFadeOut = value;
            }
        }
        public bool DestroyAfterFadeOut
        {
            get
            {
                return m_destroyAfterFadeOut;
            }

            set 
            {
                m_destroyAfterFadeOut = value;
            }
        }

        Sequence m_sequence;

        private void Start()
        {
            if (!m_autoStartFadeOut)
                return;

            StartFadeOutIfNotStarted();
        }

        public void StartFadeOutIfNotStarted()
        {
            if (m_sequence != null)
                return;

            m_sequence = DOTween.Sequence();

            Tween fadeTween = m_group.DOFade(FADE_OUT_ALPHA, m_fadeOutDuration)
                .SetDelay(m_fadeOutDelay);

            m_sequence.Insert(0, fadeTween);

            if (m_moveUpDuringFadeOut)
            {
                Tween moveUpTween = m_moveUpTarget.DOLocalMoveY(
                    m_moveUpTarget.transform.localPosition.y + m_moveUpMagnitude, m_fadeOutDuration)
                        .SetDelay(m_fadeOutDelay);

                m_sequence.Insert(0, moveUpTween);
            }

            m_sequence.SetEase(m_fadeOutEase)
            .OnComplete(OnFadeOutEnds)
            .OnKill(OnFadeOutEnds);
        }

        private void OnFadeOutEnds()
        {
            if (m_destroyAfterFadeOut)
            {
                if (m_container != null) Destroy(m_container);
                else Destroy(gameObject);
            }
        }

        public void SetText(string text)
        {
            m_text.text = text;
        }
    }
}
