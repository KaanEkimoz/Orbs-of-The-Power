using com.game.generics;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace com.game
{
    public class SoulOrb : SimpleOrb, IElemental
    {
        [Header("Soul Seal Settings")]
        [SerializeField, Range(1, 10)] private int maxSeals = 3;
        [SerializeField, Range(0.1f, 2f)] private float sealedDamageMultiplier = 0.8f;
        [SerializeField] private float sealDuration = 15f;

        [Header("Visual Effects")]
        [SerializeField] private GameObject instantSealEffectPrefab;
        [SerializeField] private GameObject continousSealEffectPrefab;

        private static List<IRenderedDamageable> sealedEnemies = new List<IRenderedDamageable>();

        protected override void ApplyCombatEffects(IDamageable damageable, float damage, bool penetrationCompleted, bool recall)
        {
            base.ApplyCombatEffects(damageable, damage, penetrationCompleted, recall);

            if (m_latestDamageEvt.CausedDeath && !penetrationCompleted)
                return;

            if (damageable is not IRenderedDamageable renderedDamageable)
                return;

            if (!renderedDamageable.IsAlive)
                return;

            if ((!recall) || m_pullFlag)
                DamageAllSealedEnemies(damage);

            if (recall)
                return;

            if (!sealedEnemies.Contains(renderedDamageable))
            {
                if (sealedEnemies.Count >= maxSeals)
                    sealedEnemies.RemoveAt(0);

                sealedEnemies.Add(renderedDamageable);

                StartCoroutine(RemoveSealAfterDelay(renderedDamageable));

                if (instantSealEffectPrefab != null)
                    CreateInstantSealEffect(renderedDamageable);

                if (continousSealEffectPrefab != null)
                    CreateContinousSealEffect(renderedDamageable);
            }
        }

        private IEnumerator RemoveSealAfterDelay(IRenderedDamageable enemy)
        {
            yield return new WaitForSeconds(sealDuration);

            if (enemy != null && enemy.IsAlive) 
                sealedEnemies.Remove(enemy);
        }
        private void CreateInstantSealEffect(IRenderedDamageable enemy)
        {
            GameObject effect = Instantiate(instantSealEffectPrefab, enemy.Renderer.bounds.center, Quaternion.identity);
            StartCoroutine(DestroyEffect(effect, sealDuration));
        }
        private void CreateContinousSealEffect(IRenderedDamageable enemy)
        {
            GameObject effect = Instantiate(continousSealEffectPrefab, enemy.transform.position, Quaternion.identity);

            var follow = effect.AddComponent<FollowTarget>();
            enemy.OnDie += (_) =>
            {
                if (effect != null)
                    Destroy(effect);
            };
            follow.Target = enemy.transform;
            follow.KeepStartingOffset = true;

            StartCoroutine(DestroyEffect(effect, sealDuration));
        }

        private IEnumerator DestroyEffect(GameObject effect, float delay)
        {
            yield return new WaitForSeconds(delay);

                if (effect != null)
                    Destroy(effect);
        }

        private void DamageAllSealedEnemies(float baseDamage)
        {
            foreach (var enemy in sealedEnemies)
            {
                if (enemy != null && enemy.IsAlive)
                    enemy.TakeDamage(baseDamage * sealedDamageMultiplier);
            }
        }

    }
}