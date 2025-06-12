using com.game;
using com.game.orbsystem.statsystemextensions;
using com.game.player;
using com.game.player.statsystemextensions;
using System;
using System.Collections;
using UnityEngine;
using com.absence.soundsystem;
using com.absence.soundsystem.internals;
using com.absence.attributes.experimental;
using com.game.orbsystem;
using com.game.utilities;
using com.absence.attributes;
using System.Collections.Generic;
using com.game.miscs;
using com.game.enemysystem;
using com.absence.timersystem;
using UnityEngine.Splines;
public enum OrbState
{
    OnEllipse,
    Sticked,
    Throwing,
    Returning
}
[RequireComponent(typeof(Rigidbody), typeof(SphereCollider), typeof(MeshRenderer))]
public class SimpleOrb : MonoBehaviour, IGatherable
{
    public OrbState currentState = OrbState.OnEllipse;

    [Header("Orb Movement")]
    [SerializeField, InlineEditor] protected OrbMovementData m_movementData;
    [SerializeField] private AnimationCurve movementCurve = AnimationCurve.EaseInOut(0f,0f,1f,1f);
    [SerializeField] private float maxDistance = 20f;
    [Space]
    [Header("Orb Stats and Combat")]
    [SerializeField] private OrbStats orbStats;
    [SerializeField, InlineEditor] private OrbCombatData m_combatData;
    [Space]
    [Header("Orb Material")]
    [SerializeField] private Material startMaterial;
    [Space]
    [Header("Components")]
    [SerializeField] private Rigidbody _rigidBody;
    [SerializeField] private SphereCollider _sphereCollider;
    [SerializeField] private Renderer _renderer;
    [Space]
    [Header("Orb Effects")]
    [SerializeField] private float onEllipseLifetime = 0.1f;
    [SerializeField] private float normalLifetime = 0.5f;
    [SerializeField] private GameObject m_light;
    [SerializeField] private GameObject m_selectionIndicator;
    [SerializeField] private GameObject m_hitFX;
    [SerializeField] private ParticleSystem trailParticle;
    [SerializeField, InlineEditor, Required] private OrbCombatEffectData m_combatEffectData;
    [Space]
    [Header("Sound Asset References")]
    [SerializeField] private SoundAsset m_throwSoundAsset;
    [SerializeField] private SoundAsset m_recallSoundAsset;
    [SerializeField] private SoundAsset m_catchSoundAsset;
    [SerializeField] private SoundAsset m_hitSoundAsset;
    

    public Func<Vector3, OrbState, Vector3> targetPositionPostProcessor;

    //Movement
    private Transform startParent;
    private Vector3 startScale;
    private Vector3 currentTargetPos;
    private bool hasReachedTargetPos = false;
    private const float ellipseReachThreshold = 0.4f;
    private Vector3 lastStickNormal;
    private Transform stickedTransform;
    private Collider stickedCollider;

    //Throw
    private float distanceTraveled;
    private float timeTravelledReturning;
    private float m_penetrationExcessDamage;
    private float m_destickLerpTime01 = 0f;
    private float m_destickLerpFactor = 1f;
    private Vector3 throwStartPosition;
    private Vector3 throwVector;
    private int penetrationCount = 0;

    //Stats
    public OrbStats Stats => orbStats;
    private PlayerStats _playerStats;

    public Transform StickedTransform => stickedTransform;
    public float penetrationExcessDamage => m_penetrationExcessDamage;
    public Rigidbody Rigidbody => _rigidBody;

    public bool InCooldown => m_useCooldown && m_inCooldown;
    public float CooldownTimer => m_cooldownTimer.CurrentTime;
    public float MaxCooldown => m_maxCooldown;

    //Events
    public event Action OnThrown;
    public event Action OnCalled;
    public event Action OnStuck;
    public event Action OnReachedToEllipse;
    public event Action<OrbState> OnStateChanged;
    public event Action OnPenetrateHit;
    public event Action OnPhysicsHit;
    public event Action<SimpleOrb> OnThrowAnimationEndOneShot;
    public event Action<SimpleOrb> OnCallDemanded;
    //Effects
    private SoundFXManager _soundFXManager;
    protected Vector3 m_secondaryInterestDirection;
    protected float m_secondaryInterestMagnitude;
    protected Vector3 m_latestVelocity;
    protected DamageEvent m_latestDamageEvt;

    List<IDamageable> m_penetratedEnemies = new();
    float m_internalRecallSpeedMultiplier;
    float m_maxCooldown;
    float m_cooldownDamageMultiplier;
    float m_cooldownStepdownOnReturnHit;
    bool m_bypassKnockback;
    bool m_inThrowAnimation = false;
    bool m_lastStickWasAnEnemy = false;
    protected bool m_pullFlag = false;
    SplineContainer m_throwAnimationSpline;

