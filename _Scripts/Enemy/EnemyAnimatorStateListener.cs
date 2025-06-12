using com.absence.attributes;
using System;
using UnityEngine;

namespace com.game.enemysystem
{
    [RequireComponent(typeof(Animator))]
    public class EnemyAnimatorStateListener : MonoBehaviour
    {
        public class Context
        {
            public Animator Animator;
            public AnimatorStateInfo StateInfo;
            public int LayerIndex;

            public Context(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
            {
                Animator = animator;
                StateInfo = stateInfo;
                LayerIndex = layerIndex;
            }
        }

        [SerializeField, Readonly] private Animator m_animator;

        public event Action<AnimatorStateStage, AnimatorStateEvent, Context> OnEventRaised;

        public void RaiseEvent(AnimatorStateStage stage, AnimatorStateEvent @event, Context context)
        {
            OnEventRaised?.Invoke(stage, @event, context);
        }

        private void Reset()
        {
            m_animator = GetComponent<Animator>();
        }
    }
}
