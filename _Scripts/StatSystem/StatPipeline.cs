using com.absence.attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace com.game.statsystem
{
    public abstract class StatPipeline<T> : MonoBehaviour where T : Enum
    {
        [SerializeField, Readonly] private List<StatPipelineComponentBase<T>> m_pipelineComponentList;

        public List<StatPipelineComponentBase<T>> Query => m_pipelineComponentList;

        public float Process(T statType, float rawValue)
        {
            float value = rawValue;
            for (int i = 0; i < m_pipelineComponentList.Count; i++)
            {
                value = m_pipelineComponentList[i].Process(statType, value);
            }

            return value;
        }

        public void OnTestGUI()
        {
            m_pipelineComponentList.ForEach(comp => comp.OnTestGUI());
        }

        [Button("Refresh Pipeline Component List")]
        public void Refresh()
        {
            m_pipelineComponentList = GetComponents<StatPipelineComponentBase<T>>().ToList().
                OrderBy(comp => comp.Order).ToList();

#if UNITY_EDITOR

#endif

            m_pipelineComponentList.ForEach(comp => comp.Initialize());
        }
    }
}