    public SplineContainer ThrowSpline => m_throwAnimationSpline;

    public float ThrowDamage => orbStats.GetStat(OrbStatType.Damage) + 
        _playerStats.GetStat(PlayerStatType.Damage) + 
        _playerStats.GetStat(PlayerStatType.OrbThrowDamage);

    public float RecallDamage => orbStats.GetStat(OrbStatType.Damage) +
        _playerStats.GetStat(PlayerStatType.Damage) +
        _playerStats.GetStat(PlayerStatType.OrbRecallDamage);

    public bool IsGatherable => (currentState == OrbState.Sticked) && (stickedCollider == null);

    bool m_useCooldown;
    bool m_inCooldown;
    ITimer m_cooldownTimer;

    public void AssignPlayerStats(PlayerStats playerStats)
    {
        _playerStats = playerStats;
    }
    public void AssingSoundFXManager(SoundFXManager soundFXManager)
    {
        _soundFXManager = soundFXManager;
    }
    private void Reset()
    {
        currentState = OrbState.OnEllipse;
        if (m_light != null) m_light.SetActive(false);
        trailParticle.startLifetime = onEllipseLifetime;
        CheckStartVariables();
    }
    void PlaySFX(ISoundAsset asset)
    {
        if (SoundManager.Instance == null)
            return;

        Sound.Create(asset)
            .AtPosition(transform.position)
            .Play();
    }
    private void CheckStartVariables()
    {
        if(_rigidBody == null)
            _rigidBody = GetComponent<Rigidbody>();
        if(_sphereCollider == null)
            _sphereCollider = GetComponent<SphereCollider>();
        if(_renderer == null)
            _renderer = GetComponent<MeshRenderer>();
        if (orbStats == null)
            orbStats = GetComponent<OrbStats>();

        startParent = transform.parent;
        startScale = transform.localScale;

        m_useCooldown = m_combatData.useCooldown;
        m_maxCooldown = m_combatData.baseCooldown;
        m_cooldownDamageMultiplier = m_combatData.cooldownDamageMultiplier;
        m_cooldownStepdownOnReturnHit = m_combatData.cooldownStepdownOnReturnHit;
        m_inCooldown = false;
    }
    protected virtual void Awake()
    {
        //m_movementData = Instantiate(m_movementData);
        SetSelected(false);
    }
    private void Start()
    {
        CheckStartVariables();

        OnThrown += () => PlaySFX(m_throwSoundAsset);
        OnCalled += () => PlaySFX(m_recallSoundAsset);
        OnReachedToEllipse += () => PlaySFX(m_catchSoundAsset);
        OnPhysicsHit += () => PlaySFX(m_hitSoundAsset);
        OnPenetrateHit += () => PlaySFX(m_hitSoundAsset);


        if (m_light != null) m_light.SetActive(false);
        trailParticle.startLifetime = onEllipseLifetime;
    }
    protected virtual void Update()
    {
        HandleStateBehaviours();
    }
    private void FixedUpdate()
    {
        if (currentState == OrbState.Throwing)
        {
            m_latestVelocity = _rigidBody.linearVelocity;

            float speedFromStats = (((orbStats.GetStat(OrbStatType.Speed) / 10) + 1) *
                ((_playerStats.GetStat(PlayerStatType.OrbThrowSpeed) / 10)) + 1);

            _rigidBody.linearVelocity = m_movementData.movementSpeed * m_movementData.throwSpeedMultiplier * speedFromStats * Time.deltaTime * throwVector;
        }
    }
    private void HandleStateBehaviours()
    {
        if (currentState == OrbState.Sticked && targetPositionPostProcessor != null)
            transform.position = transform.position = targetPositionPostProcessor.Invoke(transform.position, currentState);
        if (currentState == OrbState.Returning || currentState == OrbState.OnEllipse)
            MoveToTargetPosition();
        if (currentState == OrbState.Throwing)
        {
            CalculateDistanceTraveled();
            if (distanceTraveled >= maxDistance + _playerStats.GetStat(PlayerStatType.Range))
            {
                StickToTransform(startParent);
                stickedCollider = null;
                stickedTransform = null;
                m_destickLerpTime01 = 1f;
                m_destickLerpFactor = 1f;
            }
        }
        if (currentState == OrbState.Returning && hasReachedTargetPos)
        {
            if (m_light != null) m_light.SetActive(false);
            trailParticle.startLifetime = onEllipseLifetime;
            currentState = OrbState.OnEllipse;
            m_internalRecallSpeedMultiplier = 1f;
            transform.parent = startParent;

            m_destickLerpFactor = 1f;
            m_destickLerpTime01 = 0f;
            timeTravelledReturning = 0f;
            penetrationCount = 0;
            m_penetrationExcessDamage = 0f;
            m_penetratedEnemies.Clear();

            OnReachedToEllipse?.Invoke();
            OnStateChanged?.Invoke(currentState);
        }
    }
    public void Disable()
    {
        gameObject.SetActive(false);
    }
    public void ReturnToPosition(Vector3 returnPosition, float speedCoefficient = 1f)
    {
        if (currentState == OrbState.Sticked)
        {
            if (m_lastStickWasAnEnemy)
            {
                if (m_combatEffectData.callFromEnemyShakeProfile != null)
                    Player.Instance.Hub.Camera.Shake(m_combatEffectData.callFromEnemyShakeProfile);
            }

            if (stickedCollider != null)
            {
                m_bypassKnockback = true;
                m_pullFlag = true;
                ApplyOrbReturnTriggerEffects(stickedCollider);
                m_pullFlag = false;
                m_bypassKnockback = false;
            }

            if (stickedCollider != null)
            {
                if (stickedCollider.TryGetComponent(out IOrbStickTarget stickable))
                    stickable.CommitOrbUnstick(this);

                if (m_combatEffectData.returnKnockback && stickedCollider.TryGetComponent(out IKnockbackable knockbackable))
                    knockbackable.Knockback((returnPosition - transform.position).normalized,
                        m_combatEffectData.returnKnockbackStrength, KnockbackSourceUsage.Final);
            }

            stickedCollider = null;
            stickedTransform = null;

            m_destickLerpTime01 = 0f;
            m_destickLerpFactor = 0f;
        }

        currentState = OrbState.Returning;

        SetNewDestination(returnPosition, speedCoefficient);
        ResetParent();
        OnCalled?.Invoke();
        OnStateChanged?.Invoke(currentState);
    }
    public void ResetParent()
    {
        transform.SetParent(startParent);
        transform.localScale = startScale;
    }
    public void SetSelected(bool value)
    {
        if (m_selectionIndicator != null) 
            m_selectionIndicator.SetActive(value);
    }
    public void Throw(Vector3 forceDirection)
    {
        currentState = OrbState.Throwing;

        // Distance Calculation
        throwStartPosition = transform.position;
        distanceTraveled = 0;

        if (m_light != null) m_light.SetActive(true);

        throwVector = forceDirection;

        trailParticle.startLifetime = normalLifetime;
        _rigidBody.isKinematic = false;
        
        OnThrown?.Invoke();
        OnStateChanged?.Invoke(currentState);
    }
    private void CalculateDistanceTraveled()
    {
        distanceTraveled = Vector3.Distance(throwStartPosition, transform.position);
    }
    public void SetNewDestination(Vector3 newPos)
    {
        if (targetPositionPostProcessor != null)
        {
            currentTargetPos = targetPositionPostProcessor.Invoke(newPos, currentState);
        }

        else
        {
            currentTargetPos = newPos;
        }
    }
    public void SetNewDestination(Vector3 newPos, float multiplier)
    {
        SetNewDestination(newPos);
        m_internalRecallSpeedMultiplier = multiplier;
    }
    private void MoveToTargetPosition()
    {
        Vector3 targetPosition = currentTargetPos;
        float distanceToTarget = Vector3.Distance(transform.position, targetPosition);

        // AnimationCurve adjustments
        float dynamicMaxDistance = Mathf.Max(maxDistance + _playerStats.GetStat(PlayerStatType.Range), distanceToTarget + 10f);
        float curveValue = movementCurve.Evaluate(1 - (distanceToTarget / dynamicMaxDistance));
        float currentSpeed = m_movementData.movementSpeed * curveValue;

        if (currentState == OrbState.Returning)
        {
            currentSpeed *= m_movementData.recallSpeedMultiplier * m_internalRecallSpeedMultiplier
                * ((orbStats.GetStat(OrbStatType.Speed) / 10) + 1) * ((_playerStats.GetStat(PlayerStatType.OrbRecallSpeed) / 10) + 1);

            float maxCoefficientOverTimeTravelled = m_movementData.recallSpeedMaxCoefficientOverTimeTravelled;
            float intendedCoefficientOverTimeTravelled = Mathf.Pow(m_movementData.recallSpeedTimeTravelledBase, timeTravelledReturning * m_movementData.recallSpeedCoefficientOverTimeTravelled);

            currentSpeed *= Mathf.Min(maxCoefficientOverTimeTravelled, intendedCoefficientOverTimeTravelled);

            timeTravelledReturning += Time.deltaTime;
            m_destickLerpTime01 += Time.deltaTime / m_movementData.destickDuration;
            m_destickLerpFactor = m_movementData.destickCurve.Evaluate(m_destickLerpTime01);

            if (m_lastStickWasAnEnemy) 
                targetPosition = Vector3.Lerp(targetPosition, lastStickNormal * distanceToTarget * m_movementData.destickStrength,
                m_destickLerpFactor);
        }

        else if (currentState == OrbState.OnEllipse)
        {
            currentSpeed *= m_movementData.onEllipseSpeedMutliplier * (distanceToTarget * m_movementData.onEllipseDistanceFactor);
        }

        // MoveTowards to the target

        transform.position = Vector3.MoveTowards(transform.position, targetPosition, currentSpeed * Time.deltaTime);

        hasReachedTargetPos = distanceToTarget < ellipseReachThreshold;
    }

