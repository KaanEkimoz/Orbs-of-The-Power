using com.absence.attributes;
using com.game.abilitysystem.gamedependent;
using System;
using UnityEngine;

namespace com.game.abilitysystem
{
    public abstract class AbilityBehaviour : MonoBehaviour, IRuntimeAbility
    {
        [HelpBox("Start the game to see any runtime data.", HelpBoxType.Info)]

        [SerializeField, Runtime, Readonly] protected RuntimeAbilityState m_state;
        [SerializeField, Runtime, Readonly] protected string m_guid;
        [SerializeField, Runtime, Readonly] protected float m_durationTimer;
        [SerializeField, Runtime, Readonly] protected float m_cooldownTimer;
        [SerializeField, Runtime, Readonly] protected int m_currentStack;
        [SerializeField, Runtime, Readonly] protected bool m_canUse;

        protected float m_totalDuration;
        protected float m_totalCooldown;
        protected int m_maxStack;

        public string Guid => m_guid;

        public float Duration => m_totalDuration;
        public float Cooldown => m_totalCooldown;
        public int MaxStack => m_maxStack;

        public float DurationLeft => m_durationTimer;
        public float CooldownLeft => m_cooldownTimer;
        public int CurrentStack => m_currentStack;

        public RuntimeAbilityState State => m_state;
        public bool ReadyToUse => m_canUse && m_state == RuntimeAbilityState.ReadyToUse;
        public bool InUse => m_state == RuntimeAbilityState.InUse;
        public bool InCooldown => m_state == RuntimeAbilityState.InCooldown;

        public event Action<bool> OnUseAction = delegate {};

        private void Start()
        {
            
        }

        private void Update()
        {
            OnUpdate();
        }

        public void Initialize(AbilityProfile profile)
        {
            m_guid = profile.Guid;
            m_totalDuration = profile.DefaultDuration;
            m_totalCooldown = profile.DefaultCooldown;
            m_maxStack = profile.IsStackable ? profile.DefaultMaxStack : 1;
        }

        public void Use(AbilityUseContext context)
        {
            OnUse(context);
        }

        public virtual bool CanUse(AbilityUseContext context)
        {
            if (InCooldown) return false;

            return true;
        }

        protected virtual void OnUse(AbilityUseContext context)
        {

        }

        protected virtual void OnStartUsing(AbilityUseContext context)
        {

        }

        protected virtual void OnStopUsing(AbilityUseContext context)
        {

        }

        protected virtual void OnUpdate()
        {

        }

        public abstract string GenerateDescription();
    }
}
