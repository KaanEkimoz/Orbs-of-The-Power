using System.Collections;
using UnityEngine;
using com.game.enemysystem.statsystemextensions;
using com.absence.attributes;

namespace com.game.enemysystem
{
    public class MeleeEnemy : Enemy
    {
        [Header("Melee Enemy")]
        public Transform attackingPoint;
        [HideIf(nameof(attackingPoint), null)] public LayerMask attackLayerMask;
        [HideIf(nameof(attackingPoint), null)] public float attackRadius;

        public float meleeAttackDelay = 2f;
        public float attackingCooldown = 2f;
        private bool canAttack = true;

        public override bool IsAILocked
        {
            get
            {
                return IsAttacking && (!canMoveDuringAttack);
            }
        }

        bool insideDamagingFrames;
        bool damageDone;

        protected override void CustomUpdate()
        {
            if (isDummyModeActive)
                return;

            if (IsAttacking)
            {
                if (insideDamagingFrames)
                    DealDamage();

                if (!canMoveDuringAttack)
                    return;
            }

            if (IsFake)
                return;

            if (GetDistanceToPlayer())
            {
                Attack();
            }
        }

        private void Attack()
        {
            if (!canAttack)
                return;

            if (IsAttacking)
                return;

            if (hasAttackAnimation)
                IsAttacking = true;

            canAttack = false;

            StartCoroutine(EnableCooldown());
        }

        public void DealDamage()
        {
            if (insideDamagingFrames && damageDone)
                return;

            if (attackingPoint == null)
            {
                DamageTarget(target.gameObject);
            }
            else
            {
                Collider[] result = Physics.OverlapSphere(attackingPoint.position, attackRadius,
                    attackLayerMask, QueryTriggerInteraction.UseGlobal);

                foreach (Collider coll in result)
                {
                    if (coll == null)
                        continue;

                    if (coll.attachedRigidbody == null) DamageTarget(coll.gameObject);
                    else DamageTarget(coll.attachedRigidbody.gameObject);
                }
            }
        }

        void DamageTarget(GameObject t)
        {
            if (t.TryGetComponent(out IDamageable damageable))
            {
                damageable.TakeDamage(enemyStats.GetStat(EnemyStatType.Damage));

                if (insideDamagingFrames)
                    damageDone = true;
            }
        }

        public void EnterDamagingFrames()
        {
            if (!IsAttacking)
                return;

            insideDamagingFrames = true;
            damageDone = false;
        }

        public void ExitDamagingFrames()
        {
            insideDamagingFrames = false;
            damageDone = false;
        }

        private IEnumerator EnableCooldown()
        {
            yield return new WaitForSeconds(attackingCooldown);
            canAttack = true;
        }

        private void OnDrawGizmosSelected()
        {
            if (attackingPoint == null)
                return;

            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(attackingPoint.position, attackRadius);
        }
    }

}