    private void OnTriggerEnter(Collider triggerObject)
    {
        if (currentState == OrbState.Throwing)
        {
            if (triggerObject.CompareTag("IgnoreOrbThrow"))
                return;

            Game.Event = com.game.GameRuntimeEvent.OrbThrow;

            ApplyOrbThrowCollisionEffects(triggerObject);

            Game.Event = com.game.GameRuntimeEvent.Null;
        }

        else if (currentState == OrbState.Returning)
        {
            if (triggerObject.CompareTag("IgnoreOrbThrow"))
                return;

            Game.Event = com.game.GameRuntimeEvent.OrbCall;

            if (stickedCollider == null || stickedCollider != triggerObject) 
                ApplyOrbReturnTriggerEffects(triggerObject);

            Game.Event = com.game.GameRuntimeEvent.Null;
        }

        else
        {
            if (!InternalSettings.PLAYER_ORB_AUTO_PICKUP)
                return;

            if (triggerObject.TryGetComponent(out IGatherer gatherer))
                TryGather(gatherer);
        }
    }
    private void Stick(Collider stickCollider)
    {
        currentState = OrbState.Sticked;
        stickedCollider = stickCollider;

        Vector3 closestContact = stickCollider.ClosestPointOnBounds(transform.position);
        lastStickNormal = -m_latestVelocity.normalized; // !!!

        OnPhysicsHit?.Invoke();

        if (stickCollider.TryGetComponent(out IOrbStickTarget stickable))
            stickable.CommitOrbStick(this);

        _rigidBody.isKinematic = true;
        transform.position = closestContact;
        StickToTransform(stickCollider.transform);
    }
    
