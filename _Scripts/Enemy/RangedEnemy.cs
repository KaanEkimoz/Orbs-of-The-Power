using com.game.enemysystem.statsystemextensions;
using System.Collections;
using UnityEngine;

namespace com.game.enemysystem
{
    public class RangedEnemy : Enemy
    {
        public GameObject projectilePrebfab;

        [Header("Shooting")]
        public float shootingCooldown = 3f;
        public Transform firePoint;
        [Header("Projectile")]
        public float projectileSpeed = 5f;
        [Header("Raycast")]
        public float visionError = 20f;
        public float rayDistance = 20f;
        public GameObject raySource;
        [SerializeField] LayerMask layerMask;
        [SerializeField] bool canSeePlayer;

        public override bool IsAILocked 
        { 
            get
            {
                return IsAttacking && (!canMoveDuringAttack);
            }
        }

        private bool canShoot = true;

        private ObjectPool _objectPool;
        protected override void Start()
        {
            base.Start();
            _objectPool = FindFirstObjectByType<ObjectPool>();
        }
        protected override void CustomUpdate()
        {
            if (isDummyModeActive)
                return;

            if (IsAttacking)
            {
                if (!canMoveDuringAttack)
                    return;
            }

            canSeePlayer = CanSeePlayer();

            if ((!IsFake) && GetDistanceToPlayer() && canSeePlayer)
                Shoot();
        }

        private bool CanSeePlayer()
        {
            Vector3 rayDirection = transform.TransformDirection(Vector3.forward);
            Debug.DrawRay(raySource.transform.position, rayDirection * rayDistance, Color.blue); //ray'i default olarak mavi ciz

            if (Physics.SphereCast(raySource.transform.position, visionError, rayDirection, out RaycastHit hitInfo, rayDistance, layerMask)) //ray bir seye carpti mi?
            {
                bool isPlayer = hitInfo.collider.CompareTag("Player"); //carpilan nesne player mi?

                Debug.DrawRay(raySource.transform.position, rayDirection * rayDistance, isPlayer ? Color.green : Color.red); //ray'in carptigi nesne player'sa yesil, degilse kirmizi ciz

                return isPlayer;
            }

            return false;
        }

        private void Shoot()
        {
            if (!canShoot)
                return;

            if (IsAttacking)
                return;

            canShoot = false;

            if (hasAttackAnimation)
                IsAttacking = true;
            else
                SpawnProjectile();

            StartCoroutine(EnableCooldown());
        }

        private EnemyProjectile CreateProjectile()
        {
            EnemyProjectile projectile = _objectPool.GetPooledObject(6).GetComponent<EnemyProjectile>();
            projectile.transform.position = firePoint.transform.position;
            projectile.SetDamage(enemyStats.GetStat(EnemyStatType.Damage));

            return projectile;
        }

        public void SpawnProjectile()
        {
            Vector3 shootingDirection = (target.transform.position - firePoint.position);
            shootingDirection.y = 0f;
            shootingDirection = shootingDirection.normalized;

            EnemyProjectile projectile = CreateProjectile();
            projectile.ApplyVelocity(shootingDirection * projectileSpeed);
        }

        private IEnumerator EnableCooldown()
        {
            yield return new WaitForSeconds(shootingCooldown);
            canShoot = true;
        }
    }

}
