using com.game.enemysystem;
using com.game.generics;
using com.game.player;
using System.Collections;
using TMPro;
using UnityEngine;
using Zenject;

namespace com.game
{
    public class LifestealOrb : SimpleOrb, IElemental
    {
        [Header("Life Steal Settings")]
        [SerializeField] private float maxStealableHealthPerEnemy = 100f;
        [SerializeField] private float stealInterval = 2f;
        [SerializeField] private float healthPerSteal = 10f;

        [Header("Visual Effects")]
        [SerializeField] private GameObject instantStealEffect;
        [SerializeField] private FollowTarget continuousStealEffect;
        [SerializeField] private FollowTarget continuousStealEffectForPlayer;

        private IRenderedDamageable currentTarget = null;
        private float totalStolenHealth = 0;
        private Coroutine stealCoroutine = null;
        private FollowTarget activeEffect = null;
        private FollowTarget activePlayerEffect = null;

        [Inject] private PlayerCombatant _playerCombatant;

        private void OnEnable()
        {
            OnCalled += StopStealFromEnemy;
            if (_playerCombatant == null)
                ProvidePlayerCombatant(Player.Instance.Hub.Combatant);
        }

        private void OnDisable()
        {
            OnCalled -= StopStealFromEnemy;
            CleanUpEffects();
        }

        public void ProvidePlayerCombatant(PlayerCombatant playerCombatant)
        {
            _playerCombatant = playerCombatant;
        }

        protected override void ApplyCombatEffects(IDamageable damageable, float damage, bool penetrationCompleted, bool recall)
        {
            base.ApplyCombatEffects(damageable, damage, penetrationCompleted, recall);

            if (ShouldSkipEffects(recall, penetrationCompleted))
                return;

            CreateInstantEffect(transform.position);

            if (currentTarget == null && damageable is IRenderedDamageable enemy)
                StartStealFromEnemy(enemy);
        }

        private bool ShouldSkipEffects(bool recall, bool penetrationCompleted)
        {
            return recall || (m_latestDamageEvt.CausedDeath && !penetrationCompleted);
        }

        private void StartStealFromEnemy(IRenderedDamageable enemy)
        {
            currentTarget = enemy;
            totalStolenHealth = 0f;

            if (stealCoroutine != null)
                StopCoroutine(stealCoroutine);

            stealCoroutine = StartCoroutine(StealHealthRoutine());
            CreateContinuousEffect(enemy);
        }

        private IEnumerator StealHealthRoutine()
        {
            while (currentTarget != null && totalStolenHealth < maxStealableHealthPerEnemy)
            {
                yield return new WaitForSeconds(stealInterval);

                if (currentTarget != null)
                {
                    float actualSteal = Mathf.Min(healthPerSteal, maxStealableHealthPerEnemy - totalStolenHealth);
                    currentTarget.TakeDamage(actualSteal, out DamageEvent evt);

                    if (evt.CausedDeath)
                    {
                        StopStealFromEnemy();
                        yield break;
                    }

                    _playerCombatant.Heal(actualSteal);
                    totalStolenHealth += actualSteal;

                    // Efekt pozisyonunu güncelle
                    if (activeEffect != null)
                        activeEffect.transform.position = currentTarget.Renderer.bounds.center;
                }
            }
            StopStealFromEnemy();
        }

        private void CreateInstantEffect(Vector3 position)
        {
            if (instantStealEffect != null)
            {
                GameObject effect = Instantiate(instantStealEffect, position, Quaternion.identity);
                StartCoroutine(DestroyEffectAfterDelay(effect, 0.5f));
            }
        }

        private void CreateContinuousEffect(IRenderedDamageable target)
        {
            CleanUpEffects();
            if (continuousStealEffect != null)
            {
                activeEffect = Instantiate(continuousStealEffect, target.transform.position, Quaternion.identity);
                activeEffect.Target = target.transform;
                activeEffect.KeepStartingOffset = true;
            }
            if (continuousStealEffectForPlayer != null)
            {
                activePlayerEffect = Instantiate(continuousStealEffectForPlayer, Player.Instance.transform.position, Quaternion.identity);
                activePlayerEffect.Target = Player.Instance.transform;
                activePlayerEffect.KeepStartingOffset = true;
            }
        }

        private void CleanUpEffects()
        {
            if (activePlayerEffect != null)
            {
                Destroy(activePlayerEffect.gameObject);
                activePlayerEffect = null;
            }

            if (activeEffect != null)
            {
                Destroy(activeEffect.gameObject);
                activeEffect = null;
            }
        }

        private IEnumerator DestroyEffectAfterDelay(GameObject effect, float delay)
        {
            yield return new WaitForSeconds(delay);
            if (effect != null)
                Destroy(effect);
        }

        private void StopStealFromEnemy()
        {
            currentTarget = null;
            if (stealCoroutine != null)
            {
                StopCoroutine(stealCoroutine);
                stealCoroutine = null;
            }
            CleanUpEffects();
        }

#if UNITY_EDITOR
        private void OnDrawGizmosSelected()
        {
            if (currentTarget != null)
            {
                Gizmos.color = Color.red;
                Gizmos.DrawLine(transform.position, currentTarget.Renderer.bounds.center);
            }
        }
#endif
    }
}