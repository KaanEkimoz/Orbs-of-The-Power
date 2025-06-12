namespace com.game
{
    public class DamageEvent
    {
        public static readonly DamageEvent Empty = new()
        {
            DamageSent = 0,
            DamageDealt = 0,
            CausedDeath = false,
        };

        public float DamageSent;
        public float DamageDealt;
        public bool CausedDeath;
    }
}
