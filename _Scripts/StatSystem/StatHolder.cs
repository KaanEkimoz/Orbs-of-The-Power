using com.absence.attributes;
using com.absence.utilities;
using com.absence.variablesystem.builtin;
using com.absence.variablesystem;
using com.absence.variablesystem.mutations;
using com.game.statsystem.extensions;
using com.game.statsystem.presetobjects;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using com.absence.variablesystem.mutations.internals;

namespace com.game.statsystem
{
    /// <summary>
    /// The class responsible for anything related to stats.
    /// </summary>
    /// <typeparam name="T">The enum to use while selecting stats.</typeparam>
    [System.Serializable]
    public abstract class StatHolder<T> : IStatManipulator<T>, IStatHolder<T> where T : Enum
    {
        [HelpBox("You need to start the game to see any data.", HelpBoxType.Info)]
        [SerializeField] private List<FloatVariable> m_stats;
        [SerializeField] private Dictionary<T, FloatVariable> m_entries;
        [SerializeField] List<T> m_enumValues;

        public StatHolder()
        {
            Initialize();
        }

        public StatHolder(DefaultStats<T> defaultValues)
        {
            Initialize();
            SetDefaultValues(defaultValues);
        }

        protected virtual void Initialize()
        {
            m_enumValues = new();
            m_stats = new();
            m_entries = new();

            foreach (T enumValue in Enum.GetValues(typeof(T)))
            {
                m_enumValues.Add(enumValue);

                FloatVariable variable = CreateStatVariable(enumValue);

                m_stats.Add(variable);
                m_entries.Add(enumValue, variable);
            }
        }

        private void SetDefaultValues(DefaultStats<T> defaultValues)
        {
            if (defaultValues.Length != m_enumValues.Count) 
                throw new Exception("There is a mismatch between the length DefaultStats and corresponding enum values. Please go and refresh the targeted DefaultStats ScriptableObject in the editor.");

            foreach (T enumValue in m_enumValues)
            {
                if (defaultValues.TryGetDefaultValue(enumValue, out float value)) 
                    m_entries[enumValue].SetUnderlyingValueWithoutCallbacks(value);
            }
        }
        private FloatVariable CreateStatVariable(T targetStat)
        {
            string rawLabel = Enum.GetName(typeof(T), targetStat);

            return new FloatVariable(0f);
        }

        /*
         Old API
         */

        #region Basic Operations

        /// <summary>
        /// Use to increment a stat variable WITHOUT creating a modifier.
        /// </summary>
        /// <param name="targetStat">The stat to increment.</param>
        /// <param name="amount">Amount of incrementation.</param>
        /// <param name="setType">Selection of the 'set' logic.</param>
        /// <returns>Returns false if something goes wrong, true otherwise.</returns>
        protected virtual bool BasicIncrement(T targetStat, float amount)
        {
            if (!TryGetDesiredStatVariable(targetStat, out FloatVariable desiredStatVariable)) return false;

            desiredStatVariable.UnderlyingValue += amount;
            return true;
        }

        /// <summary>
        /// Use to multiply a stat variable WITHOUT creating a modifier.
        /// </summary>
        /// <param name="targetStat">The stat to multiply.</param>
        /// <param name="multiplier">Amount of multiplication.</param>
        /// <param name="setType">Selection of the 'set' logic.</param>
        /// <returns>Returns false if something goes wrong, true otherwise.</returns>
        protected virtual bool BasicMultiply(T targetStat, float multiplier)
        {
            if (!TryGetDesiredStatVariable(targetStat, out FloatVariable desiredStatVariable)) return false;

            desiredStatVariable.UnderlyingValue *= multiplier;
            return true;
        }

        /// <summary>
        /// Use to override a stat variable WITHOUT creating a modifier.
        /// </summary>
        /// <param name="targetStat">The stat to multiply.</param>
        /// <param name="newValue">Intended new value of the stat variable.</param>
        /// <param name="setType">Selection of the 'set' logic.</param>
        /// <param name="clearMutations">If true, the modifiers applied to the
        /// target variable of this process will all get cleared before overriding
        /// the variable value itself.</param>
        /// <returns>Returns false if something goes wrong, true otherwise.</returns>
        protected virtual bool BasicOverride(T targetStat, float newValue, bool clearMutations = true)
        {
            if (!TryGetDesiredStatVariable(targetStat, out FloatVariable desiredStatVariable)) return false;

            if (clearMutations) ClearModifiersOf(targetStat);
            desiredStatVariable.UnderlyingValue = newValue;
            return true;
        }

        #endregion

        #region Vulnerable API

        /// <summary>
        /// <b>[VULNERABLE]</b> Use to get the corresponding variable object of a stat.
        /// </summary>
        /// <param name="targetStat">The target enumeration value corresponds to a variable object.</param>
        /// <returns>Returns the desired variable if the enumeration entry is valid. Returns null otherwise.</returns>
        private FloatVariable GetDesiredStatVariable(T targetStat)
        {
            if (!m_entries.TryGetValue(targetStat, out FloatVariable value))
            {
                Debug.LogError("An error occurred determining a stat's desired variable. " +
                    "It's entry may not exist.");

                return null;
            }

            return value;
        }

        /// <summary>
        /// <b>[VULNERABLE]</b> Use to check-n-get a corresponding variable object of a stat.
        /// </summary>
        /// <param name="targetStat">The target enumeration value corresponds to a variable object.</param>
        /// <param name="desiredStatVariable">The variable object received from the getting operation. 
        /// Null if the checking operation fails.</param>
        /// <returns>Returns true if the checking operation succeeds. False otherwise.</returns>
        private bool TryGetDesiredStatVariable(T targetStat, out FloatVariable desiredStatVariable)
        {
            desiredStatVariable = GetDesiredStatVariable(targetStat);
            if (desiredStatVariable != null) return true;
            else return false;
        }

