using System.Collections.Generic;
using com.game.generics;
using UnityEngine;

namespace com.game
{
    public class ElectricOrb : SimpleOrb, IElemental
    {
        public const int MAX_DETECTABLE_COLLIDERS = 8;

        [SerializeField, Min(0f)] private int m_electricMaxBounceCount = 3;
        [SerializeField, Min(0f)] private float m_electricBounceRadius = 15f;
        [SerializeField, Min(0f)] private float m_electricBounceIntervalInSeconds = 1f;
        [SerializeField, Min(0f)] private float m_electricDamage = 1f;
        [SerializeField, Min(0f)] private float m_electricLineLifetime = 0.2f;
        [SerializeField] private LayerMask m_electricBounceLayerMask;

        [SerializeField] private GameObject m_electricInstantEffectPrefab;
        [SerializeField] private FollowTarget m_electricConstantEffectPrefab;
        [SerializeField] private ElectricLine m_electricChainEffectPrefab;

        float m_localBounceTimer;
        int m_bounceCount;
        IRenderedDamageable m_anchor;
        bool m_bouncing;

        FollowTarget m_currentConstantEffect;
        Collider[] m_nearbyPossibleAnchors;
        List<IRenderedDamageable> m_pastAnchors;

        protected override void Awake()
        {
            base.Awake();

            m_nearbyPossibleAnchors = new Collider[MAX_DETECTABLE_COLLIDERS];
        }

        protected override void Update()
        {
            base.Update();

            if (!m_bouncing)
                return;

            m_localBounceTimer -= Time.deltaTime;

            if (m_localBounceTimer <= 0f)
            {
                Bounce();
                ResetLocalTimer();
            }
        }

        protected override void ApplyCombatEffects(IDamageable damageableObject, float damage, bool penetrationCompleted, bool recall)
        {
            base.ApplyCombatEffects(damageableObject, damage, penetrationCompleted, recall);

            if (recall)
                return;

            if (m_latestDamageEvt.CausedDeath && !penetrationCompleted)
                return;

            if (damageableObject is not IRenderedDamageable renderedDamageable)
                return;

            StartBouncing(renderedDamageable);
        }

        void StartBouncing(IRenderedDamageable firstAnchor)
        {
            m_pastAnchors = new(m_bounceCount)
            {
                firstAnchor
            };

            m_bouncing = true;
            m_anchor = firstAnchor;
            m_bounceCount = 0;

            ApplyEffects(null, firstAnchor);
            ResetLocalTimer();
        }

        void StopBouncing()
        {
            if (m_anchor != null) 
                ApplyEffects(m_anchor, null);

            m_bouncing = false;
        }

        void Bounce()
        {
            m_bounceCount++;

            if (m_bounceCount > m_electricMaxBounceCount)
            {
                StopBouncing();
                return;
            }

            IRenderedDamageable newAnchor = GetClosestDamageable();
            IRenderedDamageable oldAnchor = m_anchor;

            if (oldAnchor != null && m_currentConstantEffect != null)
            {
                Destroy(m_currentConstantEffect.gameObject);
                m_currentConstantEffect = null;
            }

            m_anchor = newAnchor;

            OnBounce(newAnchor);
            ApplyEffects(oldAnchor, newAnchor);
            ResetLocalTimer();

            if (newAnchor == null)
                StopBouncing();

            if (newAnchor != null)
                m_pastAnchors.Add(newAnchor);
        }

        void ResetLocalTimer()
        {
            m_localBounceTimer = m_electricBounceIntervalInSeconds;
        }

        void ApplyEffects(IRenderedDamageable from, IRenderedDamageable to)
        {
            bool firstOrLast = from == null || to == null;

            if (!firstOrLast)
            {
                CreateElectricLineEffect(from.Renderer.bounds.center, to.Renderer.bounds.center);
            }

            if (from != null) 
            {
                if (m_currentConstantEffect != null)
                {
                    Destroy(m_currentConstantEffect.gameObject);
                    m_currentConstantEffect = null;
                }
            }

            if (to != null)
            {
                CreateElectricInstantEffect(to.Renderer.bounds.center);

                if (m_electricConstantEffectPrefab != null)
                {
                    FollowTarget effect = Instantiate(m_electricConstantEffectPrefab, to.transform.position, Quaternion.identity);
                    effect.Target = to.transform;
                    effect.KeepStartingOffset = true;
                    to.OnDie += (_) =>
                    {
                        try
                        {
                            if (effect == m_currentConstantEffect) m_currentConstantEffect = null;
                            Destroy(effect.gameObject);
                        }

                        catch
                        {

                        }
                    };

                    m_currentConstantEffect = effect;
                }
            }
        }

        void CreateElectricInstantEffect(Vector3 at)
        {
            Instantiate(m_electricInstantEffectPrefab, at, Quaternion.identity);
        }

        void CreateElectricLineEffect(Vector3 from, Vector3 to)
        {
            ElectricLine lineInstance = Instantiate(m_electricChainEffectPrefab, from, Quaternion.identity);
            lineInstance.pointAposition = from;
            lineInstance.pointBposition = to;

            Destroy(lineInstance.gameObject, m_electricLineLifetime);
        }

        void OnBounce(IRenderedDamageable anchor)
        {
            anchor?.TakeDamage(m_electricDamage);
        }

        IRenderedDamageable GetClosestDamageable()
        {
            Vector3 anchorOrigin = (m_anchor != null && m_anchor.IsAlive) ?
                m_anchor.Renderer.bounds.center : transform.position;

            Physics.OverlapSphereNonAlloc(anchorOrigin, m_electricBounceRadius, m_nearbyPossibleAnchors, m_electricBounceLayerMask);

            float lastDistance = float.MaxValue;
            IRenderedDamageable result = null;
            foreach (Collider possibleAnchor in m_nearbyPossibleAnchors)
            {
                if (possibleAnchor == null)
                    continue;

                if (!possibleAnchor.gameObject.TryGetComponent(out IRenderedDamageable renderedDamageable))
                    continue;

                if (m_anchor != null && m_pastAnchors.Contains(renderedDamageable))
                    continue;

                float localDistance = Vector3.Distance(anchorOrigin, possibleAnchor.transform.position);

                if (localDistance < lastDistance)
                {
                    lastDistance = localDistance;
                    result = renderedDamageable;
                }
            }

            return result;
        }

#if UNITY_EDITOR

        private void OnDrawGizmos()
        {
            // Draw the slow radius in the editor

            if (currentState != OrbState.OnEllipse)
            {
                Gizmos.color = Color.yellow;
                Gizmos.DrawSphere(transform.position, m_electricBounceRadius);
            }
        }

#endif

    }
}
