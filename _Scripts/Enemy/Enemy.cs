using com.absence.attributes;
using com.absence.variablesystem.builtin;
using com.game.enemysystem.ai;
using com.game.enemysystem.statsystemextensions;
using DG.Tweening;
using System;
using System.Collections;
using UnityEngine;

namespace com.game.enemysystem
{
    public class Enemy : MonoBehaviour
    {
        public static void DoFakify(Enemy target)
        {
            target.Fakify();
        }

        public static void DoVirtualize(Enemy target)
        {
            target.IsVirtual = true;
            target.enabled = false;
        }

        [Header("Movement")]
        [SerializeField] protected EnemyAIBuitlInScriptSelector enemyAISelector;
        [SerializeField] public bool hasAttackAnimation;
        [SerializeField] protected bool canMoveDuringAttack;
        [Header("Stats")]
        [SerializeField] protected EnemyStats enemyStats;

        //Dummy Mode
        [Header("Dummy Mode")]
        [SerializeField] protected bool isDummyModeActive = false;
        public bool IsFake { get; set; } = false;
        public bool IsVirtual { get; protected set; } = false;
        public bool IsAttacking { get; protected set; } = false;
        public IEnemyAI AI => ai;
        public virtual bool IsAILocked
        {
            get
            {
                return aiLocked;
            }

            set
            {
                aiLocked = value;
            }
        }

        public event Action<DeathCause> OnDeathRequest;

        //AI
        protected IEnemyAI ai;
        protected GameObject target;
        bool aiLocked;

        //Movement
        private float defaultSpeed;

        //Slow
        [Runtime] public FloatVariable moveSpeedModifier { get; set; } = 0;

        protected virtual void Start()
        {
            if (isDummyModeActive)
                return;

            if (target == null)
                target = GameObject.FindWithTag("Player");

            if (enemyStats == null)
                GetComponent<EnemyStats>();

            ai = enemyAISelector.Current;
            ai.Initialize(target.transform);
            ai.Speed = defaultSpeed + enemyStats.GetStat(EnemyStatType.WalkSpeed);
        }

        protected virtual void Update()
        {
            if (isDummyModeActive || target == null)
                return;

            ai.Speed =
                (ai.DefaultSpeed + enemyStats.GetStat(EnemyStatType.WalkSpeed)) *
                (1 + moveSpeedModifier.Value);

            if (!IsAILocked)
                ai.Refresh(target.transform);

            CustomUpdate();
        }

        protected virtual void CustomUpdate() { }
        protected virtual void Fakify() { }

        public void ApplyKnockbackForce(Vector3 orbPosition, float knocbackForce)
        {
            Vector3 forceDirection = new Vector3(transform.position.x - orbPosition.x, 0, transform.position.z - orbPosition.z).normalized;
            Vector3 force = forceDirection * knocbackForce;
            transform.DOMove(transform.position + force, 0.4f).SetEase(Ease.OutBack);
        }
        private IEnumerator StopKnockback()
        {
            yield return new WaitForSeconds(0.2f);
            //navMeshAgent.isStopped = false;
        }

        protected bool GetDistanceToPlayer()
        {
            return Vector3.Distance(transform.position, target.transform.position) <= ai.MovementData.stoppingDistance;
        }

        public void CommitEndOfAttack()
        {
            IsAttacking = false;
        }

        public void SendDeathRequest(DeathCause cause)
        {
            OnDeathRequest?.Invoke(cause);
        }
        
    }


}