    protected virtual void ApplyOrbThrowCollisionEffects(Collider collisionObject)
    {
        if (collisionObject.gameObject.TryGetComponent(out IDamageable damageable))
        {
            m_lastStickWasAnEnemy = true;

            float maxPenetrationCount = _playerStats.GetStat(PlayerStatType.Penetration);
            bool penetrationCompleted = penetrationCount >= maxPenetrationCount;
            bool alreadyPenetrated = m_penetratedEnemies.Contains(damageable);

            if (penetrationCompleted)
            {
                Stick(collisionObject);
            }

            else
            {
                if (alreadyPenetrated)
                {
                    return;
                }

                else
                {
                    m_penetratedEnemies.Add(damageable);
                }
            }

            DoApplyCombatEffects(damageable, ThrowDamage + m_penetrationExcessDamage, penetrationCompleted, false);

            if (m_hitFX != null)
            {
                Vector3 closestContact = collisionObject.ClosestPointOnBounds(transform.position);
                GameObject fx = Instantiate(m_hitFX, closestContact, Quaternion.identity);

                Vector3 direction = (transform.position - damageable.transform.position);
                direction.y = 0f;
                direction.Normalize();

                fx.transform.forward = direction;
            }

            if (!m_bypassKnockback)
            {
                if (penetrationCompleted && m_combatEffectData.throwKnockback)
                {
                    if (collisionObject.gameObject.TryGetComponent(out IKnockbackable knockbackable))
                        knockbackable.Knockback(m_latestVelocity.normalized, m_combatEffectData.throwKnockbackStrength, KnockbackSourceUsage.Final);
                }
            }

            if (penetrationCompleted && collisionObject.gameObject.TryGetComponent(out ISlowable slowable))
            {
                slowable.SlowForSeconds(m_combatEffectData.throwSlowAmount, m_combatEffectData.throwSlowDuration);
            }

            if (penetrationCompleted)
            {
                m_cooldownTimer?.Fail();
                m_inCooldown = true;
                m_cooldownTimer = Timer.Create(m_maxCooldown)
                    .OnComplete(OnCooldownTimerComplete);
            }

            if (m_combatEffectData.hitEnemyShakeProfile != null)
                Player.Instance.Hub.Camera.Shake(m_combatEffectData.hitEnemyShakeProfile);

            if (m_latestDamageEvt.CausedDeath)
                penetrationCount++;
            else if (damageable.IsAlive)
                Stick(collisionObject);
        }
        else
        {
            m_lastStickWasAnEnemy = false;
            Stick(collisionObject);
        }

    }
    protected virtual void ApplyOrbReturnTriggerEffects(Collider triggerCollider)
    {
        if (triggerCollider.gameObject.TryGetComponent(out IDamageable damageable))
        {
            DoApplyCombatEffects(damageable, RecallDamage, false, true);
            
            if (!m_bypassKnockback)
            {
                if (m_combatEffectData.returnKnockback)
                {
                    if (triggerCollider.gameObject.TryGetComponent(out IKnockbackable knockbackable))
                        knockbackable.Knockback(_rigidBody.linearVelocity, m_combatEffectData.returnKnockbackStrength, KnockbackSourceUsage.Final);
                }
            }

            if (triggerCollider.gameObject.TryGetComponent(out ISlowable slowable))
            {
                slowable.SlowForSeconds(m_combatEffectData.returnSlowAmount, m_combatEffectData.returnSlowDuration);
            }

            if (m_inCooldown && (!m_pullFlag))
                m_cooldownTimer.Shrink(m_cooldownStepdownOnReturnHit);
        }
    }
    void DoApplyCombatEffects(IDamageable damageableObject, float damage, bool penetrationCompleted, bool recall)
    {
        if (InCooldown)
            damage *= m_cooldownDamageMultiplier;

        damageableObject.TakeDamage(damage, out DamageEvent evt);
        m_latestDamageEvt = evt;

        if (evt.CausedDeath)
            return;

        if (!recall)
            m_penetrationExcessDamage += (evt.DamageSent - evt.DamageDealt);

        if (InCooldown)
            return;

        ApplyCombatEffects(damageableObject, damage, penetrationCompleted, recall);

        if (!m_useCooldown)
            return;

        if (recall)
            return;

        m_cooldownTimer?.Fail();
        m_inCooldown = true;

        m_cooldownTimer = Timer.Create(m_maxCooldown)
            .OnComplete(OnCooldownTimerComplete);
    }

