using com.absence.attributes;
using System;
using System.Text;
using UnityEngine;

namespace com.game.enemysystem
{
    public class EnemyAnimator : MonoBehaviour
    {
        public static readonly int IDLE_HASH = Animator.StringToHash("Idle");
        public static readonly int WALK_HASH = Animator.StringToHash("Walk");
        public static readonly int RUN_HASH = Animator.StringToHash("Run");
        public static readonly int ATTACK_HASH = Animator.StringToHash("Attack");

        public static int GetAnimationVariantHash(string baseName, int variant)
        {
            StringBuilder sb = new(baseName);
            sb.Append(variant.ToString());

            return Animator.StringToHash(sb.ToString());
        }

        [SerializeField, Required] private Animator m_animator;
        [SerializeField] private float m_normalizedTransitionDurations = 0.15f;

        int m_hash;

        public void SetStateByHash(int hash)
        {
            m_animator.CrossFade(hash, m_normalizedTransitionDurations);
            m_hash = hash;
        }   

        public int GetHash()
        {
            return m_hash;
        }
    }
}
