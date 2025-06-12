using com.absence.variablesystem.mutations;
using System;
using UnityEngine;

namespace com.game.statsystem
{
    /// <summary>
    /// A reference to a stat modifier.
    /// </summary>
    [System.Serializable]
    public class ModifierObject<T> where T : Enum
    {
        [SerializeField] private T m_targetStat;
        [SerializeField] private Mutation<float> m_mutationObject;

        /// <summary>
        /// Current value of the modifier this reference points to.
        /// </summary>
        public float Value
        {
            get
            {
                return m_mutationObject.Value;
            }

            set
            {
                m_mutationObject.Value = value;
            }
        }

        /// <summary>
        /// <b>[VULNERABLE]</b> Use to create a modifier object with a <see cref="Mutation{T}"/>.
        /// </summary>
        /// <param name="mutationObject">The mutation object of this stat.</param>
        public ModifierObject(T targetStat, Mutation<float> mutationObject)
        {
            m_targetStat = targetStat;
            m_mutationObject = mutationObject;
        }

        /// <summary>
        /// Use to get a modifiers target stat enum value.
        /// </summary>
        /// <returns>Returns the enum value.</returns>
        public T GetTargetStat()
        {
            return m_targetStat;
        }

        /// <summary>
        /// <b>[VULNERABLE]</b> Use to get the mutation object of this modifier object.
        /// </summary>
        /// <returns>Returns the mutation object.</returns>
        public Mutation<float> GetMutationObject() 
        {
            return m_mutationObject;
        }
    }
}
