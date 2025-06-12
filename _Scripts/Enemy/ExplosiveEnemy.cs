using com.game.enemysystem.statsystemextensions;
using System;
using System.Collections;
using UnityEngine;

namespace com.game.enemysystem
{
    public class ExploderEnemy : Enemy
    {
        [Header("Explosion Settings")]
        public float explosionDamage = 30f;
        public float preperationTime = 2f;
        public float explosionRadius = 5f;
        public MeshRenderer explosiveEnemyRenderer;
        public Color explotionColor = Color.white;
        public Color defaultColor = Color.red;

        private bool isPreparingToExplode = false;
        private Coroutine preparationCoroutine = null;

        protected override void CustomUpdate()
        {
            if (isDummyModeActive) 
                return;

            if (IsFake)
                return;

            if (GetDistanceToPlayer()) //dusman karaktere yeterince yakinsa
            {
                if (isPreparingToExplode == false) //ve halihazirda patlamaya hazirlanmiyorsa
                {
                    preparationCoroutine = StartCoroutine(PrepareToExplode()); //patlamaya hazirlan
                }
            }
            else //dusman karaktere yeterince yakin degilse
            {
                if (isPreparingToExplode == true) //ve halihazirda patlamaya hazirlaniyorsa
                {
                    CancelPreparation(); //hazirligi iptal et
                }
            }
        }
        private IEnumerator PrepareToExplode()
        {
            isPreparingToExplode = true;
            float elapsedTime = 0f;

            explosiveEnemyRenderer.material.color = explotionColor;

            while (elapsedTime < preperationTime) //patlama suresi dolmadigi surece
            {
                if (!GetDistanceToPlayer()) //karakter dusmandan uzaklasirsa
                {
                    isPreparingToExplode = false; //patlama hazirligini iptal et
                    explosiveEnemyRenderer.material.color = defaultColor;
                    yield break;
                }

                elapsedTime += Time.deltaTime; //gecen zaman         
                yield return null;
            }

            Explode();
        }

        private void Explode()
        {
            Collider[] colliders = Physics.OverlapSphere(transform.position, explosionRadius);

            foreach (Collider objectWithinRadius in colliders) //patlama yaricapi icersindeki tum colliderlarda
            {
                if (objectWithinRadius.CompareTag("Player")) //player tagli biri varsa
                {
                    //HASAR VER
                    if (target.gameObject.TryGetComponent(out IDamageable damageable))
                        damageable.TakeDamage(enemyStats.GetStat(EnemyStatType.Damage));
                }
            }

            SimpleOrb[] orbsOnEnemy = GetComponentsInChildren<SimpleOrb>();

            foreach (SimpleOrb orb in orbsOnEnemy)
            {
                orb.SetNewDestination(new Vector3(orb.transform.position.x, 0, orb.transform.position.z));
                orb.ResetParent();
            }

            SendDeathRequest(DeathCause.Self);
        }

        private void CancelPreparation()
        {
            StopCoroutine(preparationCoroutine);
            preparationCoroutine = null;
            isPreparingToExplode = false;
            explosiveEnemyRenderer.material.color = defaultColor;
        }
    }

}