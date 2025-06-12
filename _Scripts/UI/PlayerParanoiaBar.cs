using com.game.player;
using UnityEngine;

namespace com.game.ui
{
    public class PlayerParanoiaBar : MonoBehaviour
    {
        [SerializeField] private RectTransform m_content;
        [SerializeField] private ParanoiaBarSegment m_segmentPrefab; // type

        PlayerParanoiaLogic m_logic;
        ParanoiaBarSegment[] m_segments;

        private void Start()
        {
            m_logic = Player.Instance.Hub.Paranoia;
            Initialize();
        }

        private void Update()
        {
            Refresh();
        }

        private void Initialize()
        {
            int count = Constants.Paranoia.PARANOIA_SEGMENT_COUNT;
            m_segments = new ParanoiaBarSegment[count];
            for (int i = 0; i < count; i++)
            {
                ParanoiaBarSegment segment = Instantiate(m_segmentPrefab, m_content);
                m_segments[i] = segment;
            }
        }

        public void Refresh()
        {
            int count = Constants.Paranoia.PARANOIA_SEGMENT_COUNT;
            float totalPercentage = m_logic.TotalPercentage01;
            float total = totalPercentage * count;

            for (int i = 0; i < count; i++)
            {
                m_segments[i].SetFillAmount(total - (float)i);
            }
        }
    }
}
