using com.absence.variablesystem;
using com.absence.variablesystem.mutations;
using com.game.statsystem.presetobjects;
using System;

namespace com.game.statsystem
{
    public interface IStatManipulator<T> where T : Enum
    {
        /// <summary>
        /// Use to modify a stat variable with a <see cref="StatModification"/>.
        /// </summary>
        /// <param name="mod">The modification object.</param>
        /// <returns>Returns the modifier object created from this operation.</returns>
        ModifierObject<T> ModifyWith(StatModification<T> mod);
        /// <summary>
        /// Use to cap a stat variable with a <see cref="StatCap"/>.
        /// </summary>
        /// <param name="cap">The modification object.</param>
        /// <returns>Returns the modifier object created from this operation.</returns>
        ModifierObject<T> CapWith(StatCap<T> cap);
        /// <summary>
        /// Use to override a stat variable with a <see cref="StatOverride"/>.
        /// </summary>
        /// <param name="ovr">The modification object.</param>
        /// <returns>Returns the new value of the stat variable.</returns>
        float OverrideWith(StatOverride<T> ovr);

        /// <summary>
        /// <b>[VULNERABLE]</b> Use to add a custom-logic modification to a stat.
        /// </summary>
        /// <param name="targetStat">Which stat to apply the modification.</param>
        /// <param name="mutationObject">Mutation object created</param>
        /// <returns>Returns the modifier object passed as an argument.</returns>
        ModifierObject<T> ModifyCustom(T targetStat, Mutation<float> mutationObject);
        /// <summary>
        /// Use to add an incremental modification (eg. +5, -2) to a stat.
        /// </summary>
        /// <param name="targetStat">Which stat to apply the modification.</param>
        /// <param name="amount">Amount of modification.</param>
        /// <returns>
        /// Returns the modifier applied to desired stat. You can
        /// remove this modification from the desired stat via the 
        /// <see cref="Demodify(PlayerStatType, Mutation{float})"/> function.
        /// </returns>
        ModifierObject<T> ModifyIncremental(T targetStat, float amount, AffectionMethod affectionMethod = AffectionMethod.InOrder);
        /// <summary>
        /// Use to add a percentage based modification (eg. +25%, -100%) to a stat.
        /// </summary>
        /// <param name="targetStat">Which stat to apply the modification.</param>
        /// <param name="percentage">Amount of modification. (in the following form: <i>percentage</i>%).</param>
        /// <returns>
        /// Returns the modifier applied to desired stat. You can
        /// remove this modification from the desired stat via the 
        /// <see cref="Demodify(PlayerStatType, Mutation{float})"/> function.
        /// </returns>
        ModifierObject<T> ModifyPercentage(T targetStat, float percentage, AffectionMethod affectionMethod = AffectionMethod.InOrder);
        /// <summary>
        /// Use to de-modify a stat.
        /// </summary>
        /// <param name="targetStat">Which stat to de-modify.</param>
        /// <param name="modifierObject">The modifier object created when
        /// the modification took place.</param>
        void Demodify(ModifierObject<T> modifierObject);

        /// <summary>
        /// Use to perform an action for every stat value.
        /// </summary>
        /// <param name="action">Action to perform.</param>
        void ForAllStatValues(Action<float> action);
        /// <summary>
        /// Use to perform an action for every stat value with its corresponding enum value.
        /// </summary>
        /// <param name="action">Action to perform.</param>
        void ForAllStatEntries(Action<T, float> action);

        /// <summary>
        /// Use to refresh the value of every stat via removing and re-applying every
        /// modificataion on them.
        /// </summary>
        void RefreshAll();
        /// <summary>
        /// Use to remove all of the modifiers from all of the stats.
        /// </summary>
        void ClearAllModifiers();
        /// <summary>
        /// Use to refresh a single stat variable.
        /// </summary>
        /// <param name="targetStat">The stat variable to refresh.</param>
        /// <returns>Returns false if something goes wrong, true otherwise.</returns>
        bool Refresh(T targetStat);
        /// <summary>
        /// Use to clear modifiers applied to a single stat variable.
        /// </summary>
        /// <param name="targetStat">The stat to clear modifiers of.</param>
        /// <returns>Returns false if something goes wrong, true otherwise.</returns>
        bool ClearModifiersOf(T targetStat);
    }
}
