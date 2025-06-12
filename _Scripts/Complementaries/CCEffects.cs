using com.absence.attributes;
using com.game.enemysystem;
using com.game.utilities;
using System;
using UnityEngine;
using UnityEngine.Events;

namespace com.game.complementaries
{
    public class CCEffects : MonoBehaviour
    {
        [SerializeField, Required] private EnemyCombatant m_target;
        [SerializeField] private CCSource m_includeSources;
        [SerializeField] private CCBreakFlags m_includeBreakFlags;

        public UnityEvent<float> onSlow;
        //public UnityEvent<CCBreakFlags> onStun;

        private void Start()
        {
            //m_target.OnStun += OnStun;
            m_target.OnSlow += OnSlow;
        }

        private void OnSlow(float percentage, float duration, CCSource source, CCBreakFlags flags)
        {
            if (!m_includeSources.HasFlag(source))
                return;

            if (!m_includeBreakFlags.HasFlag(flags))
                return;

            onSlow.Invoke(duration);
        }
    }
}