    private void OnCooldownTimerComplete(TimerCompletionContext context)
    {
        m_cooldownTimer = null;
        
        if (context.Succeeded)
            m_inCooldown = false;
    }

    protected virtual void ApplyCombatEffects(IDamageable damageableObject, float damage, bool penetrationCompleted, bool recall)
    {

    }

    private void StickToTransform(Transform stickTransform)
    {
        currentState = OrbState.Sticked;
        _rigidBody.isKinematic = true;
        transform.SetParent(stickTransform);
        stickedTransform = stickTransform;

        penetrationCount = 0;
        m_penetrationExcessDamage = 0f;
        m_penetratedEnemies.Clear();

        OnStuck?.Invoke();
        OnStateChanged?.Invoke(currentState);
    }

    public bool TryGather(IGatherer sender)
    {
        if (!IsGatherable)
            return false;

        if (!sender.IsPlayer)
            return false;

        DemandCall();

        return true;
    }

    protected void DemandCall()
    {
        OnCallDemanded?.Invoke(this);
    }

    public void SubscribeToTargetPositionPostProcessing(Func<Vector3, OrbState, Vector3> postProcessor)
    {
        targetPositionPostProcessor = postProcessor;
    }


    public void CommitThrowStartWithFirePoint(Vector3 firePoint, SplineContainer spline)
    {
        m_throwAnimationSpline = spline;
        targetPositionPostProcessor?.Invoke(firePoint, OrbState.Throwing);
        m_inThrowAnimation = true;
    }

    public void CommitThrowAnimationEnd()
    {
        if (!m_inThrowAnimation)
            return;

        m_throwAnimationSpline = null;
        m_inThrowAnimation = false;
        OnThrowAnimationEndOneShot?.Invoke(this);
        OnThrowAnimationEndOneShot = null;
    }

    public void Unstick(IOrbStickTarget target)
    {
        m_lastStickWasAnEnemy = false;
        stickedCollider = null;
        stickedTransform = null;
    }
}
