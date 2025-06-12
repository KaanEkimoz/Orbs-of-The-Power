namespace com.game.statsystem
{
    public static class StatSystemSettings
    {
        /// <summary>
        /// If true, percentage-based modifications always affect their target stat 
        /// based on the overall modified value of that stat.
        /// </summary>
        public const bool PERCENTAGE_MODS_ON_TOP = false;    

        /// <summary>
        /// If true, overrides will clear mutations when applied.
        /// </summary>
        public const bool OVERRIDES_CLEAR_MUTATIONS = true;

        /// <summary>
        /// If true, when a cap added to or removed from a stat, both an overall cap and an
        /// in order cap will be added to the stat. This way, any modifiers during or before
        /// the cap added will be uneffective even when the cap is removed.
        /// </summary>
        public const bool CAPS_WIPE_OUT_MOD_HISTORY = true;
    }
}