        #endregion

        /*
         Public API
         */

        #region Base

        public float GetStat(T targetStat)
        {
            return GetDesiredStatVariable(targetStat).Value;
        }
        public bool TryGetStat(T targetStat, out float value)
        {
            value = 0f;

            if (!TryGetDesiredStatVariable(targetStat, out FloatVariable targetVariable))
                return false;

            value = targetVariable.Value;
            return true;
        }

        #endregion

        #region Helpers

        public virtual void ForAllStatValues(Action<float> action)
        {
            m_stats.ConvertAll(stat => stat.Value).ForEach(action);
        }
        public virtual void ForAllStatEntries(Action<T, float> action)
        {
            m_entries.ToList().ForEach(kvp => action?.Invoke(kvp.Key, kvp.Value.Value));
        }

        #endregion

        #region Utilities

        public virtual void RefreshAll()
        {
            m_stats.ForEach(stat => stat.Refresh());
        }
        public virtual void ClearAllModifiers()
        {
            m_stats.ForEach(stat => stat.ClearMutations());
        }
        public virtual bool Refresh(T targetStat)
        {
            if (!TryGetDesiredStatVariable(targetStat, out FloatVariable desiredStatVariable)) return false;

            desiredStatVariable.Refresh();
            return true;
        }
        public virtual bool ClearModifiersOf(T targetStat)
        {
            if (!TryGetDesiredStatVariable(targetStat, out FloatVariable desiredStatVariable)) return false;

            desiredStatVariable.ClearMutations();
            return true;
        }

        #endregion

        #region Modifiers with Preset Objects

        //public virtual ModifierObject<T> ApplyPreset(StatModification<T> mod)
        //{
        //    return ModifyWith(mod);
        //}

        //public virtual ModifierObject<T> ApplyPreset(StatCap<T> cap)
        //{
        //    return CapWith(cap);
        //}

        //public virtual float ApplyPreset(StatOverride<T> ovr)
        //{
        //    return OverrideWith(ovr);
        //}

        public virtual ModifierObject<T> ModifyWith(StatModification<T> mod)
        {
            ModifierObject<T> result = null;

            if (mod.ModificationType == StatModificationType.Incremental)
            {
                result = ModifyIncremental(mod.TargetStatType, mod.Value, AffectionMethod.InOrder);
            }

            else if (mod.ModificationType == StatModificationType.Percentage)
            {
                bool onTop = StatSystemSettings.PERCENTAGE_MODS_ON_TOP;
                AffectionMethod affectionMethod = onTop ? AffectionMethod.Overall : AffectionMethod.InOrder;

                result = ModifyPercentage(mod.TargetStatType, mod.Value, affectionMethod);
            }

            return result;
        }
        public virtual ModifierObject<T> CapWith(StatCap<T> cap)
        {
            FloatCapMutation result = new FloatCapMutation()
            {
                CapLow = cap.CapLow,
                CapHigh = cap.CapHigh,
                MinValue = cap.MinValue,
                MaxValue = cap.MaxValue,
            };

            ModifyCustom(cap.TargetStatType, result);

            return new ModifierObject<T>(cap.TargetStatType, result);
        }
        public virtual float OverrideWith(StatOverride<T> ovr)
        {
            if (!TryGetDesiredStatVariable(ovr.TargetStatType, out FloatVariable targetVariable))
                throw new Exception("Invalid enum value.");

            bool clear = StatSystemSettings.OVERRIDES_CLEAR_MUTATIONS;
            float newValue = ovr.NewValue;

            if (clear) ClearModifiersOf(ovr.TargetStatType);

            targetVariable.UnderlyingValue = newValue;
            return targetVariable.Value;
        }

        #endregion

        #region Modifiers with Values

        public virtual ModifierObject<T> ModifyCustom(T targetStat, Mutation<float> mutationObject)
        {
            if (!TryGetDesiredStatVariable(targetStat, out FloatVariable desiredStatVariable)) return null;

            desiredStatVariable.Mutate(mutationObject);
            return new ModifierObject<T>(targetStat, mutationObject);
        }
        public virtual ModifierObject<T> ModifyIncremental(T targetStat, float amount, AffectionMethod affectionMethod = AffectionMethod.InOrder)
        {
            if (!TryGetDesiredStatVariable(targetStat, out FloatVariable desiredStatVariable)) return null;

            FloatAdditionMutation mutationObject = new(amount, affectionMethod);

            desiredStatVariable.Mutate(mutationObject);
            return new ModifierObject<T>(targetStat, mutationObject);
        }
        public virtual ModifierObject<T> ModifyPercentage(T targetStat, float percentage, AffectionMethod affectionMethod = AffectionMethod.InOrder)
        {
            if (StatSystemSettings.PERCENTAGE_MODS_ON_TOP) affectionMethod = AffectionMethod.Overall;

            if (!TryGetDesiredStatVariable(targetStat, out FloatVariable desiredStatVariable)) return null;

            float realPercentage = 1f + (percentage / 100f);
            FloatPercentageMutation mutationObject = new(percentage, affectionMethod);

            desiredStatVariable.Mutate(mutationObject);
            return new ModifierObject<T>(targetStat, mutationObject);
        }
        public void Demodify(ModifierObject<T> modifierObject)
        {
            if (!TryGetDesiredStatVariable(modifierObject.GetTargetStat(), out FloatVariable targetVariable))
                throw new Exception("Invalid enum value.");

            try
            {
                targetVariable.Immutate(modifierObject.GetMutationObject());
            }

            catch
            {
                Debug.LogError("An error occurred while trying to demodify a stat. An " +
                    "Mutation object used may be invalid.");
            }
        }

        #endregion
    }
}
