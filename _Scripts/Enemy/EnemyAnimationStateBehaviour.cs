using UnityEngine;

namespace com.game.enemysystem
{
    public class EnemyAnimationStateBehaviour : StateMachineBehaviour
    {
        [SerializeField] private AnimatorStateEvent m_event;

        public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            if (animator.TryGetComponent(out EnemyAnimatorStateListener listener))
                listener.RaiseEvent(AnimatorStateStage.Enter, m_event,
                    new EnemyAnimatorStateListener.Context(animator, stateInfo, layerIndex));
        }

        public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            Debug.Log("agfj");
            if (animator.TryGetComponent(out EnemyAnimatorStateListener listener))
                listener.RaiseEvent(AnimatorStateStage.Exit, m_event,
                    new EnemyAnimatorStateListener.Context(animator, stateInfo, layerIndex));
        }
    }
}
