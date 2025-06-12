namespace com.game
{
    public static class InternalSettings
    {
        /// <summary>
        /// If true, fake enemies won't drop anything upon death.
        /// </summary>
        public const bool FAKE_ENEMIES_DONT_DROP = true;

        /// <summary>
        /// If true, the randomization on the aim of player is fully random. Which means
        /// the system may also let player aim correctly sometimes.
        /// </summary>
        public const bool AIM_PARANOIA_CAN_BYPASS = true;

        /// <summary>
        /// If true, player will pick an orb on ground up when close to it.
        /// </summary>
        public const bool PLAYER_ORB_AUTO_PICKUP = true;
    }
}
