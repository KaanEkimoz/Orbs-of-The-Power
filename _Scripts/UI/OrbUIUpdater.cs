using com.absence.attributes;
using com.absence.utilities;
using com.game.player;
using DG.Tweening;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace com.game.ui
{
    [DefaultExecutionOrder(100)]
    public class OrbUIUpdater : MonoBehaviour
    {
        public enum Formation
        {
            Circular,
            Horizontal,
        }

        [SerializeField] private bool m_scaleSelectionBorder = true;
        [SerializeField] private Formation m_formation;
        [SerializeField] private Ease m_transitionEase;
        [SerializeField] [Range(0f, 4f)] private float m_scalingFactor;
        [SerializeField] [Range(0f, 2f)] private float m_secondaryScalingFactor;
        [SerializeField] [Min(0f)] private float m_transitionDuration;
        [SerializeField] private Transform m_orbSelectionBorder;
        [SerializeField] private Transform m_contentRoot;
        [SerializeField] private OrbDisplayGP m_prefab;

        [Space]

        [SerializeField, ShowIf(nameof(m_formation), Formation.Circular)] private float m_diameter;
        [SerializeField, ShowIf(nameof(m_formation), Formation.Horizontal)] private HorizontalLayoutGroup m_horizontalLayoutGroup;

        PlayerOrbController m_orbController;
        PlayerOrbContainer m_orbContainer;
        List<OrbDisplayGP> m_orbDisplays;
        RectTransform m_layoutRootRect;
        int m_orbCount;
        float m_stepAngle;
        float m_realScalingFactor;
        float m_prefabWidth;
        int m_middleIndex;
        int m_initialPadding;
        int m_selectedOrbIndex;
        bool m_firstCreation = true;
        bool m_nextFlag;
        bool m_previousFlag;
        Sequence m_arrangementSequence;
        Tween m_virtualTween;

        [Inject]
        void Initialize(PlayerOrbController orbController)
        {
            m_orbController = orbController;
        }

        private void Awake()
        {
            m_prefabWidth = m_prefab.GetComponent<RectTransform>().rect.width;
            m_layoutRootRect = m_horizontalLayoutGroup.GetComponent<RectTransform>();
        }

        private void Start()
        {
            m_orbContainer = Player.Instance.Hub.OrbContainer;

            if (m_scaleSelectionBorder)
            {
                Vector3 selectorScale = m_orbSelectionBorder.transform.localScale;
                selectorScale.x *= 1f + m_scalingFactor;
                selectorScale.y *= 1f + m_scalingFactor;
                m_orbSelectionBorder.transform.localScale = selectorScale;
            }

            SubscribeToEvents();
            FetchVariables();

            OnOrbCountChanged(m_orbCount);
        }

        void SubscribeToEvents()
        {
            m_orbController.OnOrbCountChanged += OnOrbCountChanged;
            m_orbController.OnNextOrbSelected += SelectNextOrb;
            m_orbController.OnPreviousOrbSelected += SelectPreviousOrb;
            m_orbContainer.OnUndoCacheApplied += Refresh;
        }

        private void OnOrbCountChanged(int newCount)
        {
            m_orbCount = newCount;
            CreateOrbDisplays();
        }

        void FetchVariables()
        {
            m_orbCount = Player.Instance.CharacterProfile.OrbCount;
            m_stepAngle = 360f / m_orbCount;
            m_realScalingFactor = m_scalingFactor / m_orbCount;
        }

        void CreateOrbDisplays()
        {
            if (m_orbDisplays == null) m_orbDisplays = new();
            else
            {
                for (int i = 0; i < m_orbDisplays.Count; i++)
                {
                    Destroy(m_orbDisplays[i]);
                }

                m_orbDisplays.Clear();
            }

            switch (m_formation)
            {
                case Formation.Circular:
                    CreateOrbDisplays_Circular();
                    break;
                case Formation.Horizontal:
                    CreateOrbDisplays_Horizontal();
                    break;
                default:
                    break;
            }

            m_orbSelectionBorder.SetAsFirstSibling();

            if (m_firstCreation)
                m_firstCreation = false;

            UpdateArrangement();
            m_orbDisplays[m_middleIndex].SetSelected(true);
        }

        void CreateOrbDisplays_Horizontal()
        {
            m_middleIndex = Mathf.Max(0, (m_orbCount - 1) / 2);
            m_initialPadding = -Mathf.FloorToInt(m_middleIndex * (m_prefabWidth + m_horizontalLayoutGroup.spacing));
            for (int i = 0; i < m_orbCount; i++)
            {
                int realIndex = GetRealIndex(i);

                SimpleOrb orb = m_orbController.orbsOnEllipse[realIndex];

                if (orb == null)
                    continue;

                OrbDisplayGP orbDisplay = Instantiate(m_prefab, m_contentRoot);

                orbDisplay.Initialize(orb, m_orbContainer);

                m_orbDisplays.Add(orbDisplay);
            }
        }

        void CreateOrbDisplays_Circular()
        {
            for (int i = 0; i < m_orbCount; i++)
            {
                SimpleOrb orb = m_orbController.orbsOnEllipse[i];

                if (orb == null)
                    continue;

                float totalAngle = i * m_stepAngle;
                float cos = Mathf.Cos(totalAngle * Mathf.Deg2Rad);
                float sin = Mathf.Sin(totalAngle * Mathf.Deg2Rad);
                Vector2 direction = new Vector2(sin, cos);
                Vector2 position = direction * m_diameter;

                if (i == 0)
                    m_orbSelectionBorder.localPosition = position;

                OrbDisplayGP orbDisplay = Instantiate(m_prefab);
                orbDisplay.transform.SetParent(m_contentRoot, false);
                orbDisplay.transform.localPosition = position;

                orbDisplay.Initialize(orb, m_orbContainer);

                m_orbDisplays.Add(orbDisplay);
            }

            if (m_firstCreation)
            {
                m_selectedOrbIndex = -1;
                SelectNextOrb();
            }
        }

        void SelectNextOrb()
        {
            m_nextFlag = true;
            m_previousFlag = false;
            SelectOrb(m_selectedOrbIndex + 1);
        }

        void SelectPreviousOrb()
        {
            m_nextFlag = false;
            m_previousFlag = true;
            SelectOrb(m_selectedOrbIndex - 1);
        }

        void SelectOrb(int index)
        {
            m_orbDisplays[GetRealIndex(m_selectedOrbIndex - 1)].SetSelected(false);

            m_selectedOrbIndex = index;

            if (m_selectedOrbIndex >= m_orbCount) m_selectedOrbIndex -= m_orbCount;
            else if (m_selectedOrbIndex < 0) m_selectedOrbIndex += m_orbCount;

            m_orbDisplays[GetRealIndex(m_selectedOrbIndex - 1)].SetSelected(true);

            UpdateArrangement();
        }

        public void Refresh()
        {
            m_contentRoot.transform.eulerAngles = Vector3.zero;
            m_contentRoot.DestroyChildren();
            CreateOrbDisplays();

            foreach (OrbDisplayGP display in m_orbDisplays)
            {
                display.Refresh();
            }

            m_nextFlag = false;
            m_previousFlag = false;
            SelectOrb(m_orbController.SelectedOrbIndex);
        }

        public void UpdateArrangement()
        {
            if (m_arrangementSequence != null)
                m_arrangementSequence.Kill();

            m_arrangementSequence = DOTween.Sequence();
            m_arrangementSequence.SetEase(m_transitionEase);
            m_arrangementSequence.OnComplete(OnArrangementSequenceEnds);
            m_arrangementSequence.OnKill(OnArrangementSequenceEnds);

            switch (m_formation)
            {
                case Formation.Circular:
                    UpdateArrangement_Circular();
                    break;
                case Formation.Horizontal:
                    UpdateArrangement_Horizontal();
                    break;
                default:
                    break;
            }
        }

        void UpdateArrangement_Circular()
        {
            float totalAngle = m_stepAngle * m_selectedOrbIndex;
            Vector3 endingRotation = new Vector3(0f, 0f, totalAngle);

            var rotationTween = m_contentRoot.DORotate(endingRotation, m_transitionDuration, RotateMode.Fast);
            m_arrangementSequence.Insert(0f, rotationTween);

            m_orbDisplays.ForEach(display => display.SetRotating(true));
        }

        void UpdateArrangement_Horizontal()
        {
            if (m_virtualTween != null)
                m_virtualTween.Kill();

            for (int i = 0; i < m_orbCount; i++)
            {
                int index = i;

                if (m_nextFlag) index--;
                else if (m_previousFlag) index++;

                float scale = GetScaleForOrb_Horizontal(index);

                m_orbDisplays[i].Image.rectTransform.localScale = new Vector3(scale, scale, scale);

                LayoutRebuilder.ForceRebuildLayoutImmediate(m_layoutRootRect);
            }

            if (m_nextFlag)
                m_contentRoot.GetChild(0).SetAsLastSibling();

            if (m_previousFlag)
                m_contentRoot.GetChild(m_orbCount - 1).SetAsFirstSibling();

            int shift = 0;
            shift += m_nextFlag ? 1 : 0;
            shift += m_previousFlag ? -1 : 0;
            int simulatedPadding = m_initialPadding + Mathf.FloorToInt(shift * (m_prefabWidth + m_horizontalLayoutGroup.spacing));

            m_virtualTween = DOVirtual.Int(simulatedPadding,
                m_initialPadding, m_transitionDuration, (i) =>
            {
                m_horizontalLayoutGroup.padding.left = i;
                LayoutRebuilder.ForceRebuildLayoutImmediate(m_layoutRootRect);
            }).SetEase(m_transitionEase)
            .OnComplete(OnVirtualTweenEnds)
            .OnKill(OnVirtualTweenEnds);

            for (int i = 0; i < m_orbCount; i++)
            {
                float scale = GetScaleForOrb_Horizontal(i);

                Tweener tween = m_orbDisplays[i].Image.rectTransform.DOScale(scale, m_transitionDuration);
                m_arrangementSequence.Insert(0, tween);
            }
        }

        float GetScaleForOrb_Horizontal(int index)
        {
            int realIndex = GetRealIndex(index);

            float scale = 1f;
            int prevIndex = m_selectedOrbIndex - 1;
            int nextIndex = m_selectedOrbIndex + 1;

            if (prevIndex < 0) prevIndex += m_orbCount;
            if (nextIndex >= m_orbCount) nextIndex -= m_orbCount;

            if (realIndex == m_selectedOrbIndex) scale += m_scalingFactor;
            else if (realIndex == prevIndex || realIndex == nextIndex) scale += m_secondaryScalingFactor;

            return scale;
        }

        private void OnVirtualTweenEnds()
        {
            m_virtualTween = null;
            m_horizontalLayoutGroup.padding.left = m_initialPadding;
            LayoutRebuilder.ForceRebuildLayoutImmediate(m_layoutRootRect);
        }

        private void OnArrangementSequenceEnds()
        {
            m_arrangementSequence = null;
            m_orbDisplays.ForEach(display => display.SetRotating(false));
        }

        int GetRealIndex(int index)
        {
            int realIndex = index - m_middleIndex;
            if (realIndex < 0)
                realIndex += m_orbCount;

            return realIndex;
        }
    }
}
