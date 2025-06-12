using com.absence.attributes;
using System;
using UnityEngine;

namespace com.game.player
{
    public class PlayerParanoiaLogic : MonoBehaviour
    {
        [SerializeField, Range(0f, 1f)] float m_currentPercentage;
        [SerializeField, Readonly] int m_currentSegment;

        public float TotalPercentage01 => m_currentPercentage;
        public float TotalPercentage => m_currentPercentage * 100f;
        public int SegmentIndex => m_currentSegment;
        public int Segment => m_currentSegment + 1;

        public event Action OnParanoiaSegmentChange = null;

        public bool Increase(float percentage01)
        {
            if (percentage01 < 0f)
                return Decrease(-percentage01);

            return SetToPercentage(m_currentPercentage + percentage01);
        }

        public bool Decrease(float percentage01)
        {
            if (percentage01 < 0f)
                return Increase(-percentage01);

            return SetToPercentage(m_currentPercentage - percentage01);
        }

        public bool Modify(float percentage01)
        {
            return SetToPercentage(m_currentPercentage + percentage01);
        }

        public bool SetToSegment(int segment, bool asIndex = false)
        {
            int max = Constants.Paranoia.PARANOIA_SEGMENT_COUNT;

            if (asIndex) 
                segment++;

            if (segment < 0 || segment > max)
                return false;

            return SetToPercentage(segment / (float)max);
        }

        public bool SetToPercentage(float percentage01)
        {
            bool result = true;

            if (percentage01 < 0f)
            {
                percentage01 = 0f;
                result = false;
            }

            else if (percentage01 > 1f)
            {
                percentage01 = 1f;
                result = false;
            }

            m_currentPercentage = percentage01;
            FetchSegment();

            return result;
        }

        void FetchSegment()
        {
            DoFetchSegment(m_currentPercentage, Constants.Paranoia.PARANOIA_SEGMENT_COUNT);
        }

        void DoFetchSegment(float percentage, int max)
        {
            int previousSegment = m_currentSegment;
            m_currentSegment = Mathf.FloorToInt(percentage * max);

            if (previousSegment != m_currentSegment)
                OnParanoiaSegmentChange?.Invoke();
        }

        private void OnValidate()
        {
            FetchSegment();
        }
    }
}
